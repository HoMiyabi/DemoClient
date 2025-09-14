using System.Collections;
using Kirara;
using UnityEngine;
using UnityEngine.Rendering;

namespace Manager
{
    public class PostVolumeMgr : UnitySingleton<PostVolumeMgr>
    {
        public Volume volume;

        public VolumeProfile defaultProfile;
        public VolumeProfile perfectDodgeProfile;

        public void StartPerfectDodgeProfile()
        {
            StopAllCoroutines();
            StartCoroutine(DoPerfectDodgeProfileInternal());
        }

        private IEnumerator DoPerfectDodgeProfileInternal()
        {
            volume.profile = perfectDodgeProfile;
            yield return new WaitForSeconds(1f);
            volume.profile = defaultProfile;
        }
    }
}