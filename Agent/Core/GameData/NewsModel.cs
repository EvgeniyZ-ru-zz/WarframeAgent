using System.Collections.Generic;
using System.Diagnostics;

namespace Core.GameData
{
    public class NewsView
    {
        public bool HasMore { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public string Date { get; set; }
        //public string Title { get; set; }
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                //_title = char.ToUpper(value[0]) + value.Substring(1).ToLower();
                _title = value.ToUpper();
            }
        }
        public string Description { get; set; }
        public string Url { get; set; }

        private string _image;
        public string Image
        {
            get => _image;
            set => _image = "http:" + value;
        }
    }


}
