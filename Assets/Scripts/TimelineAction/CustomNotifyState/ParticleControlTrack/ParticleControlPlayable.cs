using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kirara.TimelineAction
{
    public class ParticleControlPlayable : PlayableBehaviour
    {
        private GameObject go;
        private ParticleSystem particle;

        public static ScriptPlayable<ParticleControlPlayable> Create(
            PlayableGraph graph, GameObject owner,
            GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            var playable = ScriptPlayable<ParticleControlPlayable>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.Init(owner, prefab, position, rotation, scale);
            return playable;
        }

        private void Init(GameObject owner, GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale)
        {
#if UNITY_EDITOR
            if (prefab == null) return;

            go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, owner.transform);

            go.transform.localPosition = position;
            go.transform.localRotation = rotation;
            go.transform.localScale = scale;
            SetHideFlagsRecursive(go);
            particle = go.GetComponent<ParticleSystem>();
            particle.Stop();
#endif
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);

            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            if (particle == null) return;

            particle.Simulate((float)playable.GetTime());
        }

        private static void SetHideFlagsRecursive(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            gameObject.hideFlags = HideFlags.HideAndDontSave;
            foreach (Transform child in gameObject.transform)
            {
                SetHideFlagsRecursive(child.gameObject);
            }
        }
    }
}