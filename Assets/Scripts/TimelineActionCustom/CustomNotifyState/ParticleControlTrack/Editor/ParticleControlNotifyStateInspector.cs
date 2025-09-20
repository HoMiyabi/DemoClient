using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(ParticleControlNotifyState))]
    public class ParticleControlNotifyStateInspector : UnityEditor.Editor
    {
        private ParticleControlNotifyState _target;

        private SerializedProperty prefabProp;
        private SerializedProperty positionProp;
        private SerializedProperty rotationProp;
        private SerializedProperty scaleProp;

        private enum EEditMode
        {
            None = -1,
            Position = 0,
            Rotation = 1,
            Scale = 2,
        }

        private EEditMode editMode = EEditMode.None;

        private readonly string[] editModeNames = {"位置", "旋转", "缩放"};

        private readonly List<ParticleControlNotifyStateInspector> instances = new();

        private void OnEnable()
        {
            if (target == null) return;

            _target = (ParticleControlNotifyState)target;
            prefabProp = serializedObject.FindProperty(nameof(_target.prefab));
            positionProp = serializedObject.FindProperty(nameof(_target.position));
            rotationProp = serializedObject.FindProperty(nameof(_target.rotation));
            scaleProp = serializedObject.FindProperty(nameof(_target.scale));

            if (!instances.Contains(this))
            {
                instances.Add(this);
                SceneView.duringSceneGui -= DuringSceneGUI;
                SceneView.duringSceneGui += DuringSceneGUI;
            }
        }

        private void OnDisable()
        {
            instances.Remove(this);
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        private void OnDestroy()
        {
            instances.Remove(this);
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(prefabProp);
            EditorGUILayout.PropertyField(positionProp, new GUIContent("位置"));
            EditorGUILayout.PropertyField(rotationProp, new GUIContent("旋转"));
            EditorGUILayout.PropertyField(scaleProp, new GUIContent("缩放"));

            EditorGUI.BeginChangeCheck();
            var clickMode = (EEditMode)GUILayout.Toolbar((int)editMode, editModeNames);
            if (EditorGUI.EndChangeCheck())
            {
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
            serializedObject.ApplyModifiedProperties();
        }

        private void DuringSceneGUI(SceneView sceneView)
        {
            if (_target.owner == null) return;

            serializedObject.Update();
            var parent = _target.owner.transform;
            if (editMode == EEditMode.Position)
            {
                var worldPos = parent.TransformPoint(positionProp.vector3Value);
                var newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                var localPos = parent.InverseTransformPoint(newPos);
                if (localPos != positionProp.vector3Value)
                {
                    positionProp.vector3Value = localPos;
                }
            }
            else if (editMode == EEditMode.Rotation)
            {
                var worldPos = parent.TransformPoint(positionProp.vector3Value);
                var rot = Handles.RotationHandle(rotationProp.quaternionValue, worldPos);
                if (rot != rotationProp.quaternionValue)
                {
                    rotationProp.quaternionValue = rot;
                }
            }
            else if (editMode == EEditMode.Scale)
            {
                var worldPos = parent.TransformPoint(positionProp.vector3Value);
                var newScale = Handles.ScaleHandle(scaleProp.vector3Value, worldPos, Quaternion.identity);
                if (newScale != scaleProp.vector3Value)
                {
                    scaleProp.vector3Value = newScale;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}