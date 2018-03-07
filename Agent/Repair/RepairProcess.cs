using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

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
            List<string> coruptFiles = new List<string>();
            var webList = await Tools.Network.ReadTextAsync(new Uri("https://evgeniy-z.ru/files.txt?=5"));
            var ss = webList.Split(';');
            var serverFiles = (from webFile in ss
                where !string.IsNullOrWhiteSpace(webFile)
                select webFile.Split('|')
                into array
                select new FileList
                {
                    Path = PathCombine(Directory.GetCurrentDirectory(), array[0].Replace("\r\n", null)),
                    Md5 = array[1],
                    Size = Convert.ToInt64(array[2])
                }).ToList();


            foreach (var serverFile in serverFiles)
            {
                if (CheckFile(serverFile))
                {
                    coruptFiles.Add(serverFile.Path.Replace(Directory.GetCurrentDirectory(), ""));
                }
            }

            if (coruptFiles.Any())
            {
                if (Directory.Exists("tempDir")) Directory.Delete("tempDir", true);
                if (File.Exists("tempFiles")) File.Delete("tempFiles");
                Tools.Network.DownloadFile(new Uri("https://evgeniy-z.ru/wagent/download/release"), "tempFiles" );
                System.IO.Compression.ZipFile.ExtractToDirectory("tempFiles", "tempDir");

                foreach (var coruptFile in coruptFiles)
                {
                    var sourceFile = PathCombine(Directory.GetCurrentDirectory(), coruptFile);
                    var originFile = PathCombine(Path.Combine(Directory.GetCurrentDirectory(), "tempDir"), coruptFile);
                    var fileDirectory = Path.GetDirectoryName(sourceFile);


                    if (File.Exists(sourceFile)) File.Delete(sourceFile);

                    if (!Directory.Exists(fileDirectory)) Directory.CreateDirectory(fileDirectory);

                    File.Move(originFile, sourceFile);
                }
            }

            if (Directory.Exists("tempDir")) Directory.Delete("tempDir", true);
            if (File.Exists("tempFiles")) File.Delete("tempFiles");

            Process.Start(PathCombine(Directory.GetCurrentDirectory(), "Agent.exe"));
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
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (file.Contains("Repair.exe")) continue;
                var fileinfo = new FileInfo(file);
                var md5 = Tools.GetMd5(file);
                var path = file.Replace(Directory.GetCurrentDirectory(), "");
                var compliteFile = $"{path}|{md5}|{fileinfo.Length};";
                fileList.Add(compliteFile);
                Console.WriteLine(compliteFile);
            }

            File.WriteAllLines("files.list", fileList);

            Application.Current.Shutdown();
        }
    }
}
