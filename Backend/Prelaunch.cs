using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace dionizos_backend_app
{
    public static class Prelaunch
    {
        public static void GetSecrets()
        {
            var client = new SecretClient(
                new Uri("https://dionizos-keyvault.vault.azure.net/"),
                new DefaultAzureCredential(includeInteractiveCredentials: true),
                new SecretClientOptions
                {
                    Retry =
                    {
                        Delay = TimeSpan.FromSeconds(2),
                        MaxDelay = TimeSpan.FromSeconds(16),
                        MaxRetries = 5,
                        Mode = RetryMode.Exponential
                    }
                }
            );

            KeyVaultSecret POSTGRES_CONNSTRING = client.GetSecret("POSTGRES-CONNSTRING-TSC").Value;
            KeyVaultSecret COMMUNICATIONS_CONNSTRING =  client.GetSecret("COMMUNICATIONS-CONNSTRING-2").Value;
            KeyVaultSecret BLOB_CONNSTRING =  client.GetSecret("BLOB-CONNSTRING").Value;

            string postgres = POSTGRES_CONNSTRING.Value;
            string communications = COMMUNICATIONS_CONNSTRING.Value;
            string blob = BLOB_CONNSTRING.Value;

            System.Environment.SetEnvironmentVariable("POSTGRES_CONNSTRING_TSC",postgres);
            System.Environment.SetEnvironmentVariable("COMMUNICATIONS_CONNSTRING",communications);
            System.Environment.SetEnvironmentVariable("BLOB_CONNSTRING",blob);
        }
    }
}
