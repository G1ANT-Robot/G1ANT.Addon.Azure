using System;
using System.Threading.Tasks;
using G1ANT.Addon.Azure.Extensions;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace G1ANT.Addon.Azure
{
    public class AzureKeyVaultManager
    {
        private string clientId;
        private string clientSecret;
        private int azureTimeout;
        private Uri keyVaultUri;

        public AzureKeyVaultManager(string clientId, string clientSecret, Uri keyVaultUri, int azureTimeout)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.azureTimeout = azureTimeout;
            this.keyVaultUri = keyVaultUri;

            if(!DoesUriEndWithSlash())
            {
                throw new Exception("Key Vault Url should end with '/'");
            }

            ValidateKeyVaultClient().Wait();
        }

        private bool DoesUriEndWithSlash()
        {
            return keyVaultUri.ToString().EndsWith("/");
        }

        private async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(clientId, clientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);

            if (result.AccessToken == null)
            {
                throw new Exception("Failed to obtain the JWT token");
            }

            return result.AccessToken;
        }

        public async Task<string> GetSecret(string secretName)
        {
            using (var keyVaultClient = new KeyVaultClient(GetToken))
            {
                var task = keyVaultClient.GetSecretAsync($"{keyVaultUri}secrets/{secretName}");
                await task.TimeoutAfter(azureTimeout).ConfigureAwait(false);
                return task.Result.Value;
            }
        }

        public async Task ValidateKeyVaultClient()
        {
            try
            {
                await GetSecret(Guid.NewGuid().ToString()).ConfigureAwait(false);
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is KeyVaultErrorException exception && exception.Body.Error.Code == "SecretNotFound")
                    return;
                throw ex?.InnerException ?? ex;
            }
        }
    }
}