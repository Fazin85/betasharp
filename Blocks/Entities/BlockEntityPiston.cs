using betareborn.Entities;
using betareborn.NBT;
using betareborn.Util.Maths;

namespace betareborn.Blocks.Entities
{
    public class BlockEntityPiston : BlockEntity
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(BlockEntityPiston).TypeHandle);

        private int pushedBlockId;
        private int pushedBlockData;
        private int facing;
        private bool extending;
        private readonly bool source;
        private float lastProgess;
        private float progress;
        private static readonly List<Entity> pushedEntities = [];

        public BlockEntityPiston()
        {
        }

        public BlockEntityPiston(int pushedBlockId, int pushedBlockData, int facing, bool extending, bool source)
        {
            this.pushedBlockId = pushedBlockId;
            this.pushedBlockData = pushedBlockData;
            this.facing = facing;
            this.extending = extending;
            this.source = source;
        }

        public int getPushedBlockId()
        {
            return pushedBlockId;
        }

        public override int getPushedBlockData()
        {
            return pushedBlockData;
        }

        public bool isExtending()
        {
            return extending;
        }

        public int getFacing()
        {
            return facing;
        }

        public bool isSource()
        {
            return source;
        }

        public float getProgress(float tickDelta)
        {
            if (tickDelta > 1.0F)
            {
                tickDelta = 1.0F;
            }

            return progress + (lastProgess - progress) * tickDelta;
        }

        public float getRenderOffsetX(float tickDelta)
        {
            return extending ? (getProgress(tickDelta) - 1.0F) * PistonConstants.HEAD_OFFSET_X[facing] : (1.0F - getProgress(tickDelta)) * PistonConstants.HEAD_OFFSET_X[facing];
        }

        public float getRenderOffsetY(float tickDelta)
        {
            return extending ? (getProgress(tickDelta) - 1.0F) * PistonConstants.HEAD_OFFSET_Y[facing] : (1.0F - getProgress(tickDelta)) * PistonConstants.HEAD_OFFSET_Y[facing];
        }

        public float getRenderOffsetZ(float tickDelta)
        {
            return extending ? (getProgress(tickDelta) - 1.0F) * PistonConstants.HEAD_OFFSET_Z[facing] : (1.0F - getProgress(tickDelta)) * PistonConstants.HEAD_OFFSET_Z[facing];
        }

        private void pushEntities(float collisionShapeSizeMultiplier, float entityMoveMultiplier)
        {
            if (!extending)
            {
                --collisionShapeSizeMultiplier;
            }
            else
            {
                collisionShapeSizeMultiplier = 1.0F - collisionShapeSizeMultiplier;
            }

            Box? pushCollisionBox = Block.MOVING_PISTON.getPushedBlockCollisionShape(world, x, y, z, pushedBlockId, collisionShapeSizeMultiplier, facing);
            if (pushCollisionBox != null)
            {
                var entitiesToPush = world.getEntities(null, pushCollisionBox.Value);
                if (entitiesToPush.Count > 0)
                {
                    pushedEntities.AddRange(entitiesToPush);
                    foreach (Entity entity in pushedEntities)
                    {
                        entity.move(
                            (double)(entityMoveMultiplier * PistonConstants.HEAD_OFFSET_X[facing]),
                            (double)(entityMoveMultiplier * PistonConstants.HEAD_OFFSET_Y[facing]),
                            (double)(entityMoveMultiplier * PistonConstants.HEAD_OFFSET_Z[facing])
                        );
                    }
                    pushedEntities.Clear();
                }
            }

        }

        public void finish()
        {
            if (progress < 1.0F)
            {
                progress = lastProgess = 1.0F;
                world.removeBlockEntity(x, y, z);
                markRemoved();
                if (world.getBlockId(x, y, z) == Block.MOVING_PISTON.id)
                {
                    world.setBlock(x, y, z, pushedBlockId, pushedBlockData);
                }
            }

        }

        public override void tick()
        {
            progress = lastProgess;
            if (progress >= 1.0F)
            {
                pushEntities(1.0F, 0.25F);
                world.removeBlockEntity(x, y, z);
                markRemoved();
                if (world.getBlockId(x, y, z) == Block.MOVING_PISTON.id)
                {
                    world.setBlock(x, y, z, pushedBlockId, pushedBlockData);
                }

            }
            else
            {
                lastProgess += 0.5F;
                if (lastProgess >= 1.0F)
                {
                    lastProgess = 1.0F;
                }

                if (extending)
                {
                    pushEntities(lastProgess, lastProgess - progress + 1.0F / 16.0F);
                }

            }
        }

        public override void readNbt(NBTTagCompound nbt)
        {
            base.readNbt(nbt);
            pushedBlockId = nbt.getInteger("blockId");
            pushedBlockData = nbt.getInteger("blockData");
            facing = nbt.getInteger("facing");
            progress = lastProgess = nbt.getFloat("progress");
            extending = nbt.getBoolean("extending");
        }

        public override void writeNbt(NBTTagCompound nbt)
        {
            base.writeNbt(nbt);
            nbt.setInteger("blockId", pushedBlockId);
            nbt.setInteger("blockData", pushedBlockData);
            nbt.setInteger("facing", facing);
            nbt.setFloat("progress", progress);
            nbt.setBoolean("extending", extending);
        }
    }
}