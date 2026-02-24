namespace BetaSharp;

// Note: there is regression compared to og beta, events are ordered only by their scheduled time but not also by their insertion order
public record struct BlockUpdate(int X, int Y, int Z, int BlockId, long ScheduledTime);
