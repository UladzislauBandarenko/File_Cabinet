using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp
{
    /// <summary>
    /// Represents the configuration for the application.
    /// </summary>
    public static class Config
    {
        private static IConfiguration? configuration;

        /// <summary>
        /// Initializes the configuration.
        /// </summary>
        public static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("validation-rules.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        /// <summary>
        /// Gets the validation rules configuration.
        /// </summary>
        /// <param name="ruleset">The ruleset to get configuration for.</param>
        /// <returns>The validation configuration.</returns>
        public static ValidationConfig GetValidationRules(string ruleset)
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Configuration is not initialized.");
            }

            return configuration.GetSection(ruleset).Get<ValidationConfig>() ?? new ValidationConfig();
        }

        /// <summary>
        /// Gets a configuration value.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        public static string GetValue(string key)
        {
            if (configuration == null || string.IsNullOrEmpty(key))
            {
                throw new InvalidOperationException("Configuration is not initialized.");
            }

            return configuration[key] ?? throw new KeyNotFoundException($"Configuration key '{key}' not found.");
        }

        /// <summary>
        /// Gets a strongly typed configuration section.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the section to.</typeparam>
        /// <param name="sectionName">The section name in the configuration.</param>
        /// <returns>The deserialized configuration section.</returns>
        public static T GetSection<T>(string sectionName)
            where T : new()
        {
            if (configuration == null)
            {
                throw new InvalidOperationException("Configuration is not initialized.");
            }

            return configuration.GetSection(sectionName).Get<T>() ?? new T();
        }
    }
}