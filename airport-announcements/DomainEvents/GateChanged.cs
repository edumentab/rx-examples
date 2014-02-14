using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Announcements.DomainEvents
{
    public class GateChanged
    {
        public string FlightCode;
        public string Destination;
        public string NewGate;
    }
}
