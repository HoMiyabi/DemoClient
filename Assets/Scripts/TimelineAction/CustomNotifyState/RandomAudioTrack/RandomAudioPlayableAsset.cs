using System.ComponentModel;
using Kirara.TimelineAction;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Kirara.TimelineAction
{
    [DisplayName("随机音频")]
    public class RandomAudioPlayableAsset : ActionNotifyState
    {
        public TimelineClip timelineClip;
        public float audioClipsMaxLength;

        [Range(0f, 1f)] public float dontPlayProbability;
        public AudioClip[] audioClips;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<RandomAudioPlayable>.Create(graph);
            var behaviour = playable.GetBehaviour();
            behaviour.dontPlayProbability = dontPlayProbability;
            behaviour.audioClips = audioClips;
            return playable;
        }

        private void OnValidate()
        {
            audioClipsMaxLength = 0f;
            if (audioClips == null) return;
            foreach (var clip in audioClips)
            {
                if (clip != null)
                {
                    audioClipsMaxLength = Mathf.Max(audioClipsMaxLength, clip.length);
                }
            }
        }

        public override void NotifyBegin(ActionCtrl actionCtrl)
        {
            var transform = actionCtrl.transform;
            AudioMgr.Instance.PlaySFX(
                audioClips.RandomItem(dontPlayProbability),
                transform.position);
        }
    }
}