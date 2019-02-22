using UnityEngine;

namespace Thalatta.NoiseAlgorithems
{


    public class Perlin
    {
        public float frequency = 1f;
        public int octaves = 16;
        public Vector2 noiseOffset = new Vector2(3.141592f, 3);

        public float cover = 0.65f;
        public float sharpness = 0.95f;
        public float powerFactor = 0.95f;

        public int textureSize = 128;

        public Gradient mapColours;
        public bool useGreyscale = false;
        public bool useInspectorColours = false;


        public float BasicPerlin(int x, int y)
        {
            float noise = 0;

            float nX = (((float)x / (float)textureSize) + noiseOffset.x) * frequency;
            float nY = (((float)y / (float)textureSize) + noiseOffset.y) * frequency;

            noise = Mathf.PerlinNoise(nX, nY);

            return noise;
        }


        public float FractalPerlin(float x, float y)
        {
            float nX, nY;
            float noise = .1f;
            float gain = 1f;
            float factor = 0f; 

            for (int i = 0; i < octaves; i++)
            {
                nX = (((float)x / (float)textureSize) + noiseOffset.x) * frequency * gain; 
                nY = (((float)y / (float)textureSize) + noiseOffset.y) * frequency * gain;

                noise += Mathf.PerlinNoise(nX, nY) * (1f / gain);


                factor += (1f / gain); 

                gain *= 2f; 
            }

            noise /= factor;
            /*
            noise *= 10;
            noise = Mathf.Pow(noise, 2);
            noise /= 50;
            */
            return noise;
        }

        public float Exponential(int x, int y)
        {
            float noise = FractalPerlin(x*.2f, y*.2f);
            noise = ExponentialFilter(noise)*1.4f;

            return noise;
        }


        

        public float Turbulence(int x, int y)
        {
            float nX, nY;
            float noise = 0f;
            float gain = 1f;
            float factor = 0f; 

            for (int i = 0; i < octaves; i++)
            {
                nX = (((float)x / (float)textureSize) + noiseOffset.x) * frequency * gain; 
                nY = (((float)y / (float)textureSize) + noiseOffset.y) * frequency * gain;

                noise += TurbulenceFilter(Mathf.PerlinNoise(nX, nY)) * (1f / gain); 

                factor += (1f / gain); 

                gain *= 2f; 
            }

            noise /= factor;



            return noise;
        }

        float TurbulenceFilter(float val)
        {
            val *= 2f;
            val -= 1f;

            val = Mathf.Abs(val);

            return val;
        }

        float ExponentialFilter(float val)
        {
            float v = val * 255f;
            float c = cover * 255f;

            float p = v - (255f - c);

            if (p < 0)
                p = 0;

            float n = 255f - (Mathf.Pow(sharpness, p * powerFactor) * 255f);

            n /= 255f;

            return n;
        }





    }












}
