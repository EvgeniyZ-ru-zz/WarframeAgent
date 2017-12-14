using System;
using System.Collections.Generic;
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

namespace Agent.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для HomePanel.xaml
    /// </summary>
    public partial class HomePanel : UserControl
    {
        public HomePanel()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(HomePanel), new PropertyMetadata("Text"));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Brush), typeof(HomePanel), new PropertyMetadata(Brushes.Red));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Brush Color
        {
            get => (Brush) GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

    }
}
