using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetworkV2;

namespace CNN
{
    class transformation
    {
        public static double[] Vector(int[,] Matrix)
        {
            double[] transformed = new double[Matrix.GetLength(0) * Matrix.GetLength(1)];
            int nem = 0;

            for (int i = 0; i < Matrix.GetLength(0); i++)
            {
                for (int j = 0; j < Matrix.GetLength(1); j++)
                {
                    transformed[nem] = Matrix[i, j];
                    nem++;
                }
            }


            return transformed;
        }
        public static double[] VectoroOutput(int Y, int namber)
        {
            double[] transformed = new double[namber];
            
            for (int i = 0; i < namber; i++)
            {
                transformed[i] = 0;
                if ((Y - 1) == i)
                {
                    transformed[i] = 1;
                }
            }

            return transformed;
        }

        public static int[,] ImageMatrix(string link)
        {

            Bitmap Bmp = new Bitmap(link);

            int[,] Matrix = new int[Bmp.Width, Bmp.Height];

            for (int i = 0; i < Bmp.Width; i++)
            {
                for (int j = 0; j < Bmp.Height; j++)
                {
                    Color s = Bmp.GetPixel(i, j);
                    Matrix[i, j] = Convert.ToInt32((s.R + s.G + s.B) / 3);
                }
            }

            return Matrix;
        }
    }
}
