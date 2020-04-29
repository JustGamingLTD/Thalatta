using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

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

        public int dropletsPerUnit = 1;

        public float treePercentage = 0.01f;

        public bool generateTerrain = true;
        public bool applyErosion = true;
        public bool textureTerrain = true;
        public bool addTrees = true;

        Terrain terrain;
        TerrainData tData;
        NoiseAlgorithems.DiamondSquare diamondSquare;
        NoiseAlgorithems.Perlin perlinNoise;

        float unit;

        float[,,] alphaData;


        public void Generate()
        {
            terrain = GetComponent<Terrain>();
            tData = terrain.terrainData;
            EditorUtility.DisplayProgressBar("Generating Terrain", "Preparing", .1f);

            diamondSquare = new NoiseAlgorithems.DiamondSquare(size, scale, 1);
            perlinNoise = new NoiseAlgorithems.Perlin();
            //perlinNoise.textureSize = size;
            
            EditorUtility.DisplayProgressBar("Generating Terrain", "Generating Noise Maps", .3f);

            if(generateTerrain)
                tData = GenerateTerrain(terrain.terrainData);

            EditorUtility.DisplayProgressBar("Generating Terrain", "Texturing Terrain", .5f);

            
            unit = 1f / (tData.size.x - 1);
            if(textureTerrain)
                SetAlphaMaps();

            EditorUtility.DisplayProgressBar("Generating Terrain", "Applying Erosion", .7f);

            if(applyErosion)
                ApplyErosion();

            if(addTrees)
                AddTrees();


            EditorUtility.ClearProgressBar();

        }

        public void ApplyErosion()
        {
            float[,] heights = tData.GetHeights(0, 0, size, size);
            float[] heights1D = Erosion.ErosionHelper.twoDtooneD(heights);
            Erosion.Erosion erosion = new Erosion.Erosion();

            int dropletCount = heights1D.Length * dropletsPerUnit;

            erosion.Erode(heights1D, size, dropletCount, true);

            heights = Erosion.ErosionHelper.oneDtotwoD(heights1D);

            tData.SetHeights(0, 0, heights);
        }

        public void SetAlphaMaps()
        {
            // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
            float[,,] splatmapData = new float[tData.alphamapWidth, tData.alphamapHeight, tData.alphamapLayers];

            for (int y = 0; y < tData.alphamapHeight; y++)
            {
                for (int x = 0; x < tData.alphamapWidth; x++)
                {
                    // Normalise x/y coordinates to range 0-1 
                    float y_01 = (float)y / (float)tData.alphamapHeight;
                    float x_01 = (float)x / (float)tData.alphamapWidth;

                    // Setup an array to record the mix of texture weights at this point
                    float[] splatWeights = new float[tData.alphamapLayers];

                    // get height and slope at corresponding point
                    float height = GetHeightAtPoint(x_01 * tData.size.x, y_01 * tData.size.z);
                    float slope = GetSlopeAtPoint(x_01 * tData.size.x, y_01 * tData.size.z);

                    //====Rules for applying different textures===========================
                    //splatWeights[0] = 1 - slope; // decreases with slope (ground texture)

                    splatWeights[1] = slope; // increases with slope (rocky texture)

                    splatWeights[0] = (
                        slope < .1f &&
                        height > 0.3f * tData.size.y
                        && height < 0.5f * tData.size.y
                        )
                        ? 0f : 0;



                    splatWeights[2] = ( // apply 75% only to "Mesa" uplands (NOTE: the first two textures sum 1, so 1.5 corresponds to 80%)
                    height > 0.5f * tData.size.y) // plain terrain
                        ? 1.5f : 0f;

                    splatWeights[3] = ( // apply 50% only to valley floor (NOTE: textures 2 and 3 never coexist, so 1 corresponds to 50%))
                        height < 0.3f * tData.size.y && // lower terrain
                        slope < 0.3f) // plain terrain
                        ? 1f : 0f;

    


                    //====================================================================

                    // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                    float z = splatWeights.Sum();

                    // Loop through each terrain texture
                    for (int i = 0; i < tData.alphamapLayers; i++)
                    {

                        // Normalize so that sum of all texture weights = 1
                        splatWeights[i] /= z;

                        // Assign this point to the splatmap array
                        splatmapData[y, x, i] = splatWeights[i];
                        // NOTE: Alpha map array dimensions are shifted in relation to heightmap and world space (y is x and x is y or z)
                    }
                }
            }

            // Finally assign the new splatmap to the terrainData:
            tData.SetAlphamaps(0, 0, splatmapData);
        }

        public void AddTrees()
        {
            List<TreeInstance> instances = new List<TreeInstance>();

            for (float x = 0; x < size; x++)
            {
                for (float y = 0; y < size; y++)
                {
                    if (tData.GetSteepness(x/size, y/size) < 1 && tData.GetHeight(Mathf.RoundToInt(x), Mathf.RoundToInt(y)) > 5 && tData.GetHeight(Mathf.RoundToInt(x), Mathf.RoundToInt(y)) < 40)
                    {
                        if(Random.Range(0f, 1f) < treePercentage)
                            if(perlinNoise.BasicPerlin(x / 5, y / 5) < 0.4f)
                                instances.Add(new TreeInstance() { position = new Vector3(x/size, 0, y/size), prototypeIndex = 0, color = Color.white, heightScale = 1, lightmapColor = Color.white, rotation = 0, widthScale = 1 });
                    }

                }
            }
            tData.SetTreeInstances(instances.ToArray(), true);
            terrain.Flush();
        }


        float GetSlopeAtPoint(float pointX, float pointZ, bool scaleToRatio = true)
        {
            float factor = (scaleToRatio) ? 90f : 1f;
            return tData.GetSteepness(unit * pointX, unit * pointZ) / 90f; // x and z coordinates must be scaled
        }

        float GetHeightAtPoint(float pointX, float pointZ, bool scaleToTerrain = false)
        {
            float height = tData.GetInterpolatedHeight(unit * pointX, unit * pointZ);

            // x and z coordinates must be scaled with "unit"
            if (scaleToTerrain)
                return height / tData.size.y;
            else
                return height;
        }

        TerrainData GenerateTerrain(TerrainData terrainData)
        {
            //terrainData.size = new Vector3(size, depth, size);

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

                    heights[x, y] = perlinNoise.FractalPerlin(x, y) - .2f;

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

                    heights[x, y] = perlinNoise.Exponential(x, y) + ((perlinNoise.FractalPerlin(x, y) * 0.3f));
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
