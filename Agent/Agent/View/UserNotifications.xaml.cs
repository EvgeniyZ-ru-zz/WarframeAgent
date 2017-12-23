using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace Agent.View
{
    public partial class UserNotifications : UserControl
    {
        public UserNotifications()
        {
            InitializeComponent();
            DataContextChanged += SubscribeListChanges;
        }

        void SubscribeListChanges(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldCollection = e.OldValue as INotifyCollectionChanged;
            var newCollection = e.NewValue as INotifyCollectionChanged;

            if (oldCollection != null)
                oldCollection.CollectionChanged -= OnNotificationsChanged;
            Update();
            if (newCollection != null)
                newCollection.CollectionChanged += OnNotificationsChanged;
        }

        void OnNotificationsChanged(object sender, NotifyCollectionChangedEventArgs e) => Update();

        void Update()
        {
            int itemCount = 0;
            if (DataContext is IEnumerable<ViewModel.UserNotification> ie)
            {
                itemCount = ie.Count();
                if (itemCount == 1)
                {
                    CommonText = ie.First().Text;
                }
                else if (itemCount > 1)
                {
                    switch (GetNumCase(itemCount))
                    {
                    case 1:
                        CommonText = $"Произошло {itemCount} событие";
                        break;
                    case 2:
                        CommonText = $"Произошло {itemCount} события";
                        break;
                    case 3:
                        CommonText = $"Произошло {itemCount} событий";
                        break;
                    }
                }
            }
            CollectionHasItems = itemCount > 0;
            Core.Tools.Logging.Send(NLog.LogLevel.Trace, $"Представление: CollectionHasItems = {CollectionHasItems}, текст = {CommonText}");
        }

        int GetNumCase(int n) // TODO: это общелингвистическая функция, вынести в общее место
        {
            n = Math.Abs(n);
            var lastDigit = n % 10;
            var beforeLast = (n / 10) % 10;
            if (beforeLast == 1)
                return 3;
            if (lastDigit == 1)
                return 1;
            if (lastDigit > 1 && lastDigit < 5)
                return 2;
            return 3;
        }

        #region dp bool CollectionHasItems
        public bool CollectionHasItems
        {
            get { return (bool)GetValue(CollectionHasItemsProperty); }
            set { SetValue(CollectionHasItemsProperty, value); }
        }

        public static readonly DependencyProperty CollectionHasItemsProperty =
            DependencyProperty.Register("CollectionHasItems", typeof(bool), typeof(UserNotifications), new PropertyMetadata(false));
        #endregion

        #region dp string CommonText
        public string CommonText
        {
            get { return (string)GetValue(CommonTextProperty); }
            set { SetValue(CommonTextProperty, value); }
        }

        public static readonly DependencyProperty CommonTextProperty =
            DependencyProperty.Register("CommonText", typeof(string), typeof(UserNotifications), new PropertyMetadata(null));
        #endregion
    }
}
