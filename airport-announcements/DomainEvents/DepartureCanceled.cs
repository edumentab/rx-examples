using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Announcements.DomainEvents
{
    public class DepartureCanceled
    {
        public string FlightCode;
        public string Destination;
    }
}
