using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ArchiToolkit.PureConst.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PureConstAnalyzer : BaseAnalyzer
{
    protected override IReadOnlyList<DescriptorType> Descriptors { get; } =
    [
        DescriptorType.CantUseOnAccessor,

        DescriptorType.FieldConstMethod,
        DescriptorType.PropertyConstMethod,
        DescriptorType.MethodConstMethod,
        DescriptorType.VariableConstMethod,
        DescriptorType.FieldConstMethodWarning,
        DescriptorType.PropertyConstMethodWarning,
        DescriptorType.MethodConstMethodWarning,

        DescriptorType.CheckingSymbol,
        DescriptorType.CantFindSymbol,
        DescriptorType.AdditionalVariable,
    ];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeMethodSyntax, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeGetAccessorSyntax, SyntaxKind.GetAccessorDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeLocalFunction, SyntaxKind.LocalFunctionStatement);

        context.RegisterSyntaxNodeAction(NoAccessorAnalyze,
            SyntaxKind.SetAccessorDeclaration,
            SyntaxKind.GetAccessorDeclaration,
            SyntaxKind.InitAccessorDeclaration,
            SyntaxKind.AddAccessorDeclaration,
            SyntaxKind.RemoveAccessorDeclaration,
            SyntaxKind.UnknownAccessorDeclaration);
    }

    private static void NoAccessorAnalyze(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AccessorDeclarationSyntax accessorSyntax) return;
        var constAttributes = ConstAttributes(accessorSyntax.AttributeLists, context.SemanticModel);
        foreach (var attr in constAttributes)
        {
            context.Report(DescriptorType.CantUseOnAccessor, attr.GetLocation());
        }
    }

    private static void AnalyzeGetAccessorSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AccessorDeclarationSyntax node) return;
        var body = node.Body as SyntaxNode ?? node.ExpressionBody;
        AnalyzeBody(context, body, true);
    }

    private static void AnalyzeMethodSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax node) return;
        var body = node.Body as SyntaxNode ?? node.ExpressionBody;
        AnalyzeBody(context, body, node.AttributeLists, node.ParameterList);
    }

    private static void AnalyzeLocalFunction(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not LocalFunctionStatementSyntax node) return;
        var body = node.Body as SyntaxNode ?? node.ExpressionBody;
        AnalyzeBody(context, body, node.AttributeLists, node.ParameterList);
    }

    private static void AnalyzeBody(SyntaxNodeAnalysisContext context, SyntaxNode? body,
        SyntaxList<AttributeListSyntax> attributeLists,
        ParameterListSyntax parameterList)
    {
        var model = context.SemanticModel;
        var isPure = IsPure(attributeLists, model);
        var isConst = ConstAttributes(attributeLists, model).Any() || isPure;
        var parameters = parameterList.Parameters
            .Where(param => isPure || ConstAttributes(param.AttributeLists, model).Any())
            .Select(p => model.GetDeclaredSymbol(p))
            .OfType<IParameterSymbol>();
        AnalyzeBody(context, body, isConst, [..parameters]);
    }

    private static void AnalyzeBody(SyntaxNodeAnalysisContext context, SyntaxNode? body, bool isConstMethod,
        params IParameterSymbol[] parameters)
    {
        if (body is null) return;
        var subNodes = body.DescendantNodes().ToArray();
        var locals = GetLocals(context, subNodes, parameters).ToArray();
        ISymbol[] constSymbols = [..locals, ..parameters];

        foreach (var itemNode in subNodes.OfType<AssignmentExpressionSyntax>().Select(s => s.Left)
                     .Concat(subNodes.OfType<PostfixUnaryExpressionSyntax>().Select(s => s.Operand))
                     .Concat(subNodes.OfType<PrefixUnaryExpressionSyntax>().Select(s => s.Operand)))
        {
            foreach (var name in GetFirstAccessorName(context, itemNode))
            {
                var s = context.SemanticModel.GetSymbolInfo(name).Symbol;
                context.Report(DescriptorType.CheckingSymbol, name.GetLocation(), name);
                switch (s)
                {
                    case ILocalSymbol when constSymbols.Contains(s, SymbolEqualityComparer.Default):
                    case IParameterSymbol when constSymbols.Contains(s, SymbolEqualityComparer.Default):
                        context.Report(DescriptorType.VariableConstMethod, itemNode.GetLocation(), name);
                        break;
                }
            }
        }

        foreach (var itemNode in subNodes.OfType<InvocationExpressionSyntax>().Select(s => s.Expression))
        {
            if (context.SemanticModel.GetSymbolInfo(itemNode).Symbol is not IMethodSymbol methodSymbol) continue;
            if (methodSymbol.GetAttributes().Any(a =>
                    a.AttributeClass?.GetName().FullName is "global::ArchiToolkit.PureConst.ConstAttribute" or
                        "global::System.Diagnostics.Contracts.PureAttribute")) continue;

            var isLocalDefined = methodSymbol.ContainingAssembly.Equals(context.Compilation.Assembly, SymbolEqualityComparer.Default);

            foreach (var name in GetFirstAccessorName(context, itemNode))
            {
                var s = context.SemanticModel.GetSymbolInfo(name).Symbol;
                context.Report(DescriptorType.CheckingSymbol, name.GetLocation(), name);
                switch (s)
                {
                    case IFieldSymbol when isConstMethod:
                        context.Report(isLocalDefined ? DescriptorType.FieldConstMethod : DescriptorType.FieldConstMethodWarning,
                            itemNode.GetLocation(), name);
                        break;
                    case IPropertySymbol when isConstMethod:
                        context.Report(isLocalDefined ? DescriptorType.PropertyConstMethod : DescriptorType.PropertyConstMethodWarning,
                            itemNode.GetLocation(), name);
                        break;
                    case IMethodSymbol when isConstMethod:
                        context.Report(isLocalDefined ? DescriptorType.MethodConstMethod : DescriptorType.MethodConstMethodWarning,
                            itemNode.GetLocation(), name);
                        break;
                    case ILocalSymbol when constSymbols.Contains(s, SymbolEqualityComparer.Default):
                    case IParameterSymbol when constSymbols.Contains(s, SymbolEqualityComparer.Default):
                        context.Report(isLocalDefined ? DescriptorType.VariableConstMethod : DescriptorType.VariableConstMethodWarning,
                            itemNode.GetLocation(), name);
                        break;
                }
            }
        }
    }

    private static IEnumerable<ILocalSymbol> GetLocals(SyntaxNodeAnalysisContext context, SyntaxNode[] nodes,
        IParameterSymbol[] parameters)
    {
        var referenceSymbols = parameters.Where(s => s.Type.IsReferenceType).ToList<ISymbol>();

        foreach (var node in nodes.OfType<LocalDeclarationStatementSyntax>())
        {
            var hasConst = node.GetTrailingTrivia()
                .Any(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia) && t.ToString().Contains(".const"));
            foreach (var variable in node.Declaration.Variables)
            {
                if (context.SemanticModel.GetDeclaredSymbol(variable) is not ILocalSymbol localSymbol) continue;
                var isConst = hasConst || (variable.Initializer?.Value is { } expression &&
                                           GetMemberAccess(context, expression).Any(accessor =>
                                           {
                                               var initSymbol = context.SemanticModel.GetSymbolInfo(accessor).Symbol;
                                               if (initSymbol
                                                   is not (IPropertySymbol { Type.IsReferenceType: true }
                                                   or IFieldSymbol { Type.IsReferenceType: true }
                                                   or IParameterSymbol { Type.IsReferenceType: true }
                                                   or ILocalSymbol { Type.IsReferenceType: true }))
                                                   return false;
                                               return GetFirstAccessorName(context, accessor)
                                                   .Any(name => referenceSymbols.Contains(
                                                       context.SemanticModel.GetSymbolInfo(name).Symbol,
                                                       SymbolEqualityComparer.Default));
                                           }));
                if (!isConst) continue;
                if (localSymbol.Type.IsReferenceType)
                {
                    referenceSymbols.Add(localSymbol);
                }

                yield return localSymbol;
                context.Report(DescriptorType.AdditionalVariable, variable.Identifier.GetLocation(), localSymbol.Name);
            }
        }
    }

    private static AttributeSyntax[] ConstAttributes(IEnumerable<AttributeListSyntax> attributeLists,
        SemanticModel model)
    {
        return attributeLists.SelectMany(l => l.Attributes).Where(
                attr =>
                    model.GetTypeInfo(attr).Type?.GetName().FullName is "global::ArchiToolkit.PureConst.ConstAttribute")
            .ToArray();
    }

    private static bool IsPure(IEnumerable<AttributeListSyntax> attributeLists,
        SemanticModel model)
    {
        return attributeLists.SelectMany(l => l.Attributes).Any(attr =>
            model.GetTypeInfo(attr).Type?.GetName().FullName is "global::System.Diagnostics.Contracts.PureAttribute");
    }

    private static IReadOnlyList<ExpressionSyntax> GetMemberAccess(SyntaxNodeAnalysisContext context,
        ExpressionSyntax expression)
    {
        while (true)
        {
            switch (expression)
            {
                case MemberAccessExpressionSyntax member:
                    return [member];
                case SimpleNameSyntax name:
                    return [name];

                case ConditionalAccessExpressionSyntax conditional:
                    expression = conditional.Expression;
                    break;

                case AwaitExpressionSyntax await:
                    expression = await.Expression;
                    break;

                case ParenthesizedExpressionSyntax parenthesized:
                    expression = parenthesized.Expression;
                    break;
                case ElementAccessExpressionSyntax elementAccess: //TODO: arguments?
                    expression = elementAccess.Expression;
                    break;

                // TODO: Tuple or sth like this.
                // case TupleExpressionSyntax tuple:
                //     return
                //     [
                //         ..tuple.Arguments.SelectMany(a => GetMemberAccess(context, a.Expression))
                //     ];

                default:
                    //context.Report(DescriptorType.CantFindSymbol, expression.GetLocation());
                    return [];
            }
        }
    }

    private static IReadOnlyList<SimpleNameSyntax> GetFirstAccessorName(SyntaxNodeAnalysisContext context,
        ExpressionSyntax expression)
    {
        while (true)
        {
            switch (expression)
            {
                case ConditionalAccessExpressionSyntax conditional:
                    expression = conditional.Expression;
                    break;
                case MemberAccessExpressionSyntax member:
                    expression = member.Expression;
                    break;
                case AwaitExpressionSyntax await:
                    expression = await.Expression;
                    break;
                case ParenthesizedExpressionSyntax parenthesized:
                    expression = parenthesized.Expression;
                    break;
                case ElementAccessExpressionSyntax elementAccess: //TODO: arguments?
                    expression = elementAccess.Expression;
                    break;

                case TupleExpressionSyntax tuple:
                    return
                    [
                        ..tuple.Arguments.SelectMany(a => GetFirstAccessorName(context, a.Expression))
                    ];

                case ThisExpressionSyntax:
                case BaseExpressionSyntax:
                    if (expression.Parent is MemberAccessExpressionSyntax m)
                    {
                        expression = m.Name;
                    }
                    else
                    {
                        return [];
                    }

                    break;

                case SimpleNameSyntax name:
                    return [name];

                case InvocationExpressionSyntax:
                case DeclarationExpressionSyntax:
                case PredefinedTypeSyntax:
                case TypeOfExpressionSyntax:
                case QueryExpressionSyntax:
                case LiteralExpressionSyntax:
                case MemberBindingExpressionSyntax: //TODO: Shall we do sth with it?
                case BinaryExpressionSyntax:
                case BaseObjectCreationExpressionSyntax:
                case AnonymousObjectCreationExpressionSyntax:
                    return [];

                default:
                    context.Report(DescriptorType.CantFindSymbol, expression.GetLocation());
                    return [];
            }
        }
    }
}