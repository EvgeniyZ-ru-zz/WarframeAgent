using System.IO;
using Core.GameData;
using Newtonsoft.Json;
using Core;

namespace Agent.Data
{
    /// <summary>
    ///     Взаимодействие с игровыми данными.
    /// </summary>
    public class Game : VM
    {
        private GameView Read(string fileName)
        {
            GameView data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (GameView)serializer.Deserialize(file, typeof(GameView));
            }

            return data;
        }

        GameView _data;
        public GameView Data { get => _data; set => Set(ref _data, value); }

        public void Load(string filename = "temp")
        {
            if (filename == "temp")
                filename = $"{Settings.Program.Directories.Temp}/GameData.json";
            Data = Read(filename);
        }
    }
}
