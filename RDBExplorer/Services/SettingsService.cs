using System.Text.Json;

namespace RDBExplorer.Services
{
    public class UserSettings
    {
        public bool ExportWithNames { get; set; } = false;
        public string LastRdbPath { get; set; } = string.Empty;
        public bool IndentedJson { get; set; } = true;
    }

    public class SettingsService
    {
        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "RDBExplorer");

        private static readonly string SettingsFile = Path.Combine(AppDataPath, "settings.json");

        private static SettingsService _instance;
        public static SettingsService Instance => _instance ??= new SettingsService();

        public UserSettings Config { get; private set; }

        private SettingsService()
        {
            Load();
        }

        public void Load()
        {
            try
            {
                if (File.Exists(SettingsFile))
                {
                    string json = File.ReadAllText(SettingsFile);
                    Config = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
                }
                else
                {
                    Config = new UserSettings();
                }
            }
            catch
            {
                Config = new UserSettings();
            }
        }

        public void Save()
        {
            try
            {
                if (!Directory.Exists(AppDataPath))
                    Directory.CreateDirectory(AppDataPath);

                string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFile, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
    }
}
