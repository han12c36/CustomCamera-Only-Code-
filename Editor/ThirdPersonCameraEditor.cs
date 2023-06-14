namespace CustomCamera
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ThirdPersonCamera))]
    public class ThirdPersonCameraEditor : SubCameraEditor<ThirdPersonCamera>
    {
        protected override void InspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            CommonInspectorGUI();

            cam.isShowCameraSetting = 
                EditorGUILayout.BeginFoldoutHeaderGroup(cam.isShowCameraSetting, Constants.TPC_FoldHeader);

            if (cam.isShowCameraSetting)
            {
                #region CameraSetting
                EditorGUILayout.Space(Constants.DefaultSpace);
                #region Target

                EditorGUILayout.LabelField(Constants.TPC_TargetLabel);
                EditorGUI.indentLevel++;

                CCEditor.ObjectField<Transform>
                    (Constants.TPC_CamArmLabel, ref cam.cameraArm, Constants.TPC_ObjectFieldWidth);
                CCEditor.ObjectField<GameObject>
                    (Constants.TPC_FollowTargetLabel, ref cam.followTarget, Constants.TPC_ObjectFieldWidth);
                CCEditor.ObjectField<GameObject>
                    (Constants.TPC_LookTargetLabel, ref cam.lookTarget, Constants.TPC_ObjectFieldWidth);

                EditorGUI.indentLevel--;
                #endregion

                EditorGUILayout.Space(5);

                #region Follow
                EditorGUILayout.LabelField(Constants.TPC_FollowLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Constants.TPC_SmoothLabel, OP_Label);
                cam.isFollowLerp = EditorGUILayout.Toggle(cam.isFollowLerp, OP_Field);
                if (cam.isFollowLerp)
                {
                    EditorGUILayout.LabelField(Constants.TPC_DempLabel, OP_Label);
                    cam.followDemp = 
                        EditorGUILayout.Slider(cam.followDemp, 
                                               Constants.TPC_DempSliderMin, Constants.TPC_DempSliderMax, 
                                               GUILayout.Width(Constants.TPC_DempSliderWidth));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(Constants.DefaultSpace);

                CCEditor.Field<float>(Constants.TPC_SpeedLabel, ref cam.followSpeed,        
                                            Constants.LabelWidth,Constants.FieldWidth);

                EditorGUI.indentLevel--;
                #endregion

                EditorGUILayout.Space(Constants.DefaultSpace);

                #region Zoom

                EditorGUILayout.LabelField(Constants.TPC_ZoomLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Constants.TPC_SmoothLabel, OP_Label);
                cam.isZoomLerp = EditorGUILayout.Toggle(cam.isZoomLerp, OP_Field);
                if (cam.isZoomLerp)
                {
                    EditorGUILayout.LabelField(Constants.TPC_DempLabel, OP_Label);
                    cam.zoomDemp = EditorGUILayout.Slider(cam.zoomDemp, 
                                                          Constants.TPC_DempSliderMin, Constants.TPC_DempSliderMax, 
                                                          GUILayout.Width(Constants.TPC_DempSliderWidth));
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(Constants.DefaultSpace);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Constants.TPC_SpeedLabel, OP_Label);
                cam.zoomSpeed = EditorGUILayout.FloatField(cam.zoomSpeed, OP_Field);

                EditorGUILayout.LabelField(Constants.TPC_MinMaxLabel, OP_Label);
                EditorGUILayout.LabelField(cam.minZoomDIstance.ToString("F1"), 
                                           GUILayout.Width  (Constants.TPC_SliderValueWidth));

                EditorGUILayout.MinMaxSlider
                    (ref cam.minZoomDIstance, ref cam.maxZoomDIstance, 
                     Constants.TPC_ZoomSliderMin, Constants.TPC_ZoomSliderMax, 
                     GUILayout.Width(Constants.TPC_MinMaxSliderWidth));

                EditorGUILayout.LabelField(cam.maxZoomDIstance.ToString("F1"), 
                                           GUILayout.Width(Constants.TPC_SliderValueWidth));

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
                #endregion

                EditorGUILayout.Space(Constants.DefaultSpace);

                #region Rotate

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Constants.TPC_RotateLabel, GUILayout.Width(40));
                cam.canRotation = EditorGUILayout.Toggle(cam.canRotation);
                EditorGUILayout.EndHorizontal();
                if (cam.canRotation)
                {
                    EditorGUI.indentLevel++;

                    EditorGUILayout.Space(Constants.DefaultSpace);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(Constants.TPC_SmoothLabel, OP_Label);
                    cam.isRotationLerp = EditorGUILayout.Toggle(cam.isRotationLerp, OP_Field);
                    if (cam.isRotationLerp)
                    {
                        EditorGUILayout.LabelField(Constants.TPC_DempLabel, OP_Label);
                        cam.rotationDemp = 
                            EditorGUILayout.Slider(cam.rotationDemp, 
                                                   Constants.TPC_DempSliderMin, Constants.TPC_DempSliderMax,
                                                   GUILayout.Width(Constants.TPC_DempSliderWidth));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space(Constants.DefaultSpace);

                    CCEditor.Field<float>(Constants.TPC_SpeedLabel, ref cam.rotaionSpeed, 
                                          Constants.LabelWidth, Constants.FieldWidth);

                    EditorGUILayout.Space(Constants.DefaultSpace);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(Constants.TPC_CanYLabel, OP_Label);
                    cam.canY = EditorGUILayout.Toggle(cam.canY, OP_Field);
                    if (cam.canY)
                    {
                        EditorGUILayout.LabelField(Constants.TPC_MinMaxLabel, OP_Label);
                        EditorGUILayout.LabelField(cam.minAngleY.ToString("F1"), 
                                                   GUILayout.Width(Constants.TPC_SliderValueWidth));

                        EditorGUILayout.MinMaxSlider(ref cam.minAngleY, ref cam.maxAngleY, 
                                                     Constants.TPC_ZoomSliderMin, Constants.TPC_ZoomSliderMax, 
                                                     GUILayout.Width(Constants.TPC_MinMaxSliderWidth));

                        EditorGUILayout.LabelField(cam.maxAngleY.ToString("F1"),
                                                   GUILayout.Width(Constants.TPC_SliderValueWidth));
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUI.indentLevel--;
                }
                #endregion
                EditorGUILayout.Space(Constants.DefaultSpace);
                #endregion
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            base.InspectorGUI();

            if (cam.cameraArm == null)
                EditorGUILayout.HelpBox(Constants.TPC_WarningMessage, MessageType.Warning);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(cam, Constants.TPC_UndoFileName);
                EditorUtility.SetDirty(cam);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}