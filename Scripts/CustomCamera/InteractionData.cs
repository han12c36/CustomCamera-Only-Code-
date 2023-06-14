namespace CustomCamera
{
    using UnityEngine;

    public class InteractionData : ScriptableObject
    {
        public    bool playOnAwake = Constants.I_DefaultPlayOnAwake;
        protected bool isPlay      = Constants.I_DefaultIsPlay;
        protected bool isStop      = Constants.I_DefaultIsStop;

        public float intensity = Constants.I_DefaultIntensity;
        public float frequency = Constants.I_DefaultFrequency;  
        public bool x, y, z;


        public bool isShowData;    //editorProperty

        public bool IsPlay => isPlay;
        public bool IsStop => isStop;

        public void Play() =>
            (isPlay, isStop) = (isPlay == false && isStop == true || isPlay == false && isStop == false, false);
        public void Stop() =>
            (isStop, isPlay) = (isPlay == true && isStop == false || isPlay == false && isStop == false, false);
    }
}