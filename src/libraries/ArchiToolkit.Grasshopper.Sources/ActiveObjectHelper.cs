using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Grasshopper.Kernel.Types;

namespace ArchiToolkit.Grasshopper;

internal static class ActiveObjectHelper
{
    public static string ToGooString<T>(this T value)
    {
        return value?.ToString() ?? "<null>";
    }

    public static bool CastFrom<T>(object source, out T value)
    {
        var sType = source.GetType();

        if (CastLocal(source, out value))
        {
            return true;
        }

        if (source is IGH_Goo
            && sType.GetRuntimeProperty("Value") is { } property
            && property.GetValue(source) is { } v)
        {
            return CastLocal(v, out value);
        }

        return false;

        static bool CastLocal(object src, out T value)
        {
            var sType = src.GetType();
            if (typeof(T).IsAssignableFrom(sType))
            {
                value = (T)src;
                return true;
            }

            if (GetOperatorCast(typeof(T), typeof(T), sType) is { } method)
            {
                value = (T)method.Invoke(null, [src]);
                return true;
            }

            value = default!;
            return false;
        }
    }


    public static bool CastTo<T, TQ>(T value, ref TQ target)
    {
        if (value is null) return false;
        if (CastLocal(value, typeof(TQ), out var q))
        {
            target = (TQ)q;
        }
        else if (target is IGH_Goo
                 && typeof(TQ).GetRuntimeProperty("Value") is { } property
                 && CastLocal(value, property.PropertyType, out var propValue))
        {
            property.SetValue(propValue, value);
        }

        return false;

        static bool CastLocal(object src, Type valueType, out object value)
        {
            var sType = src.GetType();
            if (valueType.IsAssignableFrom(sType))
            {
                value = src;
                return true;
            }

            if (GetOperatorCast(valueType, valueType, sType) is { } method)
            {
                value = method.Invoke(null, [src]);
                return true;
            }

            value = null!;
            return false;
        }
    }

    internal static MethodInfo? GetOperatorCast(Type type, Type returnType, Type paramType)
    {
        return type.GetAllRuntimeMethods().FirstOrDefault(m =>
        {
            if (!m.IsSpecialName) return false;
            if (m.Name is not "op_Explicit" and not "op_Implicit") return false;

            if (m.ReturnType != returnType) return false;

            var parameters = m.GetParameters();

            if (parameters.Length != 1) return false;

            return parameters[0].ParameterType.IsAssignableFrom(paramType);
        });
    }

    private static IEnumerable<MethodInfo> GetAllRuntimeMethods(this Type? type)
    {
        return type == null ? [] : type.GetRuntimeMethods().Concat(GetAllRuntimeMethods(type.BaseType));
    }
}