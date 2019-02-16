using UnityEngine;

public class HydraulicErosion
{
    TerrainData tData;

    public HydraulicErosion(TerrainData data)
    {
        tData = data;
        GenerateGradientMap();
    }

    public void GenerateGradientMap()
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

    }

    public Vector3 GetGradient(int x, int y)
    {
        Vector3 bestDir = Vector3.right;

        for (int curX = -1; curX <= 1; curX++)
        {
            for (int curY = -1; curY <= 1; curY++)
            {
                if(curX == 0 && curY == 0)
                {
                    continue;
                }

                if(tData.GetHeight(x+curX, y+curY) > bestDir.z)
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
