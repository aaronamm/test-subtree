namespace CodeSanook.Common.Modules
{
    public static class ModuleHelper
    {
        /// <summary>
        /// get name of the module from a assembly
        /// </summary>
        public static string GetModuleName<TInModule>()
            => typeof(TInModule).Assembly.GetName().Name;
    }
}