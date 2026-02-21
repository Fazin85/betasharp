using BetaSharp.Blocks;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;

namespace BetaSharp;

public class RailLogic
{
    private World _worldObj;
    private Vec3i _trackPos;
    private readonly bool _isPoweredRail;
    private readonly List<Vec3i> _connectedTracks = [];
    readonly BlockRail _rail;

    public RailLogic(BlockRail railBlock, World world, Vec3i pos)
    {
        _rail = railBlock;
        _worldObj = world;
        _trackPos = pos;

        int blockId = world.getBlockId(pos.x, pos.y, pos.z);
        int meta = world.getBlockMeta(pos.x, pos.y, pos.z);

        if (Block.Blocks[blockId] is BlockRail rail && rail.isAlwaysStraight())
        {
            _isPoweredRail = true;
            meta &= -9;
        }
        else
        {
            _isPoweredRail = false;
        }

        SetConnections(meta);
    }

    public void UpdateState(bool powered, bool forceUpdate)
    {
        bool north = AttemptConnectionAt(new Vec3i(_trackPos.x, _trackPos.y, _trackPos.z - 1));
        bool south = AttemptConnectionAt(new Vec3i(_trackPos.x, _trackPos.y, _trackPos.z + 1));
        bool west = AttemptConnectionAt(new Vec3i(_trackPos.x - 1, _trackPos.y, _trackPos.z));
        bool east = AttemptConnectionAt(new Vec3i(_trackPos.x + 1, _trackPos.y, _trackPos.z));

        int meta = -1;
        if ((north || south) && !west && !east) meta = 0;
        if ((west || east) && !north && !south) meta = 1;

        if (!_isPoweredRail)
        {
            if (south && east && !north && !west) meta = 6;
            if (south && west && !north && !east) meta = 7;
            if (north && west && !south && !east) meta = 8;
            if (north && east && !south && !west) meta = 9;
        }

        if (meta == -1)
        {
            if (north || south) meta = 0;
            if (west || east) meta = 1;

            if (!_isPoweredRail)
            {
                if (powered)
                {
                    if (south && east) meta = 6;
                    if (west && south) meta = 7;
                    if (east && north) meta = 9;
                    if (north && west) meta = 8;
                }
                else
                {
                    if (north && west) meta = 8;
                    if (east && north) meta = 9;
                    if (west && south) meta = 7;
                    if (south && east) meta = 6;
                }
            }
        }

        if (meta == 0)
        {
            if (BlockRail.isRail(_worldObj, _trackPos.x, _trackPos.y + 1, _trackPos.z - 1)) meta = 4;
            if (BlockRail.isRail(_worldObj, _trackPos.x, _trackPos.y + 1, _trackPos.z + 1)) meta = 5;
        }

        if (meta == 1)
        {
            if (BlockRail.isRail(_worldObj, _trackPos.x + 1, _trackPos.y + 1, _trackPos.z)) meta = 2;
            if (BlockRail.isRail(_worldObj, _trackPos.x - 1, _trackPos.y + 1, _trackPos.z)) meta = 3;
        }

        if (meta < 0) meta = 0;

        SetConnections(meta);

        int finalMeta = meta;
        if (_isPoweredRail)
        {
            finalMeta = _worldObj.getBlockMeta(_trackPos.x, _trackPos.y, _trackPos.z) & 8 | meta;
        }

        if (forceUpdate || _worldObj.getBlockMeta(_trackPos.x, _trackPos.y, _trackPos.z) != finalMeta)
        {
            _worldObj.setBlockMeta(_trackPos.x, _trackPos.y, _trackPos.z, finalMeta);

            foreach (Vec3i pos in _connectedTracks)
            {
                RailLogic? logic = GetMinecartTrackLogic(pos);
                if (logic != null)
                {
                    logic.RefreshConnectedTracks();
                    if (logic.CanConnectTo(this))
                    {
                        logic.ConnectTo(this);
                    }
                }
            }
        }
    }

    public static int GetNAdjacentTracks(RailLogic logic) => logic.GetAdjacentTracks();

    private void SetConnections(int meta)
    {
        _connectedTracks.Clear();

        int trackX = _trackPos.x;
        int trackY = _trackPos.y;
        int trackZ = _trackPos.z;

        _connectedTracks.AddRange(meta switch
        {
            0 => [new Vec3i(trackX, trackY, trackZ - 1), new Vec3i(trackX, trackY, trackZ + 1)],
            1 => [new Vec3i(trackX - 1, trackY, trackZ), new Vec3i(trackX + 1, trackY, trackZ)],
            2 => [new Vec3i(trackX - 1, trackY, trackZ), new Vec3i(trackX + 1, trackY + 1, trackZ)],
            3 => [new Vec3i(trackX - 1, trackY + 1, trackZ), new Vec3i(trackX + 1, trackY, trackZ)],
            4 => [new Vec3i(trackX, trackY + 1, trackZ - 1), new Vec3i(trackX, trackY, trackZ + 1)],
            5 => [new Vec3i(trackX, trackY, trackZ - 1), new Vec3i(trackX, trackY + 1, trackZ + 1)],
            6 => [new Vec3i(trackX + 1, trackY, trackZ), new Vec3i(trackX, trackY, trackZ + 1)],
            7 => [new Vec3i(trackX - 1, trackY, trackZ), new Vec3i(trackX, trackY, trackZ + 1)],
            8 => [new Vec3i(trackX - 1, trackY, trackZ), new Vec3i(trackX, trackY, trackZ - 1)],
            9 => [new Vec3i(trackX + 1, trackY, trackZ), new Vec3i(trackX, trackY, trackZ - 1)],
            _ => []
        });
    }

    private void RefreshConnectedTracks()
    {
        for (int i = _connectedTracks.Count - 1; i >= 0; i--)
        {
            Vec3i pos = _connectedTracks[i];
            RailLogic? logic = GetMinecartTrackLogic(pos);

            if (logic != null && logic.IsConnectedTo(this))
            {
                _connectedTracks[i] = new Vec3i(logic._trackPos.x, logic._trackPos.y, logic._trackPos.z);
            }
            else
            {
                _connectedTracks.RemoveAt(i);
            }
        }
    }

    private bool IsMinecartTrack(Vec3i pos)
    {
        return BlockRail.isRail(_worldObj, pos.x, pos.y, pos.z) ||
               BlockRail.isRail(_worldObj, pos.x, pos.y + 1, pos.z) ||
               BlockRail.isRail(_worldObj, pos.x, pos.y - 1, pos.z);
    }

    private RailLogic? GetMinecartTrackLogic(Vec3i pos)
    {
        if (BlockRail.isRail(_worldObj, pos.x, pos.y, pos.z))
            return new RailLogic(_rail, _worldObj, pos);

        if (BlockRail.isRail(_worldObj, pos.x, pos.y + 1, pos.z))
            return new RailLogic(_rail, _worldObj, new Vec3i(pos.x, pos.y + 1, pos.z));

        if (BlockRail.isRail(_worldObj, pos.x, pos.y - 1, pos.z))
            return new RailLogic(_rail, _worldObj, new Vec3i(pos.x, pos.y - 1, pos.z));

        return null;
    }

    private bool IsConnectedTo(RailLogic targetLogic)
    {
        foreach (Vec3i pos in _connectedTracks)
        {
            if (pos.x == targetLogic._trackPos.x && pos.z == targetLogic._trackPos.z) return true;
        }
        return false;
    }

    private bool IsInTrack(Vec3i pos)
    {
        foreach (Vec3i connectedPos in _connectedTracks)
        {
            if (connectedPos.x == pos.x && connectedPos.z == pos.z) return true;
        }
        return false;
    }

    private int GetAdjacentTracks()
    {
        int count = 0;
        if (IsMinecartTrack(new Vec3i(_trackPos.x, _trackPos.y, _trackPos.z - 1))) ++count;
        if (IsMinecartTrack(new Vec3i(_trackPos.x, _trackPos.y, _trackPos.z + 1))) ++count;
        if (IsMinecartTrack(new Vec3i(_trackPos.x - 1, _trackPos.y, _trackPos.z))) ++count;
        if (IsMinecartTrack(new Vec3i(_trackPos.x + 1, _trackPos.y, _trackPos.z))) ++count;
        return count;
    }

    private bool CanConnectTo(RailLogic targetLogic)
    {
        if (IsConnectedTo(targetLogic)) return true;
        if (_connectedTracks.Count == 2) return false;
        if (_connectedTracks.Count == 0) return true;

        // This logic originally returned true regardless of the condition in decompiled source.
        // It's a known Beta 1.7.3 quirk. Kept original behavior but cleaned up.
        // Maybe this should be removed!!!
        Vec3i pos = _connectedTracks[0];
        return true;
    }

    private void ConnectTo(RailLogic targetLogic)
    {
        _connectedTracks.Add(new Vec3i(targetLogic._trackPos.x, targetLogic._trackPos.y, targetLogic._trackPos.z));

        bool north = IsInTrack(new Vec3i(_trackPos.x, _trackPos.y, _trackPos.z - 1));
        bool south = IsInTrack(new Vec3i(_trackPos.x, _trackPos.y, _trackPos.z + 1));
        bool west = IsInTrack(new Vec3i(_trackPos.x - 1, _trackPos.y, _trackPos.z));
        bool east = IsInTrack(new Vec3i(_trackPos.x + 1, _trackPos.y, _trackPos.z));

        int meta = -1;
        if (north || south) meta = 0;
        if (west || east) meta = 1;

        if (!_isPoweredRail)
        {
            if (south && east && !north && !west) meta = 6;
            if (south && west && !north && !east) meta = 7;
            if (north && west && !south && !east) meta = 8;
            if (north && east && !south && !west) meta = 9;
        }

        if (meta == 0)
        {
            if (BlockRail.isRail(_worldObj, _trackPos.x, _trackPos.y + 1, _trackPos.z - 1)) meta = 4;
            if (BlockRail.isRail(_worldObj, _trackPos.x, _trackPos.y + 1, _trackPos.z + 1)) meta = 5;
        }

        if (meta == 1)
        {
            if (BlockRail.isRail(_worldObj, _trackPos.x + 1, _trackPos.y + 1, _trackPos.z)) meta = 2;
            if (BlockRail.isRail(_worldObj, _trackPos.x - 1, _trackPos.y + 1, _trackPos.z)) meta = 3;
        }

        if (meta < 0) meta = 0;

        int finalMeta = meta;
        if (_isPoweredRail)
        {
            finalMeta = _worldObj.getBlockMeta(_trackPos.x, _trackPos.y, _trackPos.z) & 8 | meta;
        }

        _worldObj.setBlockMeta(_trackPos.x, _trackPos.y, _trackPos.z, finalMeta);
    }

    private bool AttemptConnectionAt(Vec3i pos)
    {
        RailLogic? logic = GetMinecartTrackLogic(pos);
        if (logic == null) return false;

        logic.RefreshConnectedTracks();
        return logic.CanConnectTo(this);
    }
}