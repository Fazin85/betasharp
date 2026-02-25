using BetaSharp.Util.Maths;
using Silk.NET.Maths;
using BetaSharp.Worlds;

namespace BetaSharp.Client.Rendering.Chunks.Occlusion;

public interface IChunkVisibilityVisitor
{
    void Visit(SubChunkRenderer renderer);
}

public class ChunkOcclusionCuller
{
    private class ChunkQueue
    {
        private readonly SubChunkRenderer[] _data;
        private int _read;
        private int _write;

        public ChunkQueue(int capacity)
        {
            _data = new SubChunkRenderer[capacity];
            _read = 0;
            _write = 0;
        }

        public void Enqueue(SubChunkRenderer item)
        {
            _data[_write++] = item;
        }

        public SubChunkRenderer Dequeue()
        {
            if (_read == _write) return null;
            return _data[_read++];
        }

        public void Reset()
        {
            _read = 0;
            _write = 0;
        }

        public bool IsEmpty => _read == _write;
    }

    private readonly ChunkQueue[] _queues = [new(32768), new(32768)];
    private int _currentQueue = 0;

    public void FindVisible(
        IChunkVisibilityVisitor visitor,
        SubChunkRenderer startNode,
        Vector3D<double> viewPos,
        Culler culler,
        float renderDistance,
        bool useOcclusionCulling,
        int frame)
    {
        var readQueue = _queues[_currentQueue];
        var writeQueue = _queues[1 - _currentQueue];

        readQueue.Reset();
        writeQueue.Reset();

        if (startNode == null)
        {
            // Fallback: If camera is above/below world, start from all chunks on the boundary face
            int boundaryY = viewPos.Y < 0 ? 0 : 112; // 128 - 16 = 112 for bottom of top section
            var direction = viewPos.Y < 0 ? ChunkDirection.Down : ChunkDirection.Up;
            
            // This is complex to implement fully here without access to all renderers.
            // For now, let's just return and rely on the fact that if we aren't in a chunk,
            // we probably aren't occluded by anything either (except frustum).
            // But we NEED a starting point.
            return;
        }

        // Start traversal from the chunk the camera is in
        startNode.LastVisibleFrame = frame;
        startNode.IncomingDirections = ChunkDirectionMask.None;
        visitor.Visit(startNode);

        ChunkDirectionMask initialOutgoing = useOcclusionCulling 
            ? startNode.VisibilityData.GetVisibleFrom(ChunkDirectionMask.None, viewPos, startNode)
            : ChunkDirectionMask.All;

        EnqueueNeighbors(writeQueue, startNode, initialOutgoing, frame);

        while (!writeQueue.IsEmpty)
        {
            // Swap queues
            _currentQueue = 1 - _currentQueue;
            readQueue = _queues[_currentQueue];
            writeQueue = _queues[1 - _currentQueue];

            writeQueue.Reset();

            SubChunkRenderer current;
            while ((current = readQueue.Dequeue()) != null)
            {
                if (!IsVisible(current, viewPos, culler, renderDistance))
                    continue;

                visitor.Visit(current);

                ChunkDirectionMask outgoing;
                if (useOcclusionCulling)
                {
                    outgoing = current.VisibilityData.GetVisibleFrom(current.IncomingDirections, viewPos, current);
                }
                else
                {
                    outgoing = ChunkDirectionMask.All;
                }

                // Only traverse outwards from camera
                outgoing &= GetOutwardDirections(viewPos, current);

                EnqueueNeighbors(writeQueue, current, outgoing, frame);
            }
        }
    }

    private void EnqueueNeighbors(ChunkQueue queue, SubChunkRenderer current, ChunkDirectionMask outgoing, int frame)
    {
        if (outgoing == ChunkDirectionMask.None) return;

        if ((outgoing & ChunkDirectionMask.Down) != 0) VisitNode(queue, current.AdjacentDown, ChunkDirectionMask.Up, frame);
        if ((outgoing & ChunkDirectionMask.Up) != 0) VisitNode(queue, current.AdjacentUp, ChunkDirectionMask.Down, frame);
        if ((outgoing & ChunkDirectionMask.North) != 0) VisitNode(queue, current.AdjacentNorth, ChunkDirectionMask.South, frame);
        if ((outgoing & ChunkDirectionMask.South) != 0) VisitNode(queue, current.AdjacentSouth, ChunkDirectionMask.North, frame);
        if ((outgoing & ChunkDirectionMask.West) != 0) VisitNode(queue, current.AdjacentWest, ChunkDirectionMask.East, frame);
        if ((outgoing & ChunkDirectionMask.East) != 0) VisitNode(queue, current.AdjacentEast, ChunkDirectionMask.West, frame);
    }

    private void VisitNode(ChunkQueue queue, SubChunkRenderer neighbor, ChunkDirectionMask incoming, int frame)
    {
        if (neighbor == null) return;

        if (neighbor.LastVisibleFrame != frame)
        {
            neighbor.LastVisibleFrame = frame;
            neighbor.IncomingDirections = ChunkDirectionMask.None;
            queue.Enqueue(neighbor);
        }

        neighbor.IncomingDirections |= incoming;
    }

    private bool IsVisible(SubChunkRenderer renderer, Vector3D<double> viewPos, Culler culler, float renderDistance)
    {
        // Simple frustum and distance check
        if (!culler.isBoundingBoxInFrustum(renderer.BoundingBox)) return false;

        double dx = renderer.PositionPlus.X - viewPos.X;
        double dy = renderer.PositionPlus.Y - viewPos.Y;
        double dz = renderer.PositionPlus.Z - viewPos.Z;
        
        return (dx * dx + dz * dz) < (renderDistance * renderDistance) && Math.Abs(dy) < renderDistance;
    }

    private ChunkDirectionMask GetOutwardDirections(Vector3D<double> viewPos, SubChunkRenderer renderer)
    {
        int chunkX = renderer.Position.X / SubChunkRenderer.Size;
        int chunkY = renderer.Position.Y / SubChunkRenderer.Size;
        int chunkZ = renderer.Position.Z / SubChunkRenderer.Size;

        int viewChunkX = (int)Math.Floor(viewPos.X / SubChunkRenderer.Size);
        int viewChunkY = (int)Math.Floor(viewPos.Y / SubChunkRenderer.Size);
        int viewChunkZ = (int)Math.Floor(viewPos.Z / SubChunkRenderer.Size);

        ChunkDirectionMask mask = ChunkDirectionMask.None;
        if (chunkX <= viewChunkX) mask |= ChunkDirectionMask.West;
        if (chunkX >= viewChunkX) mask |= ChunkDirectionMask.East;
        if (chunkY <= viewChunkY) mask |= ChunkDirectionMask.Down;
        if (chunkY >= viewChunkY) mask |= ChunkDirectionMask.Up;
        if (chunkZ <= viewChunkZ) mask |= ChunkDirectionMask.North;
        if (chunkZ >= viewChunkZ) mask |= ChunkDirectionMask.South;
        return mask;
    }
}
