using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public string Title { get; set; }
        //private string title; 
        //public string Title {
        //    get
        //    {
        //        return title;
        //    }
        //    set
        //    {
        //        title = char.ToUpper(value[0]) + value.Substring(1).ToLower();
        //    }
        //}
        public string Description { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
    }


}
