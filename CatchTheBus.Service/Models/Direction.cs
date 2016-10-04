using System.Collections.Generic;
using CatchTheBus.Service.Constants;

namespace CatchTheBus.Service.Models
{
    public class Direction
    {
        public DirectionType Type { get; set; }

        public string Description { get; set; }

        public List<BusStop> BusStops { get; set; } 
    }
}
