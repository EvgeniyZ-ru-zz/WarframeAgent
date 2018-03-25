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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Agent.View
{
    public partial class ToastWindow : Window
    {
        double right, bottom;
        public ToastWindow()
        {
            InitializeComponent();
            right = SystemParameters.WorkArea.Right - 10;
            bottom = SystemParameters.WorkArea.Bottom - 10;
        }

        void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left = right - ActualWidth;
            Top = bottom - ActualHeight;
        }

        public new void Show()
        {
            base.Show();
            var storyboard = (Storyboard)Resources["In"];
            BeginStoryboard(storyboard);
        }

        public new void Close()
        {
            var storyboard = (Storyboard)Resources["Out"];
            storyboard.Completed += (o, args) => base.Close();
            BeginStoryboard(storyboard);
        }
    }
}
