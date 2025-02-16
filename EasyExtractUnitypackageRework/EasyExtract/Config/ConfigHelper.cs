using EasyExtract.Models;
using EasyExtract.Utilities;
using Newtonsoft.Json;

namespace EasyExtract.Config;

public class ConfigHelper
{
    private static readonly string ConfigPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EasyExtract",
            "Settings.json");

    private static readonly SemaphoreSlim Semaphore = new(1, 1);

    public ConfigHelper()
    {
        if (File.Exists(ConfigPath))
            Task.Run(async () => await ReadConfigAsync());
        else
            Task.Run(async () => await UpdateConfigAsync());
    }

    public ConfigModel? Config { get; private set; } = new();

    public async Task ReadConfigAsync()
    {
        try
        {
            var json = await File.ReadAllTextAsync(ConfigPath).ConfigureAwait(false);
            Config = JsonConvert.DeserializeObject<ConfigModel>(json);
        }
        catch (Exception e)
        {
            await BetterLogger.LogAsync($"Exception in ReadConfigAsync: {e.Message}", Importance.Error);
        }
    }


    public async Task UpdateConfigAsync()
    {
        await Semaphore.WaitAsync();
        try
        {
            var json = JsonConvert.SerializeObject(Config, Formatting.Indented);
            await using var sw = new StreamWriter(ConfigPath, false);
            await sw.WriteAsync(json);
            await BetterLogger.LogAsync($"Updated config file: {json}", Importance.Debug);
        }
        catch (Exception e)
        {
            await BetterLogger.LogAsync($"Exception in UpdateConfigAsync: {e.Message}", Importance.Error);
        }
        finally
        {
            Semaphore.Release();
        }
    }
}