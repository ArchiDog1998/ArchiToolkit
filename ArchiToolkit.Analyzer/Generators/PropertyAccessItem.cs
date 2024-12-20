﻿using System.Collections.Immutable;
using ArchiToolkit.Analyzer.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ArchiToolkit.Analyzer.Generators;

public class PropertyAccessItemComparer : IEqualityComparer<PropertyAccessItem>
{
    public bool Equals(PropertyAccessItem x, PropertyAccessItem y)
    {
        if (x.ValidPropertySymbols.Count != y.ValidPropertySymbols.Count) return false;

        return !x.ValidPropertySymbols
            .Where((t, i) => !t.Equals(y.ValidPropertySymbols[i], SymbolEqualityComparer.Default))
            .Any();
    }

    public int GetHashCode(PropertyAccessItem obj)
    {
        return 0;
    }
}

public readonly struct PropertyAccessItem(ExpressionSyntax expression, SemanticModel model)
{
    private readonly Lazy<IReadOnlyList<IPropertySymbol>> _list = new(() => GetPropertySymbols(expression, model));

    public ExpressionSyntax Expression { get; } = expression;

    private string InitName => "Init_" + string.Join("_", ValidPropertySymbols.Select(i => i.Name.ToString()));

    public IReadOnlyList<IPropertySymbol> PropertySymbols => _list.Value;

    private static IReadOnlyList<IPropertySymbol> GetPropertySymbols(ExpressionSyntax expression, SemanticModel model)
    {
        var symbol = model.GetSymbolInfo(expression).Symbol as IPropertySymbol;
        if (expression is MemberAccessExpressionSyntax member)
        {
            return symbol is null
                ? GetPropertySymbols(member.Expression, model)
                : [..GetPropertySymbols(member.Expression, model), symbol];
        }

        return symbol is null ? [] : [symbol];
    }

    public bool HasSymbol(IPropertySymbol symbol)
        => PropertySymbols.Any(s => s.Equals(symbol, SymbolEqualityComparer.Default));

    public IReadOnlyList<IPropertySymbol> ValidPropertySymbols =>
        PropertySymbols.TakeWhile(IsPropDpProperty).ToImmutableArray();

    public StatementSyntax InvokeInit() => ExpressionStatement(InvocationExpression(IdentifierName(InitName)));

    public LocalFunctionStatementSyntax CreateInit(string clearName)
    {
        return LocalFunctionStatement(
                PredefinedType(
                    Token(SyntaxKind.VoidKeyword)),
                Identifier(InitName))
            .WithBody(Block(CreateStatements(clearName)));
    }

    private IEnumerable<StatementSyntax> CreateStatements(string clearName)
    {
        IEnumerable<StatementSyntax> result = [];
        var symbols = ValidPropertySymbols.ToImmutableArray();
        if (symbols.Length == 0) return result;

        var firstPropertySymbol = PropertySymbols[0];
        var name = new PropDpName(firstPropertySymbol.Name);
        var i = 0;

        List<IPropertySymbol> addedSymbols = [];
        result = result.Concat(AddStatements(addedSymbols, name, i)).Append(ReturnStatement());
        addedSymbols.Add(firstPropertySymbol);

        for (; i < symbols.Length; i++)
        {
            IEnumerable<StatementSyntax> changingStates = [];
            IEnumerable<StatementSyntax> changedStates =
            [
                ExpressionStatement(
                    InvocationExpression(
                        IdentifierName(clearName)))
            ];

            var index = i + 1;
            if (index < symbols.Length)
            {
                var symbol = symbols[index];
                var symbolName = new PropDpName(symbol.Name);

                changingStates = changingStates.Concat(NullReturn(addedSymbols))
                    .Concat(RemoveStatements(addedSymbols, symbolName, index));
                changedStates = changedStates.Concat(NullReturn(addedSymbols))
                    .Concat(AddStatements(addedSymbols, symbolName, index));

                addedSymbols.Add(symbol);
            }

            result = result.Append(LocalFunctionStatement(
                    PredefinedType(Token(SyntaxKind.VoidKeyword)),
                    Identifier(Changing + i))
                .WithBody(Block(changingStates)));

            result = result.Append(LocalFunctionStatement(
                    PredefinedType(Token(SyntaxKind.VoidKeyword)),
                    Identifier(Changed + i))
                .WithBody(Block(changedStates)));
        }

        return result;
    }

    private static IReadOnlyList<IfStatementSyntax> NullReturn(IReadOnlyList<IPropertySymbol> symbols)
    {
        if (symbols.LastOrDefault()?.Type.IsReferenceType is false) return [];
        
        return [IfStatement(
            IsPatternExpression(
                IdentifierName(string.Join(".", symbols.Select(i => i.Name))),
                ConstantPattern(
                    LiteralExpression(
                        SyntaxKind.NullLiteralExpression))),
            ReturnStatement())];
    }


    private static IReadOnlyList<ExpressionStatementSyntax> RemoveStatements(IReadOnlyList<IPropertySymbol> symbols,
        in PropDpName name, int index)
    {
        return RemoveStatements(CreateLeading(symbols), name, index);
    }

    private static string CreateLeading(IReadOnlyList<IPropertySymbol> symbols)
        => symbols.Aggregate(string.Empty, (current, symbol) => current + symbol.Name + ".");

    private static IReadOnlyList<ExpressionStatementSyntax> RemoveStatements(string leading,
        in PropDpName name, int index)
    {
        return 
        [
            ExpressionStatement(
                InvocationExpression(
                    IdentifierName(Changing + index))),
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SubtractAssignmentExpression,
                    IdentifierName(leading + name.NameChanging),
                    IdentifierName(Changing + index))),
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SubtractAssignmentExpression,
                    IdentifierName(leading + name.NameChanged),
                    IdentifierName(Changed + index))),
        ];
    }

    private static IReadOnlyList<ExpressionStatementSyntax> AddStatements(IReadOnlyList<IPropertySymbol> symbols,
        in PropDpName name, int index)
    {
        var leading = CreateLeading(symbols);

        return
        [
            ..RemoveStatements(leading, name, index),
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.AddAssignmentExpression,
                    IdentifierName(leading + name.NameChanging),
                    IdentifierName(Changing + index))),
            ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.AddAssignmentExpression,
                    IdentifierName(leading + name.NameChanged),
                    IdentifierName(Changed + index))),
        ];
    }


    private const string Changing = "Changing", Changed = "Changed";

    private static bool IsPropDpProperty(IPropertySymbol symbol)
    {
        return symbol.GetAttributes().Any(a =>
            a.AttributeClass?.GetFullMetadataName() is PropertyDependencyAnalyzer.AttributeName
                or FieldDependencyAnalyzer.AttributeName);
    }
}