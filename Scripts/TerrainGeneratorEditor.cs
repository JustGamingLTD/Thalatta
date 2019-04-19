using UnityEditor;

namespace Thalatta
{
    [CustomEditor(typeof(TerrainGeneration))]
    public class TerrainGeneratorEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            TerrainGeneration myTarget = (TerrainGeneration)target;

            EditorGUILayout.LabelField("Basic Properties", EditorStyles.boldLabel);
            myTarget.depth = EditorGUILayout.IntField("Depth", myTarget.depth);
            myTarget.size = EditorGUILayout.IntField("Size", myTarget.size);
            //myTarget.seed = EditorGUILayout.DoubleField("Seed", myTarget.seed);
            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Terrain from Scratch Properties", EditorStyles.boldLabel);
            myTarget.generateTerrain = EditorGUILayout.ToggleLeft("Generate Terrain from Scratch", myTarget.generateTerrain);

            if (myTarget.generateTerrain)
            {
                EditorGUILayout.LabelField("Noise Type", EditorStyles.boldLabel);
                myTarget.noiseType = (NoiseTypes)EditorGUILayout.EnumPopup(myTarget.noiseType);

                EditorGUILayout.Space();

                if (myTarget.noiseType == NoiseTypes.DiamondSquare)
                {
                    EditorGUILayout.LabelField("Diamond Square Properties", EditorStyles.boldLabel);
                    myTarget.scale = EditorGUILayout.FloatField("Roughness", myTarget.scale);

                }
                if (myTarget.noiseType == NoiseTypes.Fractal)
                {
                    EditorGUILayout.LabelField("Fractal Noise Properties", EditorStyles.boldLabel);

                }
                if (myTarget.noiseType == NoiseTypes.ExponentialPerlinNoise || myTarget.noiseType == NoiseTypes.Hybrid)
                {
                    EditorGUILayout.LabelField("Exponential Perlin Noise Properties", EditorStyles.boldLabel);
                    myTarget.octaves = EditorGUILayout.IntField("Octaves", myTarget.octaves);
                    myTarget.cover = EditorGUILayout.FloatField("Cover", myTarget.cover);
                    myTarget.sharpness = EditorGUILayout.FloatField("Sharpness", myTarget.sharpness);
                    myTarget.powerFactor = EditorGUILayout.FloatField("Power Factor", myTarget.powerFactor);

                }
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Erosion Properties", EditorStyles.boldLabel);
            myTarget.applyErosion = EditorGUILayout.ToggleLeft("Apply Erosion", myTarget.applyErosion);
            if (myTarget.applyErosion)
            {
                myTarget.dropletsPerUnit = EditorGUILayout.IntField("Droplets per Unit", myTarget.dropletsPerUnit);

            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Texture Properties", EditorStyles.boldLabel);
            myTarget.textureTerrain = EditorGUILayout.ToggleLeft("Texture Terrain", myTarget.textureTerrain);

            bool pressed = EditorGUILayout.DropdownButton(new UnityEngine.GUIContent("Generate"), UnityEngine.FocusType.Keyboard);

            if (pressed)
            {
                myTarget.Generate();
            }
        }
        
    }
}


