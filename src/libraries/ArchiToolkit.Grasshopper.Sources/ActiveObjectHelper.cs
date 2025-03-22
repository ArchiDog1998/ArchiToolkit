using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
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

    private static MethodInfo? GetOperatorCast(Type type, Type returnType, Type paramType)
    {
        return GetAllRuntimeMethods(type).FirstOrDefault(m =>
        {
            if (!m.IsSpecialName) return false;
            if (m.Name is not "op_Explicit" and not "op_Implicit") return false;

            if (m.ReturnType != returnType) return false;

            var parameters = m.GetParameters();

            if (parameters.Length != 1) return false;

            return parameters[0].ParameterType.IsAssignableFrom(paramType);
        });

        static IEnumerable<MethodInfo> GetAllRuntimeMethods(Type? type)
        {
            return type == null ? [] : type.GetRuntimeMethods().Concat(GetAllRuntimeMethods(type.BaseType));
        }
    }

    public static Io<T> GetDataItem<T>(IGH_DataAccess da, int index)
    {
        if (typeof(T).IsEnum)
        {
            var i = 0;
            var hasGot = da.GetData(index, ref i);
            var data = (T)Enum.ToObject(typeof(T), i);
            return new Io<T>(hasGot, index, data);
        }
        else
        {
            var data = default(T);
            var hasGot = da.GetData(index, ref data);
            return new Io<T>(hasGot, index, data!);
        }
    }

    public static Io<List<T>> GetDataList<T>(IGH_DataAccess da, int index)
    {
        if (typeof(T).IsEnum)
        {
            List<int> data = [];
            var hasGot = da.GetDataList(index, data);
            return new Io<List<T>>(hasGot, index,
                data.Select(i => (T)Enum.ToObject(typeof(T), i)).ToList());
        }
        else
        {
            List<T> data =[];
            var hasGot = da.GetDataList(index, data);
            return new Io<List<T>>(hasGot, index, data);
        }
    }

    public static Io<GH_Structure<T>> GetDataTree<T>(IGH_DataAccess da, int index) where T : IGH_Goo
    {
        var hasGot = da.GetDataTree<T>(index, out var data);
        return new Io<GH_Structure<T>>(hasGot, index, data);
    }

    public static void SetData<T>(IGH_DataAccess da, int index, T data)
    {
        if (typeof(T).IsEnum)
        {
            da.SetData(index, Convert.ToInt32(data));
        }
        else
        {
            da.SetData(index, data);
        }
    }

    public static void SetData<T>(IGH_DataAccess da, int index, List<T> data)
    {
        if (typeof(T).IsEnum)
        {
            da.SetDataList(index, data.Select(i => Convert.ToInt32(i)));
        }
        else
        {
            da.SetDataList(index, data);
        }
    }

    public static void SetData(IGH_DataAccess da, int index, IGH_Structure data)
    {
        da.SetDataTree(index, data);
    }

    public static void SetData(IGH_DataAccess da, int index, IGH_DataTree data)
    {
        da.SetDataTree(index, data);
    }
}