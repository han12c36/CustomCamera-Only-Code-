namespace CustomCamera
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public struct HandleValue 
    {
        public Vector3 vec;
        public Quaternion quat;

        public HandleValue(Vector3 vec,Quaternion quat)
        {
            this.vec = vec;
            this.quat = quat;
        }

        #region Operation
        public static bool operator ==(HandleValue a, HandleValue b) => a.vec == b.vec && a.quat == b.quat;
        public static bool operator !=(HandleValue a, HandleValue b) => !(a == b);
        #endregion
        public void SetVec(Vector3 _vec) => vec = _vec;
        public void SetQuat(Quaternion _quat) => quat = _quat;
    }

    public class TrackCamera : SubCamera , ISubCamera
    {
        public List<HandleValue> trackList;
        public List<HandleValue> trackPoints;
        public float thickness     = Constants.T_DefaultThickness;
        public int   subdivision   = Constants.T_DefaultSubdivision;
        public float moveSpeed     = Constants.T_DefaultSpeed;
        public float rotationSpeed = Constants.T_DefaultRotationSpeed;

        public GameObject lookTarget;

        public Coroutine Move;
        
        protected override void Initialize() 
        {
            if (trackPoints.Count <= 0) return;

            initPos  = trackPoints[0].vec;
            initQuat = trackPoints[0].quat;
        }
        public override void CamEnter() { StartCoroutine(TrackMove(trackPoints)); }
        public override void CamExit() { }
        IEnumerator TrackMove(List<HandleValue> track)
        {
            t_Cam.position = track[0].vec;

            if(trackPoints.Count > 0) 
            {
                float curDistance = .0f;

                for (int i = 0; i < track.Count - 1; i++)
                {
                    Vector3 startPos = track[i].vec;
                    Quaternion startQuat = track[i].quat;

                    Vector3 nextPos = track[i + 1].vec;
                    Quaternion nextQuat = track[i + 1].quat;

                    Vector3 startArcPos = startPos + startQuat * Vector3.zero * thickness * 0.5f;
                    Vector3 nextArcPos = nextPos + nextQuat * Vector3.zero * thickness * 0.5f;

                    float moveDistance = Vector3.Distance(startArcPos, nextArcPos);

                    while (curDistance < moveDistance)
                    {
                        float t = curDistance / moveDistance;

                        t_Cam.position = Vector3.Lerp(startPos, nextPos, t);
                        t_Cam.rotation = Quaternion.Slerp(startQuat, nextQuat, t);

                        curDistance += Time.deltaTime * moveSpeed;

                        if(lookTarget != null) t_Cam.LookAt(lookTarget.transform.position);

                        yield return null;
                    }
                    curDistance = .0f;
                }
            }

            m_Brain.ChangeCam();
        }
    }
}