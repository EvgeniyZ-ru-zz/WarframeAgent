using System.Collections.Generic;

namespace Core.Model
{
    public class NewsModel
    {
        public bool HasMore { get; set; }
        public List<Post> Posts { get; set; }
    }

    public class Post
    {
        private string _image;
        private string _title;
        public string Date { get; set; }

        public string Title
        {
            get => _title;
            set => _title = value.ToUpper();
        }

        public string Description { get; set; }
        public string Url { get; set; }

        public string Image
        {
            get => _image;
            set => _image = "http:" + value;
        }
    }
}