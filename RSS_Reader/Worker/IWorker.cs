using System.Threading.Tasks;
using System.Timers;

namespace RSS_Reader.Worker
{
    
    interface IWorker
    {
        Task Start();

        void Stop();
    }
}
