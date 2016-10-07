using System;
using Nancy.Hosting.Self;

namespace CatchTheBus.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
			// new TrackScheduleTask().Execute();

			using (var host = new NancyHost(new Uri("http://localhost:8080")))
			{
				host.Start();
				Console.WriteLine("Running on http://localhost:8080");
				Console.ReadLine();
			}
		}
    }
}
