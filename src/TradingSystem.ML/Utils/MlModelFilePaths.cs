using System.IO;
namespace TradingSystem.ML.Utils;

public static class MlModelFilePaths
{
    public const string DefaultModelsFolder = "mlmodels";
    public const string DefaultModelFileName = "trading_model.zip";

    public static string EnsureModelsFolder()
    {
        if (!Directory.Exists(DefaultModelsFolder))
            Directory.CreateDirectory(DefaultModelsFolder);
        return DefaultModelsFolder;
    }

    public static string GetModelPath(string folder = null, string fileName = null)
    {
        var f = folder ?? DefaultModelsFolder;
        if (!Directory.Exists(f)) Directory.CreateDirectory(f);
        return Path.Combine(f, fileName ?? DefaultModelFileName);
    }
}