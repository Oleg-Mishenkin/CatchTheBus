using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CatchTheBus.Service.Tasks;

namespace CatchTheBus.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            new TrackScheduleTask().Execute();
        }
    }
}
