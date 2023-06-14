namespace CustomCamera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface ISubBrain { void SetCam(Camera camera, Transform t_cam); }
    public interface ISubCamera { }
    public enum ChangeMode { Clamp, Smooth }

    public class CameraBrain : MonoBehaviour
    {
        private static CameraBrain instance;
        public static CameraBrain Instance => instance;

        public bool showDebug = true;
        private string debugText = Constants.T_Debug;

        public Camera cam;
        public Transform t_cam;

        //public static Camera Cam => Cam;

        private List<ShakeData> shakeDatas = new List<ShakeData>();
        public List<SubCamera> subCameras = new List<SubCamera>();

        public SubCamera curCamera;
        public SubCamera prevCamera;
        public ISubCamera i_curCamera;
        public ISubCamera i_prevCamera;

        public void AddShakeData(ShakeData data) { if(data != null) shakeDatas.Add(data); }
        void Debuging(string text) => debugText = text;
        void Debuging(SubCamera nextCam)
        {
            debugText = Constants.T_Camera;
            if (nextCam == null) debugText += Constants.T_Null;
            else debugText += nextCam.gameObject.name;
            debugText += Constants.T_Enter;

            debugText += Constants.T_Noise;
            if (nextCam.noiseData == null) debugText += Constants.T_Null;
            else debugText += nextCam.noiseData.name;
        }
        private void OnGUI() 
        {
            if (showDebug)
                GUI.Label(new Rect(Constants.DebugRect_x, Constants.DebugRect_y, 
                                   Constants.DebugRect_w, Constants.DebugRect_h), debugText);
        }

        public void ChangeCam(SubCamera nextCam) => StartCoroutine(SetCurCam(nextCam));
        public void ChangeCam() => StartCoroutine(SetCurCam(FindNextCam()));
        public void ChangeCam(int depth) => StartCoroutine(SetCurCam(FindByDepth(depth)));
        public SubCamera FindByDepth(int depth)
        {
            for(int i = 0; i < subCameras.Count; i++)
                if(subCameras[i].depth == depth) return subCameras[i];
            
            return null;
        }

        void SortSubCamera() { }
        
        SubCamera FindNextCam()
        {
            if (subCameras.Count <= 0) return null;

            SubCamera minCam = subCameras[0];

            for (int i = 0; i < subCameras.Count; i++)
                if (subCameras[i].depth <= minCam.depth) minCam = subCameras[i];

            return minCam;
        }
        IEnumerator SmoothChange(SubCamera nextCam)
        {
            Vector3 startPos = cam.transform.position;
            Quaternion startQuat = cam.transform.rotation;

            Vector3 targetPos = nextCam.initPos;
            Quaternion targetQuat = nextCam.initQuat;

            float moveDistance = Vector3.Distance(startPos, targetPos);
            float curDistance = .0f;

            while (curDistance < moveDistance)
            {
                yield return null;

                float t = curDistance / moveDistance;

                cam.transform.position = Vector3.Lerp(startPos, targetPos, t);
                cam.transform.rotation = Quaternion.Slerp(startQuat, targetQuat, t);

                curDistance += Time.deltaTime * nextCam.changeCamSpeed;
            }

            cam.transform.position = nextCam.initPos;
            cam.transform.rotation = nextCam.initQuat;
        }
        void ClampChange(SubCamera nextCam)
        {
            cam.transform.position = nextCam.initPos;
            cam.transform.rotation = nextCam.initQuat;
        }
        IEnumerator SetCurCam(SubCamera nextCam)
        {
            if (nextCam != null)
            {
#if UNITY_EDITOR
                if (showDebug) Debuging(Constants.T_Changing);
#endif
                if(shakeDatas.Count > 0)
                {
                    for(int i = 0; i < shakeDatas.Count; i++)
                    { if (shakeDatas[i] != null) { shakeDatas[i].ResetData(); shakeDatas.RemoveAt(i); } }
                }

                if (curCamera != null)
                {
                    curCamera.isEnterReady = false;

                    curCamera.CamExit();
                    prevCamera = curCamera;
                    i_prevCamera = (ISubCamera)curCamera;
                }

                if (nextCam.enterMode == ChangeMode.Clamp) ClampChange(nextCam);
                else if (nextCam.enterMode == ChangeMode.Smooth) yield return StartCoroutine(SmoothChange(nextCam));

                curCamera = nextCam;
                curCamera.isEnterReady = true;

                i_curCamera = (ISubCamera)curCamera;
                curCamera.CamEnter();
#if UNITY_EDITOR
                if(showDebug) Debuging(nextCam);
#endif
            }
        }
        void Awake() 
        {
            instance = this; 
            cam = GetComponent<Camera>();
            t_cam = cam.transform;

            //사전 정렬을 필요로 한다!
            SortSubCamera();

            if(subCameras.Count > 0)
            {
                for (int i = 0; i < subCameras.Count; i++) ((ISubBrain)subCameras[i]).SetCam(cam, t_cam);

                ChangeCam();
            }
        }

        private void Update() 
        {
            if (curCamera == null || !curCamera.isEnterReady) return; curCamera.CamUpdate();
        }
        private void FixedUpdate() 
        { 
            if (curCamera == null || !curCamera.isEnterReady) return; curCamera.CamFixedUpdate();
        }
        private void LateUpdate()
        { 
            if (curCamera == null || !curCamera.isEnterReady) return; curCamera.CamLateUpdate();
            if (curCamera.noiseData == null) return; curCamera.noiseData.Noise(t_cam);

            if (shakeDatas.Count <= 0 || !curCamera.isAffectedByShake) return;

            for (int i = 0; i < shakeDatas.Count; i++)
            {
                ShakeData data = shakeDatas[i];
                data.Shake(t_cam);
                if (!data.isAlive) { data.ResetData(); shakeDatas.RemoveAt(i); }
            }
        }
    }

    public abstract class SubCamera : MonoBehaviour , ISubBrain
    {
        public bool isShowOption, isShowCameraSetting; // EditorProperty

        protected CameraBrain m_Brain => CameraBrain.Instance;
        public Camera m_Cam;
        public Transform t_Cam;

        public bool isAffectedByShake = true;

        public int depth;
        public ChangeMode enterMode;
        public bool isEnterReady = false; 
        public Vector3 initPos;
        public Quaternion initQuat;

        public float changeCamSpeed;

        public NoiseData noiseData;

        protected abstract void Initialize();
        void ISubBrain.SetCam(Camera cam,Transform t_cam) 
        {
            if (m_Cam == null) { this.m_Cam = cam; this.t_Cam = t_cam;}

            if (noiseData != null)
            {
                noiseData.Stop();

                if (noiseData.playOnAwake) noiseData.Play();
                else noiseData.Stop(); 
            }
            Initialize();
        }
        public abstract void CamEnter();
        public virtual void CamUpdate() { }
        public virtual void CamFixedUpdate() { }
        public virtual void CamLateUpdate() { }
        public abstract void CamExit();
        protected virtual void OnDestroy() => CameraBrain.Instance.subCameras.Remove(this);
    }
}