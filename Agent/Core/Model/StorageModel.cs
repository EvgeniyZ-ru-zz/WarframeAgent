using System;
using System.IO;

namespace Core.Model
{
    public static class StorageModel
    {
        static string applicationFolder = AppDomain.CurrentDomain.BaseDirectory;

        public static string ExpandRelativeName(string relativeName) =>
            Path.GetFullPath(Path.Combine(applicationFolder, relativeName));
    }
}
