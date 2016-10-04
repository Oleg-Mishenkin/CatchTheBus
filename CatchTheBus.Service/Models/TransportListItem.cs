using System.Collections.Generic;

namespace CatchTheBus.Service.Models
{
    public class TransportListItem
    {
        public string Number { get; set; } 
        public string Description { get; set; } 
        public string Url { get; set; } 

        public Direction ForwardDirection { get; set; } 
        public Direction BackwardDirection { get; set; } 
    }
}