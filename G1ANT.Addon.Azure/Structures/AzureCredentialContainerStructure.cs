using System;
using G1ANT.Language;

namespace G1ANT.Addon.Azure.Structures
{
    [Structure(Name = "azurecredentialcontainer", Tooltip = "This structure allows to use Azure Key Vault")]
    class AzureCredentialContainerStructure : StructureTyped<PasswordStructure>
    {
        private string clientSecret;
        private string clientId;
        private int azureTimeout;
        private Uri keyVaultUri;

        public AzureCredentialContainerStructure(object value, string format = null, AbstractScripter scripter = null) : base(value, format, scripter) { }

        public AzureCredentialContainerStructure(string clientSecret, string clientId, Uri keyVaultUri, int azureTimeout, string format = ""): base(null, format, null)
        {
            if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(keyVaultUri.ToString()))
            {
                throw new ArgumentException("Client Secret, Client ID and key vault uri can't be empty.");
            }

            this.clientSecret = clientSecret;
            this.clientId = clientId;
            this.azureTimeout = azureTimeout;
            this.keyVaultUri = keyVaultUri;
        }

        public override Structure Get(string index = null)
        {
            var azureManager = new AzureManager();
            if (string.IsNullOrEmpty(index))
            {
                throw new ArgumentException("Key name can't be empty");
            }

            return new PasswordStructure(azureManager.GetSecret(clientId, clientSecret, keyVaultUri, index, azureTimeout).Result);
        }

        public override void Set(Structure value, string index = null)
        {
            if (value is AzureCredentialContainerStructure azureCredentialContainer && index == null)
            {
                clientId = azureCredentialContainer.clientId;
                clientSecret = azureCredentialContainer.clientSecret;
                keyVaultUri = azureCredentialContainer.keyVaultUri;
            }
            throw new InvalidOperationException("It is not possible to set values in Azure Credential Container");
        }
    }
}
