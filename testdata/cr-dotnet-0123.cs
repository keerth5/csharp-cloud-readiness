using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace CloudSecurityExample
{
    // ❌ Violating Example: Hardcoding secrets directly in source code
    public class InsecureApiClient
    {
        // Violation: API key hardcoded directly in code
        private const string ApiKey = "AIzaSyD3VxT-Hardcoded-Secret-Key";

        // Violation: Hardcoded connection string
        private string connectionString = "Server=myserver;Database=mydb;User Id=admin;Password=P@ssw0rd!;";

        public void Connect()
        {
            Console.WriteLine($"[Violation] Connecting using hardcoded credentials: {connectionString}");
            Console.WriteLine($"[Violation] Using hardcoded API key: {ApiKey}");
        }
    }

    // ❌ Violating Example: Secrets in appsettings.json (simulated here)
    public class ConfigFileReader
    {
        public void ReadConfig()
        {
            // Violation: Secrets in configuration file instead of secure vault
            string dbPassword = "dbPasswordFromAppSettings"; 
            Console.WriteLine($"[Violation] Using password from appsettings.json: {dbPassword}");
        }
    }

    // ✅ Compliant Example: Secrets loaded securely via environment variables
    public class SecureApiClient
    {
        private readonly string apiKey;
        private readonly string connectionString;

        public SecureApiClient()
        {
            // ✅ Use environment variables (injected securely via cloud platform)
            apiKey = Environment.GetEnvironmentVariable("MYAPP_API_KEY");
            connectionString = Environment.GetEnvironmentVariable("MYAPP_DB_CONNECTION");

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("[Compliant] Secrets are missing — check cloud secret manager configuration.");
            }
        }

        public void Connect()
        {
            Console.WriteLine("[Compliant] Connecting securely using externalized credentials.");
        }
    }

    // ✅ Compliant Example: Using cloud-native secret management (e.g., Azure Key Vault)
    public class KeyVaultSecretReader
    {
        private readonly IConfiguration configuration;

        public KeyVaultSecretReader()
        {
            // ✅ Load secrets securely using configuration builder linked to Key Vault
            configuration = new ConfigurationBuilder()
                .AddAzureKeyVault(new Uri("https://my-keyvault.vault.azure.net/"), new DefaultAzureCredential())
                .Build();
        }

        public void ReadSecrets()
        {
            string secureApiKey = configuration["ApiKey"];
            string secureDbConnection = configuration["DbConnection"];
            Console.WriteLine($"[Compliant] Retrieved secrets securely from Key Vault: {secureApiKey}, {secureDbConnection}");
        }
    }
}
