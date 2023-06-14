namespace CustomCamera
{
    // 컴파일러나 런타임 최적화 등에 의해 상수 값이 상수 폴딩(Constant Folding)이라는 기술을 사용하여 효율적으로 처리되기에
    // 매번 전역클래스의 참조로 상수값을 받아 온다고 해도 성능차이는 거의 없다.

    public static partial class Constants
    {
        //==========================================================================
        // <CameraBrain>

        //Debug
        public const string T_Changing = "Changing...";

        public const string T_Debug    = "Debug Text...";
        public const string T_Camera   = "Camera : ";
        public const string T_Noise    = "Noise : ";
        public const string T_Null     = "null";
        public const string T_Enter    = "\n";

        public const float DebugRect_x = 10;
        public const float DebugRect_y = 10;
        public const float DebugRect_w = 400;
        public const float DebugRect_h = 40;
        //==========================================================================

        //==========================================================================
        // <ThirdPersonCamera>

        public const float mouseSensitivity = 1.0f;
        
        public const string MouseX = "Mouse X";
        public const string MouseY = "Mouse Y";
        public const string MouseWheel = "Mouse ScrollWheel";

        public const string CameraArmObjectName = "3DArmPivot";

        //Follow
        public const bool  F_DefaultIsLerp = true;
        public const float F_DefaultDemp   = 1.0f;
        public const float F_DefaultSpeed  = .1f;

        //Zoom
        public const bool  Z_DefaultIsLerp = true;
        public const float Z_DefaultDemp   = 1.0f;
        public const float Z_DefaultSpeed  = 3.0f;
        public const float Z_DefaultMinDistance = 20.0f;
        public const float Z_DefaultMaxDistance = 35.0f;

        //Rotation
        public const bool  R_DefaultCanRotation = true;
        public const bool  R_DefaultIsLerp   = true;
        public const bool  R_DefaultCanY     = true;
        public const float R_DefaultDemp     = 1.0f;
        public const float R_DefaultSpeed    = 3.0f;
        public const float R_DefaultMinAngle = -45.0f;
        public const float R_DefaultMaxAngle = 45.0f;
        //==========================================================================

        //==========================================================================
        // <TrackCamera>

        public const float T_DefaultThickness = 1.0f;
        public const int   T_DefaultSubdivision = 22;
        public const float T_DefaultSpeed = 6.0f;
        public const float T_DefaultRotationSpeed = 6.0f;
        //==========================================================================

        //==========================================================================
        // <InteractionData>

        public const bool  I_DefaultPlayOnAwake = true;
        public const bool  I_DefaultIsPlay      = false;
        public const bool  I_DefaultIsStop      = true;

        public const float I_DefaultIntensity  = .02f;   //0.01 ~ 1  
        public const float I_DefaultFrequency  = .7f;    //0.10 ~ 5


        //Noise
        public const string N_ScriptableFileName = "Noise Data";
        public const string N_MenuName = "Create NoiseData";
        public const int N_Order = 0;

        //Shake
        public const string S_ScriptableFileName = "Shake Data";
        public const string S_MenuName = "Create ShakeData";
        public const int S_Order = 1;
        //==========================================================================

        //==========================================================================
        // <InteractionData>

        //ImpulsePoint

        public const float P_DefaultIntensity    = 10.0f;
        public const float P_DefaultRadius       = 20.0f;
        public const int   P_DefaultSectionCount = 6;
        public const float P_DefaultSpeed        = 10.0f;

        public const bool  P_DefaultIsPlay  = false;
        public const bool  P_DefaultIsStop  = true;
        public const bool  P_DefaultIsAlive = false;

        public const float P_DefaultStartRadius = 1.0f;
        //==========================================================================

        //==========================================================================
        // <InteractionData>

        public const bool DefaultIsAffected = true;
        public const bool DefaultIsApply = false;

        public const float rotationMultiplier = 0.005f;
        //==========================================================================
    }


    //EditorProperty
    public static partial class Constants
    {
        public const float DefaultSpace = 5.0f;

        //==========================================================================
        // <CameraBrainEditor>

        public const string SubCameraListPropertyName = "subCameras";

        public const string Empty                     = "";
        public const string SubCameraListHeader       = "SubCameras";
        public const string ThirdPersonCameraName     = "ThirdPersonCamera";
        public const string TrackCameraName           = "TrackCamera";

        public const string Label_Debug               = "Show DebugText";
        public const float Label_DebugWidth = 120.0f;
        //==========================================================================

        //==========================================================================
        // <SubCameraEditor>

        public const float DefaultLabelWidth = 105.0f;
        public const float LabelWidth        = 65.0f;
        public const float FieldWidth        = 50.0f;

        public const string Depth           = "Depth";
        public const string EnterMode       = "EnterMode";
        public const string EnterSpeed      = "EnterSpeed";
        public const string AffectedByShake = "AffectedByShake";
        public const string Option          = "Option";
        public const string Noise           = "Noise";
        //==========================================================================

        //==========================================================================
        // <ThirdPersonCameraEditor>

        public const string TPC_FoldHeader        = "Camera Setting";

        public const string TPC_TargetLabel       = "Target";
        public const string TPC_CamArmLabel       = "CameraArm";
        public const string TPC_FollowTargetLabel = "FollowTarget";
        public const string TPC_LookTargetLabel   = "LookTarget";

        public const string TPC_FollowLabel       = "Follow";
        public const string TPC_ZoomLabel         = "Zoom";
        public const string TPC_RotateLabel       = "Rotate";

        public const string TPC_SmoothLabel       = "Smooth";
        public const string TPC_DempLabel         = "Demp";
        public const string TPC_SpeedLabel        = "Speed";

        public const string TPC_MinMaxLabel       = "Min/Max";
        public const string TPC_CanYLabel         = "CanY";

        public const string TPC_WarningMessage    = "If you don't have a camera arm, it won't work!";
        public const string TPC_UndoFileName      = "Modified Cam";

        public const float TPC_ObjectFieldWidth  = 90.0f;

        public const float TPC_DempSliderWidth   = 265.0f;
        public const float TPC_DempSliderMin     = 0.1f;
        public const float TPC_DempSliderMax     = 1.0f;

        public const float TPC_MinMaxSliderWidth = 150.0f;
        public const float TPC_SliderValueWidth  = 50.0f;
        public const float TPC_ZoomSliderMin     = -90.0f;
        public const float TPC_ZoomSliderMax     = 90.0f;
        //==========================================================================

        //==========================================================================
        // <TrackCameraEditor>

        public const float TC_DefaultDetectionRange     = 100.0f;
        public const int   TC_DefaultSelecedHandleIndex = -1;
        public const float TC_DefaultDistance           = 15.0f;

        public const float HandleSize                   = 0.2f;

        //HandleValue
        public const string HV_Vector     = "vec";
        public const string HV_Quaternion = "quat";

        public const string TrackListPropertyName = "trackList";


        public const string TC_FoldHeader     = "Camera Setting";

        public const string TC_TrailLabel     = "Trail";
        public const string TC_ThicknessLabel = "Thockness";
        public const string TC_Subdivision    = "Subdivision";

        public const string TrackCameraListHeader         = "TrackList";
        public const string TrackCameraListVectorName     = "P";
        public const string TrackCameraListQuaternionName = "Q";

        public const string TC_CameraLabel        = "Camera";
        public const string TC_LookTargetLabel    = "LookTarget";
        public const string TC_MoveSpeedLabel     = "CameraMoveSpeed";
        public const string TC_RotationSpeedLabel = "CameraRotaionSpeed";

        public const float TC_ButtonWidth = 20.0f;
        public const float TC_ButtonLabel = 30.0f;

        public const float TC_LabelWidth   = 185.0f;
        public const float TC_ThicknessMin = .0f;
        public const float TC_ThicknessMax = 10.0f;

        public const int TC_SubdivisionMin = 0;
        public const int TC_SubdivisionMax = 100;

        public const float TC_MoveSpeedMin = .0f;
        public const float TC_MoveSpeedMax = 50.0f;

        public const string TC_UndoFileName = "Track Handle";
        //==========================================================================

        //==========================================================================
        // <ImpulseEditor>

        public const float I_DefaultStartAlpha = .4f;
        public const float I_Half              = .5f;

        public const float I_Label  = 185.0f;
        public const float I_Toggle = 15.0f;

        public const string PhysicMaterialPath   = "PhysicMaterials";

        public const string I_AffectLabel        = "IsAffected";
        public const string I_RigidbodyLabel     = "Rigidbody";
        public const string I_ColliderLabel      = "Collider";
        public const string I_RigidbodyTypeLabel = "RigidType";
        public const string I_PositionLockLabel  = "Position Lock";
        public const string I_RotationLockLabel  = "Rotation Lock";

        public const string I_XLabel = "X";
        public const string I_YLabel = "Y";
        public const string I_ZLabel = "Z";
        //==========================================================================

        //==========================================================================
        // <InteractionDataEditor>

        public const float DataLabel  = 120.0f;
        public const float DataToggle = 15.0f;

        public const string DataPlayOnAwake = "PlayOnAwake";
        public const string DataIntensity   = "Intensity";
        public const string DataFrequency   = "Frequency";

        public const float DataIntensityMin = .01f;
        public const float DataIntensityMax = 1.0f;

        public const float DataFrequencyMin = .1f;
        public const float DataFrequencyMax = 5.0f;
        //==========================================================================
    }
}


