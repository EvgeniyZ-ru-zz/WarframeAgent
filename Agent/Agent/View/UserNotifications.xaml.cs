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
                    ActiveNotification = ie.Single();
                }
                else if (itemCount > 1)
                {
                    string text = null;
                    switch (GetNumCase(itemCount))
                    {
                    case 1:
                        text = $"Произошло {itemCount} событие";
                        break;
                    case 2:
                        text = $"Произошло {itemCount} события";
                        break;
                    case 3:
                        text = $"Произошло {itemCount} событий";
                        break;
                    }
                    ActiveNotification = new CollectedNotification(text);
                }
            }
            CollectionHasItems = itemCount > 0;
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

        #region dp ViewModel.UserNotification ActiveNotification
        public ViewModel.UserNotification ActiveNotification
        {
            get { return (ViewModel.UserNotification)GetValue(ActiveNotificationProperty); }
            set { SetValue(ActiveNotificationProperty, value); }
        }

        public static readonly DependencyProperty ActiveNotificationProperty =
            DependencyProperty.Register("ActiveNotification", typeof(ViewModel.UserNotification), typeof(UserNotifications));
        #endregion
    }

    class CollectedNotification : ViewModel.UserNotification
    {
        public string Text { get; }
        public CollectedNotification(string text) => Text = text;
    }
}
