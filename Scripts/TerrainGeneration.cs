using UnityEngine;


namespace Thalatta
{

    public class TerrainGeneration : MonoBehaviour
    {
        [Header("Basic Properties")]
        public int depth = 20;

        public int size = 256;

        public float scale = 20f;
        public double seed = 1;

        public NoiseTypes noiseType;

        public int octaves = 16;
        public float cover = 0.65f;
        public float sharpness = 0.95f;
        public float powerFactor = 0.95f;


        Terrain terrain;
        TerrainData tData;
        NoiseAlgorithems.DiamondSquare diamondSquare;

        NoiseAlgorithems.Perlin perlinNoise;

        float[,,] alphaData;


        public void Generate()
        {
            diamondSquare = new NoiseAlgorithems.DiamondSquare(size, scale, 1);
            perlinNoise = new NoiseAlgorithems.Perlin();
            //perlinNoise.textureSize = size;
            terrain = GetComponent<Terrain>();
            tData = terrain.terrainData;
            tData = GenerateTerrain(terrain.terrainData);
            SetAlphaMaps();

            new HydraulicErosion(tData);
        }

        public void SetAlphaMaps()
        {
            alphaData = tData.GetAlphamaps(0, 0, tData.alphamapWidth, tData.alphamapWidth);

            Debug.Log("Setting alpha mpas");

            float[,] heights = tData.GetHeights(0, 0, size, size);

            for(int y = 0; y<tData.alphamapHeight; y++)
            {
                for (int x = 0; x < tData.alphamapWidth; x++)
                {
                    float percentage = heights[x, y] * 2;
                    alphaData[x, y, 0] = percentage;
                    alphaData[x, y, 1] = 1 - percentage;
                }
            }

            tData.SetAlphamaps(0, 0, alphaData);
        }

        TerrainData GenerateTerrain(TerrainData terrainData)
        {
            terrainData.size = new Vector3(size, depth, size);

            switch (noiseType)
            {
                case NoiseTypes.DiamondSquare:
                    terrainData.SetHeights(0, 0, GenerateHeightsDiamondSquare());
                    break;

                case NoiseTypes.ExponentialPerlinNoise:
                    terrainData.SetHeights(0, 0, GenerateHeightsExponential());
                    break;
                case NoiseTypes.Turbulence:
                    terrainData.SetHeights(0, 0, GenerateHeightsTurbulence());
                    break;
                case NoiseTypes.Hybrid:
                    terrainData.SetHeights(0, 0, GenerateHeightsHybrid());
                    break;
                case NoiseTypes.Fractal:
                    terrainData.SetHeights(0, 0, GenerateHeightsFractal());
                    break;
            }



            return terrainData;
        }

        float[,] GenerateHeightsDiamondSquare()
        {

            double[,] heightsDouble = diamondSquare.getData();
            float[,] heights = new float[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    heights[x, y] = (float)heightsDouble[x, y];
                }
            }

            return heights;
        }


        float[,] GenerateHeightsExponential()
        {
            perlinNoise.noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            perlinNoise.octaves = octaves;
            perlinNoise.cover = cover;
            perlinNoise.sharpness = sharpness;
            perlinNoise.powerFactor = powerFactor;

            float[,] heights = new float[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {

                    heights[x, y] = perlinNoise.Exponential(x, y);
                }
            }

            return heights;
        }

        float[,] GenerateHeightsTurbulence()
        {
            perlinNoise.noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            perlinNoise.octaves = octaves;
            perlinNoise.cover = cover;
            perlinNoise.sharpness = sharpness;
            perlinNoise.powerFactor = powerFactor;

            float[,] heights = new float[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {

                    heights[x, y] = perlinNoise.Turbulence(x, y);
                }
            }

            return heights;
        }

        float[,] GenerateHeightsFractal()
        {
            perlinNoise.noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

            perlinNoise.octaves = octaves;
            perlinNoise.cover = cover;
            perlinNoise.sharpness = sharpness;
            perlinNoise.powerFactor = powerFactor;

            float[,] heights = new float[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {

                    heights[x, y] = perlinNoise.FractalPerlin(x, y);
                }
            }

            return heights;
        }

        float[,] GenerateHeightsHybrid()
        {
            perlinNoise.noiseOffset = new Vector2(Random.Range(0f, 10f), Random.Range(0f, 10f));

            perlinNoise.octaves = octaves;
            perlinNoise.cover = cover;
            perlinNoise.sharpness = sharpness;
            perlinNoise.powerFactor = powerFactor;

            float[,] heights = new float[size, size];
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {

                    heights[x, y] = perlinNoise.Exponential(x, y) + ((perlinNoise.FractalPerlin(x, y) * 0.7f) - .2f);
                }
            }

            return heights;
        }

    }

    public enum NoiseTypes
    {
        DiamondSquare,
        ExponentialPerlinNoise,
        Turbulence,
        Fractal,
        Hybrid,
    }

}
