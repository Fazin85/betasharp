using BetaSharp.Blocks;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using java.util;

namespace BetaSharp;

public class RailLogic
{
    private World _worldObj;
    private int _trackX;
    private int _trackY;
    private int _trackZ;
    private readonly bool _isPoweredRail;
    private List connectedTracks;
    readonly BlockRail _rail;

    public RailLogic(BlockRail railBlock, World world, int x, int y, int z)
    {
        _rail = railBlock;
        connectedTracks = new ArrayList();
        _worldObj = world;
        _trackX = x;
        _trackY = y;
        _trackZ = z;
        int blockId = world.getBlockId(x, y, z);
        int meta = world.getBlockMeta(x, y, z);
        if (((BlockRail)Block.Blocks[blockId]).isAlwaysStraight())
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

    private void SetConnections(int meta)
    {
        connectedTracks.clear();
        if (meta == 0)
        {
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ - 1));
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ + 1));
        }
        else if (meta == 1)
        {
            connectedTracks.add(new BlockPos(_trackX - 1, _trackY, _trackZ));
            connectedTracks.add(new BlockPos(_trackX + 1, _trackY, _trackZ));
        }
        else if (meta == 2)
        {
            connectedTracks.add(new BlockPos(_trackX - 1, _trackY, _trackZ));
            connectedTracks.add(new BlockPos(_trackX + 1, _trackY + 1, _trackZ));
        }
        else if (meta == 3)
        {
            connectedTracks.add(new BlockPos(_trackX - 1, _trackY + 1, _trackZ));
            connectedTracks.add(new BlockPos(_trackX + 1, _trackY, _trackZ));
        }
        else if (meta == 4)
        {
            connectedTracks.add(new BlockPos(_trackX, _trackY + 1, _trackZ - 1));
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ + 1));
        }
        else if (meta == 5)
        {
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ - 1));
            connectedTracks.add(new BlockPos(_trackX, _trackY + 1, _trackZ + 1));
        }
        else if (meta == 6)
        {
            connectedTracks.add(new BlockPos(_trackX + 1, _trackY, _trackZ));
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ + 1));
        }
        else if (meta == 7)
        {
            connectedTracks.add(new BlockPos(_trackX - 1, _trackY, _trackZ));
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ + 1));
        }
        else if (meta == 8)
        {
            connectedTracks.add(new BlockPos(_trackX - 1, _trackY, _trackZ));
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ - 1));
        }
        else if (meta == 9)
        {
            connectedTracks.add(new BlockPos(_trackX + 1, _trackY, _trackZ));
            connectedTracks.add(new BlockPos(_trackX, _trackY, _trackZ - 1));
        }

    }

    private void RefreshConnectedTracks()
    {
        for (int var1 = 0; var1 < connectedTracks.size(); ++var1)
        {
            RailLogic var2 = GetMinecartTrackLogic((BlockPos)connectedTracks.get(var1));
            if (var2 != null && var2.IsConnectedTo(this))
            {
                connectedTracks.set(var1, new BlockPos(var2._trackX, var2._trackY, var2._trackZ));
            }
            else
            {
                connectedTracks.remove(var1--);
            }
        }

    }

    private bool IsMinecartTrack(int x, int y, int z)
    {
        return BlockRail.isRail(_worldObj, x, y, z) ? true : (BlockRail.isRail(_worldObj, x, y + 1, z) ? true : BlockRail.isRail(_worldObj, x, y - 1, z));
    }

    private RailLogic? GetMinecartTrackLogic(BlockPos pos)
    {
        return BlockRail.isRail(_worldObj, pos.x, pos.y, pos.z) ? new RailLogic(_rail, _worldObj, pos.x, pos.y, pos.z) : (BlockRail.isRail(_worldObj, pos.x, pos.y + 1, pos.z) ? new RailLogic(_rail, _worldObj, pos.x, pos.y + 1, pos.z) : (BlockRail.isRail(_worldObj, pos.x, pos.y - 1, pos.z) ? new RailLogic(_rail, _worldObj, pos.x, pos.y - 1, pos.z) : null));
    }

    private bool IsConnectedTo(RailLogic var1)
    {
        for (int var2 = 0; var2 < connectedTracks.size(); ++var2)
        {
            BlockPos var3 = (BlockPos)connectedTracks.get(var2);
            if (var3.x == var1._trackX && var3.z == var1._trackZ)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsInTrack(int x, int y, int z)
    {
        for (int var4 = 0; var4 < connectedTracks.size(); ++var4)
        {
            BlockPos var5 = (BlockPos)connectedTracks.get(var4);
            if (var5.x == x && var5.z == z)
            {
                return true;
            }
        }

        return false;
    }

    private int GetAdjacentTracks()
    {
        int var1 = 0;
        if (IsMinecartTrack(_trackX, _trackY, _trackZ - 1))
        {
            ++var1;
        }

        if (IsMinecartTrack(_trackX, _trackY, _trackZ + 1))
        {
            ++var1;
        }

        if (IsMinecartTrack(_trackX - 1, _trackY, _trackZ))
        {
            ++var1;
        }

        if (IsMinecartTrack(_trackX + 1, _trackY, _trackZ))
        {
            ++var1;
        }

        return var1;
    }

    private bool CanConnectTo(RailLogic var1)
    {
        if (IsConnectedTo(var1))
        {
            return true;
        }
        else if (connectedTracks.size() == 2)
        {
            return false;
        }
        else if (connectedTracks.size() == 0)
        {
            return true;
        }
        else
        {
            BlockPos var2 = (BlockPos)connectedTracks.get(0);
            return var1._trackY == _trackY && var2.y == _trackY ? true : true;
        }
    }

    private void ConnectTo(RailLogic targetLogic)
    {
        connectedTracks.add(new BlockPos(targetLogic._trackX, targetLogic._trackY, targetLogic._trackZ));
        bool var2 = IsInTrack(_trackX, _trackY, _trackZ - 1);
        bool var3 = IsInTrack(_trackX, _trackY, _trackZ + 1);
        bool var4 = IsInTrack(_trackX - 1, _trackY, _trackZ);
        bool var5 = IsInTrack(_trackX + 1, _trackY, _trackZ);
        int var6 = -1;
        if (var2 || var3)
        {
            var6 = 0;
        }

        if (var4 || var5)
        {
            var6 = 1;
        }

        if (!_isPoweredRail)
        {
            if (var3 && var5 && !var2 && !var4)
            {
                var6 = 6;
            }

            if (var3 && var4 && !var2 && !var5)
            {
                var6 = 7;
            }

            if (var2 && var4 && !var3 && !var5)
            {
                var6 = 8;
            }

            if (var2 && var5 && !var3 && !var4)
            {
                var6 = 9;
            }
        }

        if (var6 == 0)
        {
            if (BlockRail.isRail(_worldObj, _trackX, _trackY + 1, _trackZ - 1))
            {
                var6 = 4;
            }

            if (BlockRail.isRail(_worldObj, _trackX, _trackY + 1, _trackZ + 1))
            {
                var6 = 5;
            }
        }

        if (var6 == 1)
        {
            if (BlockRail.isRail(_worldObj, _trackX + 1, _trackY + 1, _trackZ))
            {
                var6 = 2;
            }

            if (BlockRail.isRail(_worldObj, _trackX - 1, _trackY + 1, _trackZ))
            {
                var6 = 3;
            }
        }

        if (var6 < 0)
        {
            var6 = 0;
        }

        int var7 = var6;
        if (_isPoweredRail)
        {
            var7 = _worldObj.getBlockMeta(_trackX, _trackY, _trackZ) & 8 | var6;
        }

        _worldObj.setBlockMeta(_trackX, _trackY, _trackZ, var7);
    }

    private bool AttemptConnectionAt(int x, int y, int z)
    {
        RailLogic var4 = GetMinecartTrackLogic(new BlockPos(x, y, z));
        if (var4 == null)
        {
            return false;
        }
        else
        {
            var4.RefreshConnectedTracks();
            return var4.CanConnectTo(this);
        }
    }

    public void UpdateState(bool var1, bool var2)
    {
        bool var3 = AttemptConnectionAt(_trackX, _trackY, _trackZ - 1);
        bool var4 = AttemptConnectionAt(_trackX, _trackY, _trackZ + 1);
        bool var5 = AttemptConnectionAt(_trackX - 1, _trackY, _trackZ);
        bool var6 = AttemptConnectionAt(_trackX + 1, _trackY, _trackZ);
        int var7 = -1;
        if ((var3 || var4) && !var5 && !var6)
        {
            var7 = 0;
        }

        if ((var5 || var6) && !var3 && !var4)
        {
            var7 = 1;
        }

        if (!_isPoweredRail)
        {
            if (var4 && var6 && !var3 && !var5)
            {
                var7 = 6;
            }

            if (var4 && var5 && !var3 && !var6)
            {
                var7 = 7;
            }

            if (var3 && var5 && !var4 && !var6)
            {
                var7 = 8;
            }

            if (var3 && var6 && !var4 && !var5)
            {
                var7 = 9;
            }
        }

        if (var7 == -1)
        {
            if (var3 || var4)
            {
                var7 = 0;
            }

            if (var5 || var6)
            {
                var7 = 1;
            }

            if (!_isPoweredRail)
            {
                if (var1)
                {
                    if (var4 && var6)
                    {
                        var7 = 6;
                    }

                    if (var5 && var4)
                    {
                        var7 = 7;
                    }

                    if (var6 && var3)
                    {
                        var7 = 9;
                    }

                    if (var3 && var5)
                    {
                        var7 = 8;
                    }
                }
                else
                {
                    if (var3 && var5)
                    {
                        var7 = 8;
                    }

                    if (var6 && var3)
                    {
                        var7 = 9;
                    }

                    if (var5 && var4)
                    {
                        var7 = 7;
                    }

                    if (var4 && var6)
                    {
                        var7 = 6;
                    }
                }
            }
        }

        if (var7 == 0)
        {
            if (BlockRail.isRail(_worldObj, _trackX, _trackY + 1, _trackZ - 1))
            {
                var7 = 4;
            }

            if (BlockRail.isRail(_worldObj, _trackX, _trackY + 1, _trackZ + 1))
            {
                var7 = 5;
            }
        }

        if (var7 == 1)
        {
            if (BlockRail.isRail(_worldObj, _trackX + 1, _trackY + 1, _trackZ))
            {
                var7 = 2;
            }

            if (BlockRail.isRail(_worldObj, _trackX - 1, _trackY + 1, _trackZ))
            {
                var7 = 3;
            }
        }

        if (var7 < 0)
        {
            var7 = 0;
        }

        SetConnections(var7);
        int var8 = var7;
        if (_isPoweredRail)
        {
            var8 = _worldObj.getBlockMeta(_trackX, _trackY, _trackZ) & 8 | var7;
        }

        if (var2 || _worldObj.getBlockMeta(_trackX, _trackY, _trackZ) != var8)
        {
            _worldObj.setBlockMeta(_trackX, _trackY, _trackZ, var8);

            for (int var9 = 0; var9 < connectedTracks.size(); ++var9)
            {
                RailLogic var10 = GetMinecartTrackLogic((BlockPos)connectedTracks.get(var9));
                if (var10 != null)
                {
                    var10.RefreshConnectedTracks();
                    if (var10.CanConnectTo(this))
                    {
                        var10.ConnectTo(this);
                    }
                }
            }
        }

    }

    public static int GetNAdjacentTracks(RailLogic logic) => logic.GetAdjacentTracks();
}