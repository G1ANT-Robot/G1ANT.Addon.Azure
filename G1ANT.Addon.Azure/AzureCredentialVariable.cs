using G1ANT.Language;
using System;

namespace G1ANT.Addon.Azure.Structures
{
    [Variable(Name = VariableName, ReadOnly = true, Tooltip = "Reads the value of a specified key stored in the Credential Container")]
    public class AzureCredentialVariable : Variable
    {
        public const string VariableName = "azurecredential";
        public const string AllKeysIndex = "Keys";
        public const char SplitingChar = ':';

        public AzureCredentialVariable(AbstractScripter scripter = null) : base(scripter)
        {
        }

        public override Structure GetValue(string index = null)
        {
            if (string.IsNullOrWhiteSpace(index))
                return new TextStructure("Azure credential variable");
            try
            {
                //return new PasswordStructure(AzureManager.GetSecret(index));
                throw new Exception();
            }
            catch
            {
                //AbstractSettingsContainer.Variable variable = Scripter.Settings.Credentials[index];
                //return Scripter.Structures.CreateStructure(variable.Value, variable.Type, variable.Format);
                //wtf
                throw;
            }
        }

        public override void SetValue(Structure value, string index = null)
        {
            throw new NotSupportedException("This method should have never been invoked, because this variable is read only.");
        }
    }
}
