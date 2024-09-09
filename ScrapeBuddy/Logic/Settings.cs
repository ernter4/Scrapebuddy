using System;
using System.IO;
using Newtonsoft.Json;

namespace ScrapeBuddy.Logic
{
    /// <summary>
    /// Represents the settings for the application.
    /// </summary>
    public class SettingsModel
    {
        public bool DarkMode { get; set; } = false;
        public bool EmailNotificationsEnabled { get; set; } = false;
        public byte[]? Email { get; set; } = null;
        public byte[]? Name { get; set; } = null;
        public byte[]? Salt { get; set; } = null;
        public byte[]? IV { get; set; } = null;
    }

    /// <summary>
    /// Manages loading, saving, and updating application settings.
    /// </summary>
    public class SettingsManager
    {
        private const string SettingsFilePath = @"Data/Settings.json";

        /// <summary>
        /// Loads settings from the settings file.
        /// </summary>
        /// <returns>The loaded settings, or default settings if the file does not exist or deserialization fails.</returns>
        public SettingsModel LoadSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                string json = File.ReadAllText(SettingsFilePath);
                var loadedSettings = JsonConvert.DeserializeObject<SettingsModel>(json);
                return loadedSettings ?? new SettingsModel();
            }
            return new SettingsModel();
        }

        /// <summary>
        /// Saves the specified settings to the settings file.
        /// </summary>
        /// <param name="settings">The settings to save.</param>
        public void SaveSettings(SettingsModel settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsFilePath, json);
        }

        /// <summary>
        /// Deletes the settings by saving an empty settings object to the settings file.
        /// </summary>
        public void DeleteSettings()
        {
            var emptySettings = new SettingsModel();
            SaveSettings(emptySettings);
        }

        /// <summary>
        /// Updates the settings with the provided values.
        /// </summary>
        /// <param name="email">The new encrypted email value.</param>
        /// <param name="name">The new encrypted name value.</param>
        /// <param name="salt">The new salt value.</param>
        /// <param name="iv">The new initialization vector (IV) value.</param>
        public void UpdateSettings(byte[]? email, byte[]? name, byte[]? salt, byte[]? iv)
        {
            var settings = LoadSettings();
            settings.Email = email;
            settings.Name = name;
            settings.Salt = salt;
            settings.IV = iv;
            SaveSettings(settings);
        }
    }
}