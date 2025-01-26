using System.Reflection;

namespace NativeSceneAutomation.Helpers
{
    public static class PathHelper
    {
        public static string ExecutingDirectory
        {
            get => Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location) ??
                throw new InvalidOperationException($"Unable to retrieve executing directory from '{Assembly.GetExecutingAssembly()}'");
        }

        public static void SetExecutingAsCurrentDirectory()
        {
            Directory.SetCurrentDirectory(ExecutingDirectory);
        }
    }
}
