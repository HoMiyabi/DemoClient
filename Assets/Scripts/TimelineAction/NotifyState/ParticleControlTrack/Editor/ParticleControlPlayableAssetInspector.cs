using System;
using UnityEditor;
using UnityEngine;

namespace Kirara.TimelineAction
{
    [CustomEditor(typeof(ParticleControlPlayableAsset))]
    public class ParticleControlPlayableAssetInspector : UnityEditor.Editor
    {
        private ParticleControlPlayableAsset _target;

        private enum EditMode
        {
            None = -1,
            Move = 0,
            Rotate = 1,
            Scale = 2,
        }

        private EditMode editMode = EditMode.None;

        private readonly string[] texts = {"移动", "旋转", "缩放" };

        private void OnEnable()
        {
            _target = (ParticleControlPlayableAsset)target;
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            int newSelected = GUILayout.Toolbar((int)editMode, texts);
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

        private void DuringSceneGUI(SceneView sceneView)
        {
            if (_target == null) return;
            if (_target.owner == null) return;
            var parent = _target.owner.transform;

            if (editMode == EditMode.Move)
            {
                var worldPos = parent.TransformPoint(_target.position);
                var newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                var localPos = parent.InverseTransformPoint(newPos);
                if (localPos != _target.position)
                {
                    Undo.RecordObject(_target, "修改位置");
                    _target.position = localPos;
                }
            }
            else if (editMode == EditMode.Rotate)
            {
                var worldPos = parent.TransformPoint(_target.position);
                var angle = Handles.RotationHandle(Quaternion.Euler(_target.rotation), worldPos).eulerAngles;
                if (angle != _target.rotation)
                {
                    Undo.RecordObject(_target, "修改旋转");
                    _target.rotation = angle;
                }
            }
            else if (editMode == EditMode.Scale)
            {
                var worldPos = parent.TransformPoint(_target.position);
                var newScale = Handles.ScaleHandle(_target.scale, worldPos, Quaternion.identity);
                if (newScale != _target.scale)
                {
                    Undo.RecordObject(_target, "修改缩放");
                    _target.scale = newScale;
                }
            }
        }
    }
}