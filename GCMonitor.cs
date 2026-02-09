using System.Diagnostics;

namespace betareborn
{
    public sealed class GCMonitor : IDisposable
    {
        public long MaxMemoryBytes { get; private set; }
        public long UsedMemoryBytes { get; private set; }
        public long UsedHeapBytes { get; private set; }

        private readonly System.Threading.Timer _timer;
        private readonly Process _process;

        private const int UpdateIntervalMs = 250;

        public GCMonitor()
        {
            _process = Process.GetCurrentProcess();
            MaxMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

            _timer = new System.Threading.Timer(
                _ => Update(),
                null,
                0,
                UpdateIntervalMs
            );
        }

        private void Update()
        {
            UsedMemoryBytes = _process.WorkingSet64;
            UsedHeapBytes = GC.GetTotalMemory(false);
        }

        public void Dispose()
        {
            _timer.Dispose();
            _process.Dispose();
        }
    }
}
