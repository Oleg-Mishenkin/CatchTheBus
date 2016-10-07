using System.Collections.Generic;
using CatchTheBus.Service.Constants;

namespace CatchTheBus.Service.Models
{
    public class Direction
    {
        public DirectionType Type { get; set; }

        public string Description { get; set; }

        public Dictionary<string, List<TimeEntry>> BusStops { get; set; } 
    }
}
