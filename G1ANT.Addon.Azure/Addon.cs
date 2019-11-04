using G1ANT.Language;

namespace G1ANT.Addon.Azure
{
    [Addon(Name = "Azure", Tooltip = "This Addon allows to use Azure services")]
    [Copyright(Author = "G1ANT Ltd", Copyright = "G1ANT Ltd", Email = "support@g1ant.com", Website = "www.g1ant.com")]
    [License(Type = "LGPL", ResourceName = "License.txt")]
    [CommandGroup(Name = "azure", Tooltip = "Azure commands")]
    public class Addon : Language.Addon
    {

        public override void Check()
        {
            base.Check();
            // Check integrity of your Addon
            // Throw exception if this Addon needs something that doesn't exists
        }

        public override void LoadDlls()
        {
            base.LoadDlls();
            // All dlls embeded in resources will be loaded automatically,
            // but you can load here some additional dlls:

            // Assembly.Load("...")
        }

        public override void Initialize()
        {
            base.Initialize();
            // Insert some code here to initialize Addon's objects
        }

        public override void Dispose()
        {
            base.Dispose();
            // Insert some code here which will dispose all unecessary objects when this Addon will be unloaded
        }
    }
}