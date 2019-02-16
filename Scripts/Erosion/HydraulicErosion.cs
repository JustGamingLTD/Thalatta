using UnityEngine;
using System.Collections.Generic;

namespace Thalatta.Erosion
{
    public class HydraulicErosion
    {
        TerrainData tData;

        List<Droplet> droplets = new List<Droplet>();

        Vector3[,] gradients;

        public HydraulicErosion(TerrainData data)
        {
            tData = data;
            gradients = GenerateGradientMap();
            CreateDroplets();
            GetNewHeights();
        }

        public Vector3[,] GenerateGradientMap()
        {
            Vector3[,] gradients = new Vector3[tData.heightmapWidth, tData.heightmapHeight];

            Debug.Log(tData.heightmapWidth);
            GetGradient(2, 2);
            for (int x = 0; x < tData.heightmapWidth; x++)
            {
                for (int y = 0; y < tData.heightmapHeight; y++)
                {
                    gradients[x, y] = GetGradient(x, y);
                }
            }

            return gradients;

        }

        public void CreateDroplets()
        {
            for (int x = 0; x < tData.heightmapWidth; x++)
            {
                for (int y = 0; y < tData.heightmapHeight; y++)
                {
                    droplets.Add(new Droplet(new Vector2Int(x, y)));
                }
            }
        }

        public void GetNewHeights()
        {
            float[,] heights = tData.GetHeights(0, 0, tData.heightmapWidth, tData.heightmapHeight);

            for (int i = 0; i < droplets.Count; i++)
            {
                droplets[i].ProcessLocation(heights, gradients);
            }
        }

        public Vector3 GetGradient(int x, int y)
        {
            Vector3 bestDir = Vector3.right;

            for (int curX = -1; curX <= 1; curX++)
            {
                for (int curY = -1; curY <= 1; curY++)
                {
                    if (curX == 0 && curY == 0)
                    {
                        continue;
                    }

                    if (tData.GetHeight(x + curX, y + curY) > bestDir.z)
                    {
                        bestDir.x = curX;
                        bestDir.y = curY;
                        bestDir.z = tData.GetHeight(x + curX, y + curY);
                    }
                }
            }

            return bestDir;
        }
    }

}