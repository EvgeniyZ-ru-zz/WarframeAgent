using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Repair
{
    public class FileList
    {
        public string Path { get; set; }
        public string Md5 { get; set; }
        public long Size { get; set; }
    }

    public class RepairProcess : VM
    {
        private static readonly string CurrentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly Uri UpdateUri = new Uri("https://evgeniy-z.ru/wagent/download/release");
        private static readonly Uri FileListUri = new Uri("https://evgeniy-z.ru/files.txt?=5");
        private const string TempFile = "tempFile";
        private const string TempDir = "tempDir";
        private const string StartFile = "Agent.exe";
        private readonly List<string> Md5BlockFiles = new List<string>
        {
            "Repair.exe"
        };


        private string status;
        public string Status
        {
            get => status;
            set => Set(ref status, value);
        }

        public async Task Start(string command = null)
        {
            switch (command)
            {
                case "/update":
                    Status = "Update started...".ToUpper();
                    break;
                case "/md5":
                    Status = "Generate MD5...".ToUpper();
                    Md5();
                    Status = "MD5 generated!".ToUpper();
                    break;
                default:
                    Status = "Repair started...".ToUpper();
                    await StartRepair();

                    break;
            }

            await Task.Delay(1000);
        }

        private async Task StartRepair()
        {
            //TODO: Ping test
            List<string> coruptFiles = new List<string>();
            var webList = await Tools.Network.ReadTextAsync(new Uri(FileListUri.AbsoluteUri));
            var ss = webList.Split(';');
            var serverFiles = (from webFile in ss
                where !string.IsNullOrWhiteSpace(webFile)
                select webFile.Split('|')
                into array
                select new FileList
                {
                    Path = PathCombine(CurrentDir, array[0].Replace("\r\n", null)),
                    Md5 = array[1],
                    Size = Convert.ToInt64(array[2])
                }).ToList();


            foreach (var serverFile in serverFiles)
            {
                if (CheckFile(serverFile))
                {
                    coruptFiles.Add(serverFile.Path.Replace(CurrentDir, ""));
                }
            }

            if (coruptFiles.Any())
            {
                if (Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
                if (File.Exists(TempFile)) File.Delete(TempFile);
                Tools.Network.DownloadFile(UpdateUri, TempFile);
                System.IO.Compression.ZipFile.ExtractToDirectory(TempFile, TempDir);

                foreach (var coruptFile in coruptFiles)
                {
                    var sourceFile = PathCombine(CurrentDir, coruptFile);
                    var originFile = PathCombine(Path.Combine(CurrentDir, TempDir), coruptFile);
                    var fileDirectory = Path.GetDirectoryName(sourceFile);


                    if (File.Exists(sourceFile)) File.Delete(sourceFile);
                    if (!Directory.Exists(fileDirectory)) Directory.CreateDirectory(fileDirectory);

                    File.Move(originFile, sourceFile);
                }
            }

            if (Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
            if (File.Exists(TempFile)) File.Delete(TempFile);

            Process.Start(PathCombine(CurrentDir, StartFile));
            Application.Current.Shutdown();

        }

        private bool CheckFile(FileList file)
        {
            var fileinfo = new FileInfo(file.Path);
            if (!File.Exists(file.Path)) return true;
            if (Tools.GetMd5(file.Path) != file.Md5) return true;
            if (fileinfo.Length != file.Size) return true;

            return false;

        }

        private string PathCombine(string path1, string path2)
        {
            if (Path.IsPathRooted(path2))
            {
                path2 = path2.TrimStart(Path.DirectorySeparatorChar);
                path2 = path2.TrimStart(Path.AltDirectorySeparatorChar);
            }

            return Path.Combine(path1, path2);
        }

        private void Md5()
        {
            List<string> fileList = new List<string>();
            var files = Directory.GetFiles(CurrentDir, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (Md5BlockFiles.Any(x => file.Contains(x))) continue;
                var fileinfo = new FileInfo(file);
                var md5 = Tools.GetMd5(file);
                var path = file.Replace(Directory.GetCurrentDirectory(), "");
                var compliteFile = $"{path}|{md5}|{fileinfo.Length};";
                fileList.Add(compliteFile);
            }

            File.WriteAllLines("files.list", fileList);

            Application.Current.Shutdown();
        }
    }
}
