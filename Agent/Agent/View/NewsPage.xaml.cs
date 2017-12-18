using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Core.ViewModel;

namespace Agent.View
{
    /// <summary>
    ///     Логика взаимодействия для NewsPage.xaml
    /// </summary>
    public partial class NewsPage
    {
        public NewsPage()
        {
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            if (item?.Content is PostViewModel obj)
                Process.Start(obj.Url.AbsoluteUri);
        }
    }
}