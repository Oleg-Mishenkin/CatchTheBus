using Nancy;

namespace CatchTheBus.Service.ApiModules
{
    public class NancyTestModule : NancyModule
    {
        public NancyTestModule()
        {
            Get["/{category}"] = parameters => "My category is " + parameters.category;

            Get["/sayhello"] = _ => "Hello from Nancy";

        }
    }
}
