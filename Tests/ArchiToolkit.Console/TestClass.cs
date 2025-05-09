﻿using ArchiToolkit.Fluent;
using ArchiToolkit.PureConst;
using FluentValidation;

namespace ArchiToolkit.Console;

/// <summary>
/// Test summary.
/// </summary>
[FluentApi(typeof(int))]
public static class TestClass
{
    public static void AddIt(this int i)
    {
        i += 1;

        new Validator().Validate(new(), a => { });
    }
}

public class Item
{
    public int Value { get; set; } = 0;
}

public class Validator : AbstractValidator<Item>
{
    public Validator()
    {
        RuleFor(i => i.Value).GreaterThan(0);
        RuleFor(i => i.Value).SetValidator(new IntValidator());
    }
}

public class IntValidator : AbstractValidator<int>
{
    public IntValidator()
    {
        RuleFor(i => i).GreaterThan(0).WithSeverity(Severity.Info);
    }
}

