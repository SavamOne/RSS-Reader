using System.Threading.Tasks;

namespace RSS_Reader.Worker
{
    /// <summary>
    /// Интерфейс Worker'a
    /// </summary>
    interface IWorker
    {
        Task Start();

        void Stop();
    }
}
