using UnityEngine;
using UnityEngine.Playables;

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

            go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, owner.transform);

            go.transform.localPosition = position;
            go.transform.localRotation = rotation;
            go.transform.localScale = scale;
            SetHideFlagsAll(go, HideFlags.DontSave);
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
                go = null;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
            if (particle == null) return;

            particle.Simulate((float)playable.GetTime());
        }

        // public override void OnBehaviourPlay(Playable playable, FrameData info)
        // {
        //     base.OnBehaviourPlay(playable, info);
        //     if (particle == null) return;
        //     if (!playable.GetGraph().IsPlaying()) return;
        //
        //     particle.Play();
        // }

        private static void SetHideFlagsAll(GameObject gameObject, HideFlags hideFlags)
        {
            if (gameObject == null)
                return;

            gameObject.hideFlags = hideFlags;
            foreach (Transform child in gameObject.transform)
            {
                SetHideFlagsAll(child.gameObject, hideFlags);
            }
        }
    }
}