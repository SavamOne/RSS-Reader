using System.Timers;

namespace RSS_Reader.Worker
{
    interface IWorker
    {
        Timer Timer { get; set; }

        double Interval { get; set; }

        bool WorkIsDone { get; set; }

    }
}
