using System.Collections.Generic;
using System.Windows.Media;

namespace Agent.ViewModel
{
    public enum NotificationTarget
    {
        Alert,
        Invasion
    }

    interface INotificationTargetDescription
    {
        string Name { get; }
        Brush Color { get; }
        NotificationTarget Target { get; }
    }

    static class NotificationTargetDescriptions
    {
        public static IEnumerable<INotificationTargetDescription> All { get; } =
            new[]
            {
                // TODO: подумать, как синхронизировать цвета с константами в Light.xaml
                new NotificationTargetDescription(NotificationTarget.Alert, "Тревога", new SolidColorBrush(Color.FromRgb(0x6E, 0xCD, 0x37))),
                new NotificationTargetDescription(NotificationTarget.Invasion, "Вторжение", new SolidColorBrush(Color.FromRgb(0x7B, 0x37, 0xCD)))
            };

        class NotificationTargetDescription : INotificationTargetDescription
        {
            public NotificationTargetDescription(NotificationTarget target, string name, Brush color) =>
                (Target, Name, Color) = (target, name, color);

            public string Name { get; }
            public Brush Color { get; }
            public NotificationTarget Target { get; }
        }
    }
}
