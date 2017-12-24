using System;
using System.Windows.Media;
using System.Windows.Threading;
using Core.Model;

namespace Core.ViewModel
{
    public class CetusTimeViewModel : VM
    {
        public CetusTimeViewModel() => UpdateTime();

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

        public string LocationName { get; } = "NULL";

        public void UpdateTime()
        {

            //            function updateTime() {
            //    $(".time").each(function(index) {
            //        var starttime = moment.unix($(this).data("starttime"));
            //        var endtime = moment.unix($(this).data("endtime"));
            //        var timetext = "";
            //        if (moment() < starttime) {
            //            var duration = moment.duration(starttime.diff(moment(), 'seconds'), 'seconds');
            //            if (duration.days() != 0)
            //                timetext = "Start: " + pad2(duration.days()) + "d " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else if (duration.hours() != 0)
            //                timetext = "Start: " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else
            //                timetext = "Start: " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            $(this).css("background-color", "#999");
            //        } else if (starttime < moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            if (duration.days() != 0)
            //                timetext = pad2(duration.days()) + "d " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else
            //                timetext = pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            $(this).css("background-color", "green");
            //            if (endtime < moment()) {
            //                $(this).css("background-color", "red");
            //                timetext = "Expired: " + timetext;
            //            }
            //        }
            //        $(this).html("<span title=\"Start: " + starttime.format("dddd, MMMM Do YYYY, h:mm:ss a") + " End: " + endtime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    $(".vttime").each(function(index) {
            //        var starttime = moment.unix($(this).data("starttime"));
            //        var endtime = moment.unix($(this).data("endtime"));
            //        var timetext = "";
            //        if (moment() < starttime) {
            //            var duration = moment.duration(starttime.diff(moment(), 'seconds'), 'seconds');
            //            if (duration.days() != 0)
            //                timetext = "Start: " + pad2(duration.days()) + "d " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else if (duration.hours() != 0)
            //                timetext = "Start: " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else
            //                timetext = "Start: " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //        } else if (starttime < moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            if (duration.days() != 0)
            //                timetext = pad2(duration.days()) + "d " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else
            //                timetext = pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            $(this).css("color", "green");
            //            if (endtime < moment()) {
            //                timetext = "Expired: " + timetext;
            //                $(this).css("color", "red");
            //            }
            //        }
            //        $(this).html("<span title=\"Start: " + starttime.format("dddd, MMMM Do YYYY, h:mm:ss a") + " End: " + endtime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    $(".bltime").each(function(index) {
            //        var endtime = moment.unix($(this).data("endtime"));
            //        var timetext = "Time remaining: ";
            //        if (endtime > moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += pad2(duration.hours() + duration.days() * 24) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //        } else if (endtime < moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += "-" + pad2(duration.hours() + duration.days() * -24) + "h " + pad2(duration.minutes() * -1) + "m " + pad2(duration.seconds() * -1) + "s";
            //        }
            //        $(this).html("<span title=\"End: " + endtime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    $(".sortietime").each(function(index) {
            //        var endtime = moment.unix($(this).data("endtime"));
            //        var timetext = "";
            //        if (endtime > moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += pad2(duration.hours() + duration.days() * 24) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //        } else if (endtime < moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += "-" + pad2(duration.hours() + duration.days() * -24) + "h " + pad2(duration.minutes() * -1) + "m " + pad2(duration.seconds() * -1) + "s";
            //        }
            //        $(this).html("<span title=\"End: " + endtime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    $(".bldeploytime").each(function(index) {
            //        var endtime = moment.unix($(this).data("time"));
            //        var timetext = "Time to deployment: ";
            //        if (endtime > moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += pad2(duration.hours() + duration.days() * 24) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //        } else if (endtime < moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += "-" + pad2(duration.hours() + duration.days() * -24) + "h " + pad2(duration.minutes() * -1) + "m " + pad2(duration.seconds() * -1) + "s";
            //        }
            //        $(this).html("<span title=\"Deployment: " + endtime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    $(".taxtime").each(function(index) {
            //        var endtime = moment.unix($(this).data("time"));
            //        var timetext = "Allowed in: ";
            //        if (endtime > moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += pad2(duration.hours() + duration.days() * 24) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //        } else if (endtime < moment()) {
            //            timetext = "Tribute change allowed.";
            //        }
            //        $(this).html("<span title=\"Allowed: " + endtime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    $(".header-time").each(function(index) {
            //        if ($(this).data("time") == '0')
            //            return;
            //        var endtime = moment.unix($(this).data("time"));
            //        var timetext = "";
            //        if (endtime > moment()) {
            //            var duration = moment.duration(endtime.diff(moment(), 'seconds'), 'seconds');
            //            timetext += pad2(duration.hours() + duration.days() * 24) + ":" + pad2(duration.minutes()) + ":" + pad2(duration.seconds()) + ' |';
            //        } else if (endtime < moment()) {
            //            timetext = "Completed.";
            //        }
            //        $(this).html("<span title=\"" + endtime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    $(".target-time").each(function(index) {
            //        var starttime = moment.unix($(this).data("starttime"));
            //        var timetext = "";
            //        var duration = '';
            //        if (moment() < starttime) {
            //            var duration = moment.duration(starttime.diff(moment(), 'seconds'), 'seconds');
            //            if (duration.days() != 0)
            //                timetext = "Start: " + pad2(duration.days()) + "d " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else if (duration.hours() != 0)
            //                timetext = "Start: " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else
            //                timetext = "Start: " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //        } else if (starttime < moment()) {
            //            var duration = moment.duration(moment().diff(starttime, 'seconds'), 'seconds');
            //            if (duration.days() != 0)
            //                timetext = pad2(duration.days()) + "d " + pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            else
            //                timetext = pad2(duration.hours()) + "h " + pad2(duration.minutes()) + "m " + pad2(duration.seconds()) + "s";
            //            $(this).css("background-color", "green");
            //        }
            //        $(this).html("<span title=\"" + starttime.format("dddd, MMMM Do YYYY, h:mm:ss a") + "\">" + timetext + "</span>");
            //    });
            //    var hour = Math.floor(moment().valueOf() / 3600000) % 24;
            //    var cycle = 'Night';
            //    var colour = 'darkblue';
            //    if ((hour >= 0 && hour < 4) || (hour >= 8 && hour < 12) || (hour >= 16 && hour < 20)) {
            //        cycle = 'Day';
            //        colour = 'orange';
            //    }
            //    var hourleft = 3 - (hour % 4);
            //    minutes = 59 - moment().minutes();
            //    seconds = 59 - moment().seconds();
            //    $('#daynight').text(cycle).css('color', colour);
            //    $('#daynight-timeleft').text("Time left: " + pad2(hourleft) + "h " + pad2(minutes) + "m " + pad2(seconds) + "s");
            //    updateCetus();
            //}


        }
    }
}
