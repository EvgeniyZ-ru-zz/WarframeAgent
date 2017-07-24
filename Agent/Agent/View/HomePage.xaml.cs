using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using Core.Model;

namespace Agent.View
{
    /// <summary>
    ///     Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            var obj = item?.Content as Post;
            if (obj != null) Process.Start(obj.Url);
        }
    }
}