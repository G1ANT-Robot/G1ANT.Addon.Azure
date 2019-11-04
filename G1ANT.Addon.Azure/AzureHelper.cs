using System;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace G1ANT.Addon.Azure
{
    public class AzureHelper
    {
        private static string _clientId;
        private static string _clientSecret;

        private static async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(_clientId, _clientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }

        public static string GetSecret(string clientId, string clientSecret, Uri keyVaultUri, string secretName)
        {
            _clientId = clientId;
            _clientSecret = clientSecret;
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetToken));
            var secret = Task.Run(() => keyVaultClient.GetSecretAsync(keyVaultUri + @"secrets/" + secretName)).ConfigureAwait(false).GetAwaiter().GetResult();
            keyVaultClient.Dispose();
            return secret.Value;
        }

    }
}
