using System;
using System.Threading.Tasks;
using G1ANT.Addon.Azure.Extensions;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace G1ANT.Addon.Azure
{
    public class AzureManager
    {
        private string clientId;
        private string clientSecret;

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(clientId, clientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result.AccessToken == null)
            {
                throw new InvalidOperationException("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }

        public async Task<string> GetSecret(string clientId, string clientSecret, Uri keyVaultUri, string secretName, int azureTimeout)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            using (var keyVaultClient = new KeyVaultClient(GetToken))
            {
                var task = keyVaultClient.GetSecretAsync(keyVaultUri + "secrets/" + secretName);
                await task.TimeoutAfter(azureTimeout + 100000000).ConfigureAwait(false);
                return task.Result.Value;
            }
        }

        public async Task<bool> AreCredentialsCorrectAsync(string clientId, string clientSecret, Uri keyVaultUri, int azureTimeout)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            var task = GetToken("", "https://vault.azure.net", "");

            try
            {
                await task.TimeoutAfter(azureTimeout).ConfigureAwait(false);
                return !string.IsNullOrEmpty(task.Result);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(TimeoutException))
                {
                    throw new TimeoutException("Connection to Azure timed out. This could happen if you provided wrong ClientId, Secret, Url or there is problem with internet connection.");
                }

                throw;
            }
        }
    }
}
