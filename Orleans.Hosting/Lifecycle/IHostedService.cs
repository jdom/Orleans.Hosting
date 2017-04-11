using System.Threading.Tasks;

namespace Orleans.Hosting
{
    // BTW, Jason is working to create a fine-grained application lifecycle abstraction. This is just the one from AspNetCore right now.
    public interface IHostedService
    {
        Task Start();
        Task Stop();
    }
}
