namespace CustomCamera
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ImpulsePoint))]
    public class ImpulsePointEditor : Editor
    {
        ImpulsePoint point;

        public void OnEnable() => point = target as ImpulsePoint;
        void RenderingRange(Color col, Vector3 centerPoint, float radius)
        {
            Handles.color = Color.white;
            Handles.DrawWireDisc(centerPoint, Vector3.up, radius);

            Color fillColor = col;

            for (int i = 1; i <= point.SectionCount; i++)
            {
                fillColor.a = Constants.I_DefaultStartAlpha * Mathf.Pow(Constants.I_Half, (i - 1));
                Handles.color = fillColor;
                Handles.DrawSolidDisc(centerPoint, Vector3.up,
                    point.GetCurRadius(point.GetRatio, i));
            }
        }
        public void OnSceneGUI() => RenderingRange(Color.red, point.transform.position, point.radius);
        public override void OnInspectorGUI() => base.OnInspectorGUI();
    }

    [CustomEditor(typeof(ImpulseInteractionObject))]
    public class ImpulseInteractionObjectEditor : Editor
    {
        ImpulseInteractionObject obj;
        PhysicMaterial[] materials;

        #region Option
        float label = Constants.I_Label;
        float toggle = Constants.I_Toggle;
        #endregion

        GUILayoutOption[] Width(float width) => new GUILayoutOption[] { GUILayout.Width(width) };

        public void OnEnable()
        {
            obj = target as ImpulseInteractionObject;

            if (materials == null)
            {
                materials = new PhysicMaterial[2];
                Object[] resources = Resources.LoadAll(Constants.PhysicMaterialPath);
                int i = 0;
                foreach (Object resource in resources) materials[i++] = (PhysicMaterial)resource;
            }

            if (obj.rigid == null) obj.rigid = obj.gameObject.GetComponent<Rigidbody>();
            if (obj.rigid == null) obj.rigid = obj.gameObject.AddComponent<Rigidbody>();

            if (obj.col == null) obj.col = obj.gameObject.GetComponent<Collider>();

            if (obj.col != null)
            {
                if (obj.type == RigidyType.Metallic) { obj.col.material = materials[(int)RigidyType.Metallic]; }
                else { obj.col.material = materials[(int)RigidyType.NonMetallic]; }
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(Constants.I_AffectLabel, Width(label));
            obj.isAffectedNoise = GUILayout.Toggle(obj.isAffectedNoise, Constants.Empty, Width(toggle));
            EditorGUILayout.EndHorizontal();
            obj.rigid = 
                (Rigidbody)EditorGUILayout.ObjectField(Constants.I_RigidbodyLabel, obj.rigid, typeof(Rigidbody), true);
            obj.col = (Collider)EditorGUILayout.ObjectField(Constants.I_ColliderLabel, obj.col, typeof(Collider), true);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(Constants.I_RigidbodyTypeLabel, Width(label));
            obj.type = (RigidyType)EditorGUILayout.EnumPopup(obj.type);
            if (EditorGUI.EndChangeCheck())
            { if (obj.col != null) obj.col.material = materials[(int)obj.type]; }
            EditorGUILayout.EndHorizontal();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.Space(Constants.DefaultSpace);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(Constants.I_PositionLockLabel, Width(label));

            CCEditor.RightToggle(Constants.I_XLabel, ref obj.posX, toggle);
            CCEditor.RightToggle(Constants.I_YLabel, ref obj.posY, toggle);
            CCEditor.RightToggle(Constants.I_ZLabel, ref obj.posZ, toggle);

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(Constants.I_RotationLockLabel, Width(label));


            CCEditor.RightToggle(Constants.I_XLabel, ref obj.rotX, toggle);
            CCEditor.RightToggle(Constants.I_YLabel, ref obj.rotY, toggle);
            CCEditor.RightToggle(Constants.I_ZLabel, ref obj.rotZ, toggle);

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                if (obj.posX) obj.rigid.constraints = obj.rigid.constraints | RigidbodyConstraints.FreezePositionX;
                else obj.rigid.constraints = obj.rigid.constraints & ~RigidbodyConstraints.FreezePositionX;
                if (obj.posY) obj.rigid.constraints = obj.rigid.constraints | RigidbodyConstraints.FreezePositionY;
                else obj.rigid.constraints = obj.rigid.constraints & ~RigidbodyConstraints.FreezePositionY;
                if (obj.posZ) obj.rigid.constraints = obj.rigid.constraints | RigidbodyConstraints.FreezePositionZ;
                else obj.rigid.constraints = obj.rigid.constraints & ~RigidbodyConstraints.FreezePositionZ;

                if (obj.rotX) obj.rigid.constraints = obj.rigid.constraints | RigidbodyConstraints.FreezeRotationX;
                else obj.rigid.constraints = obj.rigid.constraints & ~RigidbodyConstraints.FreezeRotationX;
                if (obj.rotY) obj.rigid.constraints = obj.rigid.constraints | RigidbodyConstraints.FreezeRotationY;
                else obj.rigid.constraints = obj.rigid.constraints & ~RigidbodyConstraints.FreezeRotationY;
                if (obj.rotZ) obj.rigid.constraints = obj.rigid.constraints | RigidbodyConstraints.FreezeRotationZ;
                else obj.rigid.constraints = obj.rigid.constraints & ~RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}