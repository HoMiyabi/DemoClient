using DG.Tweening;
using Kirara;
using UnityEngine.Rendering;

namespace Manager
{
    public class PostVolumeMgr : UnitySingleton<PostVolumeMgr>
    {
        public Volume defaultVolume;
        public Volume perfectDodgeVolume;

        public float enterDuration = 0.3f;
        public float holdDuration = 1f;
        public float exitDuration = 0.3f;

        private Sequence sequence;

        public void StartPerfectDodgeProfile()
        {
            sequence?.Kill();

            sequence = DOTween.Sequence();
            sequence.Append(DOTween.To(
                () => perfectDodgeVolume.weight,
                x => perfectDodgeVolume.weight = x,
                1f, enterDuration));
            sequence.AppendInterval(holdDuration);
            sequence.Append(DOTween.To(
                () => perfectDodgeVolume.weight,
                x => perfectDodgeVolume.weight = x,
                0f, exitDuration));
            sequence.OnComplete(() => sequence = null);
            sequence.Play();
        }
    }
}