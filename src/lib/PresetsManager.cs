using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickWinstall.Lib
{
    #region PresetsManager
    public static class PresetsManager
    {
        private static readonly string PresetsDirectory = Path.Combine(AppContext.BaseDirectory, "res", "presets");

        #region PresetInfo
        public class PresetInfo
        {
            [JsonPropertyName("PresetName")]
            public string Name { get; set; } = LangManager.GetString("PresetForm_Info_NoData", "");

            [JsonPropertyName("PresetDescription")]
            public string Description { get; set; } = LangManager.GetString("PresetForm_Info_NoData", "");

            [JsonPropertyName("PresetAuthor")]
            public string Author { get; set; } = LangManager.GetString("PresetForm_Info_NoData", "");

            [JsonPropertyName("PresetVersion")]
            public string Version { get; set; } = LangManager.GetString("PresetForm_Info_NoData", "");
        }
        #endregion

        #region PresetData
        public class PresetData
        {
            [JsonPropertyName("Preset")]
            public PresetInfo Preset { get; set; } = new PresetInfo();

            [JsonPropertyName("GeneralConfig")]
            public GeneralConfigDefaults GeneralConfig { get; set; }

            [JsonPropertyName("LangRegionConfig")]
            public LangRegionConfigDefaults LangRegionConfig { get; set; }

            [JsonPropertyName("BypassChecksConfig")]
            public BypassConfigDefaults BypassConfig { get; set; }

            [JsonPropertyName("DiskConfig")]
            public DiskConfigDefaults DiskConfig { get; set; }

            [JsonPropertyName("AccountConfig")]
            public AccountConfigDefaults AccountConfig { get; set; }

            [JsonPropertyName("OOBEConfig")]
            public OOBEConfigDefaults OOBEConfig { get; set; }

            [JsonPropertyName("BitLockerConfig")]
            public BitLockerConfigDefaults BitLockerConfig { get; set; }

            [JsonPropertyName("PersonalizeConfig")]
            public PersonalizeConfigDefaults PersonalizeConfig { get; set; }

            [JsonPropertyName("AppConfig")]
            public AppConfigDefaults AppConfig { get; set; }
        }
        #endregion

        #region GetAvailablePresets
        public static List<string> GetAvailablePresets()
        {
            try
            {
                if (!Directory.Exists(PresetsDirectory))
                {
                    System.Diagnostics.Debug.WriteLine($"PresetsManager: Presets directory not found: {PresetsDirectory}");
                    return new List<string>();
                }

                var presetFiles = Directory.GetFiles(PresetsDirectory, "*.json");
                var presetNames = presetFiles
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .OrderBy(n => n)
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"PresetsManager: Found {presetNames.Count} presets");
                return presetNames;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error listing presets: {ex.Message}");
                return new List<string>();
            }
        }
        #endregion

        #region LoadPresetInfo
        public static PresetInfo LoadPresetInfo(string presetName)
        {
            try
            {
                string filePath = Path.Combine(PresetsDirectory, $"{presetName}.json");

                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"PresetsManager: Preset file not found: {filePath}");
                    return null;
                }

                var json = File.ReadAllText(filePath);
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    if (document.RootElement.TryGetProperty("Preset", out JsonElement presetElement))
                    {
                        return JsonSerializer.Deserialize<PresetInfo>(presetElement.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error loading preset info for '{presetName}': {ex.Message}");
                return null;
            }
        }
        #endregion

        #region LoadPresetData
        public static PresetData LoadPresetData(string presetName)
        {
            try
            {
                string filePath = Path.Combine(PresetsDirectory, $"{presetName}.json");
                var json = File.ReadAllText(filePath);

                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"PresetsManager: Preset file not found: {filePath}");
                    return null;
                }

                var preset = JsonSerializer.Deserialize<PresetData>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                });

                System.Diagnostics.Debug.WriteLine($"PresetsManager: Loaded preset '{presetName}'");
                return preset;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error loading preset '{presetName}': {ex.Message}");
                return null;
            }
        }
        #endregion

        #region SavePreset
        public static bool SavePreset(string presetName, PresetData presetData)
        {
            try
            {
                // Ensure presets directory exists
                if (!Directory.Exists(PresetsDirectory))
                {
                    Directory.CreateDirectory(PresetsDirectory);
                }

                string filePath = Path.Combine(PresetsDirectory, $"{presetName}.json");

                var json = JsonSerializer.Serialize(presetData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true
                });

                File.WriteAllText(filePath, json);
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Saved preset '{presetName}' to {filePath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error saving preset '{presetName}': {ex.Message}");
                return false;
            }
        }
        #endregion

        #region DeletePreset
        public static bool DeletePreset(string presetName)
        {
            try
            {
                string filePath = Path.Combine(PresetsDirectory, $"{presetName}.json");

                if (!File.Exists(filePath))
                {
                    System.Diagnostics.Debug.WriteLine($"PresetsManager: Preset file not found for deletion: {filePath}");
                    return false;
                }

                File.Delete(filePath);
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Deleted preset '{presetName}'");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error deleting preset '{presetName}': {ex.Message}");
                return false;
            }
        }
        #endregion

        #region GetPresetInfo
        public static PresetInfo GetPresetInfo(string presetName)
        {
            try
            {
                string filePath = Path.Combine(PresetsDirectory, $"{presetName}.json");

                if (!File.Exists(filePath))
                {
                    return null;
                }

                var json = File.ReadAllText(filePath);
                using (JsonDocument document = JsonDocument.Parse(json))
                {
                    if (document.RootElement.TryGetProperty("Preset", out JsonElement presetElement))
                    {
                        return JsonSerializer.Deserialize<PresetInfo>(presetElement.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error getting preset info for '{presetName}': {ex.Message}");
                return null;
            }
        }
        #endregion

        #region PresetExists
        public static bool PresetExists(string presetName)
        {
            string filePath = Path.Combine(PresetsDirectory, $"{presetName}.json");
            return File.Exists(filePath);
        }
        #endregion

        #region ExportPreset
        public static bool ExportPreset(string presetName, string destinationPath)
        {
            try
            {
                string sourcePath = Path.Combine(PresetsDirectory, $"{presetName}.json");

                if (!File.Exists(sourcePath))
                {
                    System.Diagnostics.Debug.WriteLine($"PresetsManager: Source preset not found: {sourcePath}");
                    return false;
                }

                File.Copy(sourcePath, destinationPath, true);
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Exported preset '{presetName}' to {destinationPath}");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error exporting preset '{presetName}': {ex.Message}");
                return false;
            }
        }
        #endregion

        #region ImportPreset
        public static bool ImportPreset(string sourcePath, string presetName)
        {
            try
            {
                if (!File.Exists(sourcePath))
                {
                    System.Diagnostics.Debug.WriteLine($"PresetsManager: Source file not found: {sourcePath}");
                    return false;
                }

                // Validate JSON structure before importing
                var json = File.ReadAllText(sourcePath);
                var preset = JsonSerializer.Deserialize<PresetData>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (preset == null)
                {
                    System.Diagnostics.Debug.WriteLine($"PresetsManager: Invalid preset file format");
                    return false;
                }

                // Ensure presets directory exists
                if (!Directory.Exists(PresetsDirectory))
                {
                    Directory.CreateDirectory(PresetsDirectory);
                }

                string destinationPath = Path.Combine(PresetsDirectory, $"{presetName}.json");
                File.Copy(sourcePath, destinationPath, true);

                System.Diagnostics.Debug.WriteLine($"PresetsManager: Imported preset as '{presetName}'");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"PresetsManager: Error importing preset: {ex.Message}");
                return false;
            }
        }
        #endregion
    }
    #endregion
}
