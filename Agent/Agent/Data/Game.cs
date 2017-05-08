using System.IO;
using Core.GameData;
using Newtonsoft.Json;

namespace Agent.Data
{
    /// <summary>
    ///     Взаимодействие с игровыми данными.
    /// </summary>
    internal class Game : VM
    {
        private static GameView Read(string fileName)
        {
            GameView data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (GameView)serializer.Deserialize(file, typeof(GameView));
            }

            return data;
        }

        GameView data;
        public GameView Data { get => data; set => Set(ref data, value); }

        public void Load(string filename = "temp")
        {
            if (filename == "temp")
                filename = $"{Settings.Program.Directories.Temp}/GameData.json";
            Data = Read(filename);
        }
    }
}
