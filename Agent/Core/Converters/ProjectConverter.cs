using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Core.Converters
{
    [Obsolete]
    public class ProjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var list = new List<ProjectsModel>();
            var token = JToken.Load(reader);
            for (var i = 0; i < token.Count(); i++)
            {
                if (token[i] != null && i == 0 && (double) token[i] > 0)
                {
                    list.Add(new ProjectsModel
                    {
                        Name = "Фоморианец",
                        Value = (double) token[i],
                        Color = Brushes.DarkRed
                    });
                    continue;
                }

                if (token[i] != null && i == 1 && (double) token[i] > 0)
                {
                    list.Add(new ProjectsModel
                    {
                        Name = "Армада Секачей",
                        Value = (double) token[i],
                        Color = Brushes.Teal
                    });
                    continue;
                }

                if (token[i] != null && (double) token[i] > 0)
                    list.Add(new ProjectsModel {Name = "NULL", Value = (double) token[i]});
            }
            return list.ToArray();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}