using System.Threading.Tasks;

namespace RSS_Reader.Worker
{
    interface IWorker
    {
        Task Start();

        void Stop();
    }
}
