using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kirara.UI
{
    public class UIDialogueText : MonoBehaviour
    {
        #region View
        private TextMeshProUGUI       ContentText;
        private HorizontalLayoutGroup Layout;
        private void InitUI()
        {
            var c       = GetComponent<KiraraDirectBinder.KiraraDirectBinder>();
            ContentText = c.Q<TextMeshProUGUI>(0, "ContentText");
            Layout      = c.Q<HorizontalLayoutGroup>(1, "Layout");
        }
        #endregion

        public bool IsPlaying { get; private set; }

        private float typeInterval = 0.03f;
        private WaitForSeconds typeIntervalWait;

        private float chFadeDuration = 0.04f;

        private readonly List<Vector3> chVertices = new();

        private void Awake()
        {
            InitUI();

            typeIntervalWait = new WaitForSeconds(typeInterval);
        }

        public void PlayType(string text)
        {
            StopAllCoroutines();
            StartCoroutine(PlayType_Internal(text));
        }

        public void ForceFinish()
        {
            if (!IsPlaying) return;
            IsPlaying = false;
            StopAllCoroutines();
            ContentText.ForceMeshUpdate();
        }

        private IEnumerator FadeInChar(TMP_MeshInfo[] meshInfos, TMP_CharacterInfo charInfo, int chVerticesStart)
        {
            // 字符所在网格和顶点数组
            var meshInfo = meshInfos[charInfo.materialReferenceIndex];
            var vertices = meshInfo.vertices;

            // 取出中心，前面将四个顶点都设置为中心点
            var center = vertices[charInfo.vertexIndex];

            // 逐帧放大
            float t = 0;
            while (t < chFadeDuration)
            {
                t += Time.deltaTime;
                for (int i = 0; i < 4; i++)
                {
                    vertices[charInfo.vertexIndex + i] = Vector3.Lerp(
                        center, chVertices[chVerticesStart + i],t / chFadeDuration);
                }
                meshInfo.mesh.vertices = vertices;
                ContentText.UpdateGeometry(meshInfo.mesh, charInfo.materialReferenceIndex);
                yield return null;
            }
        }

        private static Vector3 GetCenter(IList<Vector3> vertices, int start, int n)
        {
            var ret = Vector3.zero;
            for (int i = start; i < start + n; i++)
            {
                ret += vertices[i];
            }
            ret /= n;
            return ret;
        }

        private IEnumerator PlayType_Internal(string text)
        {
            IsPlaying = true;

            ContentText.text = text;
            if (string.IsNullOrEmpty(text) || typeInterval <= 0)
            {
                IsPlaying = false;
                yield break;
            }
            if (chFadeDuration <= 0)
            {
                Debug.LogWarning("chFadeDuration不能 <= 0");
                IsPlaying = false;
                yield break;
            }
            ContentText.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();

            var textInfo = ContentText.textInfo;
            // Debug.Log(new string(textInfo.characterInfo
            //     .Where(it => it.isVisible)
            //     .Select(it => it.character).ToArray()));

            chVertices.Clear();
            // 将每个字符的顶点保存后都设置为字符中心点隐藏，后续向外移动放大效果
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                // 得到字符
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                var vertices = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
                var center = GetCenter(vertices, charInfo.vertexIndex, 4);
                for (int j = 0; j < 4; j++)
                {
                    chVertices.Add(vertices[charInfo.vertexIndex + j]);
                    vertices[charInfo.vertexIndex + j] = center;
                }
            }

            // 更新所有网格的顶点信息
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                var meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                ContentText.UpdateGeometry(meshInfo.mesh, i);
            }

            // 打字效果
            int chVerticesStart = 0;
            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                StartCoroutine(FadeInChar(textInfo.meshInfo, charInfo, chVerticesStart));

                chVerticesStart += 4;
                yield return typeIntervalWait;
            }

            IsPlaying = false;
        }
    }
}