using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Model;
using Newtonsoft.Json.Linq;

namespace Core.ViewModel
{
    public class ItemComparer : IEqualityComparer<Item>
    {
        public bool Equals(Item x, Item y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(Item obj)
        {
            return obj.Name.GetHashCode();
        }
    }

    public class FiltersViewModel
    {
        public class Items
        {
            public static async void Update(string name, string url, ApplicationContext db)
            {
                var jsonText = Tools.Network.ReadText(url);
                var json = JObject.Parse(jsonText);
                await db.Items.LoadAsync();
                var dbCount = db.Items.Count();
                var version = 0;

                var type = "";
                var jsonList = new HashSet<Item>();
                var dbitems = new HashSet<Item>(db.Items.Local.ToList());

                #region Считываем JSON файл

                foreach (var val in json["Items"])
                foreach (var jToken in val)
                {
                    var item = (JProperty)jToken;
                    if (item.Name == "type")
                        type = item.Value.ToString();

                    if (item.Name != "type" && item.Name != "enable")
                        jsonList.Add(new Item()
                        {
                            Type = type,
                            Name = item.Name,
                            Value = item.Value.ToString()
                        });
                }

                var jsonCount = jsonList.Count;

                #endregion

                if (json["Version"] != null) version = (int) json["Version"];

                if (jsonCount != dbCount || Settings.Program.Verisons.Items < version)
                {
                    Debug.WriteLine($"Начинаю обновление базы [Items] с версии {Settings.Program.Verisons.Items} на {version}!");
                    var newValues = jsonList.Except(dbitems, new ItemComparer()).ToList();
                    var oldValues = dbitems.Except(jsonList, new ItemComparer()).ToList();

                    #region Удаляю старые значения

                    foreach (var delItem in oldValues)
                    {
                        db.Items.Remove(delItem);
                        Debug.WriteLine($"Удаляю предмет из базы [Items]: {delItem.Name}");
                    }

                    #endregion

                    #region Добавление новых значений

                    foreach (var addItem in newValues)
                    {
                        db.Items.Add(addItem);
                        Debug.WriteLine($"Добавляю предмет в базу [Items]: {addItem.Name}");
                    }

                    #endregion

                    await db.SaveChangesAsync();
                    Debug.WriteLine("База успешно сохранена!");
                }
                else
                {
                    Debug.WriteLine($"База [Items] не требует обновления v{version}!");
                }
            }
        }
    }
}
