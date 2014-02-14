using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Announcements.AnnouncementSystem
{
    /// <summary>
    /// Implements the airport announcement logic, observing domain event
    /// streams and turning them into a single stream of strings to
    /// announce.
    /// </summary>
    public class Announcer
    {
        /// <summary>
        /// The stream of announcements.
        /// </summary>
        public IObservable<string> Announcements { get; private set; }

        /// <summary>
        /// Constructor does the interesting setup work.
        /// </summary>
        public Announcer()
        {
            Announcements = SecurityNotices()
                .Merge(Delays())
                .Merge(Cancellations())
                .Merge(GateChanges())
                .Merge(GateOpenings());
        }

        private IObservable<string> SecurityNotices()
        {
            return Observable.Interval(TimeSpan.FromSeconds(30))
                .Select(_ => "Please do not leave luggage unattended");
        }

        private IObservable<string> Delays()
        {
            return
                from delay in EventStreams.DepartureDelayed
                let duration =
                    (delay.NewHour * 60 + delay.NewMinute) -
                    (delay.ScheduledHour * 60 + delay.ScheduledMinute)
                where duration >= 10
                select string.Format(
                    "Flight {0} to {1} is delayed until {2} {3}",
                    delay.FlightCode, delay.Destination,
                    delay.NewHour, delay.NewMinute);
        }

        private IObservable<string> Cancellations()
        {
            return EventStreams.DepartureCanceled
                .Select(e => string.Format(
                    "Sorry, but flight {0} to {1} has been canceled",
                    e.FlightCode, e.Destination));
        }

        private IObservable<string> GateChanges()
        {
            return EventStreams.GateChanged
                .Select(e => string.Format(
                    "Flight {0} to {1} will now depart from gate {2}",
                    e.FlightCode, e.Destination, e.NewGate));
        }

        private IObservable<string> GateOpenings()
        {
            var latestGates = Observable.Merge(
                EventStreams.GateAssigned.Select(
                    e => Tuple.Create(e.FlightCode, e.Gate)),
                EventStreams.GateChanged.Select(
                    e => Tuple.Create(e.FlightCode, e.NewGate))
            ).Scan(
                ImmutableDictionary<string, string>.Empty,
                (agg, update) => agg.SetItem(update.Item1, update.Item2)
            );
            return EventStreams.GateOpened
                .CombineLatest(latestGates, Tuple.Create)
                .DistinctUntilChanged(e => e.Item1)
                .Select(e => string.Format(
                    "Flight {0} to {1} is now boarding at gate {2}",
                    e.Item1.FlightCode, e.Item1.Destination, 
                    e.Item2[e.Item1.FlightCode]));
        }
    }
}










