namespace CustomCamera
{
    using UnityEngine;

    public enum RigidyType { Metallic, NonMetallic }
    public class ImpulseInteractionObject : MonoBehaviour, IImpulse
    {
        public  Rigidbody rigid;
        public  Collider  col;
        public  bool      isAffectedNoise = Constants.DefaultIsAffected;
        private bool      isAppliedNoise  = Constants.DefaultIsApply;

        [HideInInspector] public RigidyType type;
        [HideInInspector] public bool posX, posY, posZ;
        [HideInInspector] public bool rotX, rotY, rotZ;


        public bool IsApplied { get { return isAppliedNoise; } set { isAppliedNoise = value; } }

        public void ImpulseTrigger(float power)
        {
            rigid?.AddForce(Vector3.up * power, ForceMode.Impulse);

            float torquePower = power * Constants.rotationMultiplier;

            Vector3 torqueAxis =
                new Vector3(Random.Range(-1.0f, 2.0f), Random.Range(-1.0f, 2.0f), Random.Range(-1.0f, 2.0f));

            rigid?.AddTorque(torqueAxis * torquePower, ForceMode.Impulse);
        }
    }
}


