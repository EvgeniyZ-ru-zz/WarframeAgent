using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Model;

namespace Core.ViewModel
{
    public class NotificationViewModel
    {
        public string Text { get; }

        public NotificationViewModel(Alert ntf)
        {
            Text = ntf.MissionInfo.Location;
        }
    }
}
