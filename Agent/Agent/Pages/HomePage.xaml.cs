using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Core.Model;

namespace Agent.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = MainWindow.GameData;
            NewsPanel.DataContext = MainWindow.NewsData;
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListBoxItem;
            var obj = item?.Content as Post;
            if (obj != null) Process.Start(obj.Url);
        }
    }
}
