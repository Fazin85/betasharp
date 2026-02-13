namespace betareborn.Client
{
    public class Timer : java.lang.Object
    {
        public float ticksPerSecond;
        private double lastHRTime;
        public int elapsedTicks;
        public float renderPartialTicks;
        public float timerSpeed = 1.0F;
        public float elapsedPartialTicks = 0.0F;
        public float deltaTime;
        private long lastSyncSysClock;
        private long lastSyncHRClock;
        private long field_28132_i;
        private double timeSyncAdjustment = 1.0D;

        public Timer(float var1)
        {
            ticksPerSecond = var1;
            lastSyncSysClock = java.lang.System.currentTimeMillis();
            lastSyncHRClock = java.lang.System.nanoTime() / 1000000L;
        }

        public void updateTimer()
        {
            long var1 = java.lang.System.currentTimeMillis();
            long var3 = var1 - lastSyncSysClock;
            long var5 = java.lang.System.nanoTime() / 1000000L;
            double var7 = (double)var5 / 1000.0D;
            if (var3 > 1000L)
            {
                lastHRTime = var7;
            }
            else if (var3 < 0L)
            {
                lastHRTime = var7;
            }
            else
            {
                field_28132_i += var3;
                if (field_28132_i > 1000L)
                {
                    long var9 = var5 - lastSyncHRClock;
                    double var11 = (double)field_28132_i / (double)var9;
                    timeSyncAdjustment += (var11 - timeSyncAdjustment) * (double)0.2F;
                    lastSyncHRClock = var5;
                    field_28132_i = 0L;
                }

                if (field_28132_i < 0L)
                {
                    lastSyncHRClock = var5;
                }
            }

            lastSyncSysClock = var1;
            double var13 = (var7 - lastHRTime) * timeSyncAdjustment;
            deltaTime = (float)Math.Clamp(var13, 1.0f / 1000.0f, 1.0f);
            lastHRTime = var7;
            if (var13 < 0.0D)
            {
                var13 = 0.0D;
            }

            if (var13 > 1.0D)
            {
                var13 = 1.0D;
            }

            elapsedPartialTicks = (float)((double)elapsedPartialTicks + var13 * (double)timerSpeed * (double)ticksPerSecond);
            elapsedTicks = (int)elapsedPartialTicks;
            elapsedPartialTicks -= (float)elapsedTicks;
            if (elapsedTicks > 10)
            {
                elapsedTicks = 10;
            }

            renderPartialTicks = elapsedPartialTicks;
        }
    }
}