using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(BoxNotifyState))]
    public class BoxNotifyStateInspector : UnityEditor.Editor
    {
        private BoxNotifyState _target;

        private SerializedProperty boxTypeProp;
        private SerializedProperty boxShapeProp;
        private SerializedProperty centerProp;
        private SerializedProperty radiusProp;
        private SerializedProperty sizeProp;
        private SerializedProperty attackStrengthProp;
        private SerializedProperty hitIdProp;
        private SerializedProperty particlePrefabProp;
        private SerializedProperty setRotProp;
        private SerializedProperty rotValueProp;
        private SerializedProperty rotMaxValueProp;
        private SerializedProperty hitGatherDistProp;
        private SerializedProperty hitAudioProp;
        private SerializedProperty hitstopDurationProp;
        private SerializedProperty hitstopSpeedProp;

        private enum EEditMode
        {
            None = -1,
            Center = 0,
            Size = 1,
        }

        private EEditMode editMode = EEditMode.None;

        private readonly string[] editModeNames = {"中心", "尺寸"};

        private static readonly List<BoxNotifyStateInspector> instances = new();
        private bool bPreview = true;

        private void OnEnable()
        {
            _target = target as BoxNotifyState;

            boxTypeProp = serializedObject.FindProperty(nameof(_target.boxType));

            boxShapeProp = serializedObject.FindProperty(nameof(_target.boxShape));
            centerProp = serializedObject.FindProperty(nameof(_target.center));
            radiusProp = serializedObject.FindProperty(nameof(_target.radius));
            sizeProp = serializedObject.FindProperty(nameof(_target.size));
            attackStrengthProp = serializedObject.FindProperty(nameof(_target.hitStrength));
            hitIdProp = serializedObject.FindProperty(nameof(_target.hitId));
            particlePrefabProp = serializedObject.FindProperty(nameof(_target.particlePrefab));
            setRotProp = serializedObject.FindProperty(nameof(_target.setParticleRot));
            rotValueProp = serializedObject.FindProperty(nameof(_target.rotValue));
            rotMaxValueProp = serializedObject.FindProperty(nameof(_target.rotMaxValue));
            hitGatherDistProp = serializedObject.FindProperty(nameof(_target.hitGatherDist));
            hitAudioProp = serializedObject.FindProperty(nameof(_target.hitAudio));
            hitstopDurationProp = serializedObject.FindProperty(nameof(_target.hitstopDuration));
            hitstopSpeedProp = serializedObject.FindProperty(nameof(_target.hitstopSpeed));

            if (!instances.Contains(this))
            {
                instances.Add(this);
                SceneView.duringSceneGui += DuringSceneGUI;
                // Debug.Log($"add instance {GetInstanceID()}, instances.Count = {instances.Count}");
            }
        }

        private void OnDisable()
        {
            instances.Remove(this);
            // Debug.Log($"OnDisable remove instance {GetInstanceID()}, instances.Count = {instances.Count}");
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        private void OnDestroy()
        {
            instances.Remove(this);
            // Debug.Log($"OnDisable remove instance {GetInstanceID()}, instances.Count = {instances.Count}");
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawProperty();
            DrawToolbar();
            DrawPreviewController();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperty()
        {
            EditorGUILayout.PropertyField(boxTypeProp, new GUIContent("类型"));
            EditorGUILayout.PropertyField(boxShapeProp, new GUIContent("形状"));
            EditorGUILayout.PropertyField(centerProp, new GUIContent("中心"));
            switch ((EBoxShape)boxShapeProp.enumValueIndex)
            {
                case EBoxShape.Sphere:
                {
                    EditorGUILayout.PropertyField(radiusProp, new GUIContent("半径"));
                    break;
                }
                case EBoxShape.Box:
                {
                    EditorGUILayout.PropertyField(sizeProp, new GUIContent("大小"));
                    break;
                }
                default:
                    Debug.LogWarning("未处理的形状 " + (EBoxShape)boxShapeProp.enumValueIndex);
                    break;
            }
            if ((EBoxType)boxTypeProp.enumValueIndex == EBoxType.HitBox)
            {
                EditorGUILayout.PropertyField(attackStrengthProp, new GUIContent("攻击强度"));
                EditorGUILayout.PropertyField(hitIdProp, new GUIContent("Hit Id"));
                EditorGUILayout.PropertyField(particlePrefabProp, new GUIContent("命中粒子 Prefab"));
                EditorGUILayout.PropertyField(setRotProp, new GUIContent("设置粒子旋转"));
                if (setRotProp.boolValue)
                {
                    EditorGUI.indentLevel += 1;
                    EditorGUILayout.PropertyField(rotValueProp, new GUIContent("旋转最小"));
                    EditorGUILayout.PropertyField(rotMaxValueProp, new GUIContent("旋转最大"));
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.PropertyField(hitGatherDistProp, new GUIContent("命中聚集距离"));
                EditorGUILayout.PropertyField(hitAudioProp, new GUIContent("命中音频"));
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(hitstopDurationProp, new GUIContent("命中停顿时长"));
                EditorGUILayout.PropertyField(hitstopSpeedProp, new GUIContent("命中停顿速度"));
            }
        }

        private void DrawToolbar()
        {
            EditorGUI.BeginChangeCheck();
            var clickMode = (EEditMode)GUILayout.Toolbar((int)editMode, editModeNames);
            if (EditorGUI.EndChangeCheck())
            {
                // Debug.Log($"set editMode clickMode: {clickMode}, editMode: {EditMode}");
                if (editMode == clickMode)
                {
                    editMode = EEditMode.None;
                }
                else
                {
                    editMode = clickMode;
                }
                SceneView.RepaintAll();
            }
        }

        private void DuringSceneGUI(SceneView sceneView)
        {
            // Debug.Log($"DuringSceneGUI editMode: {editMode}, _target.owner: {_target.owner}");
            if (_target.owner == null)
            {
                // Debug.LogWarning("BoxPlayableAssetInspector: owner is null");
                return;
            }

            serializedObject.Update();
            DrawHandleInScene();
            DrawBoxInScene();
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawBoxInScene()
        {
            if (!bPreview) return;

            var parent = _target.owner.transform;
            var center = parent.TransformPoint(centerProp.vector3Value);

            using (var s = new Handles.DrawingScope())
            {
                Handles.color = Color.white;
                if ((EBoxType)boxShapeProp.enumValueIndex == EBoxType.HitBox)
                {
                    Handles.color = Color.red;
                }
                switch ((EBoxShape)boxShapeProp.enumValueIndex)
                {
                    case EBoxShape.Sphere:
                    {
                        // Debug.Log($"Draw center: {center}, radius: {radiusProp.floatValue}");
                        Handles.DrawWireDisc(center, Vector3.up, radiusProp.floatValue);
                        Handles.DrawWireDisc(center, Vector3.right, radiusProp.floatValue);
                        Handles.DrawWireDisc(center, Vector3.forward, radiusProp.floatValue);
                        break;
                    }
                    case EBoxShape.Box:
                    {
                        Handles.DrawWireCube(center, sizeProp.vector3Value);
                        break;
                    }
                    default:
                        Debug.LogWarning("未处理的形状 " + (EBoxShape)boxShapeProp.enumValueIndex);
                        break;
                }
            }
        }

        private void DrawHandleInScene()
        {
            var parent = _target.owner.transform;

            // Debug.Log("editMode = " + EditMode);
            if (editMode == EEditMode.Center)
            {
                // Handles.PositionHandle(centerProp.vector3Value + Vector3.up * 2, Quaternion.identity);
                var worldPos = parent.TransformPoint(centerProp.vector3Value);
                var newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                var newLocalPos = parent.InverseTransformPoint(newWorldPos);
                if (newLocalPos != centerProp.vector3Value)
                {
                    centerProp.vector3Value = newLocalPos;
                }
            }
            else if (editMode == EEditMode.Size)
            {
                var worldPos = parent.TransformPoint(centerProp.vector3Value);

                if ((EBoxShape)boxShapeProp.enumValueIndex == EBoxShape.Sphere)
                {
                    float newRadius = Handles.RadiusHandle(Quaternion.identity, worldPos, radiusProp.floatValue, true);
                    if (newRadius != radiusProp.floatValue)
                    {
                        radiusProp.floatValue = newRadius;
                    }
                }
                else if ((EBoxShape)boxShapeProp.enumValueIndex == EBoxShape.Box)
                {
                    var newScale = Handles.ScaleHandle(sizeProp.vector3Value, worldPos, Quaternion.identity);
                    if (newScale != sizeProp.vector3Value)
                    {
                        sizeProp.vector3Value = newScale;
                    }
                }
            }
        }


        private void DrawPreviewController()
        {
            if (GUILayout.Button("只显示当前预览"))
            {
                // SceneView.duringSceneGui -= DuringSceneGUI;
                // SceneView.duringSceneGui += DuringSceneGUI;
                bPreview = true;
                foreach (var item in instances)
                {
                    if (item != this)
                    {
                        item.bPreview = false;
                    }
                }
                SceneView.RepaintAll();
            }
        }
    }
}