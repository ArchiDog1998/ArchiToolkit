using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using SyntaxExtensions = ArchiToolkit.RoslynHelper.Extensions.SyntaxExtensions;

namespace ArchiToolkit.CppInteropGen.SourceGenerator;

using static SyntaxExtensions;
using static SyntaxFactory;

public class CppClassGenerator
{
    private readonly string _className;
    private readonly string _dllName = string.Empty;
    private readonly Config _config;
    private readonly IReadOnlyList<string> _fields;
    private readonly IReadOnlyList<CMethodGenerator> _methods;

    public CppClassGenerator(SourceText text, string className, Config config)
    {
        _config = config;
        _className = className;
        List<string> fields = new(4);
        List<CMethodGenerator> methods = [];
        StringBuilder? builder = null;

        foreach (var line in text.Lines)
        {
            var lineText = line.ToString().Trim();
            if (IsStartWith(ref lineText, "//STRUCT:"))
            {
                fields.Add(lineText.Trim());
            }
            else if (IsStartWith(ref lineText, "//DLL_NAME:"))
            {
                _dllName = lineText.Trim();
            }
            else if (IsStartWith(ref lineText, "CSHARP_WRAPPER("))
            {
                if (builder is not null) CheckBuilder(builder);
                builder = new StringBuilder(lineText.Trim());
            }
            else if (builder is not null)
            {
                builder.Append(lineText.Trim());
            }
        }

        if (builder is not null) CheckBuilder(builder);
        _fields = fields;
        _methods = methods;
        return;

        void CheckBuilder(StringBuilder stringBuilder)
        {
            var str = stringBuilder.ToString();
            var privateIt = str.Contains("//PRIVATE");
            var index = str.IndexOf('{');
            if (index < 0) return;
            var methodDeclare = str.Substring(0, index);
            methods.Add(new CMethodGenerator(methodDeclare, className, privateIt));
        }
    }

    private static bool IsStartWith(ref string content, string start)
    {
        if (!content.StartsWith(start)) return false;
        content = content.Substring(start.Length).Trim();
        return true;
    }

    public ClassDeclarationSyntax Generate()
    {
        IEnumerable<MemberDeclarationSyntax> members =
        [
            FieldDeclaration(VariableDeclaration(PointerType(IdentifierName("Data")))
                    .WithVariables([
                        VariableDeclarator(Identifier("Ptr"))
                    ]))
                .WithAttributeLists([GeneratedCodeAttribute(typeof(CppClassGenerator))])
                .WithModifiers(
                    TokenList(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.ReadOnlyKeyword))),

            StructDeclaration("Data")
                .WithAttributeLists(
                [
                    _fields.Any()
                        ? GeneratedCodeAttribute(typeof(CppClassGenerator))
                            .AddAttributes(Attribute(
                                    IdentifierName("global::System.Runtime.InteropServices.StructLayout"))
                                .WithArgumentList(AttributeArgumentList(
                                [
                                    AttributeArgument(MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("global::System.Runtime.InteropServices.LayoutKind"),
                                        IdentifierName("Sequential")))
                                ])))
                        : GeneratedCodeAttribute(typeof(CppClassGenerator))
                ])
                .WithModifiers(TokenList(
                    Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.PartialKeyword)))
                .WithMembers([
                    .._fields.Select(f => ParseMemberDeclaration(f.Trim().TrimEnd(';') + ";"))
                        .OfType<MemberDeclarationSyntax>()
                        .Select(m => m
                            .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword)))
                            .WithAttributeLists([GeneratedCodeAttribute(typeof(CppClassGenerator))]))
                ]),

            ConstructorDeclaration(Identifier(_className))
                .WithModifiers(TokenList(Token(SyntaxKind.InternalKeyword)))
                .WithAttributeLists([GeneratedCodeAttribute(typeof(CMethodGenerator))])
                .WithParameterList(ParameterList(
                [
                    Parameter(Identifier("ptr"))
                        .WithType(PointerType(IdentifierName("Data")))
                ]))
                .WithInitializer(ConstructorInitializer(SyntaxKind.BaseConstructorInitializer,
                    ArgumentList(
                    [
                        Argument(LiteralExpression(SyntaxKind.TrueLiteralExpression))
                    ])))
                .WithBody(Block(
                    ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                        IdentifierName("Ptr"), IdentifierName("ptr")))))
        ];
        if (!string.IsNullOrEmpty(_dllName))
            members = members.Append(
                PropertyDeclaration(PredefinedType(Token(SyntaxKind.StringKeyword)), Identifier("DllName"))
                    .WithAttributeLists([GeneratedCodeAttribute(typeof(CppClassGenerator))])
                    .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithExpressionBody(ArrowExpressionClause(LiteralExpression(SyntaxKind.StringLiteralExpression,
                        Literal(_dllName))))
                    .WithSemicolonToken(Token(SyntaxKind.SemicolonToken)));

        return ClassDeclaration(_className)
            .WithModifiers(
                TokenList(Token(_config.IsInternal ? SyntaxKind.InternalKeyword : SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.SealedKeyword), Token(SyntaxKind.UnsafeKeyword), Token(SyntaxKind.PartialKeyword)))
            .WithAttributeLists([GeneratedCodeAttribute(typeof(CppClassGenerator))])
            .WithBaseList(BaseList(
            [
                SimpleBaseType(IdentifierName("global::ArchiToolkit.CppInteropGen.CppObject"))
            ]))
            .WithMembers(
            [
                ..members,
                .._methods.Select(p => p.Generate())
            ]);
    }

    private class CMethodGenerator
    {
        private readonly string _className;
        private readonly string _csMethodName;
        private readonly bool _isPrivate;
        private readonly string _methodName;
        private readonly IReadOnlyList<ParameterGenerator> _parameters;

        public CMethodGenerator(string methodDeclare, string className, bool isPrivate)
        {
            _className = className;
            var startIndex = methodDeclare.IndexOf('(');
            _methodName = methodDeclare.Substring(0, startIndex);
            _csMethodName = _methodName.Substring(className.Length).Split('_').First(x => !string.IsNullOrEmpty(x));
            var endIndex = methodDeclare.LastIndexOf(')');

            _parameters = methodDeclare.Substring(startIndex + 1, endIndex - startIndex - 1)
                .Split(',')
                .Select(x => new ParameterGenerator(x))
                .ToArray();
            _isPrivate = isPrivate || _parameters.Any(p => p.HasHandle);
        }

        public MemberDeclarationSyntax Generate()
        {
            if (_methodName.Contains("Create", StringComparison.InvariantCultureIgnoreCase))
                return ConstructorDeclaration(_className)
                    .WithModifiers(TokenList(Token(_isPrivate ? SyntaxKind.PrivateKeyword : SyntaxKind.PublicKeyword)))
                    .WithAttributeLists([GeneratedCodeAttribute(typeof(CMethodGenerator))])
                    .WithParameterList(ParameterList(
                    [
                        .._parameters.Take(_parameters.Count - 1).Select(p => p.GenerateParameter())
                    ]))
                    .WithBody(Block(
                        GetRunMethod(Block(
                            GetFunctionPointer(_parameters.Select(p => p.GenerateFunctionPointer())),
                            FixedStatement(VariableDeclaration(PointerType(PointerType(IdentifierName("Data"))))
                                    .WithVariables([
                                        VariableDeclarator(Identifier("ptr"))
                                            .WithInitializer(EqualsValueClause(PrefixUnaryExpression(
                                                SyntaxKind.AddressOfExpression, IdentifierName("Ptr"))))
                                    ]),
                                ReturnStatement(InvocationExpression(IdentifierName("method"))
                                    .WithArgumentList(ArgumentList(
                                    [
                                        .._parameters.Take(_parameters.Count - 1).Select(p => p.GenerateArgument()),
                                        Argument(IdentifierName("ptr"))
                                    ]))))
                        ))));

            if (_methodName.Contains("Delete", StringComparison.InvariantCultureIgnoreCase))
                return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier("Delete"))
                    .WithModifiers(TokenList(Token(SyntaxKind.ProtectedKeyword), Token(SyntaxKind.OverrideKeyword)))
                    .WithAttributeLists([GeneratedCodeAttribute(typeof(CMethodGenerator))])
                    .WithBody(Block(
                        GetRunMethod(Block(
                            GetFunctionPointer(_parameters.Select(p => p.GenerateFunctionPointer())),
                            ReturnStatement(InvocationExpression(IdentifierName("method"))
                                .WithArgumentList(ArgumentList(
                                [
                                    Argument(IdentifierName("Ptr"))
                                ])))
                        ))));

            return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)),
                    Identifier(_isPrivate ? "_" + _csMethodName : _csMethodName))
                .WithModifiers(TokenList(Token(_isPrivate ? SyntaxKind.PrivateKeyword : SyntaxKind.PublicKeyword)))
                .WithAttributeLists([GeneratedCodeAttribute(typeof(CMethodGenerator))])
                .WithParameterList(ParameterList(
                [
                    .._parameters.Skip(1).Select(p => p.GenerateParameter())
                ]))
                .WithBody(Block((StatementSyntax[])
                [
                    .._parameters.Where(p => p.IsRefOrOut).SelectMany(p => p.GenerateLocalDeclaration()),
                    GetRunMethod(Block(
                        GetFunctionPointer(_parameters.Select(p => p.GenerateFunctionPointer())),
                        ReturnStatement(InvocationExpression(IdentifierName("method"))
                            .WithArgumentList(ArgumentList(
                            [
                                Argument(IdentifierName("Ptr")),
                                .._parameters.Skip(1).Select(p => p.GenerateArgument())
                            ]))))),
                    .._parameters.Where(p => p.IsRefOrOut).Select(p => p.GenerateAssignment())
                ]));

            ExpressionStatementSyntax GetRunMethod(BlockSyntax block)
            {
                return ExpressionStatement(InvocationExpression(IdentifierName("SafeRun"))
                    .WithArgumentList(ArgumentList(
                    [
                        Argument(SimpleLambdaExpression(Parameter(Identifier("loader")))
                            .WithBlock(block))
                    ])));
            }

            LocalDeclarationStatementSyntax GetFunctionPointer(IEnumerable<FunctionPointerParameterSyntax> parameters)
            {
                return LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                    .WithVariables(
                    [
                        VariableDeclarator(Identifier("method")).WithInitializer(EqualsValueClause(
                            CastExpression(FunctionPointerType().WithCallingConvention(
                                        FunctionPointerCallingConvention(Token(SyntaxKind.UnmanagedKeyword))
                                            .WithUnmanagedCallingConventionList(
                                                FunctionPointerUnmanagedCallingConventionList(
                                                [
                                                    FunctionPointerUnmanagedCallingConvention(
                                                        Identifier("Cdecl"))
                                                ])))
                                    .WithParameterList(FunctionPointerParameterList(
                                    [
                                        ..parameters,
                                        FunctionPointerParameter(IdentifierName("global::System.IntPtr"))
                                    ])),
                                InvocationExpression(MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("loader"), IdentifierName("GetFunctionPointer")))
                                    .WithArgumentList(ArgumentList(
                                    [
                                        Argument(LiteralExpression(SyntaxKind.StringLiteralExpression,
                                            Literal(_methodName)))
                                    ])))))
                    ]));
            }
        }

        private class ParameterGenerator
        {
            private readonly string _name;
            private readonly int _ptrCount;
            private readonly string _rawTypeName;
            private readonly ParameterType _type;

            public ParameterGenerator(string parameterString)
            {
                var cleanedString = parameterString
                    .Replace("const", string.Empty)
                    .Replace('&', '*')
                    .Trim();
                _ptrCount = cleanedString.Count(c => c == '*');
                var beautifulStrings = cleanedString
                    .Replace("*", "")
                    .Split(' ');
                var typeName = beautifulStrings.First(s => !string.IsNullOrEmpty(s));
                _name = beautifulStrings.Last(s => !string.IsNullOrEmpty(s));
                _rawTypeName = GetTypeName();
                _type = CheckType();
                return;

                string GetTypeName()
                {
                    return typeName switch
                    {
                        // 整型
                        "char" => "sbyte", // 通常为 signed char
                        "signed char" => "sbyte",
                        "unsigned char" => "byte",
                        "short" or "short int" or "signed short" or "signed short int" => "short",
                        "unsigned short" or "unsigned short int" => "ushort",
                        "int" or "signed int" or "signed" or "long" => "int", // 注意 C++ 中 long == int（Windows）
                        "unsigned" or "unsigned int" => "uint",
                        "long long" or "signed long long" => "long",
                        "unsigned long long" => "ulong",

                        // 特殊整数类型
                        "size_t" => "global::System.UIntPtr",
                        "ptrdiff_t" => "global::System.IntPtr",

                        // 字符
                        "wchar_t" or "char16_t" => "char", // 注意 C++ wchar_t 通常是 UTF-16 或 UTF-32，平台相关
                        "char32_t" => "int", // 可考虑使用 Rune 或自定义结构封装 Unicode Code Point

                        // 浮点数
                        "float" => "float",
                        "double" => "double",
                        "long double" => "double", // C# 没有 long double，通常映射为 double

                        // 布尔
                        "bool" => "bool",

                        // void 类型
                        "void" => "void",
                        _ => typeName + ".Data"
                    };
                }

                ParameterType CheckType()
                {
                    var hasConst = parameterString.Contains("const");
                    var hasRef = parameterString.Contains('&');
                    if (hasConst || _name is "self") return ParameterType.None;

                    return hasRef ? ParameterType.Out : ParameterType.Ref;
                }
            }

            public bool HasHandle => _name is not "self" and not "handle"
                                     && MethodTypeName.Contains('*');

            public bool IsRefOrOut => _type is ParameterType.Ref or ParameterType.Out;

            private string TypeNameWithPointer => _type switch
            {
                ParameterType.Ref or ParameterType.Out => _rawTypeName + new string('*', _ptrCount - 1),
                _ => _rawTypeName + new string('*', _ptrCount)
            };

            private string MethodTypeName
            {
                get
                {
                    var s = TypeNameWithPointer;
                    return s.EndsWith(".Data*") ? s.Substring(0, s.Length - 6) : s;
                }
            }

            private bool IsData => TypeNameWithPointer.EndsWith(".Data*");

            public ParameterSyntax GenerateParameter()
            {
                var parameter = Parameter(Identifier(_name));
                if (_ptrCount is 0) return parameter.WithType(IdentifierName(_rawTypeName));

                return _type switch
                {
                    ParameterType.Ref => parameter.WithModifiers(TokenList(Token(SyntaxKind.RefKeyword)))
                        .WithType(IdentifierName(MethodTypeName)),
                    ParameterType.Out => parameter.WithModifiers(TokenList(Token(SyntaxKind.OutKeyword)))
                        .WithType(IdentifierName(MethodTypeName)),
                    _ => parameter.WithType(IdentifierName(MethodTypeName))
                };
            }

            public FunctionPointerParameterSyntax GenerateFunctionPointer()
            {
                return FunctionPointerParameter(IdentifierName(_rawTypeName + new string('*', _ptrCount)));
            }

            public ArgumentSyntax GenerateArgument()
            {
                var name = IsData && _type is not ParameterType.Out ? _name + ".Ptr" : _name;
                if (_type is ParameterType.Out or ParameterType.Ref) name = "__" + name;
                return Argument(IdentifierName(name));
            }

            public IEnumerable<LocalDeclarationStatementSyntax> GenerateLocalDeclaration()
            {
                if (_type is ParameterType.Ref)
                    yield return LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                        .WithVariables(
                        [
                            VariableDeclarator(Identifier("_" + _name))
                                .WithInitializer(EqualsValueClause(IdentifierName(_name)))
                        ]));
                else
                    yield return LocalDeclarationStatement(
                        VariableDeclaration(IdentifierName(_rawTypeName + new string('*', _ptrCount - 1)))
                            .WithVariables(
                            [
                                VariableDeclarator(Identifier("_" + _name))
                                    .WithInitializer(EqualsValueClause(LiteralExpression(
                                        SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))))
                            ]));

                yield return LocalDeclarationStatement(VariableDeclaration(IdentifierName("var"))
                    .WithVariables(
                    [
                        VariableDeclarator(Identifier("__" + _name))
                            .WithInitializer(EqualsValueClause(PrefixUnaryExpression(
                                SyntaxKind.AddressOfExpression, IdentifierName("_" + _name))))
                    ]));
            }

            public ExpressionStatementSyntax GenerateAssignment()
            {
                return ExpressionStatement(AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    IdentifierName(_name),
                    IsData
                        ? ImplicitObjectCreationExpression()
                            .WithArgumentList(ArgumentList(
                            [
                                Argument(IdentifierName("_" + _name))
                            ]))
                        : IdentifierName("_" + _name)));
            }

            private enum ParameterType : byte
            {
                None,
                Ref,
                Out
            }
        }
    }
}