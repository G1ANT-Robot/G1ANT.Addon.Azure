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
                await task.TimeoutAfter(azureTimeout).ConfigureAwait(false);
                return task.Result.Value;
            }
        }

        public async Task ValidateKeyVaultClient(string clientId, string clientSecret, Uri keyVaultUri, int azureTimeout)
        {
            try
            {
                await GetSecret(clientId, clientSecret, keyVaultUri, "fCuJc6bA9N", azureTimeout).ConfigureAwait(false);
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