using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
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

    public class UpdateModel
    {
        public string File { get; set; }
        public string Md5 { get; set; }
        public long Length { get; set; }
    }

    public class RepairProcess : VM
    {
        public enum Mod
        {
            Update,
            Md5,
            Default
        }

        public enum Revision
        {
            Release,
            Develop
        }

        private static readonly string CurrentDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static readonly string UpdateUri = "https://evgeniy-z.ru/wagent/download/";
        private static readonly string FileListUri = "https://evgeniy-z.ru/api/v2/Agent/GetFiles?type=";
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

        public async Task Start(Mod command = Mod.Default, Revision revision = Revision.Release)
        {
            switch (command)
            {
                case Mod.Update:
                    Status = "Update started...".ToUpper();
                    break;
                case Mod.Md5:
                    Status = "Generate MD5...".ToUpper();
                    Md5();
                    Status = "MD5 generated!".ToUpper();
                    break;
                case Mod.Default:
                    Status = "Repair started...".ToUpper();
                    await StartRepair(revision);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, null);
            }

            await Task.Delay(1000);
        }

        private async Task StartRepair(Revision revision)
        {
            //TODO: Ping test
            var webList = await Tools.Network.ReadTextAsync(new Uri(FileListUri + revision.ToString().ToLower()));


            List<UpdateModel> deserializedFiles = new List<UpdateModel>();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(webList));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedFiles.GetType());
            deserializedFiles = ser.ReadObject(ms) as List<UpdateModel>;
            ms.Close();

            var serverFiles = deserializedFiles.Select(x => new FileList
            {
                Path = PathCombine(CurrentDir, x.File.Replace("\r\n", null)),
                Md5 = x.Md5,
                Size = x.Length
            });


            List<string> coruptFiles = (from serverFile in serverFiles where CheckFile(serverFile) select serverFile.Path.Replace(CurrentDir, "")).ToList();

            if (coruptFiles.Any()) File.WriteAllLines(PathCombine(CurrentDir, "CoruptFiles.list"), coruptFiles);

            if (coruptFiles.Any())
            {
                if (Directory.Exists(TempDir)) Directory.Delete(TempDir, true);
                if (File.Exists(TempFile)) File.Delete(TempFile);
                Tools.Network.DownloadFile(new Uri(UpdateUri + revision.ToString().ToLower()), TempFile);
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

            File.WriteAllLines("_files.sum", fileList);

            Application.Current.Shutdown();
        }
    }
}
