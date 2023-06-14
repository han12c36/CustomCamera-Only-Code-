namespace CustomCamera
{
    using UnityEngine;

    [CreateAssetMenu(fileName = Constants.N_ScriptableFileName, menuName = Constants.N_MenuName, order = Constants.N_Order)]
    public class NoiseData : InteractionData
    {
        Vector3 GetNoiseOffset(float frequency,float intensity)
        {
            float time = Time.time * frequency;

            return new Vector3(Mathf.PerlinNoise(time, 0.0f) * 2.0f - 1.0f,
                               Mathf.PerlinNoise(0.0f, time) * 2.0f - 1.0f,
                               Mathf.PerlinNoise(time, time) * 2.0f - 1.0f) * intensity;
        }

        public void Noise(Transform curCamPos)
        {
            if (isPlay && !isStop)
            {
                Vector3 noiseOffset = GetNoiseOffset(this.frequency, this.intensity);

                if (x) noiseOffset.x = .0f;
                if (y) noiseOffset.y = .0f;
                if (z) noiseOffset.z = .0f;

                curCamPos.position += noiseOffset;
            }
        }
    }
}