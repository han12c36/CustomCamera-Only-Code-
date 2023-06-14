namespace CustomCamera
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(InteractionData))]
    public class InteractionDataEditor<T> : Editor where T : InteractionData
    {
        protected T data;

        #region Option
        protected float label = Constants.DataLabel;
        protected float toggle = Constants.DataToggle;
        #endregion

        protected GUILayoutOption[] Width(float width) => new GUILayoutOption[] { GUILayout.Width(width) };

        public void OnEnable() => data = target as T;

        public virtual void InspectorGUI(string dataName)
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space(Constants.DefaultSpace);

            data.isShowData = EditorGUILayout.BeginFoldoutHeaderGroup(data.isShowData, dataName);
            if (data.isShowData)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.Space(Constants.DefaultSpace);

                CCEditor.Field<bool>(Constants.DataPlayOnAwake, ref data.playOnAwake, label - toggle);

                EditorGUILayout.Space(Constants.DefaultSpace);
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(Constants.I_PositionLockLabel, Width(label));

                CCEditor.RightToggle(Constants.I_XLabel, ref data.x, toggle);
                CCEditor.RightToggle(Constants.I_YLabel, ref data.y, toggle);
                CCEditor.RightToggle(Constants.I_ZLabel, ref data.z, toggle);

                EditorGUILayout.EndHorizontal();

                CCEditor.Slider<float>(Constants.DataIntensity, ref data.intensity, 
                    Constants.DataIntensityMin, Constants.DataIntensityMax, label - toggle);
                CCEditor.Slider<float>(Constants.DataFrequency, ref data.frequency, 
                    Constants.DataFrequencyMin, Constants.DataFrequencyMax, label - toggle);

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }

    [CustomEditor(typeof(NoiseData))]
    public class NoiseDataEditor : InteractionDataEditor<NoiseData>
    {
        public override void OnInspectorGUI()
        {
            InspectorGUI("NoiseData");

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Modified NoiseData");
                EditorUtility.SetDirty(data);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
    [CustomEditor(typeof(ShakeData))]
    public class ShakeDataEditor : InteractionDataEditor<ShakeData>
    {
        public override void OnInspectorGUI()
        {
            InspectorGUI("ShakeData");

            EditorGUI.indentLevel++;

            CCEditor.Field<float>("Duration", ref data.duration, label - toggle);

            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(data, "Modified ShakeData");
                EditorUtility.SetDirty(data);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}