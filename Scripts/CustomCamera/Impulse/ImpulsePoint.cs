namespace CustomCamera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal static class YieldCache
    {
        public static readonly WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();
        public static readonly WaitForFixedUpdate WaitForFixedUpdate = new WaitForFixedUpdate();
        //public static readonly WaitForSeconds WaitForSeconds = new WaitForSeconds();
    }

    public interface IImpulse { void ImpulseTrigger(float power); }
    public class ImpulsePoint : MonoBehaviour
    {
        public bool playOnAwake;

        public float intensity  = Constants.P_DefaultIntensity;
        public float radius     = Constants.P_DefaultRadius;
        public int SectionCount = Constants.P_DefaultSectionCount;
        public float speed      = Constants.P_DefaultSpeed;

        public ShakeData shakeData;

        private bool isPlay = Constants.P_DefaultIsPlay;
        private bool isStop = Constants.P_DefaultIsStop;
        [HideInInspector] public bool isAlive = Constants.P_DefaultIsAlive;
        [HideInInspector] public float startRadius = Constants.P_DefaultStartRadius;
        [HideInInspector] public float radiusRatio;

        Transform tran;

        public void Update() { if (Input.GetKeyDown(KeyCode.Mouse0)) Impulse(); } //test

        public float GetRatio => Mathf.Pow((radius / startRadius), (1.0f / (float)SectionCount));

        float GetPower(int index) { return intensity * Mathf.Max(.2f, 1.0f - (float)index * .1f); }

        public float GetCurRadius(float ratio, int i) => startRadius * Mathf.Pow(ratio, i);

        int GetCurRadiusIndex(float distance)
            => Mathf.RoundToInt(Mathf.Log((distance <= 1.0f ? 1.0f : distance / startRadius), GetRatio));

        void Awake() { tran = transform;  if (playOnAwake) Impulse(); }

        public void Impulse() 
        {
            if (!isPlay && isStop)
            {
                CameraBrain brain = CameraBrain.Instance;
                if (shakeData != null)
                {
                    shakeData.ResetData(intensity);
                    brain?.AddShakeData(shakeData);
                }

                StartCoroutine(ImpulseProgress());
            }
        }
        IEnumerator ImpulseProgress()
        {
            if (speed > .0f)
            {
                isStop = false;
                isPlay = isAlive = !isStop;

                float timer = .0f;
                float durationTime = radius / speed;

                Collider[] colliders;
                List<ImpulseInteractionObject> appliedObjects = new List<ImpulseInteractionObject>();

                while (timer < durationTime)
                {
                    timer += Time.fixedDeltaTime;

                    int index = GetCurRadiusIndex(speed * timer);

                    colliders = Physics.OverlapSphere(tran.position, radius);

                    foreach (Collider collider in colliders)
                    {
                        //비용 줄이기
                        ImpulseInteractionObject interactionObject = collider.GetComponent<ImpulseInteractionObject>();

                        if (interactionObject == null) continue;

                        if (interactionObject.isAffectedNoise)
                        {
                            int detectionIndex =
                            GetCurRadiusIndex(Vector3.Distance(interactionObject.transform.root.position, tran.position));

                            if (detectionIndex == index && !interactionObject.IsApplied)
                            {
                                interactionObject.IsApplied = true;
                                ((IImpulse)interactionObject).ImpulseTrigger(GetPower(index));
                                appliedObjects.Add(interactionObject);
                            }
                        }
                    }

                    yield return YieldCache.WaitForFixedUpdate;
                }

                foreach (ImpulseInteractionObject obj in appliedObjects) obj.IsApplied = false;
                appliedObjects.Clear();

                timer = .0f;
                isStop = isPlay;
                isPlay = isAlive = !isStop;
            }
        }
    }
}