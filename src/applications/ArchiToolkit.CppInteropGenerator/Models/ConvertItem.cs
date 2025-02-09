namespace ArchiToolkit.CppInteropGenerator.Models;

public class ConvertItem(
    string filePath,
    string leadingNameSpace,
    string libraryName,
    ConvertTypeModel type)
{
    public string Convert()
    {
        Task.Delay(500).Wait();
        return string.Empty;
    }
}