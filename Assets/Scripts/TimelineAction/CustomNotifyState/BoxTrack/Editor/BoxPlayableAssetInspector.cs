using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(BoxPlayableAsset))]
    public class BoxPlayableAssetInspector : UnityEditor.Editor
    {
        private BoxPlayableAsset _target;

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

        private enum EditMode
        {
            None = -1,
            Center = 0,
            Size = 1,
        }

        private EditMode editMode = EditMode.None;
        private readonly string[] editNames = {"中心", "尺寸"};

        private static readonly List<BoxPlayableAssetInspector> instances = new();
        private bool bPreview = true;

        private void OnEnable()
        {
            _target = target as BoxPlayableAsset;

            boxTypeProp = serializedObject.FindProperty(nameof(_target.boxType));

            boxShapeProp = serializedObject.FindProperty(nameof(_target.boxShape));
            centerProp = serializedObject.FindProperty(nameof(_target.center));
            radiusProp = serializedObject.FindProperty(nameof(_target.radius));
            sizeProp = serializedObject.FindProperty(nameof(_target.size));
            attackStrengthProp = serializedObject.FindProperty(nameof(_target.hitStrength));
            hitIdProp = serializedObject.FindProperty(nameof(_target.hitId));
            particlePrefabProp = serializedObject.FindProperty(nameof(_target.particlePrefab));
            setRotProp = serializedObject.FindProperty(nameof(_target.setRot));
            rotValueProp = serializedObject.FindProperty(nameof(_target.rotValue));
            rotMaxValueProp = serializedObject.FindProperty(nameof(_target.rotMaxValue));
            hitGatherDistProp = serializedObject.FindProperty(nameof(_target.hitGatherDist));

            SceneView.duringSceneGui -= DuringSceneGUI;
            SceneView.duringSceneGui += DuringSceneGUI;
            if (!instances.Contains(this))
            {
                instances.Add(this);
            }
        }

        public override void OnInspectorGUI()
        {
            DrawProp();
            DrawToolbar();
            DrawControlPreview();
        }

        private void DrawProp()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(boxTypeProp, new GUIContent("类型"));
            EditorGUILayout.PropertyField(boxShapeProp, new GUIContent("形状"));
            EditorGUILayout.PropertyField(centerProp, new GUIContent("中心"));
            switch (_target.boxShape)
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
                    Debug.LogWarning("未处理的形状 " + _target.boxShape);
                    break;
            }
            if (_target.boxType == EBoxType.HitBox)
            {
                EditorGUILayout.PropertyField(attackStrengthProp, new GUIContent("攻击强度"));
                EditorGUILayout.PropertyField(hitIdProp, new GUIContent("Hit Id"));
                EditorGUILayout.PropertyField(particlePrefabProp, new GUIContent("命中粒子 Prefab"));
                EditorGUILayout.PropertyField(setRotProp, new GUIContent("设置旋转"));
                if (setRotProp.boolValue)
                {
                    EditorGUILayout.PropertyField(rotValueProp, new GUIContent("旋转最小"));
                    EditorGUILayout.PropertyField(rotMaxValueProp, new GUIContent("旋转最大"));
                }
                EditorGUILayout.PropertyField(hitGatherDistProp, new GUIContent("命中聚集距离"));
            }
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawToolbar()
        {
            int newSelected = GUILayout.Toolbar((int)editMode, editNames);
            if (GUI.changed)
            {
                if ((int)editMode == newSelected)
                {
                    editMode = EditMode.None;
                }
                else
                {
                    editMode = (EditMode)newSelected;
                }
                SceneView.RepaintAll();
            }
        }


        private void OnDestroy()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            instances.Remove(this);
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
            instances.Remove(this);
        }

        private void DuringSceneGUI(SceneView sceneView)
        {
            if (_target.owner == null) return;

            DrawBoxInScene();
            DrawHandleInScene();
        }

        private void DrawBoxInScene()
        {
            if (!bPreview) return;

            var parent = _target.owner.transform;
            var center = parent.TransformPoint(_target.center);

            Handles.color = Color.white;
            if (_target.boxType == EBoxType.HitBox)
            {
                Handles.color = Color.red;
            }
            switch (_target.boxShape)
            {
                case EBoxShape.Sphere:
                {
                    Handles.DrawWireDisc(center, Vector3.up, _target.radius);
                    Handles.DrawWireDisc(center, Vector3.right, _target.radius);
                    Handles.DrawWireDisc(center, Vector3.forward, _target.radius);
                    break;
                }
                case EBoxShape.Box:
                {
                    Handles.DrawWireCube(center, _target.size);
                    break;
                }
                default:
                    Debug.LogWarning("未处理的形状 " + _target.boxShape);
                    break;
            }
        }

        private void DrawHandleInScene()
        {
            var parent = _target.owner.transform;

            if (editMode == EditMode.Center)
            {
                var worldPos = parent.TransformPoint(_target.center);
                var newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                var newLocalPos = parent.InverseTransformPoint(newWorldPos);
                if (newLocalPos != _target.center)
                {
                    Undo.RecordObject(_target, "修改中心");
                    _target.center = newLocalPos;
                }
            }
            else if (editMode == EditMode.Size)
            {
                var worldPos = parent.TransformPoint(_target.center);

                if (_target.boxShape == EBoxShape.Sphere)
                {
                    var newScale = Handles.ScaleHandle(new Vector3(_target.radius, 0f, 0f), worldPos, Quaternion.identity);
                    if (newScale.x != _target.radius)
                    {
                        Undo.RecordObject(_target, "修改尺寸");
                        _target.radius = newScale.x;
                    }
                }
                else if (_target.boxShape == EBoxShape.Box)
                {
                    var newScale = Handles.ScaleHandle(_target.size, worldPos, Quaternion.identity);
                    if (newScale != _target.size)
                    {
                        Undo.RecordObject(_target, "修改尺寸");
                        _target.size = newScale;
                    }
                }
            }
        }


        private void DrawControlPreview()
        {
            if (GUILayout.Button("只显示当前预览"))
            {
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