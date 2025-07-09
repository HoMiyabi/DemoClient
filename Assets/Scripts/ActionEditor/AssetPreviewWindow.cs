using UnityEngine;
using UnityEditor;
public class AssetPreviewWindow : EditorWindow
{
    private Object unityAsset;
    private Editor assetEditor;
    private bool isAnimationClip;
    [MenuItem("Kirara/AssetPreview")]
    public static void OpenWindow()
    {
        GetWindow<AssetPreviewWindow>().Show();
    }
    private void OnGUI()
    {
        CheckAsset();
        ShowPreview();
    }
    private void CheckAsset()
    {
        EditorGUI.BeginChangeCheck(); //开始检查编辑器资源是否发生变动
        unityAsset = EditorGUILayout.ObjectField(unityAsset, typeof(Object), true); //显示Object
        if (EditorGUI.EndChangeCheck()) //如果发生变动（比如拖拽新的资源）
        {
            isAnimationClip = false;
            var go = unityAsset as GameObject; //如果是物体
            if (go != null)
            {
                assetEditor = Editor.CreateEditor(go); //创建新的Editor
                assetEditor.OnInspectorGUI(); //Editor初始化
                return;
            }
            var clip = unityAsset as AnimationClip; //如果是动画片
            if (clip != null)
            {
                assetEditor = Editor.CreateEditor(clip);
                assetEditor.OnInspectorGUI();
                isAnimationClip = true;
                Debug.Log(assetEditor.GetType());
                return;
            }
            var mesh = unityAsset as Mesh; //如果是网格
            if (mesh != null)
            {
                assetEditor = Editor.CreateEditor(mesh);
                assetEditor.OnInspectorGUI();
                return;
            }
            var tex = unityAsset as Texture; //如果是纹理资源
            if (tex != null)
            {
                assetEditor = Editor.CreateEditor(tex);
                assetEditor.OnInspectorGUI();
                return;
            }
            var mat = unityAsset as Material;
            if (mat != null)
            {
                assetEditor = Editor.CreateEditor(mat);
                assetEditor.OnInspectorGUI();
                return;
            }
        }
    }
    private void ShowPreview()
    {
        if (assetEditor != null) //如果资源Editor非空
        {
            using (new EditorGUILayout.HorizontalScope()) //水平布局
            {
                GUILayout.FlexibleSpace();//填充间隔
                assetEditor.OnPreviewSettings(); //显示预览设置项
            }
            if (isAnimationClip)
            {
                AnimationMode.StartAnimationMode(); //为了播放正常播放预览动画而进行的设置
                AnimationMode.BeginSampling();
                assetEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(512, 512), EditorStyles.whiteLabel);
                AnimationMode.EndSampling();
                AnimationMode.StopAnimationMode();
            }
            else
            {
                assetEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(512, 512), EditorStyles.whiteLabel);
            }
        }
    }
}