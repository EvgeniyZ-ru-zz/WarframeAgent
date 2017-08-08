using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    static public class StorageModel
    {
        static string applicationFolder = AppDomain.CurrentDomain.BaseDirectory;

        public static string ExpandRelativeName(string relativeName) =>
            Path.GetFullPath(Path.Combine(applicationFolder, relativeName));
    }
}
