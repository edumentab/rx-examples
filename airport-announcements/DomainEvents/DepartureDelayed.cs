using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Announcements.DomainEvents
{
    public class DepartureDelayed
    {
        public string FlightCode;
        public string Destination;
        public int ScheduledHour;
        public int ScheduledMinute;
        public int NewHour;
        public int NewMinute;
    }
}
