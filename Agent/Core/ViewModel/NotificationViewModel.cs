using Core.Model;

namespace Core.ViewModel
{
    public class NotificationViewModel
    {
        public NotificationViewModel(Alert ntf)
        {
            Id = ntf.Id.Oid;
            Text = ntf.MissionInfo.Location;
        }

        public string Id { get; }
        public string Text { get; }
    }
}