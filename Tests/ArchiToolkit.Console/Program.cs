// ReSharper disable LocalizableElement

using System.Collections;
using ArchiToolkit.InterpolatedParser;

var a = "";
"I am sooooo cool!!".Parse($"I am so+ {a}!+");
Console.WriteLine(a);