using System.Diagnostics;

namespace betareborn.Util
{
    public sealed class GCMonitor : IDisposable
    {
        public long MaxMemoryBytes { get; private set; }
        public long UsedMemoryBytes { get; private set; }
        public long UsedHeapBytes { get; private set; }
        public bool AllowUpdating { get; set; } = true;

        private readonly Timer _timer;
        private readonly Process _process;

        private const int UpdateIntervalMs = 250;

        public GCMonitor()
        {
            _process = Process.GetCurrentProcess();
            MaxMemoryBytes = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

            _timer = new Timer(
                _ => Update(),
                null,
                0,
                UpdateIntervalMs
            );
        }

        private void Update()
        {
            if (AllowUpdating)
            {
                UsedMemoryBytes = _process.WorkingSet64;
                UsedHeapBytes = GC.GetTotalMemory(false);
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _process.Dispose();
        }
    }
}
