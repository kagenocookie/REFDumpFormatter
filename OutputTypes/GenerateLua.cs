using System.Text;

namespace REFDumpFormatter;

public partial class GenerateLua
{
    private string basePath = null!;
    private HashSet<string>? overloads;

    private static readonly string[] implicitParents = new[] { "System.Object", "System.ValueType" };

    private static readonly Dictionary<string, string> baseTypes = new() {
        { "System.Boolean", "boolean" },
        { "System.Single", "number" },
        { "System.Double", "number" },
        { "System.Int16", "integer" },
        { "System.UInt16", "integer" },
        { "System.Int32", "integer" },
        { "System.UInt32", "integer" },
        { "System.Int64", "integer" },
        { "System.UInt64", "integer" },
        { "System.SByte", "integer" },
        { "System.Byte", "integer" },
        { "System.String", "string" },
    };

    public void GenerateOutput(OutputOptions options, IEnumerable<(string name, ObjectDef obj)> entries)
    {
        basePath ??= options.OutputFilepath ?? Path.Combine(Directory.GetParent(options.InputFilepath)!.FullName, "lua_reference");
        if (File.Exists(basePath)) {
            Console.Error.WriteLine("ERROR: output path must be a folder, not a file.");
            return;
        }

        if (options.IgnoreOverloads) {
            overloads = new();
        }

        var ctx = new GeneratorContext(entries.ToDictionary(x => x.name, x => x.obj), options, basePath);

        var sb = new StringBuilder();
        var props = new HashSet<string>();

        foreach (var (name, item) in ctx.classNames) {
            var cls = ctx.GenerateSummary(name, item);
            if (cls == null) {
                continue;
            }

            if (item.parent == "System.Enum") {
                HandleEnum(sb, item, cls.Name, ctx);
            } else {
                HandleClass(sb, props, item, cls, item.flags, ctx);
            }
            sb.AppendLine();

            var (outputPath, newFile) = ctx.GetOutputFile(cls);

            if (newFile) {
                File.WriteAllText(outputPath, sb.ToString());
            } else {
                File.AppendAllText(outputPath, sb.ToString());
            }
            sb.Clear();
        }
    }

    private void HandleEnum(StringBuilder sb, ObjectDef item, Classname cls, GeneratorContext ctx)
    {
        var enumType = ctx.GetEnumType(item, cls);

        sb.Append("---@enum ").Append(cls.ToStringFullName(true, true));
        if (item.fields == null || enumType == null) {
            sb.AppendLine().Append("local ").Append(cls.Name).Append(" = {}").AppendLine("-- empty or invalid enum");
            return;
        }

        sb.Append(' ').Append(enumType).AppendLine();

        sb.Append("local ").Append(cls.Name).Append(" = {").AppendLine();

        foreach (var (enumName, enumValue) in ctx.GetSortedEnumValues(item)) {
            sb.AppendLine($"\t{enumName} = {enumValue},");
        }
        sb.Append('}').AppendLine();
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

    private void HandleClass(StringBuilder sb, HashSet<string> props, ObjectDef item, ClassSummary cls, string? flags, GeneratorContext ctx)
    {
        overloads?.Clear();
        sb.Append("---");
        if (item.flags?.Contains("ClassSemanticsMask") == true) {
            sb.Append(" interface (not directly instantiable)");
        } else if (item.parent == "System.ValueType") {
            sb.Append(" value type");
        } else if (item.IsAbstract) {
            sb.Append(" abstract (not directly instantiable)");
        }
        if (item.IsNative) {
            sb.Append(" (native)");
        }

        sb.AppendLine();
        var fullname = cls.Name.ToStringFullName(true, true);
        sb.Append("---@class ").Append(fullname);

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
            hasParent = true;
        }
        if (baseTypes.TryGetValue(fullname, out var basetypeStr)) {
            sb.Append(hasParent ? ", " : " : ");
            sb.Append(basetypeStr);
        }
        sb.AppendLine();

        if (item.fields != null) {
            foreach (var (fieldName, field) in item.fields) {
                var friendlyTypeName = GetFriendlyTypeName(field.Type, ctx, cls.Name);

                if (fieldName.EndsWith("k__BackingField")) {
                    // example property name: <IsFinishWait>k__BackingField;
                    var propName = fieldName[1..fieldName.IndexOf('>')];
                    var getter = item.methods?.FirstOrDefault(k => k.Key.StartsWith($"get_{propName}"));
                    var setter = item.methods?.FirstOrDefault(k => k.Key.StartsWith($"set_{propName}"));
                    var baseAccess = "public";
                    if (getter?.Value?.IsPrivate == true && setter?.Value?.IsPrivate != false) {
                        baseAccess = "private";
                    }
                    string KvToString(KeyValuePair<string, MethodDef> val)
                    {
                        var mn = val.Key.Replace(val.Value.id.ToString(), "").Replace("`", "");
                        return val.Value.IsPrivate ? $"private {mn}()" : $"{mn}()";
                    }
                    var gtstr = getter?.Value != null ? KvToString(getter.Value) : "";
                    var ststr = setter?.Value != null ? KvToString(setter.Value) : "";
                    if (getter?.Value?.IsAbstract == true || setter?.Value?.IsAbstract == true) {
                        baseAccess = baseAccess + " abstract";
                    }
                    var isAbstract = getter?.Value?.IsAbstract == true || setter?.Value?.IsAbstract == true;

                    sb.Append($"---@field {propName}_k__BackingField {friendlyTypeName} ")
                        .Append(fieldName)
                        .Append(isAbstract ? " ABSTRACT" : "")
                        .Append($" (Property: {gtstr} {ststr})");
                } else {
                    var access = field.IsPrivate ? "private" : "public";
                    sb.Append("---@field ").Append(fieldName).Append(' ').Append(friendlyTypeName).Append(' ').Append(access);
                }
                if (field.IsStatic) {
                    sb.Append(" static");
                }

                if (ctx.options.FieldOffsets == true) {
                    sb.Append(" / offset: ").Append(field.OffsetFromBase);
                }
                sb.AppendLine();
            }
        }

        if (item.methods != null) {
            var methodsList = item.methods
                .Where(m => !m.Key.StartsWith('<'))// autogenerated (mostly lambdas), skip
                ;

            foreach (var (methodName, method) in methodsList) {
                // if (methodName.StartsWith("get_") || methodName.StartsWith("set_") || methodName.StartsWith("add_") || methodName.StartsWith("remove_")) {
                //     continue;
                // }

                var cleanName = methodName.Replace(method.id.ToString(), "").Replace("`", "");
                if (overloads?.Add(cleanName) == false) {
                    continue;
                }

                IEnumerable<(Classname? type, string name, string mod)> paramsList = method.Params == null || method.Params.Length == 0
                    ? []
                    : method.Params
                        .Select((p, i) => p == null ? (null, $"arg{i}", "") : (Classname.Parse(p.Type, ctx, cls.Name), p.Name, p.ByRef ? "byref__" : ""));
                var paramsStr = string.Join(", ", paramsList
                    .Select(c => c.mod + (c.name ?? "UNNAMED") + ": " + (c.type?.ToStringFullName(true, true) ?? "any")));
                if (!method.IsStatic) {
                    paramsStr = string.IsNullOrEmpty(paramsStr) ? "self" : "self, " + paramsStr;
                }

                if (cleanName.StartsWith(".ctor") || cleanName.StartsWith(".cctor")) {
                    sb.AppendLine($"---@field __{cleanName[1..]} fun({paramsStr}): nil {methodName}");
                } else {
                    var returnType = method.Returns?.Type switch {
                        null => "nil",
                        "System.Void" => "nil",
                        "unknown" => "any",
                        _ => GetFriendlyTypeName(method.Returns.Type, ctx, cls.Name)
                    };
                    var genArgs = paramsList.Where(x => x.Item1 != null && GeneratorContext.GenericParamNameRegex().IsMatch(x.Item1.Name));
                    if (genArgs.Any()) {
                        cleanName += "<" + string.Join(", ", genArgs.Select(a => a.Item1!.Name).Distinct()) + ">";
                    }
                    if (cleanName.Contains('.')) {
                        // explicit interface implementations
                        sb.Append($"---@field {cleanName} fun({paramsStr}): {returnType}");
                    } else {
                        var baseAccess = method.IsPrivate ? "private" : "public";
                        sb.Append($"---@field {cleanName} fun({paramsStr}): {returnType} {baseAccess}");
                    }

                    if (method.IsAbstract) {
                        sb.Append(" abstract");
                    } else if (method.IsVirtual) {
                        sb.Append(" virtual");
                        if (HasVirtualInAnyBaseClass(cls.OriginalName, cleanName, ctx.classNames)) {
                            sb.Append(" (overrides base class method)");
                        }
                    } else if (method.IsStatic) {
                        sb.Append(" static");
                    }

                    sb.AppendLine();
                }
            }
        }
    }

    private static string GetFriendlyTypeName(string? typeString, GeneratorContext ctx, Classname containingClass)
    {
        if (string.IsNullOrEmpty(typeString)) {
            return "any";
        }
        var parsedFieldType = Classname.Parse(typeString, ctx, containingClass);
        return parsedFieldType?.ToStringFullName(true, true) ?? PreprocessTypeForDisplay(typeString);
    }

    private static string PreprocessTypeForDisplay(string? typeString)
    {
        if (string.IsNullOrEmpty(typeString)) {
            return "any";
        }
        if (typeString.StartsWith('!')) {
            // these are generic parameters
            // !0 = first owner generic class parameter
            // !1 = second owner generic class parameter

            // !!0 (double exclamation) - maybe method generic parameters?
            return $"any --[[Invalid type: {typeString}]]";
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
}
