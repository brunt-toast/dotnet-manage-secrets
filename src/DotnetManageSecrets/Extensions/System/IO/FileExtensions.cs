using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.JoshBrunton.DotnetManageSecrets.Extensions.System.IO;
internal static class FileExtensions
{
    public static void CreateWithoutLease(string path)
    {
        using var _ = File.Create(path);
    }
}
