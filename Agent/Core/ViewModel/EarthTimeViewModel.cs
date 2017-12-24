using System;
using System.Windows.Media;
using System.Windows.Threading;

namespace Core.ViewModel
{
    public class EarthTimeViewModel : VM
    {
        public EarthTimeViewModel() => UpdateTime();

        DateTime time = new DateTime(2017, 12, 20);
        public DateTime Time
        {
            get => time;
            set => Set(ref time, value);
        }

        private string cycle;
        public string Cycle
        {
            get => cycle;
            set => Set(ref cycle, value);
        }

        private string cycleIcon;
        public string CycleIcon
        {
            get => cycleIcon;
            set => Set(ref cycleIcon, value);
        }

        private string colorOne;
        public string ColorOne
        {
            get => colorOne;
            set => Set(ref colorOne, value);
        }

        private string colorTwo;
        public string ColorTwo
        {
            get => colorTwo;
            set => Set(ref colorTwo, value);
        }

        public string LocationName { get; } = "Земля";

        public void UpdateTime()
        {

//function updateCetus() {
//    var color = 'black';
//    var timeLeft = 'Loading';
//    var cycle = '???';
//    if (syndicateData === undefined) {
//        pullSyndicates();
//    } else {
//        for (var i = 0; i < syndicateData.length; i++) {
//            if (syndicateData[i].Tag != 'CetusSyndicate')
//                continue;
//            var activation = syndicateData[i].Activation.sec * 1000 + syndicateData[i].Activation.usec;
//            var expiry = syndicateData[i].Expiry.sec * 1000 + syndicateData[i].Expiry.usec;
//            var now = moment().valueOf();
//            if (now > expiry) {
//                pullSyndicates();
//                break;
//            }
//            if (expiry - now > 0 && expiry - now < 30000) {
//                pullSyndicates();
//            }
//            if (now > activation && now < expiry) {
//                var timeSinceDusk = now - activation;
//                if (timeSinceDusk < 100 * 60 * 1000) {
//                    color = 'orange';
//                    cycle = 'Day';
//                    timeLeft = 'Time left: ' + formatCetus((100 * 60 * 1000 - timeSinceDusk) / 1000);
//                } else {
//                    color = 'darkblue';
//                    cycle = 'Night';
//                    timeLeft = 'Time left: ' + formatCetus((150 * 60 * 1000 - timeSinceDusk) / 1000);
//                }
//            }
//            break;
//        }
//    }
//    $('#cetusdaynight').text(cycle).css('color', color);
//    $('#cetusdaynight-timeleft').text(timeLeft);
//}

//function formatCetus(t) {
//    var hours = Math.floor(t / 3600);
//    var minutes = Math.ceil((t - (hours * 3600)) / 60);
//    var ret = '';
//    if (hours > 0)
//        ret = hours + 'h ';
//    ret += minutes + 'm';
//    return ret;
//}


            var now = DateTime.Now;

            var hour = Math.Floor((double)(Tools.Time.ToUnixTime(DateTime.Now) / 3600000)) % 24;
            var hour1 = now.Hour;

            if ((hour >= 0 && hour < 4) || (hour >= 8 && hour < 12) || (hour >= 16 && hour < 20))
            {
                Cycle = "День";
                CycleIcon = "SunOutline";
                ColorOne = "#CCFFA500";
                ColorTwo = "#4CFFA500";
            }
            else
            {
                Cycle = "Ночь";
                CycleIcon = "MoonOutline";
                ColorOne = "#CC3782CD";
                ColorTwo = "#4C3782CD";
            }

            var hourleft = 3 - (hour % 4);
            var minutes = 59 - now.Minute;
            var seconds = 59 - now.Second;

            Time = now.Date + new TimeSpan((int)hourleft, minutes, seconds);
        }
    }
}
