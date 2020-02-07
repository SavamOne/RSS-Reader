using System.Timers;

namespace RSS_Reader.Worker
{
    
    interface IWorker
    {
        void Start();

        void Stop();
    }
}
