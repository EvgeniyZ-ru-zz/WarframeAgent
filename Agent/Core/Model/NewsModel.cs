using System.Collections.Generic;
using System.Diagnostics;

namespace Core.Model
{
    public class NewsModel
    {
        public bool HasMore { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        public string Date { get; set; }
        private string _title;
        public string Title
        {
            get => _title;
            set => _title = value.ToUpper();
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
