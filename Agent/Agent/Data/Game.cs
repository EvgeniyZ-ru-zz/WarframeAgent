using System.IO;
using Core.GameData;
using Newtonsoft.Json;

namespace Agent.Data
{
    internal class Game
    {
        public static GameJsonView Read(string fileName)
        {
            GameJsonView data;
            using (var file = File.OpenText(fileName))
            {
                var serializer = new JsonSerializer();
                data = (GameJsonView)serializer.Deserialize(file, typeof(GameJsonView));
            }

            return data;
        }
    }

    public static class Json
    {
        public static GameJsonView Model;

        public static void Load(string filename)
        {
            Model = Game.Read(filename);
        }
    }
}
