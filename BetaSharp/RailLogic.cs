using BetaSharp.Blocks;
using BetaSharp.Util.Maths;
using BetaSharp.Worlds;
using java.util;

namespace BetaSharp;

public class RailLogic
{
    private World worldObj;
    private int trackX;
    private int trackY;
    private int trackZ;
    private readonly bool isPoweredRail;
    private List connectedTracks;
    readonly BlockRail rail;

    public RailLogic(BlockRail var1, World var2, int var3, int var4, int var5)
    {
        rail = var1;
        connectedTracks = new ArrayList();
        worldObj = var2;
        trackX = var3;
        trackY = var4;
        trackZ = var5;
        int var6 = var2.getBlockId(var3, var4, var5);
        int var7 = var2.getBlockMeta(var3, var4, var5);
        if (((BlockRail)Block.Blocks[var6]).isAlwaysStraight())
        {
            isPoweredRail = true;
            var7 &= -9;
        }
        else
        {
            isPoweredRail = false;
        }

        setConnections(var7);
    }

    private void setConnections(int var1)
    {
        connectedTracks.clear();
        if (var1 == 0)
        {
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ - 1));
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ + 1));
        }
        else if (var1 == 1)
        {
            connectedTracks.add(new BlockPos(trackX - 1, trackY, trackZ));
            connectedTracks.add(new BlockPos(trackX + 1, trackY, trackZ));
        }
        else if (var1 == 2)
        {
            connectedTracks.add(new BlockPos(trackX - 1, trackY, trackZ));
            connectedTracks.add(new BlockPos(trackX + 1, trackY + 1, trackZ));
        }
        else if (var1 == 3)
        {
            connectedTracks.add(new BlockPos(trackX - 1, trackY + 1, trackZ));
            connectedTracks.add(new BlockPos(trackX + 1, trackY, trackZ));
        }
        else if (var1 == 4)
        {
            connectedTracks.add(new BlockPos(trackX, trackY + 1, trackZ - 1));
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ + 1));
        }
        else if (var1 == 5)
        {
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ - 1));
            connectedTracks.add(new BlockPos(trackX, trackY + 1, trackZ + 1));
        }
        else if (var1 == 6)
        {
            connectedTracks.add(new BlockPos(trackX + 1, trackY, trackZ));
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ + 1));
        }
        else if (var1 == 7)
        {
            connectedTracks.add(new BlockPos(trackX - 1, trackY, trackZ));
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ + 1));
        }
        else if (var1 == 8)
        {
            connectedTracks.add(new BlockPos(trackX - 1, trackY, trackZ));
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ - 1));
        }
        else if (var1 == 9)
        {
            connectedTracks.add(new BlockPos(trackX + 1, trackY, trackZ));
            connectedTracks.add(new BlockPos(trackX, trackY, trackZ - 1));
        }

    }

    private void func_785_b()
    {
        for (int var1 = 0; var1 < connectedTracks.size(); ++var1)
        {
            RailLogic var2 = getMinecartTrackLogic((BlockPos)connectedTracks.get(var1));
            if (var2 != null && var2.isConnectedTo(this))
            {
                connectedTracks.set(var1, new BlockPos(var2.trackX, var2.trackY, var2.trackZ));
            }
            else
            {
                connectedTracks.remove(var1--);
            }
        }

    }

    private bool isMinecartTrack(int var1, int var2, int var3)
    {
        return BlockRail.isRail(worldObj, var1, var2, var3) ? true : (BlockRail.isRail(worldObj, var1, var2 + 1, var3) ? true : BlockRail.isRail(worldObj, var1, var2 - 1, var3));
    }

    private RailLogic getMinecartTrackLogic(BlockPos var1)
    {
        return BlockRail.isRail(worldObj, var1.x, var1.y, var1.z) ? new RailLogic(rail, worldObj, var1.x, var1.y, var1.z) : (BlockRail.isRail(worldObj, var1.x, var1.y + 1, var1.z) ? new RailLogic(rail, worldObj, var1.x, var1.y + 1, var1.z) : (BlockRail.isRail(worldObj, var1.x, var1.y - 1, var1.z) ? new RailLogic(rail, worldObj, var1.x, var1.y - 1, var1.z) : null));
    }

    private bool isConnectedTo(RailLogic var1)
    {
        for (int var2 = 0; var2 < connectedTracks.size(); ++var2)
        {
            BlockPos var3 = (BlockPos)connectedTracks.get(var2);
            if (var3.x == var1.trackX && var3.z == var1.trackZ)
            {
                return true;
            }
        }

        return false;
    }

    private bool isInTrack(int var1, int var2, int var3)
    {
        for (int var4 = 0; var4 < connectedTracks.size(); ++var4)
        {
            BlockPos var5 = (BlockPos)connectedTracks.get(var4);
            if (var5.x == var1 && var5.z == var3)
            {
                return true;
            }
        }

        return false;
    }

    private int getAdjacentTracks()
    {
        int var1 = 0;
        if (isMinecartTrack(trackX, trackY, trackZ - 1))
        {
            ++var1;
        }

        if (isMinecartTrack(trackX, trackY, trackZ + 1))
        {
            ++var1;
        }

        if (isMinecartTrack(trackX - 1, trackY, trackZ))
        {
            ++var1;
        }

        if (isMinecartTrack(trackX + 1, trackY, trackZ))
        {
            ++var1;
        }

        return var1;
    }

    private bool handleKeyPress(RailLogic var1)
    {
        if (isConnectedTo(var1))
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
            return var1.trackY == trackY && var2.y == trackY ? true : true;
        }
    }

    private void func_788_d(RailLogic var1)
    {
        connectedTracks.add(new BlockPos(var1.trackX, var1.trackY, var1.trackZ));
        bool var2 = isInTrack(trackX, trackY, trackZ - 1);
        bool var3 = isInTrack(trackX, trackY, trackZ + 1);
        bool var4 = isInTrack(trackX - 1, trackY, trackZ);
        bool var5 = isInTrack(trackX + 1, trackY, trackZ);
        int var6 = -1;
        if (var2 || var3)
        {
            var6 = 0;
        }

        if (var4 || var5)
        {
            var6 = 1;
        }

        if (!isPoweredRail)
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
            if (BlockRail.isRail(worldObj, trackX, trackY + 1, trackZ - 1))
            {
                var6 = 4;
            }

            if (BlockRail.isRail(worldObj, trackX, trackY + 1, trackZ + 1))
            {
                var6 = 5;
            }
        }

        if (var6 == 1)
        {
            if (BlockRail.isRail(worldObj, trackX + 1, trackY + 1, trackZ))
            {
                var6 = 2;
            }

            if (BlockRail.isRail(worldObj, trackX - 1, trackY + 1, trackZ))
            {
                var6 = 3;
            }
        }

        if (var6 < 0)
        {
            var6 = 0;
        }

        int var7 = var6;
        if (isPoweredRail)
        {
            var7 = worldObj.getBlockMeta(trackX, trackY, trackZ) & 8 | var6;
        }

        worldObj.setBlockMeta(trackX, trackY, trackZ, var7);
    }

    private bool func_786_c(int var1, int var2, int var3)
    {
        RailLogic var4 = getMinecartTrackLogic(new BlockPos(var1, var2, var3));
        if (var4 == null)
        {
            return false;
        }
        else
        {
            var4.func_785_b();
            return var4.handleKeyPress(this);
        }
    }

    public void updateState(bool var1, bool var2)
    {
        bool var3 = func_786_c(trackX, trackY, trackZ - 1);
        bool var4 = func_786_c(trackX, trackY, trackZ + 1);
        bool var5 = func_786_c(trackX - 1, trackY, trackZ);
        bool var6 = func_786_c(trackX + 1, trackY, trackZ);
        int var7 = -1;
        if ((var3 || var4) && !var5 && !var6)
        {
            var7 = 0;
        }

        if ((var5 || var6) && !var3 && !var4)
        {
            var7 = 1;
        }

        if (!isPoweredRail)
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

            if (!isPoweredRail)
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
            if (BlockRail.isRail(worldObj, trackX, trackY + 1, trackZ - 1))
            {
                var7 = 4;
            }

            if (BlockRail.isRail(worldObj, trackX, trackY + 1, trackZ + 1))
            {
                var7 = 5;
            }
        }

        if (var7 == 1)
        {
            if (BlockRail.isRail(worldObj, trackX + 1, trackY + 1, trackZ))
            {
                var7 = 2;
            }

            if (BlockRail.isRail(worldObj, trackX - 1, trackY + 1, trackZ))
            {
                var7 = 3;
            }
        }

        if (var7 < 0)
        {
            var7 = 0;
        }

        setConnections(var7);
        int var8 = var7;
        if (isPoweredRail)
        {
            var8 = worldObj.getBlockMeta(trackX, trackY, trackZ) & 8 | var7;
        }

        if (var2 || worldObj.getBlockMeta(trackX, trackY, trackZ) != var8)
        {
            worldObj.setBlockMeta(trackX, trackY, trackZ, var8);

            for (int var9 = 0; var9 < connectedTracks.size(); ++var9)
            {
                RailLogic var10 = getMinecartTrackLogic((BlockPos)connectedTracks.get(var9));
                if (var10 != null)
                {
                    var10.func_785_b();
                    if (var10.handleKeyPress(this))
                    {
                        var10.func_788_d(this);
                    }
                }
            }
        }

    }

    public static int getNAdjacentTracks(RailLogic var0)
    {
        return var0.getAdjacentTracks();
    }
}