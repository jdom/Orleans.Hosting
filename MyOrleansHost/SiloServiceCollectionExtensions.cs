using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    //
    // Summary:
    //     Extension methods for setting up MVC services in an Microsoft.Extensions.DependencyInjection.IServiceCollection.
    public static class SiloServiceCollectionExtensions
    {
        //
        // Summary:
        //     Adds MVC services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection.
        //
        // Parameters:
        //   services:
        //     The Microsoft.Extensions.DependencyInjection.IServiceCollection to add services
        //     to.
        //
        // Returns:
        //     An Microsoft.Extensions.DependencyInjection.IMvcBuilder that can be used to further
        //     configure the MVC services.
        public static void AddOrleans(this IServiceCollection services) { }

        public static void AddTestHooks(this IServiceCollection services) { }
    }
}