using System;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media.Media3D;

namespace MathUtil
{
    public class MathUtil
    {
	    public MathUtil()
	    {

	    }

        public static double GetAngle(Point Center, Point Point)
        {
            Vector Vec = new Vector(Point.X - Center.X, Point.Y - Center.Y);
            double radian = Math.Atan2(Vec.X, Vec.Y);
            double degrees = radian * (180 / Math.PI);
            return degrees;
        }

        public static double GetRadian(double Angle)
        {
            return Angle * Math.PI / 180;
        }

        public static Point RotatePoint(Point P, double Angle)
        {
            double radians = Angle == 0 ? 0: Angle* Math.PI / 180;
            
            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double px = P.X * cos - P.Y * sin;
            double py = P.X * sin + P.Y * cos;
            
            return new Point(px,py);
        }

        public static Point RotatePoint(double X, double Y, double Angle)
        {
            double radians = Angle == 0 ? 0 : Angle * Math.PI / 180;

            double cos = Math.Cos(radians);
            double sin = Math.Sin(radians);

            double px = X * cos - Y * sin;
            double py = X * sin + Y * cos;

            return new Point(px, py);
        }


        public static int[] generateFibonocciNumbers(int SeedX, int SeedY, int X, int DigitLength)
        {
            int[] values = new int[X];

            long num = 2 + 10 + SeedX;
            long lastNum = 1 + 10 + SeedY;

            long n1 = num;
            long n2 = lastNum;

            int x = 0;

            while (true)
            {
                string numString = num + "";

                for (int i = 0; i < numString.Length; i += DigitLength)
                {
                    if (i + DigitLength < numString.Length)
                    {
                        if (x < X)
                        {
                            String tempString = numString.Substring(i, DigitLength);
                            //String tempString = numString.Substring();
                            
                            if (tempString != "")
                            {
                                values[x] = Int32.Parse(tempString);
                                x++;
                            }
                        }
                        else
                        {
                            return values;
                        }
                    }
                }

                if (Math.Log10(num) < 17)
                {
                    long tempNum = num;
                    num += lastNum;
                    lastNum = tempNum;
                }
                else
                {
                    num = 2 + 10 + n1;
                    lastNum = 2 + 10 + n2;
                    n1 = num;
                    n2 = lastNum;
                }
            }
        }

        public static float bilerp(float posX, float posY, float X1, float X2, float Y1, float Y2, float Q11, float Q21, float Q12, float Q22)
        {
            float valX1 = ((Y2 - posY) / (Y2 - Y1)) * (((X2 - posX) / (X2 - X1)) * Q11 + ((posX - X1) / (X2 - X1)) * Q21);
            float valX2 = ((posY - Y1) / (Y2 - Y1)) * (((X2 - posX) / (X2 - X1)) * Q12 + ((posX - X1) / (X2 - X1)) * Q22);

            return (valX1 + valX2);
        }



        public static double distance(int X1, int X2, int Y1, int Y2)
        {
            return Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2));
        }

        public static double distance(Point V1, int X2, int Y2)
        {
            return Math.Sqrt(Math.Pow(V1.X - X2, 2) + Math.Pow(V1.Y - Y2, 2));
        }

        public static double distance(Point V1, Point V2)
        {
            return Math.Sqrt(Math.Pow(V1.X - V2.X, 2) + Math.Pow(V1.Y - V2.Y, 2));
        }

        public static double distance(Point3D P1, Point3D P2)
        {
            return Math.Sqrt(Math.Pow(P1.X - P2.X, 2) + Math.Pow(P1.Y - P2.Y, 2) + Math.Pow(P1.Z - P2.Z, 2));
        }

        public Boolean[,] BooleanBilerp(Boolean V1, Boolean V2, Boolean V3, Boolean V4)
        {
            /*
             * v1---------v2
             * |           |
             * |           |
             * |           |
             * |           |
             * v3---------v4
            */
            float v1 = V1 ? 1f : 0;
            float v2 = V2 ? 1f : 0;
            float v3 = V3 ? 1f : 0;
            float v4 = V4 ? 1f : 0;

            float[,] normalizedMap = new float[main.ApplicationSettings.chunkSize + 1, main.ApplicationSettings.chunkSize + 1];
            Boolean[,] BooleanMap = new Boolean[main.ApplicationSettings.chunkSize + 1, main.ApplicationSettings.chunkSize + 1];
            for (int i = 0; i < normalizedMap.GetLength(0); i++)
            {
                for (int j = 0; j < normalizedMap.GetLength(0); j++)
                {
                    float valx1 = (((normalizedMap.GetLength(0) - i) / normalizedMap.GetLength(0)) * v1) + ((i / normalizedMap.GetLength(0)) * v2);
                    float valx2 = (((normalizedMap.GetLength(0) - i) / normalizedMap.GetLength(0)) * v3) + ((i / normalizedMap.GetLength(0)) * v4);

                    float valy1 = (((normalizedMap.GetLength(0) - j) / normalizedMap.GetLength(0)) * v1) + ((j / normalizedMap.GetLength(0)) * v2);
                    float valy2 = (((normalizedMap.GetLength(0) - j) / normalizedMap.GetLength(0)) * v3) + ((j / normalizedMap.GetLength(0)) * v4);

                    float val = (valx1 + valx2 + valy1 + valy2);
                    BooleanMap[i, j] = val < 0.5 ? false : true;
                }
            }
            return BooleanMap;
        }
    }
}


