﻿using System.Numerics;

namespace ArchiToolkit.Test.PropDp;

internal partial class SubTest
{
    [FieldDp]
    public partial float X { get; set; }
    
    [FieldDp]
    public partial float Y { get; set; }
    
    [PropDp]
    public partial Vector2 Test { get; }

    private partial Vector2 GetTest()
    {
        var result = new Vector2(X, Y);
        return result;
    }
}

internal partial class PropTest
{
    [FieldDp]
    public partial SubTest Test { get; set; }
    
    [PropDp]
    public partial SubTest TestRef { get; set; }

    partial void SetTestRef(SubTest value)
    {
        throw new NotImplementedException();
    }

    private partial SubTest GetTestRef() => new();

    [PropDp]

    public partial int Add { get; set; }

    partial void SetAdd(int value)
    {
        this.Test.X = value;
    }

    private partial int GetAdd() => SetValue(SetValue(SetValue((int)(Test.Test.X + Test.Test.Y)))) ;

    [Const]
    private int GetValue() => (int)Test.Test.X + 1;

    [Const]
    private int SetValue(int value) => value;
}
