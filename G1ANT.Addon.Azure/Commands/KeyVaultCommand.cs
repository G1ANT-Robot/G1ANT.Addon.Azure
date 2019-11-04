using System;
using G1ANT.Addon.Azure.Structures;
using G1ANT.Language;

namespace G1ANT.Addon.Azure.Commands
{
    [Command(Name = "azure.keyvault", Tooltip = "This command allows to access data stored in Azure Key Vault")]
    public class KeyVaultCommand : Command
    {
        public class Arguments : CommandArguments
        {
            [Argument(Required = true, Tooltip = "Azure Key Vault client secret")]
            public TextStructure Secret { get; set; }

            [Argument(Required = true, Tooltip = "Azure Key Vault client ID")]
            public TextStructure Id { get; set; }

            [Argument(Required = true, Tooltip = "Azure Key Vault Url")]
            public TextStructure Url { get; set; }

            [Argument(Required = true, Tooltip = "Name of the key vault variable")]
            public TextStructure Name { get; set; } = new TextStructure("result");
        }

        public KeyVaultCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            Scripter.Variables.SetVariableValue(arguments.Name.Value, new AzureCredentialContainerStructure(arguments.Secret.Value, arguments.Id.Value, new Uri(arguments.Url.Value)));
        }       
    }
}