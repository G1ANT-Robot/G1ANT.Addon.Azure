﻿/**
*    Copyright(C) G1ANT Ltd, All rights reserved
*    Solution G1ANT.Addon, Project G1ANT.Addon.Selenium
*    www.g1ant.com
*
*    Licensed under the G1ANT license.
*    See License.txt file in the project root for full license information.
*
*/
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
            public TextStructure ClientId { get; set; }

            [Argument(Required = true, Tooltip = "Azure Key Vault Url")]
            public TextStructure Url { get; set; }

            [Argument(Required = true, Tooltip = "Connection timeout to Azure in ms")]
            public IntegerStructure ConnectionTimeout { get; set; } = new IntegerStructure(10000);

            [Argument(Required = true, Tooltip = "Name of the key vault variable")]
            public VariableStructure Result { get; set; } = new VariableStructure("result");
        }

        public KeyVaultCommand(AbstractScripter scripter) : base(scripter)
        {
        }

        public void Execute(Arguments arguments)
        {
            var azureKeyVaultManager = new AzureKeyVaultManager(arguments.ClientId.Value, arguments.Secret.Value, new Uri(arguments.Url.Value), arguments.ConnectionTimeout.Value);
            Scripter.Variables.SetVariableValue(arguments.Result.Value, new AzureCredentialContainerStructure(arguments.Secret.Value, arguments.ClientId.Value, new Uri(arguments.Url.Value), arguments.ConnectionTimeout.Value));
        }
    }
}