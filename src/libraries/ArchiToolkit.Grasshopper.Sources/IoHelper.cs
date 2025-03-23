using System.Drawing;
using GH_IO.Serialization;
using GH_IO.Types;

namespace ArchiToolkit.Grasshopper;

internal static partial class IoHelper
{
    #region Write

    public static void Write(GH_IWriter writer, string key, bool value) => writer.SetBoolean(key, value);
    public static void Write(GH_IWriter writer, string key, byte value) => writer.SetByte(key, value);
    public static void Write(GH_IWriter writer, string key, int value) => writer.SetInt32(key, value);
    public static void Write(GH_IWriter writer, string key, long value) => writer.SetInt64(key, value);
    public static void Write(GH_IWriter writer, string key, float value) => writer.SetSingle(key, value);
    public static void Write(GH_IWriter writer, string key, double value) => writer.SetDouble(key, value);
    public static void Write(GH_IWriter writer, string key, decimal value) => writer.SetDecimal(key, value);
    public static void Write(GH_IWriter writer, string key, DateTime value) => writer.SetDate(key, value);
    public static void Write(GH_IWriter writer, string key, Guid value) => writer.SetGuid(key, value);
    public static void Write(GH_IWriter writer, string key, string value) => writer.SetString(key, value);
    public static void Write(GH_IWriter writer, string key, Point value) => writer.SetDrawingPoint(key, value);
    public static void Write(GH_IWriter writer, string key, PointF value) => writer.SetDrawingPointF(key, value);
    public static void Write(GH_IWriter writer, string key, Size value) => writer.SetDrawingSize(key, value);
    public static void Write(GH_IWriter writer, string key, SizeF value) => writer.SetDrawingSizeF(key, value);
    public static void Write(GH_IWriter writer, string key, Rectangle value) => writer.SetDrawingRectangle(key, value);

    public static void Write(GH_IWriter writer, string key, RectangleF value) =>
        writer.SetDrawingRectangleF(key, value);

    public static void Write(GH_IWriter writer, string key, Color value) => writer.SetDrawingColor(key, value);
    public static void Write(GH_IWriter writer, string key, Bitmap value) => writer.SetDrawingBitmap(key, value);
    public static void Write(GH_IWriter writer, string key, byte[] value) => writer.SetByteArray(key, value);
    public static void Write(GH_IWriter writer, string key, double[] value) => writer.SetDoubleArray(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Point2D value) => writer.SetPoint2D(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Point3D value) => writer.SetPoint3D(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Point4D value) => writer.SetPoint4D(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Interval1D value) => writer.SetInterval1D(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Interval2D value) => writer.SetInterval2D(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Line value) => writer.SetLine(key, value);
    public static void Write(GH_IWriter writer, string key, GH_BoundingBox value) => writer.SetBoundingBox(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Plane value) => writer.SetPlane(key, value);
    public static void Write(GH_IWriter writer, string key, GH_Version value) => writer.SetVersion(key, value);

    #endregion

    #region Read

    public static bool Read(GH_IReader reader, string key, ref bool value) => reader.TryGetBoolean(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref byte value) => reader.TryGetByte(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref int value) => reader.TryGetInt32(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref long value) => reader.TryGetInt64(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref float value) => reader.TryGetSingle(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref double value) => reader.TryGetDouble(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref decimal value) => reader.TryGetDecimal(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref DateTime value) => reader.TryGetDate(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref Guid value) => reader.TryGetGuid(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref string value) => reader.TryGetString(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref Point value) => reader.TryGetDrawingPoint(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref PointF value) => reader.TryGetDrawingPointF(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref Size value) => reader.TryGetDrawingSize(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref SizeF value) => reader.TryGetDrawingSizeF(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref Rectangle value) => reader.TryGetDrawingRectangle(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref RectangleF value) => reader.TryGetDrawingRectangleF(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref Color value) => reader.TryGetDrawingColor(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Point2D value) => reader.TryGetPoint2D(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Point3D value) => reader.TryGetPoint3D(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Point4D value) => reader.TryGetPoint4D(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Interval1D value) => reader.TryGetInterval1D(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Interval2D value) => reader.TryGetInterval2D(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Line value) => reader.TryGetLine(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_BoundingBox value) => reader.TryGetBoundingBox(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Plane value) => reader.TryGetPlane(key, ref value);
    public static bool Read(GH_IReader reader, string key, ref GH_Version value) => reader.TryGetVersion(key, ref value);

    public static bool Read(GH_IReader reader, string key, ref byte[] value)
    {
        try
        {
            value = reader.GetByteArray(key);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Read(GH_IReader reader, string key, ref double[] value)
    {
        try
        {
            value = reader.GetDoubleArray(key);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool Read(GH_IReader reader, string key, ref Bitmap value)
    {
        try
        {
            value = reader.GetDrawingBitmap(key);
            return true;
        }
        catch
        {
            return false;
        }
    }
    #endregion
}