using Core.ViewModel;

namespace Agent.ViewModel
{
    public abstract class UserNotification { }

    public class AlertUserNotification : UserNotification
    {
        public AlertViewModel Alert { get; }
        public AlertUserNotification(AlertViewModel alert) => Alert = alert;
        public override string ToString() => $"Нотификация о тревоге: {Alert}";
    }

    public class InvasionUserNotification : UserNotification
    {
        public InvasionViewModel Invasion { get; }
        public InvasionUserNotification(InvasionViewModel invasion) => Invasion = invasion;
        public override string ToString() => $"Нотификация о вторжении: {Invasion}";
    }
}
