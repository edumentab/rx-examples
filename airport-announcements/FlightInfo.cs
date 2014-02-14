using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Announcements
{
    public class FlightInfo
    {
        public string FlightCode { get; private set; }
        public string Destination { get; private set; }
        public byte Hour { get; private set; }
        public byte Minute { get; private set; }

        public FlightInfo(string flightCode, string destination, byte hour, byte minute)
        {
            this.FlightCode = flightCode;
            this.Destination = destination;
            this.Hour = hour;
            this.Minute = minute;
        }
    }
}
