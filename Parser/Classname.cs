namespace REFDumpFormatter;

using System.Text;

public class Classname
{
    public string Namespace { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Classname? Parent { get; set; }
    public Classname[]? Generic { get; set; }
    public bool IsArray { get; private set; }
    public Classname[]? InheritedGeneric => Generic ?? Parent?.InheritedGeneric;
    public GeneratorContext Context { get; }

    public Classname(GeneratorContext context)
    {
        Context = context;
    }

    public Classname AsArray(bool array) => !array ? this : new Classname(Context) {
        Name = Name,
        Parent = Parent,
        Namespace = Namespace,
        IsArray = true,
        Generic = Generic,
    };

    public override string ToString() => ToStringFullName(true, true);

    private static readonly StringBuilder sb = new();
    private string ToStringSelfNameOnly()
    {
        if (Generic?.Length > 0) {
            return $"{Name}<{string.Join(", ", Generic.Select(g => g.ToStringFullName(true, true)))}>" + (IsArray ? "[]" : "");
        } else {
            return Name;
        }
    }

    private static readonly string[] globalPrefixNamespaces = ["AISituation"];

    public string ToStringFullName(bool includeNamespace, bool includeParents)
    {
        if (includeNamespace && !string.IsNullOrEmpty(Namespace)) {
            if (Context.options.Type == OutputType.GenerateCsharp && includeParents && globalPrefixNamespaces.Contains(Namespace)) {
                sb.Append("global::");
            }
            sb.Append(Namespace).Append('.');
        }

        if (includeParents && Parent != null) {
            parents.Clear();
            var parent = Parent;
            while (parent != null) {
                parents.Add(parent.ToStringSelfNameOnly());
                parent = parent.Parent;
            }
            parents.Reverse();
            sb.AppendJoin('.', parents).Append('.');
        }
        sb.Append(Name);
        var baseName = sb.ToString();
        sb.Clear();
        if (Context.remappedTypeNames?.TryGetValue(baseName, out var shorter) == true) {
            baseName = shorter;
        }
        if (Generic?.Length > 0) {
            return $"{baseName}<{string.Join(", ", Generic.Select(g => g.ToStringFullName(true, true)))}>" + (IsArray ? "[]" : "");
        } else {
            return IsArray ? $"{baseName}[]" : baseName;
        }
    }
    public string StringifyForFilename()
    {
        if (Parent != null) {
            return Parent.StringifyForFilename() + "__" + Name;
        }

        return Name;
    }

    public string PreprocessTypeForDisplay(string? typeString)
    {
        if (string.IsNullOrEmpty(typeString)) {
            return "object?";
        }
        if (Context.remappedTypeNames?.TryGetValue(typeString, out var shorter) == true) {
            return shorter;
        }
        if (typeString.StartsWith('!')) {
            // UPDATE: these are probably generic parameters
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

    public static Classname ParseBracketedElement(ref StringParser parser, GeneratorContext ctx)
    {
        // single [] element e.g.:
        // [System.UInt32, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]
        // [System.ValueTuple`3[[System.UInt32, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null], [System.Boolean, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null], [System.Boolean, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
        var sep = parser.text.IndexOfAnyOffset('`', ',', parser.pos);
        if (sep == -1) {
            throw new Exception("Invalid bracketed element, no [`,] separator found");
        }

        var res = new Classname(ctx);
        parser.pos++;
        ParseBaseName(ref res, ref parser, sep, ctx);
        if (parser.text[sep] == ',') {
            // plan, non-generic element
            parser.pos = parser.text.IndexOfOffset(']', sep) + 1;
            return res;
        }

        parser.pos = sep;
        res.Generic = ParseBracketed(ref parser, ctx);
        // skip until the end of the metadata part, like: , System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]
        parser.pos = parser.text.IndexOfOffset(']', parser.pos + 1) + 1;
        return res;
    }
    public static Classname[] ParseBracketed(ref StringParser parser, GeneratorContext ctx)
    {
        // input: `1[[System.UInt32, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
        // input: `1[[System.ValueTuple`3[[System.UInt32, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[System.Boolean, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[System.Boolean, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]], System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]
        List<Classname> generics = new(1);
        var doubleBracketStart = parser.text.IndexOfOffset("[[", parser.pos);
        if (doubleBracketStart == -1) {
            var fallbackIdx = parser.text.IndexOfOffset("[", parser.pos);
            if (fallbackIdx != -1) {
                // example: System.Collections.Generic.KeyValuePair`2[app.CharacterID,app.CharacterLookAt][][]
                // TODO
                parser.pos = parser.text.IndexOf(']') + 1;
                return Array.Empty<Classname>();
            } else {
                throw new Exception("Missing double bracket oi");
            }
        }

        parser.pos = doubleBracketStart + 1;
        do {
            if (parser.Cur == ',') {
                parser.pos++;
            }
            var inner = ParseBracketedElement(ref parser, ctx);
            generics.Add(inner);
        } while (parser.Cur == ',');

        if (parser.Cur != ']') {
            throw new Exception("Hmm is this right?");
        }
        parser.pos++;
        return generics.ToArray();
    }
    private static Classname ParsePlainGenericSingle(ref StringParser parser, GeneratorContext ctx)
    {
        var sep = parser.text.IndexOfAnyOffset('`', ',', '>', parser.pos);
        if (sep == -1) {
            throw new Exception("Oiii");
        }
        var cls = new Classname(ctx);
        // cls.Parent = parser.parentClass;
        if (sep == parser.pos && parser.text[sep] == '>') {
            // edge case, empty name
            cls.Name = "object";
            return cls;
        }
        var prevParent = parser.parentClass;
        parser.parentClass = cls;
        ParseBaseName(ref cls, ref parser, sep, ctx);
        if (parser.text[sep] == '`') {
            var dot = parser.text.IndexOfAnyOffset('.', '<', '[', sep);
            if (parser.text[dot] == '[') {
                // capcom please stop cooking your spaghetti what do you mean System.Collections.Generic.Dictionary`2<app.GeneratorID,System.Collections.Generic.List`1[[app.EnvironmentLoadManager.LocalCellData, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]][]>
                parser.pos = dot;
                cls.Generic = ParseBracketed(ref parser, ctx);
                if (parser.text.Slice(parser.pos).StartsWith("[]")) {
                    cls.IsArray = true;
                    parser.pos += 2;
                    // array of array ...
                    if (parser.text.Slice(parser.pos).StartsWith("[]")) {
                        // TODO
                        parser.pos += 2;
                    }
                }
            } else {
                if (parser.text[dot] == '.') {
                    var reallyGenericSep = parser.text.IndexOfOffset('<', dot);
                    parser.pos = reallyGenericSep;
                    if (parser.Cur == '`') {
                        // nested generic within generic class, eugh
                        throw new NotImplementedException("Nested generics within generics are not yet supported");
                    }
                    var nestedName = parser.text[(dot + 1)..reallyGenericSep];
                    var realCls = new Classname(ctx) {
                        Parent = cls,
                        Namespace = cls.Namespace,
                        Name = nestedName.ToString(),
                    };
                    cls = realCls;
                }

                cls.Generic = ParsePlainGenericsList(ref parser, ctx);
            }
        }
        parser.parentClass = prevParent;
        return cls;
    }

    private static Classname[] ParsePlainGenericsList(ref StringParser parser, GeneratorContext ctx)
    {
        // input: `2<AISituation.IAISituationAgent,>
        // full: System.Collections.Generic.Dictionary`2<AISituation.IAISituationAgent,>
        // input: `1.Enumerator<app.AISituationObject>
        var lt = parser.text.IndexOfAnyOffset('<', '.', parser.pos);
        if (lt == -1) {
            throw new Exception("Oi");
        }
        if (parser.text[lt] == '.') {
            // example: `1.Enumerator<app.AISituationObject>
            // I don't think these have a way to write them out in c# source, so uhhhh
            parser.pos = parser.text.IndexOfOffset('>', lt) + 1;
            return Array.Empty<Classname>();
        }
        parser.pos = lt + 1;
        if (parser.Cur == '>') {
            // exception, like: System.Collections.Generic.Comparer`1<>, app.AIBlackBoardBase`1<>
            // let's just return empty here for now, or maybe replace with `object`
            parser.pos++;
            return Array.Empty<Classname>();
        }
        var list = new List<Classname>(1);

        int sep;
        do {
            list.Add(ParsePlainGenericSingle(ref parser, ctx));
            sep = parser.text.IndexOfAnyOffset(',', '>', parser.pos);
            if (sep == -1) {
                throw new Exception("Oii");
            }
            parser.pos = sep + 1;
        } while (parser.text[sep] != '>');

        return list.ToArray();
    }
    private static List<string> parents = new();

    private static void ParseBaseName(ref Classname cls, ref StringParser parser, int endIndex, GeneratorContext ctx)
    {
        var name = parser.text[parser.pos..endIndex];
        var segments = name.ToString().Split('.');
        parents.Clear();

        int nsParentSplitIndex = segments.Length - 1;
        cls.Name = segments[nsParentSplitIndex];
        while (nsParentSplitIndex > 0) {
            var joinedParentStr = string.Join('.', segments.Take(nsParentSplitIndex));
            if (ctx.classNames.ContainsKey(joinedParentStr)) {
                parents.Add(string.Join('.', segments.Take(nsParentSplitIndex)));
                nsParentSplitIndex--;
                continue;
            } else {
                cls.Namespace = joinedParentStr;
                break;
            }
        }
        if (parents.Count > 0) {
            var parent = new Classname(ctx) { Name = parents[0].Substring(parents[0].LastIndexOf('.') + 1), Namespace = cls.Namespace };
            cls.Parent = parent;
            int i = 1;
            while (i < parents.Count) {
                var nextParent = new Classname(ctx) { Name = parents[i].Substring(parents[i].LastIndexOf('.') + 1), Namespace = cls.Namespace };
                parent.Parent = nextParent;
                parent = nextParent;
                i++;
            }
        }
        if (cls.Name.StartsWith("!!")) {
            // method generics

            // if (parser.parentClass == null) {
            //     throw new Exception("Generic type parameter is supposed to have a containing class oi");
            // }
            int idx;
            if (cls.Name.EndsWith("[][]")) {
                // dmc5 template args end with &, skip that when parsing
                idx = int.Parse(cls.Name.EndsWith('&') ? cls.Name.AsSpan()[2..^5] : cls.Name.AsSpan()[2..^4]);
                cls.IsArray = true;
            } else if (cls.Name.EndsWith("[]")) {
                // dmc5 template args end with &, skip that when parsing
                idx = int.Parse(cls.Name.EndsWith('&') ? cls.Name.AsSpan()[2..^3] : cls.Name.AsSpan()[2..^2]);
                cls.IsArray = true;
            } else {
                idx = int.Parse(cls.Name.EndsWith('&') ? cls.Name.AsSpan()[2..^1] : cls.Name.AsSpan().Slice(2));
                cls.IsArray = false;
            }
            // cls = parser.parentClass.Generic![idx];
            cls.Name = "T" + idx;
        } else if (cls.Name.StartsWith('!')) {
            // class generics

            if (parser.containingClass == null) {
                throw new Exception("Generic type parameter is supposed to have a parent oi");
            }
            int idx;
            bool isArray;
            if (cls.Name.EndsWith("[][]")) {
                idx = int.Parse(cls.Name.EndsWith('&') ? cls.Name.AsSpan()[1..^5] : cls.Name.AsSpan()[1..^4]);
                isArray = true;
            } else if (cls.Name.EndsWith("[]")) {
                idx = int.Parse(cls.Name.EndsWith('&') ? cls.Name.AsSpan()[1..^3] : cls.Name.AsSpan()[1..^2]);
                isArray = true;
            } else {
                idx = int.Parse(cls.Name.EndsWith('&') ? cls.Name.AsSpan()[1..^1] : cls.Name.AsSpan().Slice(1));
                isArray = false;
            }
            var gen = parser.containingClass.InheritedGeneric;

            cls = gen != null && idx < gen.Length ? gen[idx].AsArray(isArray) : new Classname(ctx) { Name = "T" + idx, IsArray = isArray };
        }
        parser.pos = endIndex;
    }
    public static Classname? Parse(ref StringParser parser, GeneratorContext ctx, bool ignoreNonDefinitions)
    {
        var res = new Classname(ctx);
        var genericSeparator = parser.text.IndexOf('`');
        ParseBaseName(ref res, ref parser, genericSeparator == -1 ? parser.text.Length : genericSeparator, ctx);
        if (genericSeparator != -1) {
            if (parser.text.Length == 1023) {
                // edge case: the il2cpp dump doesn't print past 1023 length (or the game devs limited them I guess)
                // discard everything past whatever the last separator was... though we know there should've been at least one more param
                // but also there could be more, perhaps if we automatically append them when we see a member trying to access an unknown generic arg?
                parser = new StringParser(parser.text.Slice(0, parser.text.LastIndexOf("],[") + 1).ToString() + "]");
            }
            var genericSep = parser.text.IndexOfAnyOffset('[', '<', '.', genericSeparator);
            if (genericSep == -1) {
                throw new Exception("Oi how is this not there!!!!");
            }

            if (parser.text[genericSep] == '.') {
                var reallyGenericSep = parser.text.IndexOfAnyOffset('`', '[', '<', genericSep);
                parser.pos = reallyGenericSep;
                if (parser.Cur == '`') {
                    // nested generic within generic class, eugh
                    return null;
                }
                // handle these properly (nested types within generic classes)
                // "System.Collections.Generic.Dictionary`2.Entry
                //   [[System.Int32, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null],[app.QuestDeliverManager.Context, System, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]][]"

                var nestedName = parser.text[(genericSep + 1)..reallyGenericSep];
                var parent = res;
                res = new Classname(ctx);
                res.Name = nestedName.ToString();
                res.Parent = parent;
                res.Namespace = parent.Namespace;

                if (parser.Cur == '[') {
                    res.Parent.Generic = ParseBracketed(ref parser, ctx);
                } else {
                    if (ignoreNonDefinitions) {
                        return null;
                    }
                    res.Parent.Generic = ParsePlainGenericsList(ref parser, ctx);
                }
                // res.Parent.Generic =

                // res.Generic = Array.Empty<Classname>();
                // var i1 = parser.text.LastIndexOf("]]") + 2;
                // if (i1 < genericSeparator) i1 = parser.text.LastIndexOf('>') + 1;
                // parser.pos = i1;
            } else if (parser.text[genericSep] == '[') {
                res.Generic = ParseBracketed(ref parser, ctx);
            } else {
                if (ignoreNonDefinitions) {
                    return null;
                }
                res.Generic = ParsePlainGenericsList(ref parser, ctx);
            }
        }
        if (parser.pos < parser.text.Length) {
            if (parser.text.Slice(parser.pos).StartsWith("[]")) {
                res.IsArray = true;
                parser.pos += 2;
                // TODO array of array
                if (parser.text.Slice(parser.pos).StartsWith("[]")) {
                    parser.pos += 2;
                }
            }
        }

        return res;
    }
    public static Classname? Parse(string name, GeneratorContext ctx, Classname? containingClass = null, bool ignoreNonDefinitions = false)
    {
        if (name.Contains("c__DisplayClass") || name.Contains(".<>c")) {
            // compiler-generated lambda thingies, just return them as is
            return new Classname(ctx) { Name = name };
        }
        var parser = new StringParser(name.AsSpan());
        parser.containingClass = containingClass;
        var cls = Parse(ref parser, ctx, ignoreNonDefinitions);
        if (parser.pos < parser.text.Length) {
            // throw new Exception("Expected EOF");
            return null;
        }
        return cls;
    }
}
