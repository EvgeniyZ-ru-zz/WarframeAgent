using System;
using Core.Model;

namespace Core.ViewModel
{
    public class PostViewModel
    {
        public PostViewModel(NewsPost post)
        {
            Title = post.Title;
            Date = post.Date;
            Description = post.Description;
            Image = post.Image;
            Url = post.Url;
        }

        public string Title { get; }
        public DateTime Date { get; }
        public string Description { get; }
        public string Image { get; }
        public Uri Url { get; }
    }
}
