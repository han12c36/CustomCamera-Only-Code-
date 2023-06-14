namespace CustomCamera
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditorInternal;
    using System.Reflection.Emit;

    //[ExecuteInEditMode]

    [CustomEditor(typeof(CameraBrain))]
    public class CameraBrainEditor : Editor
    {
        ReorderableList list;
        CameraBrain brain;

        SubCamera CreateCamera<T>(string cameraName) where T : SubCamera
        {
            GameObject camera = new GameObject(cameraName);
            camera.AddComponent<T>();
            camera.transform.position = brain.GetComponent<Camera>().transform.position;
            return camera.GetComponent<T>();
        }
        bool IsSubCameraInList(SubCamera subCamera, ReorderableList list)
        {
            for (int i = 0; i < list.serializedProperty.arraySize; i++)
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(i);
                if (element.objectReferenceValue == subCamera) return true;
            }
            return false;
        }
        bool IsSubCameraInHierarchy(SubCamera subCamera)
        {
            var go = subCamera != null ? subCamera.gameObject : null;
            return go != null && go.scene.IsValid() && go.scene.isLoaded;
        }
        void AddSubCameraToList(SubCamera subCamera, ReorderableList list)
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.objectReferenceValue = subCamera;
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }
        void UpdateListFromHierarchyChange(SubCamera[] existingSubCameras)
        {
            if (existingSubCameras.Length != list.serializedProperty.arraySize)
            {
                for (int i = 0; i < list.serializedProperty.arraySize; i++)
                {
                    var element = list.serializedProperty.GetArrayElementAtIndex(i);
                    var subCamera = element.objectReferenceValue as SubCamera;

                    if (subCamera == null || !IsSubCameraInHierarchy(subCamera))
                    {
                        list.serializedProperty.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        Debug.Log(list.serializedProperty.arraySize);
                    }
                    else { if (!IsSubCameraInList(subCamera, list)) AddSubCameraToList(subCamera, list); }
                }
            }
        }
        void HierarchyChanged() => UpdateListFromHierarchyChange(GameObject.FindObjectsOfType<SubCamera>());
        void OnEnable()
        {
            brain = target as CameraBrain;

            list =
             new ReorderableList(serializedObject, serializedObject.FindProperty(Constants.SubCameraListPropertyName), true, true, true, true);

            SubCamera[] existingSC = GameObject.FindObjectsOfType<SubCamera>();

            UpdateListFromHierarchyChange(existingSC);

            list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, Constants.SubCameraListHeader); };

            list.onAddDropdownCallback = (Rect buttonRect, ReorderableList list) => {

                var menu = new GenericMenu();
                menu.AddItem(new GUIContent(Constants.ThirdPersonCameraName), false, () =>
                {
                    var tpc = CreateCamera<ThirdPersonCamera>(Constants.ThirdPersonCameraName);
                    var index = list.serializedProperty.arraySize;
                    list.serializedProperty.arraySize++;
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    element.objectReferenceValue = tpc;
                    serializedObject.ApplyModifiedProperties();
                });
                menu.AddSeparator(Constants.Empty);
                menu.AddItem(new GUIContent(Constants.TrackCameraName), false, () =>
                {
                    var tc = CreateCamera<TrackCamera>(Constants.TrackCameraName);
                    var index = list.serializedProperty.arraySize;
                    list.serializedProperty.arraySize++;
                    var element = list.serializedProperty.GetArrayElementAtIndex(index);
                    element.objectReferenceValue = tc;
                    serializedObject.ApplyModifiedProperties();
                });
                menu.DropDown(buttonRect);
            };

            list.onRemoveCallback = (ReorderableList l) =>
            {
                var index = l.index;
                var element = l.serializedProperty.GetArrayElementAtIndex(index);
                var obj = element.objectReferenceValue as SubCamera;
                if (obj != null)
                {
                    DestroyImmediate(obj.gameObject);
                    element.objectReferenceValue = null;
                }
                l.serializedProperty.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = list.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element, GUIContent.none);
            };

            EditorApplication.hierarchyChanged += HierarchyChanged;
        }

        void OnDisable() => EditorApplication.hierarchyChanged -= HierarchyChanged;

        public override void OnInspectorGUI()
        {
            EditorGUILayout.Space(Constants.DefaultSpace);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Constants.Label_Debug, GUILayout.Width(Constants.Label_DebugWidth));
            brain.showDebug = EditorGUILayout.Toggle(brain.showDebug);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(Constants.DefaultSpace);

            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }

    public static class CCEditor
    {

        public static void RightToggle(string label, ref bool value,float width)
        {
            GUILayout.Label(label, GUILayout.Width(width));
            value = GUILayout.Toggle(value, Constants.Empty, GUILayout.Width(width));
        }

        public static void Slider<T>(string label, ref T value, T min, T max, float labelWidth = .0f, float fieldWidth = .0f) where T : struct
        {
            EditorGUILayout.BeginHorizontal();

            if(labelWidth == .0f) EditorGUILayout.LabelField(label);
            else EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

            if(typeof(T) == typeof(float))
            {
                float floatValue = System.Convert.ToSingle(value);
                float floatMin = System.Convert.ToSingle(min);
                float floatMax = System.Convert.ToSingle(max);

                if (fieldWidth == .0f) floatValue = EditorGUILayout.Slider(floatValue, floatMin, floatMax);
                else floatValue = EditorGUILayout.Slider(floatValue, floatMin, floatMax, GUILayout.Width(fieldWidth));

                value = (T)System.Convert.ChangeType(floatValue, typeof(T));
            }
            else if(typeof(T) == typeof(int))
            {
                int intValue = System.Convert.ToInt32(value);
                int intMin = System.Convert.ToInt32(min);
                int intMax = System.Convert.ToInt32(max);

                if (fieldWidth == .0f) intValue = EditorGUILayout.IntSlider(intValue, intMin, intMax);
                else intValue = EditorGUILayout.IntSlider(intValue, intMin, intMax, GUILayout.Width(fieldWidth));

                value = (T)System.Convert.ChangeType(intValue, typeof(T));
            }

            EditorGUILayout.EndHorizontal();
        }

        public static SerializedProperty Property(ReorderableList list,int index, string propertyName)
        => list.serializedProperty.GetArrayElementAtIndex(index).FindPropertyRelative(propertyName);

        public static void ObjectField<T>(string label, ref T value, float labelWidth = .0f, float fieldWidth = .0f)
            where T : Object
        {
            EditorGUILayout.BeginHorizontal();

            if (labelWidth == .0f) EditorGUILayout.LabelField(label);
            else EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

            if (fieldWidth == .0f) value = (T)EditorGUILayout.ObjectField(value, typeof(T), true);
            else value = (T)EditorGUILayout.ObjectField(value, typeof(T), true, GUILayout.Width(labelWidth));

            EditorGUILayout.EndHorizontal();
        }

        public static void Field<T>(string label, ref T value, float labelWidth = .0f,float fieldWidth = .0f)
            where T : struct
        {
            EditorGUILayout.BeginHorizontal();
            if(labelWidth == .0f) EditorGUILayout.LabelField(label);
            else EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));

            if (typeof(T) == typeof(int))
            {
                int intValue = System.Convert.ToInt32(value);

                if(fieldWidth == .0f) intValue = EditorGUILayout.IntField(intValue);
                else intValue = EditorGUILayout.IntField(intValue, GUILayout.Width(fieldWidth));

                value = (T)System.Convert.ChangeType(intValue, typeof(T));
            }
            else if (typeof(T) == typeof(float))
            {
                float floatValue = System.Convert.ToSingle(value);

                if (fieldWidth == .0f) floatValue = EditorGUILayout.FloatField(floatValue);
                else floatValue = EditorGUILayout.FloatField(floatValue, GUILayout.Width(fieldWidth));

                value = (T)System.Convert.ChangeType(floatValue, typeof(T));
            }
            else if(typeof(T) == typeof(bool)) 
            {
                bool boolValue = System.Convert.ToBoolean(value);

                if (fieldWidth == .0f) boolValue = EditorGUILayout.Toggle(boolValue);
                else boolValue = EditorGUILayout.Toggle(boolValue, GUILayout.Width(fieldWidth));

                value = (T)System.Convert.ChangeType(boolValue, typeof(T));
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public abstract class SubCameraEditor<T> : Editor where T : SubCamera
    {
        protected T cam;

        #region Options

        //protected float label = 65f;
        //protected float field = 50f;

        protected GUILayoutOption[] OP_Label;
        protected GUILayoutOption[] OP_Field;
        #endregion

        protected virtual void OnEnable()
        {
            cam = target as T;

            #region GenerateOption
            OP_Label = new GUILayoutOption[] { GUILayout.Width(Constants.LabelWidth) };
            OP_Field = new GUILayoutOption[] { GUILayout.Width(Constants.FieldWidth) };
            #endregion
        }

        public override void OnInspectorGUI() => InspectorGUI();
        protected void CommonInspectorGUI()
        {
            #region Depth

            CCEditor.Field<int>(Constants.Depth, ref cam.depth, Constants.DefaultLabelWidth);
            
            #endregion

            #region ChangeMode

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Constants.EnterMode, GUILayout.Width(Constants.DefaultLabelWidth));
            cam.enterMode = (ChangeMode)EditorGUILayout.EnumPopup(cam.enterMode);
            EditorGUILayout.EndHorizontal();

            #endregion

            #region ChangeSpeed
            if (cam.enterMode == ChangeMode.Smooth)
                CCEditor.Field<float>(Constants.EnterSpeed, ref cam.changeCamSpeed, Constants.DefaultLabelWidth);
            #endregion


            #region isAffectedByShake
            CCEditor.Field<bool>(Constants.AffectedByShake, ref cam.isAffectedByShake, Constants.DefaultLabelWidth);
            #endregion

            GUILayout.Space(Constants.DefaultSpace * 2.0f);
        }
        protected virtual void InspectorGUI()
        {
            EditorGUILayout.Space(Constants.DefaultSpace);

            cam.isShowOption = EditorGUILayout.BeginFoldoutHeaderGroup(cam.isShowOption, Constants.Option);

            EditorGUILayout.Space(Constants.DefaultSpace);

            if (cam.isShowOption)
            {
                EditorGUI.indentLevel++;

                CCEditor.ObjectField<NoiseData>(Constants.Noise, ref cam.noiseData, Constants.LabelWidth);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space(Constants.DefaultSpace);
        }
    }
}