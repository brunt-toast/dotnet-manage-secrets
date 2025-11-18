namespace Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.IO;

internal static class FileExtensions
{
    extension(File)
    {
        public static void Create(string path, string initialContent)
        {
            using FileStream stream = File.Create(path);
            using StreamWriter writer = new StreamWriter(stream);
            stream.Write(initialContent.Select(x => (byte)x).ToArray());
        }
    }
}
