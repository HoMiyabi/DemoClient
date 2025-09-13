using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Kirara.TimelineAction
{
    public class RandomAudioPlayable : PlayableBehaviour
    {
        public float dontPlayProbability;
        public AudioClip[] audioClips;
        private AudioSource audioSource;

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);

            #if UNITY_EDITOR
            audioSource = EditorUtility.CreateGameObjectWithHideFlags("AudioSource",
                    HideFlags.HideAndDontSave, typeof(AudioSource))
                .GetComponent<AudioSource>();
            #endif
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);
            Object.DestroyImmediate(audioSource.gameObject);
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            if (!playable.GetGraph().IsPlaying()) return;
            if (playable.GetTime() < 0.001)
            {
                if (audioClips == null || audioClips.Length == 0 || Random.Range(0f, 1f) < dontPlayProbability) return;

                var clip = audioClips[Random.Range(0, audioClips.Length)];
                audioSource.clip = clip;
                audioSource.Play();
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // Debug.Log("OnBehaviourPlay");
            // Play指针进入Clip，Pause指针离开Clip
            if (!playable.GetGraph().IsPlaying()) return;

            if (audioClips == null || audioClips.Length == 0 || Random.Range(0f, 1f) < dontPlayProbability) return;

            var clip = audioClips[Random.Range(0, audioClips.Length)];
            audioSource.clip = clip;
            audioSource.Play();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);

            // Debug.Log("OnBehaviourPause");

            // audioSource.Pause();
        }
    }
}