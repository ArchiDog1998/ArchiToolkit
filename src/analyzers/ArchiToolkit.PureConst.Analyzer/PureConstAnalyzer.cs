using ArchiToolkit.RoslynHelper.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ArchiToolkit.PureConst.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PureConstAnalyzer : BaseAnalyzer
{
    protected override IReadOnlyList<DescriptorType> Descriptors { get; }
        = [DescriptorType.CantUseOnAccessor];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeMethodSyntax, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeAccessorSyntax, SyntaxKind.GetAccessorDeclaration);
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

    private static void AnalyzeAccessorSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not AccessorDeclarationSyntax node) return;
        var body = node.Body as SyntaxNode ?? node.ExpressionBody;
        if (body is null) return;
        // var diagnostic = Diagnostic.Create(NoAccessor,
        //     context.Node.GetLocation(),
        //     context.Node.GetType());
        // context.ReportDiagnostic(diagnostic);
    }


    private static void AnalyzeMethodSyntax(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax node) return;
        var body = node.Body as SyntaxNode ?? node.ExpressionBody;
        if (body is null) return;

        foreach (var subNode in body.DescendantNodes())
        {
            //context.Report(DescriptorType.CantUseOnAccessor, subNode.GetLocation());
        }
    }

    private static void AnalyzeLocalFunction(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not LocalFunctionStatementSyntax node) return;
        var body = node.Body as SyntaxNode ?? node.ExpressionBody;
        if (body is null) return;
    }


    private static AttributeSyntax[] ConstAttributes(IEnumerable<AttributeListSyntax> attributeLists,
        SemanticModel model)
    {
        return attributeLists.SelectMany(l => l.Attributes).Where(
                attr =>
                    model.GetTypeInfo(attr).Type?.GetName().FullName is "global::ArchiToolkit.PureConst.ConstAttribute")
            .ToArray();
    }

    private static IReadOnlyList<SimpleNameSyntax> GetFirstAccessorName(SyntaxNodeAnalysisContext context,
        ExpressionSyntax exp)
    {
        while (true)
        {
            switch (exp)
            {
                case ConditionalAccessExpressionSyntax conditional:
                    exp = conditional.Expression;
                    break;
                case MemberAccessExpressionSyntax member:
                    exp = member.Expression;
                    break;
                case AwaitExpressionSyntax await:
                    exp = await.Expression;
                    break;
                case InvocationExpressionSyntax invocation:
                    exp = invocation.Expression;
                    break;
                case ParenthesizedExpressionSyntax parenthesized:
                    exp = parenthesized.Expression;
                    break;
                case ElementAccessExpressionSyntax elementAccess: //TODO: arguments?
                    exp = elementAccess.Expression;
                    break;

                case TupleExpressionSyntax tuple:
                    return
                    [
                        ..tuple.Arguments.SelectMany(a => GetFirstAccessorName(context, a.Expression))
                    ];

                case ThisExpressionSyntax:
                case BaseExpressionSyntax:
                    if (exp.Parent is MemberAccessExpressionSyntax m)
                    {
                        exp = m.Name;
                    }
                    else
                    {
                        return [];
                    }

                    break;

                case SimpleNameSyntax name:
                    return [name];

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
                    //context.ReportCantFind(exp);
                    return [];
            }
        }
    }
}