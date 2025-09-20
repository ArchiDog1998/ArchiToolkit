using UnitsNet;
using UnitsNet.Units;

namespace ArchiToolkit.QuantExtensions;

public static class UnitsExtensions
{
    extension(Area area)
    {
        /// <inheritdoc cref="Math.Sqrt"/>
        public Length Sqrt()
        {
            var lengthUnit = area.Unit switch
            {
                AreaUnit.SquareMeter => LengthUnit.Meter,
                AreaUnit.SquareKilometer => LengthUnit.Kilometer,
                AreaUnit.SquareCentimeter => LengthUnit.Centimeter,
                AreaUnit.SquareMillimeter => LengthUnit.Millimeter,
                AreaUnit.SquareMicrometer => LengthUnit.Micrometer,
                AreaUnit.SquareMile => LengthUnit.Mile,
                AreaUnit.SquareYard => LengthUnit.Yard,
                AreaUnit.SquareFoot => LengthUnit.Foot,
                AreaUnit.SquareInch => LengthUnit.Inch,
                AreaUnit.SquareDecimeter => LengthUnit.Decimeter,
                AreaUnit.SquareNauticalMile => LengthUnit.NauticalMile,
                AreaUnit.UsSurveySquareFoot => LengthUnit.UsSurveyFoot,
                _ => throw new NotSupportedException($"unsupported AreaUnit: {area.Unit}")
            };
            var sqrt = Math.Sqrt(area.Value);
            return new Length(sqrt, lengthUnit);
        }
    }

    #region trigonometric functions

    extension(Angle angle)
    {
        /// <inheritdoc cref="Math.Tan"/>
        public double Tan => Math.Tan(angle.Radians);


        /// <inheritdoc cref="Math.Sin"/>
        public double Sin => Math.Sin(angle.Radians);


        /// <inheritdoc cref="Math.Cos"/>
        public double Cos => Math.Cos(angle.Radians);
    }

    /// <inheritdoc cref="Math.Atan"/>
    public static Angle Atan(this double d) => Angle.FromRadians(Math.Atan(d));


    /// <inheritdoc cref="Math.Atan2"/>
    public static Angle Atan2(this double y, double x) => Angle.FromRadians(Math.Atan2(y, x));


    /// <inheritdoc cref="Math.Atan2"/>
    public static Angle Atan2(this Length y, Length x)
        => Angle.FromRadians(Math.Atan2(y.As(UnitSystem.SI), x.As(UnitSystem.SI)));


    /// <inheritdoc cref="Math.Acos"/>
    public static Angle Acos(this double d) => Angle.FromRadians(Math.Acos(d));


    /// <inheritdoc cref="Math.Asin"/>
    public static Angle Asin(this double d) => Angle.FromRadians(Math.Asin(d));

    #endregion
}