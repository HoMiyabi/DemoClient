using UnityEngine;
using UnityEngine.Playables;

namespace Kirara.TimelineAction
{
    public class BoxPlayable : PlayableBehaviour
    {
        public BoxPlayableAsset asset;
        private AudioSource audioSource;

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);

#if UNITY_EDITOR
            audioSource = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags("AudioSource",
                    HideFlags.HideAndDontSave, typeof(AudioSource))
                .GetComponent<AudioSource>();
#endif
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            base.OnPlayableDestroy(playable);
            Object.DestroyImmediate(audioSource.gameObject);
        }

        // public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        // {
        //     base.ProcessFrame(playable, info, playerData);
        //
        //     if (!playable.GetGraph().IsPlaying()) return;
        //     if (playable.GetTime() < 0.001)
        //     {
        //         if (asset.boxType == EBoxType.HitBox && asset.hitAudio != null)
        //         {
        //             audioSource.clip = asset.hitAudio;
        //             audioSource.Play();
        //             Debug.Log($"Play {asset.hitAudio.name}");
        //         }
        //     }
        // }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // 无论暂停还是播放，无论左到右还是右到左，只要Timeline指针进入Clip就会调用OnBehaviourPlay
            if (!playable.GetGraph().IsPlaying()) return;

            if (asset.boxType == EBoxType.HitBox && asset.hitAudio != null)
            {
                audioSource.clip = asset.hitAudio;
                audioSource.Play();
                // Debug.Log($"Play {asset.hitAudio.name}");
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            // Timeline指针离开Clip
            base.OnBehaviourPause(playable, info);

            // Debug.Log("OnBehaviourPause");

            // audioSource.Pause();
        }
    }
}