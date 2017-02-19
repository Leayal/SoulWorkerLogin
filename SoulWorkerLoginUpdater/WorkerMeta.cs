namespace SoulWorkerLoginUpdater
{
    public class WorkerMeta
    {
        public string Patch { get; }
        public string Destination { get; }

        public WorkerMeta(string sPatch, string sDestination)
        {
            this.Patch = sPatch;
            this.Destination = sDestination;
        }
    }
}
