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
            myTarget.scale = EditorGUILayout.FloatField("Scale", myTarget.scale);
            //myTarget.seed = EditorGUILayout.DoubleField("Seed", myTarget.seed);
            myTarget.size = EditorGUILayout.IntField("Size", myTarget.size);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Noise Type", EditorStyles.boldLabel);
            myTarget.noiseType = (NoiseTypes)EditorGUILayout.EnumPopup(myTarget.noiseType);

            EditorGUILayout.Space();


            if (myTarget.noiseType == NoiseTypes.ExponentialPerlinNoise || myTarget.noiseType == NoiseTypes.Hybrid)
            {
                EditorGUILayout.LabelField("Exponential Perlin Noise Properties", EditorStyles.boldLabel);
                myTarget.octaves = EditorGUILayout.IntField("Octaves", myTarget.octaves);
                myTarget.cover = EditorGUILayout.FloatField("Cover", myTarget.cover);
                myTarget.sharpness = EditorGUILayout.FloatField("Sharpness", myTarget.sharpness);
                myTarget.powerFactor = EditorGUILayout.FloatField("Power Factor", myTarget.powerFactor);

            }

            bool pressed = EditorGUILayout.DropdownButton(new UnityEngine.GUIContent("Generate"), UnityEngine.FocusType.Keyboard);

            if (pressed)
            {
                myTarget.Generate();
            }
        }
        
    }
}


