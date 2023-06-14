namespace CustomCamera
{
    using UnityEngine;

    [CreateAssetMenu(fileName = Constants.S_ScriptableFileName, menuName = Constants.S_MenuName, order = Constants.S_Order)]
    public class ShakeData : InteractionData
    {
        public float duration;
        private float remainTime;
        private float correctionPower;

        public bool isAlive => remainTime > .0f;
        public void ResetData(float power = 1.0f)
        {
            correctionPower = intensity * power;
            remainTime = duration;
        }
        Vector3 GetShakeOffset(float frequency, float intensity)
        {
            return new Vector3(Random.value * frequency * 2 - frequency,
                               Random.value * frequency * 2 - frequency,
                               Random.value * frequency * 2 - frequency) * intensity;
        }
        public void Shake(Transform cam)
        {
            if(remainTime > 0) remainTime -= Time.deltaTime;

            Vector3 shakeOffset = GetShakeOffset(this.frequency, this.correctionPower);

            if (x) shakeOffset.x = .0f;
            if (y) shakeOffset.y = .0f;
            if (z) shakeOffset.z = .0f;

            cam.position += shakeOffset;
        }
    }
}