namespace CustomCamera
{
    using UnityEngine;

    //================================================
    // use ChangeCam
    //if (Input.GetKeyDown(KeyCode.Mouse0)) 
    //    m_Brain.ChangeCam(m_Brain.subCameras[1]);

    // use ImpulsePoint
    //if (Input.GetKeyDown(KeyCode.Mouse0)) ImpulseTrigger(2f);

    //================================================

    public class ThirdPersonCamera : SubCamera , ISubCamera
    {
        public GameObject followTarget;
        public GameObject lookTarget;
        public Transform cameraArm;
        private Transform armPivot;
        
        public Transform Arm => cameraArm;
        private Vector3 offset;


        //Cached
        private Transform t_LookTarget;
        private Transform t_FollowTarget;

        #region Follow
        public bool isFollowLerp = Constants.F_DefaultIsLerp;
        public float followDemp  = Constants.F_DefaultDemp;
        public float followSpeed = Constants.F_DefaultSpeed;
        #endregion

        #region Zoom
        public bool  isZoomLerp      = Constants.Z_DefaultIsLerp;
        public float zoomDemp        = Constants.Z_DefaultDemp;
        public float zoomSpeed       = Constants.Z_DefaultSpeed;
        public float minZoomDIstance = Constants.Z_DefaultMinDistance;
        public float maxZoomDIstance = Constants.Z_DefaultMaxDistance;

        private float scroll;
        #endregion

        #region Rotaion
        public bool  canRotation    = Constants.R_DefaultCanRotation;
        public bool  isRotationLerp = Constants.R_DefaultIsLerp;
        public bool  canY           = Constants.R_DefaultCanY;
        public float rotationDemp   = Constants.R_DefaultDemp;
        public float rotaionSpeed   = Constants.R_DefaultSpeed;
        public float minAngleY      = Constants.R_DefaultMinAngle;
        public float maxAngleY      = Constants.R_DefaultMaxAngle;

        private float inputX, inputY, angleX, angleY;
        #endregion

        protected override void Initialize()
        {
            if (cameraArm != null)
            {
                if (armPivot != null) return;
                armPivot = new GameObject(Constants.CameraArmObjectName).transform;
                armPivot.transform.position = cameraArm.position;
                armPivot.transform.rotation = Quaternion.identity;
                armPivot.hideFlags = HideFlags.HideInHierarchy;

                offset = transform.position - armPivot.position;
            }

            if(followTarget != null) t_FollowTarget = followTarget.transform;

            initPos = armPivot.position + armPivot.TransformDirection(offset);
            if (lookTarget != null)
            {
                t_LookTarget = lookTarget.transform;
                initQuat = Quaternion.LookRotation(t_LookTarget.position - initPos);
            }
            else initQuat = Quaternion.identity;
        }
        protected override void OnDestroy() { base.OnDestroy(); if (armPivot != null) Destroy(armPivot); }
        public override void CamEnter() { }
        public override void CamUpdate()
        {
            UpdateInput();

            //test
            if (Input.GetKeyDown(KeyCode.Mouse1)) 
                m_Brain.ChangeCam(m_Brain.subCameras[1]);
        }
        public override void CamFixedUpdate() { }
        public override void CamLateUpdate() 
        {
            if (this.followTarget != null) FollowTarget(cameraArm.position, Time.fixedDeltaTime);

            if (this.lookTarget != null) 
            {
                if(canRotation) Rotate(armPivot);
                Zoom(t_LookTarget.position);
            }
        }
        public override void CamExit()
        {
            initPos = armPivot.position + armPivot.TransformDirection(offset);
            if (lookTarget != null) initQuat = Quaternion.LookRotation(t_LookTarget.position - initPos);
            else initQuat = Quaternion.identity;
        }
        public void UpdateInput()
        {
            inputX = Input.GetAxisRaw(Constants.MouseX);
            inputY = Input.GetAxisRaw(Constants.MouseY);

            if(canRotation) scroll = Input.GetAxisRaw(Constants.MouseWheel);
        }
        void FollowTarget(Vector3 followTarget, float delta)
        {
            if(isFollowLerp)
            armPivot.position =
                    Vector3.Lerp(armPivot.position, followTarget, delta * followSpeed * followDemp);
            else armPivot.position = followTarget;
        }
        public void Zoom(Vector3 target)
        {
            if(scroll != .0f)
            {
                float force = scroll * zoomSpeed * Constants.mouseSensitivity;
                Vector3 vec = (target - offset).normalized;
                vec *= force;
                offset += vec;
            }

            if (!isZoomLerp) t_Cam.position = armPivot.position + armPivot.TransformDirection(offset);

            //if (!isZoomLerp) m_Cam.transform.position = armPivot.position + armPivot.TransformDirection(offset);
            else
            {
                t_Cam.position =
                Vector3.Lerp(t_Cam.position, armPivot.position + armPivot.TransformDirection(offset), Time.fixedDeltaTime * zoomSpeed * zoomDemp);
            }
        }
        public void Rotate(Transform armPivot)
        {
            angleX += inputX * (rotaionSpeed * Constants.mouseSensitivity);
            angleY -= inputY * (rotaionSpeed * Constants.mouseSensitivity);
            angleY = Mathf.Clamp(angleY, minAngleY, maxAngleY);

            Vector3 rotation = Vector3.zero;

            rotation.y = angleX;
            if (canY) rotation.x = angleY;
            else rotation.x = .0f;

            if (!isRotationLerp) armPivot.localRotation = Quaternion.Euler(rotation);
            else
            {
                armPivot.localRotation =
                Quaternion.Lerp(armPivot.localRotation, Quaternion.Euler(rotation), Time.fixedDeltaTime * rotaionSpeed * rotationDemp);
            }

            t_Cam.LookAt(t_LookTarget.position);
        }
    }
}