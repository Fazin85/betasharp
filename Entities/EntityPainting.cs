using betareborn.Blocks.Materials;
using betareborn.Items;
using betareborn.NBT;
using betareborn.Util.Maths;
using betareborn.Worlds;
using java.util;

namespace betareborn.Entities
{
    public class EntityPainting : Entity
    {
        public static readonly new java.lang.Class Class = ikvm.runtime.Util.getClassFromTypeHandle(typeof(EntityPainting).TypeHandle);

        private int tickCounter;
        public int direction;
        public int xPosition;
        public int yPosition;
        public int zPosition;
        public EnumArt art;

        public EntityPainting(World world) : base(world)
        {
            tickCounter = 0;
            direction = 0;
            standingEyeHeight = 0.0F;
            setBoundingBoxSpacing(0.5F, 0.5F);
        }

        public EntityPainting(World world, int xPosition, int yPosition, int zPosition, int direction) : this(world)
        {
            this.xPosition = xPosition;
            this.yPosition = yPosition;
            this.zPosition = zPosition;
            ArrayList validPaintings = new ArrayList();
            EnumArt[] availablePaintings = EnumArt.values;
            int artCount = availablePaintings.Length;

            for (int i = 0; i < artCount; ++i)
            {
                EnumArt art = availablePaintings[i];
                this.art = art;
                setFacing(direction);
                if (canHangOnWall())
                {
                    validPaintings.add(art);
                }
            }

            if (validPaintings.size() > 0)
            {
                art = (EnumArt)validPaintings.get(random.nextInt(validPaintings.size()));
            }

            setFacing(direction);
        }

        public EntityPainting(World world, int x, int y, int z, int direction, String title) : this(world)
        {
            xPosition = x;
            yPosition = y;
            zPosition = z;
            EnumArt[] availablePaintings = EnumArt.values;
            int artCount = availablePaintings.Length;

            for (int i = 0; i < artCount; ++i)
            {
                EnumArt art = availablePaintings[i];
                if (art.title.Equals(title))
                {
                    this.art = art;
                    break;
                }
            }

            setFacing(direction);
        }

        protected override void initDataTracker()
        {
        }

        public void setFacing(int facing)
        {
            direction = facing;
            prevYaw = yaw = (float)(facing * 90);
            float halfWidth = (float)art.sizeX;
            float halfHeight = (float)art.sizeY;
            float halfDepth = (float)art.sizeX;
            if (facing != 0 && facing != 2)
            {
                halfWidth = 0.5F;
            }
            else
            {
                halfDepth = 0.5F;
            }

            halfWidth /= 32.0F;
            halfHeight /= 32.0F;
            halfDepth /= 32.0F;
            float centerX = (float)xPosition + 0.5F;
            float centerY = (float)yPosition + 0.5F;
            float centerZ = (float)zPosition + 0.5F;
            float wallOffset = 9.0F / 16.0F;
            if (facing == 0)
            {
                centerZ -= wallOffset;
            }

            if (facing == 1)
            {
                centerX -= wallOffset;
            }

            if (facing == 2)
            {
                centerZ += wallOffset;
            }

            if (facing == 3)
            {
                centerX += wallOffset;
            }

            if (facing == 0)
            {
                centerX -= getArtOffset(art.sizeX);
            }

            if (facing == 1)
            {
                centerZ += getArtOffset(art.sizeX);
            }

            if (facing == 2)
            {
                centerX += getArtOffset(art.sizeX);
            }

            if (facing == 3)
            {
                centerZ -= getArtOffset(art.sizeX);
            }

            centerY += getArtOffset(art.sizeY);
            setPosition((double)centerX, (double)centerY, (double)centerZ);
            float margin = -(0.1F / 16.0F);
            boundingBox = new Box((double)(centerX - halfWidth - margin), (double)(centerY - halfHeight - margin), (double)(centerZ - halfDepth - margin), (double)(centerX + halfWidth + margin), (double)(centerY + halfHeight + margin), (double)(centerZ + halfDepth + margin));
        }

        private float getArtOffset(int artSize)
        {
            return artSize == 32 ? 0.5F : (artSize == 64 ? 0.5F : 0.0F);
        }

        public override void tick()
        {
            if (tickCounter++ == 100 && !world.isRemote)
            {
                tickCounter = 0;
                if (!canHangOnWall())
                {
                    markDead();
                    world.spawnEntity(new EntityItem(world, x, y, z, new ItemStack(Item.PAINTING)));
                }
            }

        }

        public bool canHangOnWall()
        {
            if (world.getEntityCollisions(this, boundingBox).Count > 0)
            {
                return false;
            }
            else
            {
                int widthInBlocks = art.sizeX / 16;
                int heightInBlocks = art.sizeY / 16;
                int startX = xPosition;
                int startY = yPosition;
                int startZ = zPosition;
                if (direction == 0)
                {
                    startX = MathHelper.floor_double(x - (double)((float)art.sizeX / 32.0F));
                }

                if (direction == 1)
                {
                    startZ = MathHelper.floor_double(z - (double)((float)art.sizeX / 32.0F));
                }

                if (direction == 2)
                {
                    startX = MathHelper.floor_double(x - (double)((float)art.sizeX / 32.0F));
                }

                if (direction == 3)
                {
                    startZ = MathHelper.floor_double(z - (double)((float)art.sizeX / 32.0F));
                }

                startY = MathHelper.floor_double(y - (double)((float)art.sizeY / 32.0F));

                int dy;
                for (int dx = 0; dx < widthInBlocks; ++dx)
                {
                    for (dy = 0; dy < heightInBlocks; ++dy)
                    {
                        Material material;
                        if (direction != 0 && direction != 2)
                        {
                            var8 = world.GetMaterial(xPosition, var4 + var7, var5 + var6);
                        }
                        else
                        {
                            var8 = world.GetMaterial(var3 + var6, var4 + var7, zPosition);
                        }

                        if (!material.isSolid())
                        {
                            return false;
                        }
                    }
                }

                var entitiesInBox = world.getEntities(this, boundingBox);

                for (dy = 0; dy < entitiesInBox.Count; ++dy)
                {
                    if (entitiesInBox[dy] is EntityPainting)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public override bool isCollidable()
        {
            return true;
        }

        public override bool damage(Entity entity, int amount)
        {
            if (!dead && !world.isRemote)
            {
                markDead();
                scheduleVelocityUpdate();
                world.spawnEntity(new EntityItem(world, x, y, z, new ItemStack(Item.PAINTING)));
            }

            return true;
        }

        public override void writeNbt(NBTTagCompound nbt)
        {
            nbt.setByte("Dir", (sbyte)direction);
            nbt.setString("Motive", art.title);
            nbt.setInteger("TileX", xPosition);
            nbt.setInteger("TileY", yPosition);
            nbt.setInteger("TileZ", zPosition);
        }

        public override void readNbt(NBTTagCompound nbt)
        {
            direction = nbt.getByte("Dir");
            xPosition = nbt.getInteger("TileX");
            yPosition = nbt.getInteger("TileY");
            zPosition = nbt.getInteger("TileZ");
            String motiveTitle = nbt.getString("Motive");
            EnumArt[] allArt = EnumArt.values;
            int artCount = allArt.Length;

            for (int i = 0; i < artCount; ++i)
            {
                EnumArt candidateArt = allArt[i];
                if (candidateArt.title.Equals(motiveTitle))
                {
                    art = candidateArt;
                }
            }

            if (art == null)
            {
                art = EnumArt.Kebab;
            }

            setFacing(direction);
        }

        public override void move(double dx, double dy, double dz)
        {
            if (!world.isRemote && dx * dx + dy * dy + dz * dz > 0.0D)
            {
                markDead();
                world.spawnEntity(new EntityItem(world, x, y, z, new ItemStack(Item.PAINTING)));
            }

        }

        public override void addVelocity(double dx, double dy, double dz)
        {
            if (!world.isRemote && dx * dx + dy * dy + dz * dz > 0.0D)
            {
                markDead();
                world.spawnEntity(new EntityItem(world, x, y, z, new ItemStack(Item.PAINTING)));
            }

        }
    }

}