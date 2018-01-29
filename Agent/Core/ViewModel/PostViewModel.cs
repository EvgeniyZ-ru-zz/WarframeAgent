using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Core.Model;

namespace Core.ViewModel
{
    public class PostViewModel : VM
    {
        public PostViewModel(NewsPost post)
        {
            Title = post.Title;
            Date = post.Date;
            Description = post.Description;
            var url = post.Image.StartsWith("//") ? "http:" : "https://www.warframe.com";
            Image = url + post.Image;
            Url = post.Url;
            DisplayInBrowserCommand = new RelayCommand(DisplayInBrowser);
        }

        void DisplayInBrowser()
        {
            // запуск просесса длительный, поэтому выносим в фон
            var uri = Url.AbsoluteUri;
            Task.Run(() => Process.Start(uri));
        }

        public ICommand DisplayInBrowserCommand { get; }

        public string Title { get; }
        public DateTime Date { get; }
        public string Description { get; }
        public string Image { get; }
        public Uri Url { get; }
    }
}
