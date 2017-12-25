using System;

namespace Core.ViewModel
{
    public class CetusTimeViewModel : VM
    {
        public CetusTimeViewModel() => UpdateTime();

        private DateTime time;
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

        public string LocationName { get; } = "NULL";

        public void UpdateTime()
        {
            #region JS Code

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

            #endregion
        }
    }
}
