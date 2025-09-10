using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(ParticleControlPlayableAsset))]
    public class ParticleControlPlayableAssetInspector : UnityEditor.Editor
    {
        private ParticleControlPlayableAsset _target;

        private SerializedProperty positionProp;
        private SerializedProperty rotationProp;
        private SerializedProperty scaleProp;

        private enum EEditMode
        {
            None = -1,
            Move = 0,
            Rotate = 1,
            Scale = 2,
        }

        private EEditMode editMode = EEditMode.None;

        private readonly string[] editModeNames = {"位置", "旋转", "缩放"};

        private List<ParticleControlPlayableAssetInspector> instances = new();

        private void OnEnable()
        {
            if (target == null) return;

            _target = (ParticleControlPlayableAsset)target;
            positionProp = serializedObject.FindProperty(nameof(_target.position));
            rotationProp = serializedObject.FindProperty(nameof(_target.rotation));
            scaleProp = serializedObject.FindProperty(nameof(_target.scale));

            if (!instances.Contains(this))
            {
                instances.Add(this);
                SceneView.duringSceneGui += DuringSceneGUI;
            }
        }

        private void OnDisable()
        {
            if (instances.Remove(this))
            {
                SceneView.duringSceneGui -= DuringSceneGUI;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
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
        }

        private void DuringSceneGUI(SceneView sceneView)
        {
            if (_target.owner == null) return;

            serializedObject.Update();
            var parent = _target.owner.transform;

            if (editMode == EEditMode.Move)
            {
                var worldPos = parent.TransformPoint(positionProp.vector3Value);
                var newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                var localPos = parent.InverseTransformPoint(newPos);
                if (localPos != positionProp.vector3Value)
                {
                    positionProp.vector3Value = localPos;
                }
            }
            else if (editMode == EEditMode.Rotate)
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