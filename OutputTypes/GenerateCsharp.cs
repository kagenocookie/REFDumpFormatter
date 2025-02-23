using System.Globalization;
using System.Text;
using System.Text.Json;

namespace REFDumpFormatter;

public partial class GenerateCsharp
{
    private string basePath = null!;
    private List<string> parents = new();
    private HashSet<string>? overloads;

    private static readonly string[] implicitParents = new[] { "System.Object", "System.ValueType" };

    private static readonly Dictionary<string, string> presetTypeMaps = new() {
        { "System.Boolean", "bool" },
        { "System.Single", "float" },
        { "System.Double", "double" },
        { "System.Int16", "short" },
        { "System.UInt16", "ushort" },
        { "System.Int32", "int" },
        { "System.UInt32", "uint" },
        { "System.Int64", "long" },
        { "System.UInt64", "ulong" },
        { "System.SByte", "sbyte" },
        { "System.Byte", "byte" },
        { "System.String", "string" },
    };
    private static readonly HashSet<string> integerTypes = new() {
        "System.Int16",
        "System.UInt16",
        "System.Int32",
        "System.UInt32",
        "System.Int64",
        "System.UInt64",
        "System.SByte",
        "System.Byte",
    };

    public GeneratorContext? GenerateOutput(OutputOptions options, IEnumerable<(string name, ObjectDef obj)> entries)
    {
        basePath ??= options.OutputFilepath ?? Path.Combine(Directory.GetParent(options.InputFilepath)!.FullName, "csharp_reference");
        if (File.Exists(basePath)) {
            Console.Error.WriteLine("ERROR: output path must be a folder, not a file.");
            return null;
        }

        if (options.IgnoreOverloads) {
            overloads = new();
        }

        Directory.CreateDirectory(basePath);
        File.WriteAllText(Path.Combine(basePath, "SdkCsharp.sln"), """
        Microsoft Visual Studio Solution File, Format Version 12.00
        # Visual Studio Version 17
        VisualStudioVersion = 17.0.31903.59
        MinimumVisualStudioVersion = 10.0.40219.1
        Project("{4CFB1610-32C6-46BE-A5A0-9070E9C72975}") = "SdkCsharp", "SdkCsharp.csproj", "{CB7DDB1E-8784-49B9-A653-F902273C737F}"
        EndProject
        Global
            GlobalSection(SolutionConfigurationPlatforms) = preSolution
                Debug|Any CPU = Debug|Any CPU
                Release|Any CPU = Release|Any CPU
            EndGlobalSection
            GlobalSection(SolutionProperties) = preSolution
                HideSolutionNode = FALSE
            EndGlobalSection
            GlobalSection(ProjectConfigurationPlatforms) = postSolution
                {CB7DDB1E-8784-49B9-A653-F902273C737F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
                {CB7DDB1E-8784-49B9-A653-F902273C737F}.Debug|Any CPU.Build.0 = Debug|Any CPU
                {CB7DDB1E-8784-49B9-A653-F902273C737F}.Release|Any CPU.ActiveCfg = Release|Any CPU
                {CB7DDB1E-8784-49B9-A653-F902273C737F}.Release|Any CPU.Build.0 = Release|Any CPU
            EndGlobalSection
        EndGlobal
        """);

        File.WriteAllText(Path.Combine(basePath, "SdkCsharp.csproj"), """
        <Project Sdk="Microsoft.NET.Sdk">
            <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>disable</Nullable>
                <RunAnalyzers>false</RunAnalyzers>
            </PropertyGroup>

            <ItemGroup>
                <Analyzer Remove="@(Analyzer)" />
            </ItemGroup>
        </Project>
        """);

        // hopefully comprehensive minimal test cases (DD2):
        // entries = entries
        //     .Where(e => false
        //         // enum
        //         || e.name == "app.GUITextureLoaderDefine.LoaderType"
        //         // multiline string default value
        //         || e.name == "app.CollisionSystem"
        //         // void delegate
        //         || e.name == "app.HitController.EventDie"
        //         // required for app.HitController.EventDie to detect the namespace properly
        //         || e.name == "app.HitController"
        //         // static sorting test
        //         || e.name == "app.NPCManager"
        //         // contains a Dictionary<GeneratorID,List<LocalCellData>[]> which ends up as both generic syntaxes in one line
        //         || e.name == "app.GenerateManager"
        //         // class with abstract property
        //         || e.name == "app.QuestVariableBase"
        //         // class with override for abstract property
        //         || e.name == "app.QuestTalkResultVariable"
        //         // class with virtual property
        //         || e.name == "app.TalkEventDialogueUserData.ChoiceSegment"
        //         // class with events
        //         || e.name == "app.AIDecisionMaker"
        //         // property with private getter and no setter
        //         || e.name == "via.UserData"
        //         // auto-property with private setter
        //         || e.name == "app.quest.action.QuestActionBase"
        //         // needs global:: prefix because AISituation and app.AISituation name conflict
        //         || e.name == "app.AISituationTask"
        //         // potential enum filename overlap
        //         || e.name == "app.QuestProcessor.Phase"
        //         || e.name == "app.QuestProcessor.ProcessEntity.Phase"
        //         // requestCh220BuildFromPreset has a "null" param
        //         || e.name == "app.CharacterEditManager"
        //         // requestCh220BuildFromPreset has a "null" param
        //         || e.name == "app.CharacterEditManager.ch220Builder"
        //         // case conflict
        //         || e.name == "via.quaternion"
        //         || e.name == "via.Quaternion"
        //         // singleton (generic base class)
        //         || e.name == "app.GenerateManager"
        //         // array of generic in method args
        //         || e.name == "app.ArrayExtension"
        //         // C# struct
        //         || e.name == "app.Ch224Controller.vec3x3"
        //         // native struct
        //         || e.name == "via.Float4"
        //         // generic
        //         || e.name == "app.CharacterBindController.Module`1[[TParameter, application, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]]"
        //         // class within nested generic:
        //         || e.name == "System.Collections.Generic.List`1.Enumerator<app.AISituationObject>"
        //         || e.name == "System.Collections.Generic.Dictionary`2.Enumerator[[TKey, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[TValue, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]"
        //         // generic base class with generic param passed from subclass, abstract methods
        //         || e.name.StartsWith("app.FilterParamAccessor`2")
        //         // generic base class with generic param passed from subclass
        //         || e.name.StartsWith("app.FilterParamBase`1")
        //         // mr. excessive
        //         || e.name.StartsWith("app.AIBlackBoardCollection`13")
        //     );

        var ctx = new GeneratorContext(entries.ToDictionary(x => x.name, x => x.obj), options, basePath);
        ctx.classNames.Add("System.Object", new ObjectDef() {
            methods = new Dictionary<string, MethodDef>() {
                { "Equals0", new MethodDef() { Returns = new ParamDef() { Type = "System.Boolean" }, Flags = "Virtual" }},
                { "GetHashCode0", new MethodDef() { Returns = new ParamDef() { Type = "System.Int32" }, Flags = "Virtual" } },
                { "ToString0", new MethodDef() { Returns = new ParamDef() { Type = "System.String" }, Flags = "Virtual" } },
            }
        });
        ctx.classNames["System.ValueType"] = ctx.classNames["System.Object"];
        ctx.ClassnameFilter = FilterClassnames;
        ctx.remappedTypeNames = presetTypeMaps;

        var sb = new StringBuilder();
        var props = new HashSet<string>();

        foreach (var (name, item) in ctx.classNames) {
            var cls = ctx.GenerateSummary(name, item);
            if (cls == null) {
                continue;
            }

            // name examples:
            // app.GUICharaEditData
            // JsonParser.Value
            // JsonParser.ValueType
            // ReMotion.RotationCompress.Methdo
            // System.Collections.Generic.List`1<JsonParser.Value>
            // app.AIBlackBoardBase`1<app.AITarget>
            // via.gui.GUIPath`1[[T, viacore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
            // via.gui.GUIPath`1<via.gui.View>
            // app.BaseNonCylcleDelegateWrapper.Enumerator`1<System.Action`1<System.Single>>
            // TODO app.EPVExpertMonsterSpell.<getTargetElements>d__23
            // TODO System.Array.InternalEnumerator<System.Collections.Generic.KeyValuePair<app.TalkDefine.NpcTalkTiming,System.Collections.Generic.List<app.NpcTalkSituationWrapper>>>

            var (outputPath, newFile) = ctx.GetOutputFile(cls);

            parents.Clear();
            var parent = cls.Name.Parent;
            while (parent != null) {
                parents.Add(parent.ToStringFullName(false, false));
                parent = parent.Parent;
            }
            parents.Reverse();
            var tabs = new string('\t', parents.Count);

            if (newFile && !string.IsNullOrEmpty(cls.Name.Namespace)) {
                sb.Append("namespace ").Append(cls.Name.Namespace).AppendLine(";").AppendLine();
            }

            for (int i = 0; i < parents.Count; ++i) {
                sb.Append('\t', i).Append("public partial class ").Append(parents[i]).AppendLine()
                    .Append('\t', i).Append('{').AppendLine();
            }

            sb.Append('\t', parents.Count).Append("/// <remarks>");
            if (item.IsNative) {
                sb.Append("NATIVE TYPE. ");
            }
            sb.Append("Full class path: ").Append(name);
            sb.Append("</remarks>").AppendLine();

            if (item.parent == "System.Enum") {
                HandleEnum(sb, item, tabs, cls.Name, ctx);
            } else if (item.parent == "System.MulticastDelegate") {
                HandleDelegate(sb, props, item, cls, tabs, ctx);
            } else {
                HandleClass(sb, props, item, cls, tabs, ctx);
            }

            for (int i = parents.Count - 1; i >= 0; --i) {
                sb.Append('\t', i).Append('}').AppendLine();
            }

            sb.AppendLine();

            Directory.CreateDirectory(Directory.GetParent(outputPath)!.FullName);

            if (newFile) {
                File.WriteAllText(outputPath, sb.ToString());
            } else {
                File.AppendAllText(outputPath, sb.ToString());
            }
            sb.Clear();
        }
        return ctx;
    }

    private static bool FilterClassnames(Classname cls, GeneratorContext ctx)
    {
        if (cls.Namespace == "via" && char.IsAsciiLetterLower(cls.Name[0]) && ctx.classNames.ContainsKey(string.Concat(cls.Namespace, ".", cls.Name.Substring(0, 1).ToUpperInvariant(), cls.Name.AsSpan(1)))) {
            // non-native class, possibly with ext methods or nested classes or whatever
            // concrete example, there's a via.quaternion and via.Quaternion name conflict
            // windows paths aren't case sensitive so emitting only lowercase ones cause they're the real native type and ignoring the c# side of it
            return false;
        }
        return true;
    }

    private void HandleEnum(StringBuilder sb, ObjectDef item, string tabs, Classname cls, GeneratorContext ctx)
    {
        var enumType = ctx.GetEnumType(item, cls);

        if (item.fields == null || enumType == null) {
            sb.Append(tabs); sb.Append("public enum ").Append(cls.Name).Append(" { }").AppendLine()
                .AppendLine("// empty or invalid enum");
            return;
        }


        sb.Append(tabs); sb.Append("public enum ").Append(cls.Name).Append(" : ").AppendLine(enumType)
            .Append(tabs); sb.Append('{').AppendLine();

        var subtabs = tabs + '\t';
        foreach (var (enumName, enumValue, enumHex) in ctx.GetSortedEnumValues(item, enumType)) {
            if (ctx.options.FieldOffsets == true) {
                sb.AppendLine($"{subtabs}{enumName} = {enumValue}, // 0x{enumHex}");
            } else {
                sb.AppendLine($"{subtabs}{enumName} = {enumValue},");
            }
        }

        sb.Append(tabs).AppendLine("}");
    }

    private static bool HasVirtualInAnyBaseClass(string? classname, string method, Dictionary<string, REFDumpFormatter.ObjectDef> classes)
    {
        if (classname == null || !classes.TryGetValue(classname, out var parent)) {
            return false;
        }
        classname = parent.parent;
        while (classname != null && classes.TryGetValue(classname, out parent)) {
            if (parent.methods != null) {
                foreach (var mm in parent.methods) {
                    if (mm.Key.StartsWith(method) && mm.Key == method + mm.Value.id) {
                        return mm.Value.IsVirtual || mm.Value.IsAbstract;
                    }
                }
            }
            classname = parent.parent;
        }
        return false;
    }

    private void HandleDelegate(StringBuilder sb, HashSet<string> props, ObjectDef item, ClassSummary cls, string tabs, GeneratorContext ctx)
    {
        var invokeMethod = item.methods?.FirstOrDefault(m => m.Key == "Invoke" + m.Value.id).Value;
        if (invokeMethod == null) {
            HandleClass(sb, props, item, cls, tabs, ctx);
            return;
        }

        var returntype = invokeMethod.Returns?.Type;
        sb.Append(tabs)
            .Append("public delegate ")
            .Append(returntype == null || returntype == "System.Void" ? "void" : PreprocessTypeForDisplay(returntype))
            .Append(' ')
            .Append(cls.Name.Name)
            .Append('(');
        if (invokeMethod.Params != null) {
            sb.AppendJoin(", ", invokeMethod.Params.Select((param, i) => PreprocessTypeForDisplay(param?.Type) + " " + PreprocessNameForDisplay(param?.Name, i)));
        }
        sb.AppendLine(");");

        // HandleClass(sb, props, item, cls, tabs + "// ", ctx);
    }
    private void HandleClass(StringBuilder sb, HashSet<string> props, ObjectDef item, ClassSummary cls, string tabs, GeneratorContext ctx)
    {
        overloads?.Clear();
        props.Clear();
        string defType;
        if (item.flags?.Contains("ClassSemanticsMask") == true) {
            defType = "interface ";
        } else if (item.parent == "System.ValueType") {
            defType = "partial struct ";
        } else if (item.IsAbstract) {
            defType = "abstract partial class ";
        } else {
            defType = "partial class ";
        }

        var subtabs = tabs + '\t';
        sb.Append(tabs); sb.Append("public ").Append(defType).Append(cls.Name.ToStringFullName(false, false));
        // handle parents, interfaces
        var hasParent = !string.IsNullOrEmpty(item.parent) && !implicitParents.Contains(item.parent);
        if (hasParent) {
            var parentCls = Classname.Parse(item.parent!, ctx, cls.Name);
            sb.Append(" : ").Append(parentCls?.ToStringFullName(true, true) ?? item.parent);
        }
        var interfaces = item.methods?.Where(m => m.Key.AsSpan()[1..].Contains('.')).Select(m => m.Key.Substring(0, m.Key.LastIndexOf('.'))).Distinct();
        if (interfaces?.Any() == true) {
            sb.Append(hasParent ? ", " : " : ");
            sb.AppendJoin(", ", interfaces);
        }

        sb.AppendLine().Append(tabs); sb.Append('{').AppendLine();

        if (item.fields != null) {
            foreach (var (fieldName, field) in item.fields.OrderBy(f => (!f.Value.IsStatic, f.Value.OffsetNumber))) {
                var friendlyTypeName = GetFriendlyTypeName(field.Type, ctx, cls.Name);
                if (fieldName.EndsWith("k__BackingField")) {
                    var defaultVal = EvaluateFieldDefault(field, ";");
                    // example property name: <IsFinishWait>k__BackingField;
                    var propName = fieldName[1..fieldName.IndexOf('>')];
                    var getter = item.methods?.FirstOrDefault(k => k.Key.StartsWith($"get_{propName}"));
                    var setter = item.methods?.FirstOrDefault(k => k.Key.StartsWith($"set_{propName}"));
                    var baseAccess = "public";
                    if (getter?.Value?.IsPrivate == true && setter?.Value?.IsPrivate != false) {
                        baseAccess = "private";
                    }
                    var gtstr = getter != null ? (getter?.Value?.IsPrivate == true && baseAccess != "private" ? "private get;" : "get;") : "";
                    var ststr = setter != null ? (setter?.Value?.IsPrivate == true && baseAccess != "private" ? "private set;" : "set;") : "";
                    if (setter?.Value?.IsStatic == true || getter?.Value?.IsStatic == true) {
                        baseAccess = baseAccess + " static";
                    }
                    var offset = ctx.options.FieldOffsets == true ? $" // offset: {field.OffsetFromBase} ({field.OffsetNumber})" : string.Empty;
                    if (getter?.Value?.IsAbstract == true || setter?.Value?.IsAbstract == true) {
                        baseAccess = baseAccess + " abstract";
                    } else if (getter?.Value?.IsVirtual == true || setter?.Value?.IsVirtual == true) {
                        baseAccess = HasVirtualInAnyBaseClass(cls.OriginalName, getter.HasValue ? $"get_{propName}" : $"set_{propName}", ctx.classNames)
                            ? baseAccess + " override"
                            : baseAccess + " virtual";
                    }
                    sb.AppendLine($"{subtabs}{baseAccess} {friendlyTypeName} {propName} {{ {gtstr} {ststr} }}{defaultVal}{offset}");
                    props.Add(propName);
                } else if (item.methods?.FirstOrDefault(m => m.Key.StartsWith("add_") && m.Key.AsSpan()[4..^((int)Math.Log10(m.Value.id) + 1)].SequenceEqual(fieldName)).Value is MethodDef eventMethod) {
                    // events included for completeness sake but probably useless for the foreseeable future
                    var access = eventMethod.IsPrivate ? "private " : "public ";
                    var evtType = eventMethod.Params != null && eventMethod.Params.Length > 0 && eventMethod.Params[0] != null ? GetFriendlyTypeName(eventMethod.Params[0]!.Type, ctx, cls.Name) : "/*Could not determine event type*/System.Action";
                    var offset = ctx.options.FieldOffsets == true ? $" // offset: {field.OffsetFromBase} ({field.OffsetNumber})" : string.Empty;
                    sb.Append(subtabs).AppendLine($"{access}{eventMethod.Modifiers}event {evtType} {fieldName};{offset}");
                } else {
                    var defaultVal = EvaluateFieldDefault(field, "");
                    var baseAccess = field.IsPrivate ? "private" : "public";
                    var offset = ctx.options.FieldOffsets == true ? $" // offset: {field.OffsetFromBase} ({field.OffsetNumber})" : string.Empty;
                    sb.AppendLine($"{subtabs}{baseAccess} {field.Modifiers}{friendlyTypeName} {fieldName}{defaultVal};{offset}");
                }
            }
        }

        if (item.methods != null) {
            sb.AppendLine();
            var methodsList = item.methods
                .Where(m => !m.Key.StartsWith('<'))// autogenerated (mostly lambdas), skip
                ;

            foreach (var (methodName, method) in methodsList.OrderBy(m => m.Value.id)) {
                if (methodName.StartsWith("get_") || methodName.StartsWith("set_")) {
                    var propName = methodName.Replace(method.id.ToString(), "")[4..];
                    // non-auto property
                    // if props already contains the propName, then it's an auto-prop with a backing field, therefore no need to handle it here
                    if (!props.Contains(propName)) {
                        var propType = methodName.StartsWith('g') ? method.Returns?.Type : method.Params?[0]?.Type;
                        var getter = item.methods?.FirstOrDefault(k => k.Key.StartsWith($"get_{propName}"));
                        var setter = item.methods?.FirstOrDefault(k => k.Key.StartsWith($"set_{propName}"));

                        var baseAccess = "public";
                        if (getter?.Value?.IsPrivate == true && setter?.Value?.IsPrivate != false) {
                            baseAccess = "private";
                        }
                        bool hasBody = true;
                        if (getter?.Value?.IsAbstract == true || setter?.Value?.IsAbstract == true) {
                            baseAccess = baseAccess + " abstract";
                            hasBody = false;
                        } else if (getter?.Value?.IsVirtual == true || setter?.Value?.IsVirtual == true) {
                            baseAccess = HasVirtualInAnyBaseClass(cls.OriginalName, getter.HasValue ? $"get_{propName}" : $"set_{propName}", ctx.classNames)
                                ? baseAccess + " override"
                                : baseAccess + " virtual";
                        }
                        sb.Append(subtabs).Append(baseAccess).Append($" {GetFriendlyTypeName(propType, ctx, cls.Name)} {propName} {{");

                        if (getter?.Value != null) {
                            if (getter.Value.Value.IsPrivate && baseAccess != "private") sb.Append(" private");
                            sb.Append(hasBody ? " get => throw new NotImplementedException();" : " get;");
                        }
                        if (setter?.Value != null) {
                            if (setter.Value.Value.IsPrivate && baseAccess != "private") sb.Append(" private");
                            sb.Append(hasBody ? " set => throw new NotImplementedException();" : " set;");
                        }
                        sb.AppendLine(" }");
                        props.Add(propName);
                    }
                }
            }
            sb.AppendLine();

            foreach (var (methodName, method) in methodsList.OrderBy(m => m.Value.id)) {
                if (methodName.StartsWith("get_") || methodName.StartsWith("set_") || methodName.StartsWith("add_") || methodName.StartsWith("remove_")) {
                    continue;
                }

                var cleanName = methodName.Replace(method.id.ToString(), "").Replace("`", "");
                if (overloads?.Add(cleanName) == false) {
                    continue;
                }

                // TODO explicit interface methods? (if methodName is a FQN classname e.g. System.Collections.IEnumerable.GetEnumerator146947)

                IEnumerable<(Classname? type, string name, string mod)> paramsList = method.Params == null || method.Params.Length == 0
                    ? []
                    : method.Params
                        .Select((p, i) => p == null ? (null, $"arg{i}", "") : (Classname.Parse(p.Type, ctx, cls.Name), p.Name, p.ByRef ? "out " : ""));
                var paramsStr = string.Join(", ", paramsList
                    .Select((c, i) => c.mod + (c.type?.ToStringFullName(true, true) ?? "object?") + " " + PreprocessNameForDisplay(c.name, i)));

                if (cleanName.StartsWith(".ctor")) {
                    sb.AppendLine($"{subtabs}public {cls.Name.Name}({paramsStr}) {{}}");
                } else if (cleanName.StartsWith(".cctor")) {
                    // idk what these are, maybe be a compiler-generated default assigner?
                    // sb.AppendLine($"{tabs}/// {methodName} -- {cls.Name.Name}({paramsStr})
                } else {
                    var returnType = method.Returns?.Type switch {
                        null => "void",
                        "System.Void" => "void",
                        "unknown" => "object",
                        _ => GetFriendlyTypeName(method.Returns.Type, ctx, cls.Name)
                    };
                    var genArgs = paramsList.Where(x => x.Item1 != null && GeneratorContext.GenericParamNameRegex().IsMatch(x.Item1.Name));
                    if (genArgs.Any()) {
                        cleanName += "<" + string.Join(", ", genArgs.Select(a => a.Item1!.Name).Distinct()) + ">";
                    }
                    if (cleanName.Contains('.')) {
                        // explicit interface implementations
                        sb.AppendLine($"{subtabs}{returnType} {cleanName}({paramsStr}) => throw new NotImplementedException();");
                    } else {
                        var baseAccess = method.IsPrivate ? "private " : "public ";
                        if (method.IsAbstract) {
                            baseAccess = baseAccess + "abstract ";
                        } else if (method.IsVirtual) {
                            baseAccess = HasVirtualInAnyBaseClass(cls.OriginalName, cleanName, ctx.classNames)
                                ? baseAccess + "override "
                                : baseAccess + "virtual ";
                        } else if (method.IsStatic) {
                            baseAccess = baseAccess + "static ";
                        }
                        var offsets = ctx.options.FieldOffsets == true ? $" // id: {method.id}" : "";
                        var body = method.IsAbstract ? ";" : " => throw new NotImplementedException();";
                        sb.AppendLine($"{subtabs}{baseAccess}{returnType} {cleanName}({paramsStr}){body}{offsets}");
                    }
                }
            }
        }

        sb.Append(tabs).AppendLine("}");
    }

    private static string PreprocessNameForDisplay(string? name, int i)
    {
        if (name == "object") return "@object";
        if (name == "delegate") return "@delegate";
        return string.IsNullOrEmpty(name) ? "arg" + i : name;
    }

    private static string GetFriendlyTypeName(string? typeString, GeneratorContext ctx, Classname containingClass)
    {
        if (string.IsNullOrEmpty(typeString)) {
            return "object?";
        }
        var parsedFieldType = Classname.Parse(typeString, ctx, containingClass);
        return parsedFieldType?.ToStringFullName(true, true) ?? PreprocessTypeForDisplay(typeString);
    }

    private static string PreprocessTypeForDisplay(string? typeString)
    {
        if (string.IsNullOrEmpty(typeString)) {
            return "object?";
        }
        if (presetTypeMaps.TryGetValue(typeString, out var shorter)) {
            return shorter;
        }
        if (typeString.StartsWith('!')) {
            // these are generic parameters
            // !0 = first owner generic class parameter
            // !1 = second owner generic class parameter

            // !!0 (double exclamation) - maybe method generic parameters?
            return $"object /* Invalid type: {typeString} */";
        }

        int backtickPos;
        do {
            backtickPos = typeString.IndexOf('`');
            if (backtickPos != -1) {
                typeString = typeString[0..backtickPos] + typeString[(backtickPos + 2)..];
            }
        } while (backtickPos != -1);
        return typeString;
    }

    private static string? EvaluateFieldDefault(FieldDef field, string suffix)
    {
        if (field.Default == null) return null;

        if (field.Default is JsonElement elem) {
            if (field.Type == "System.Boolean") {
                return !elem.GetBoolean() ? null : " = true" + suffix;
            }

            if (elem.ValueKind == JsonValueKind.Number) {
                if (field.Type == "System.UInt64") {
                    return elem.GetUInt64() == 0 ? null : $" = {elem}{suffix}";
                } else if (integerTypes.Contains(field.Type)) {
                    return elem.GetInt64() == 0 ? null : $" = {elem}{suffix}";
                } else if (field.Type == "System.Single") {
                    return elem.GetSingle() == 0 ? null : $" = {elem}f{suffix}";
                } else if (field.Type == "System.Double") {
                    return elem.GetDouble() == 0 ? null : $" = {elem}{suffix}";
                }
            }

            if (field.Type == "System.String") {
                return $" = \"{EscapeString(elem.ToString())}\"{suffix}";
            }
        }

        return null;
    }

    private static string EscapeString(string str)
    {
        return str.Replace("\"", "\\\"").Replace("\n", "\\n");
    }
}
