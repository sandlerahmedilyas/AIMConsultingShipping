namespace AIMShipping.ClientConsole
{
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // let's set up our startup options/services etc...
            var services = Startup.ConfigureServices();

            // finally, build it!
            using (var sp = services.BuildServiceProvider())
            {

                // and now run our main entry point which we abstracted away for a few reasons including making it easier to port to a different platform.
                await sp.GetService<MainEntryPoint>().Run(args);
            }
        }
    }
}
