namespace CustomCamera
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    [CustomEditor(typeof(TrackCamera))]
    public class TrackCameraEditor : SubCameraEditor<TrackCamera>
    {
        ReorderableList list;

        private float detectionRange      = Constants.TC_DefaultDetectionRange;
        private int   selectedHandleIndex = Constants.TC_DefaultSelecedHandleIndex;
        private float defaultDistance     = Constants.TC_DefaultDistance;

        bool IsItemInList(ReorderableList list) => list.serializedProperty.arraySize == 0 ? false : true;
        //size : 0 ~ 1
        void RenderingHandle(ReorderableList list, int selectionIndex, Color PointColor, Color defaultColor, float size)
        {
            for (int i = 0; i < list.serializedProperty.arraySize; i++)
            {
                EditorGUI.BeginChangeCheck();

                Handles.color = i == selectionIndex ? PointColor : defaultColor;

                Vector3 pointPos = CCEditor.Property(list, i, Constants.HV_Vector).vector3Value;
                Quaternion quat = CCEditor.Property(list, i, Constants.HV_Quaternion).quaternionValue;

                Vector3 handlePosition = pointPos;
                Quaternion handleRotation = quat;
                float handleSize = HandleUtility.GetHandleSize(handlePosition) * size;

                Handles.SphereHandleCap(0, handlePosition, handleRotation, handleSize, EventType.Repaint);

                if (i == selectionIndex)
                {
                    handlePosition = Handles.PositionHandle(handlePosition, handleRotation);
                    handleRotation = Handles.RotationHandle(handleRotation, handlePosition);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(target, Constants.TC_UndoFileName);

                    CCEditor.Property(list, i, Constants.HV_Vector).vector3Value = handlePosition;
                    CCEditor.Property(list, i, Constants.HV_Quaternion).quaternionValue = handleRotation;

                    serializedObject.ApplyModifiedProperties();
                    SaveTrackPoints();
                }
            }
            Handles.color = defaultColor;
        }
        void RenderingTrail(Vector3 startPoint, Vector3 endPoint, Quaternion startQuat, Quaternion endQuat)
        {
            float width = cam.thickness;
            float radius = width * 0.5f;

            Handles.color = Color.blue;
            Vector3 curArcPos = startPoint + startQuat * Vector3.zero * radius;
            Vector3 endArcPos = endPoint + endQuat * Vector3.zero * radius;

            Handles.DrawLine(curArcPos, endArcPos);

            Handles.color = Color.green;

            Vector3 R_curArcPos = startPoint + startQuat * Vector3.right * radius;
            Vector3 R_endArcPos = endPoint + endQuat * Vector3.right * radius;

            Vector3 L_curArcPos = startPoint + startQuat * Vector3.left * radius;
            Vector3 L_endArcPos = endPoint + endQuat * Vector3.left * radius;

            Handles.DrawLine(R_curArcPos, R_endArcPos);
            Handles.DrawLine(L_curArcPos, L_endArcPos);




            if (cam.subdivision < 1) return;

            float r_length = Vector3.Distance(R_curArcPos, R_endArcPos) / cam.subdivision;
            float l_length = Vector3.Distance(L_curArcPos, L_endArcPos) / cam.subdivision;

            Vector3 r_dir = (R_endArcPos - R_curArcPos).normalized;
            Vector3 l_dir = (L_endArcPos - L_curArcPos).normalized;

            for (int i = 1; i < cam.subdivision; i++)
            {
                Vector3 point0 = R_curArcPos + r_dir * r_length * i;
                Vector3 point1 = L_curArcPos + l_dir * l_length * i;

                Handles.DrawLine(point0, point1);
            }
        }
        void SaveTrackPoints()
        {
            if (cam.trackPoints == null) cam.trackPoints = new List<HandleValue>();

            int inspectorListCount = list.serializedProperty.arraySize;

            if (inspectorListCount <= 0) { cam.trackPoints.Clear(); return; }

            if (cam.trackPoints.Count != inspectorListCount)
            {
                cam.trackPoints.Clear();

                for (int i = 0; i < inspectorListCount; i++)
                {
                    HandleValue value = new HandleValue(CCEditor.Property(list, i, Constants.HV_Vector).vector3Value,
                                                   CCEditor.Property(list, i, Constants.HV_Quaternion).quaternionValue);

                    cam.trackPoints.Add(value);
                }
            }
            else
            {
                for (int i = 0; i < inspectorListCount; i++)
                {
                    HandleValue value = new HandleValue(CCEditor.Property(list, i, Constants.HV_Vector).vector3Value,
                                                   CCEditor.Property(list, i, Constants.HV_Quaternion).quaternionValue);

                    if (cam.trackPoints[i] != value) cam.trackPoints[i] = value;
                }
            }
        }
        void UpdateSelectionIndex()
        {
            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
            {
                int result = -1;

                Vector2 mousePosition = currentEvent.mousePosition;
                float minDistance = detectionRange;

                for (int i = 0; i < list.serializedProperty.arraySize; i++)
                {
                    Vector3 handlePosition = CCEditor.Property(list, i, Constants.HV_Vector).vector3Value;

                    Vector2 handleScreenPosition = HandleUtility.WorldToGUIPoint(handlePosition);
                    float distance = Vector2.Distance(mousePosition, handleScreenPosition);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        result = i;
                    }
                }
                selectedHandleIndex = result;
            }
        }
        protected override void OnEnable()
        {
            base.OnEnable();

            list = new ReorderableList(serializedObject, serializedObject.FindProperty(Constants.TrackListPropertyName), true, true, true, true);

            list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, Constants.TrackCameraListHeader); };

            #region AddCallBack
            list.onAddCallback = (ReorderableList l) =>
            {
                if (!IsItemInList(l))
                {
                    Transform pos = Camera.main.transform;
                    float h = pos.position.y;

                    var index = list.serializedProperty.arraySize;

                    Vector3 trackPos0 = pos.position;
                    Vector3 trackPos1 = pos.position + pos.forward * defaultDistance;
                    trackPos0.y = trackPos1.y = h;

                    Quaternion desiredRotation = Quaternion.LookRotation(trackPos1 - trackPos0, Vector3.up);

                    list.serializedProperty.arraySize += 2;

                    CCEditor.Property(list, index, Constants.HV_Vector).vector3Value = trackPos0;
                    CCEditor.Property(list, index, Constants.HV_Quaternion).quaternionValue = desiredRotation;

                    CCEditor.Property(list, index + 1, Constants.HV_Vector).vector3Value = trackPos1;
                    CCEditor.Property(list, index + 1, Constants.HV_Quaternion).quaternionValue = desiredRotation;
                }
                else
                {
                    Transform pos = Camera.main.transform;
                    float h = pos.position.y;

                    var index = list.serializedProperty.arraySize;

                    Vector3 prev1TrackPos = CCEditor.Property(list, index - 1, Constants.HV_Vector).vector3Value;
                    Vector3 prev0TrackPos = CCEditor.Property(list, index - 2, Constants.HV_Vector).vector3Value;

                    Vector3 nextTrackPos = prev1TrackPos + (prev1TrackPos - prev0TrackPos).normalized * defaultDistance;

                    list.serializedProperty.arraySize++;

                    CCEditor.Property(list, index, Constants.HV_Vector).vector3Value = nextTrackPos;
                    CCEditor.Property(list, index, Constants.HV_Quaternion).quaternionValue =
                            CCEditor.Property(list, index - 1, Constants.HV_Quaternion).quaternionValue;

                }

                serializedObject.ApplyModifiedProperties();
                SaveTrackPoints();
            };
            #endregion

            #region RemoveCallback
            list.onRemoveCallback = (ReorderableList l) =>
            {
                int indexToRemove = l.index;

                if (indexToRemove >= 0 && indexToRemove < l.serializedProperty.arraySize)
                {
                    l.serializedProperty.DeleteArrayElementAtIndex(indexToRemove);

                    l.index = Mathf.Clamp(l.index, 0, l.serializedProperty.arraySize - 1);

                    serializedObject.ApplyModifiedProperties();
                    SaveTrackPoints();
                }
            };
            #endregion

            #region DrawElemnetCallBack
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index); rect.y += 2;
                float buttonWidth = Constants.TC_ButtonWidth;
                float labelWidth = Constants.TC_ButtonLabel;
                float fieldWidth = (rect.width - labelWidth) * 0.5f;

                if (GUI.Button(new Rect(rect.x, rect.y, buttonWidth, EditorGUIUtility.singleLineHeight), $"{index}"))
                {
                    // 버튼 클릭 시 실행할 코드
                }

                EditorGUI.LabelField(new Rect(rect.x + 19, rect.y, labelWidth, EditorGUIUtility.singleLineHeight),
                                        Constants.TrackCameraListVectorName);

                var vecProperty = element.FindPropertyRelative(Constants.HV_Vector);
                vecProperty.vector3Value = EditorGUI.Vector3Field(new Rect(rect.x + labelWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), GUIContent.none, vecProperty.vector3Value);

                EditorGUI.LabelField(new Rect(rect.x + 19 + labelWidth + fieldWidth, rect.y, labelWidth, EditorGUIUtility.singleLineHeight), Constants.TrackCameraListQuaternionName);

                Quaternion quaternionValue = element.FindPropertyRelative(Constants.HV_Quaternion).quaternionValue;
                Vector3 eulerValue = quaternionValue.eulerAngles;

                eulerValue = EditorGUI.Vector3Field(new Rect(rect.x + labelWidth * 2 + fieldWidth, rect.y, fieldWidth, EditorGUIUtility.singleLineHeight), GUIContent.none, eulerValue);

                element.FindPropertyRelative(Constants.HV_Quaternion).quaternionValue = Quaternion.Euler(eulerValue);
            };
            #endregion

            SaveTrackPoints();
        }
        protected override void InspectorGUI()
        {
            CommonInspectorGUI();

            cam.isShowCameraSetting = 
                EditorGUILayout.BeginFoldoutHeaderGroup(cam.isShowCameraSetting, Constants.TC_FoldHeader);

            if (cam.isShowCameraSetting)
            {
                EditorGUI.BeginChangeCheck();
                #region Trail
                GUILayout.Label(Constants.TC_TrailLabel);
                GUILayout.Space(Constants.DefaultSpace * 2);

                CCEditor.Slider<float>(Constants.TC_ThicknessLabel, ref cam.thickness,
                    Constants.TC_ThicknessMin, Constants.TC_ThicknessMax,Constants.TC_LabelWidth);

                CCEditor.Slider<int>(Constants.TC_Subdivision, ref cam.subdivision,
                    Constants.TC_SubdivisionMin, Constants.TC_SubdivisionMax, Constants.TC_LabelWidth);

                if (EditorGUI.EndChangeCheck()) SceneView.RepaintAll();

                GUILayout.Space(Constants.DefaultSpace * 2);
                serializedObject.Update();
                list.DoLayoutList();
                serializedObject.ApplyModifiedProperties();
                #endregion

                #region Camera
                GUILayout.Label(Constants.TC_CameraLabel);
                GUILayout.Space(Constants.DefaultSpace * 2);


                CCEditor.ObjectField<GameObject>(Constants.TC_LookTargetLabel,ref cam.lookTarget,
                                                    Constants.TC_LabelWidth);

                CCEditor.Slider<float>(Constants.TC_MoveSpeedLabel, ref cam.moveSpeed,
                    Constants.TC_MoveSpeedMin, Constants.TC_MoveSpeedMax, Constants.TC_LabelWidth);

                CCEditor.Slider<float>(Constants.TC_RotationSpeedLabel, ref cam.rotationSpeed,
                    Constants.TC_MoveSpeedMin, Constants.TC_MoveSpeedMax, Constants.TC_LabelWidth);

                #endregion
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            base.InspectorGUI();
        }
        void OnSceneGUI()
        {
            if (list.serializedProperty.arraySize < 2) return;

            UpdateSelectionIndex();

            RenderingHandle(list, selectedHandleIndex, Color.red, Color.green, Constants.HandleSize);

            Handles.color = Color.green;

            for (int i = 0; i < list.serializedProperty.arraySize - 1; i++)
            {
                RenderingTrail(CCEditor.Property(list, i,     Constants.HV_Vector).vector3Value,
                               CCEditor.Property(list, i + 1, Constants.HV_Vector).vector3Value,
                               CCEditor.Property(list, i,     Constants.HV_Quaternion).quaternionValue,
                               CCEditor.Property(list, i + 1, Constants.HV_Quaternion).quaternionValue);
            }
        }
    }
}