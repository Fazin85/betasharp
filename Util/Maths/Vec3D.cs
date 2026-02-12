namespace betareborn.Util.Maths
{
    public record struct Vec3D
    {
        public static readonly Vec3D Zero = new Vec3D(0.0D, 0.0D, 0.0D);
        
        public double xCoord;
        public double yCoord;
        public double zCoord;
        
        public Vec3D(double xCoord, double yCoord, double zCoord)
        {
            if (xCoord == -0.0D) xCoord = 0.0D;
            if (yCoord == -0.0D) yCoord = 0.0D;
            if (zCoord == -0.0D) zCoord = 0.0D;

            this.xCoord = xCoord;
            this.yCoord = yCoord;
            this.zCoord = zCoord;
        }

        public static Vec3D createVector(double xCoord, double yCoord, double zCoord)
        {
            return new Vec3D(xCoord, yCoord, zCoord);
        }

        public Vec3D normalize()
        {
            double var1 = (double)MathHelper.sqrt_double(xCoord * xCoord + yCoord * yCoord + zCoord * zCoord);
            return var1 < 1.0E-4D ? createVector(0.0D, 0.0D, 0.0D) : createVector(xCoord / var1, yCoord / var1, zCoord / var1);
        }

        public Vec3D crossProduct(Vec3D var1)
        {
            return createVector(yCoord * var1.zCoord - zCoord * var1.yCoord, zCoord * var1.xCoord - xCoord * var1.zCoord, xCoord * var1.yCoord - yCoord * var1.xCoord);
        }
        
        public double squareDistanceTo(Vec3D other)
        {
            double dx = other.xCoord - xCoord;
            double dy = other.yCoord - yCoord;
            double dz = other.zCoord - zCoord;
            return dx * dx + dy * dy + dz * dz;
        }
        
        public double distanceTo(Vec3D other)
        {
            return Math.Sqrt(squareDistanceTo(other));
        }

        public double magnitude()
        {
            return distanceTo(Zero);
        }

        public Vec3D? getIntermediateWithXValue(Vec3D var1, double var2)
        {
            double var4 = var1.xCoord - xCoord;
            double var6 = var1.yCoord - yCoord;
            double var8 = var1.zCoord - zCoord;
            if (var4 * var4 < (double)1.0E-7F)
            {
                return null;
            }
            else
            {
                double var10 = (var2 - xCoord) / var4;
                return var10 >= 0.0D && var10 <= 1.0D ? createVector(xCoord + var4 * var10, yCoord + var6 * var10, zCoord + var8 * var10) : null;
            }
        }

        public Vec3D? getIntermediateWithYValue(Vec3D var1, double var2)
        {
            double var4 = var1.xCoord - xCoord;
            double var6 = var1.yCoord - yCoord;
            double var8 = var1.zCoord - zCoord;
            if (var6 * var6 < (double)1.0E-7F)
            {
                return null;
            }
            else
            {
                double var10 = (var2 - yCoord) / var6;
                return var10 >= 0.0D && var10 <= 1.0D ? createVector(xCoord + var4 * var10, yCoord + var6 * var10, zCoord + var8 * var10) : null;
            }
        }

        public Vec3D? getIntermediateWithZValue(Vec3D var1, double var2)
        {
            double var4 = var1.xCoord - xCoord;
            double var6 = var1.yCoord - yCoord;
            double var8 = var1.zCoord - zCoord;
            if (var8 * var8 < (double)1.0E-7F)
            {
                return null;
            }
            else
            {
                double var10 = (var2 - zCoord) / var8;
                return var10 >= 0.0D && var10 <= 1.0D ? createVector(xCoord + var4 * var10, yCoord + var6 * var10, zCoord + var8 * var10) : null;
            }
        }

        public override string ToString()
        {
            return "(" + xCoord + ", " + yCoord + ", " + zCoord + ")";
        }

        public void rotateAroundX(float var1)
        {
            float var2 = MathHelper.cos(var1);
            float var3 = MathHelper.sin(var1);
            double var4 = xCoord;
            double var6 = yCoord * (double)var2 + zCoord * (double)var3;
            double var8 = zCoord * (double)var2 - yCoord * (double)var3;
            xCoord = var4;
            yCoord = var6;
            zCoord = var8;
        }

        public void rotateAroundY(float var1)
        {
            float var2 = MathHelper.cos(var1);
            float var3 = MathHelper.sin(var1);
            double var4 = xCoord * (double)var2 + zCoord * (double)var3;
            double var6 = yCoord;
            double var8 = zCoord * (double)var2 - xCoord * (double)var3;
            xCoord = var4;
            yCoord = var6;
            zCoord = var8;
        }

        public static Vec3D operator +(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.xCoord + b.xCoord, a.yCoord + b.yCoord, a.zCoord + b.zCoord);
        }
        public static Vec3D operator -(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.xCoord - b.xCoord, a.yCoord - b.yCoord, a.zCoord - b.zCoord);
        }
        
        public static Vec3D operator *(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.xCoord * b.xCoord, a.yCoord * b.yCoord, a.zCoord * b.zCoord);
        }
        
        public static Vec3D operator /(Vec3D a, Vec3D b)
        {
            return new Vec3D(a.xCoord / b.xCoord, a.yCoord / b.yCoord, a.zCoord / b.zCoord);
        }
        
        public static Vec3D operator *(double a, Vec3D b)
        {
            return new Vec3D(a * b.xCoord, a * b.yCoord, a * b.zCoord);
        }
        
        public static Vec3D operator /(double a, Vec3D b)
        {
            return new Vec3D(a / b.xCoord, a / b.yCoord, a / b.zCoord);
        }
    }
}