using System.Collections.Generic;

namespace Thalatta.Erosion
{
    public static class ErosionHelper
    {
        public static float[] twoDtooneD(float[,] original)
        {
            int size = original.GetLength(0);

            List<float> newList = new List<float>();

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    newList.Add(original[x,y]);
                }
            }

            return newList.ToArray();
        }

        public static float[,] oneDtotwoD(float[] original)
        {
            int size = (int)System.Math.Sqrt(original.Length);

            float[,] newArray = new float[size,size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    newArray[x, y] = original[y * size + x];
                }
            }

            return newArray;
        }
    }

}

