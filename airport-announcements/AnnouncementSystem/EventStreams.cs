using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Announcements.DomainEvents;

namespace Announcements.AnnouncementSystem
{
    /// <summary>
    /// In reality, these domain events would come from some kind of message
    /// bus. But for the sake of this simpler demonstration, we will just have
    /// a bunch of Subject instances, and send the events out on them.
    /// </summary>
    public static class EventStreams
    {
        public static readonly Subject<DepartureCanceled> DepartureCanceled
            = new Subject<DepartureCanceled>();
        public static readonly Subject<DepartureDelayed> DepartureDelayed
            = new Subject<DepartureDelayed>();
        public static readonly Subject<DepartureReturnedToOnTime> DepartureReturnedToOnTime
            = new Subject<DepartureReturnedToOnTime>();
        public static readonly Subject<GateAssigned> GateAssigned
            = new Subject<GateAssigned>();
        public static readonly Subject<GateChanged> GateChanged
            = new Subject<GateChanged>();
        public static readonly Subject<GateOpened> GateOpened
            = new Subject<GateOpened>();
    }
}
