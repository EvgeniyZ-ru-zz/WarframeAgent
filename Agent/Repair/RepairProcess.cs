using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var webList = await Tools.Network.ReadTextAsync(new Uri("https://evgeniy-z.ru/files.txt"));
            var ss = webList.Split(';');
            var serverFiles = (from webFile in ss
                where !string.IsNullOrWhiteSpace(webFile)
                select webFile.Split('|')
                into array
                select new FileList
                {
                    Path = array[0].Replace("\r\n", null),
                    Md5 = array[1],
                    Size = Convert.ToInt64(array[2])
                }).ToList();




        }

        private void Md5()
        {
            List<string> fileList = new List<string>();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileinfo = new FileInfo(file);
                var md5 = Tools.GetMd5(file);
                var path = file.Replace(Directory.GetCurrentDirectory(), "");
                fileList.Add($"{path}|{md5}|{fileinfo.Length};");
            }

            File.WriteAllLines("files.list", fileList);
        }
    }
}
