using System;
using G1ANT.Language;

namespace G1ANT.Addon.Azure.Structures
{
    [Structure(Name = "azurecredentialcontainer", Tooltip = "This structure allows to use Azure Key Vault")]
    class AzureCredentialContainerStructure : StructureTyped<PasswordStructure>
    {
        private AzureKeyVaultManager azureKeyVaultManager;
        public AzureCredentialContainerStructure(object value, string format = null, AbstractScripter scripter = null) : base(value, format, scripter) { }

        public AzureCredentialContainerStructure(string clientSecret, string clientId, Uri keyVaultUri, int connectionTimeout, string format = ""): base(null, format, null)
        {
            if (string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(keyVaultUri.ToString()))
            {
                throw new ArgumentException("Client Secret, Client ID and key vault uri can't be empty.");
            }
            azureKeyVaultManager = new AzureKeyVaultManager(clientId, clientSecret, keyVaultUri, connectionTimeout);
        }

        public override Structure Get(string index = null)
        {
            if (string.IsNullOrEmpty(index))
            {
                throw new ArgumentException("Key name can't be empty");
            }

            return new PasswordStructure(azureKeyVaultManager.GetSecret(index).Result);
        }

        public override void Set(Structure value, string index = null)
        {
            if (value is AzureCredentialContainerStructure azureCredentialContainer && index == null)
            {
                azureKeyVaultManager = azureCredentialContainer.azureKeyVaultManager;
            }
            else
            {
                throw new InvalidOperationException("It is not possible to set values in Azure Credential Container");
            }
        }
    }
}
