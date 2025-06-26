using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace Kirara
{
    public class AudioMgr : UnitySingleton<AudioMgr>
    {
        [SerializeField] private GameObject audioPrefab;
        [SerializeField] private GameObject musicPrefab;
        [SerializeField] private AudioMixerGroup mainGroup;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup dialogGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        private ObjectPool<GameObject> audioPool;
        private GameObject musicGO;

        protected override void Awake()
        {
            base.Awake();

            audioPool = new ObjectPool<GameObject>(
                () => Instantiate(audioPrefab, transform),
                go => go.SetActive(true),
                go => go.SetActive(false),
                go => Destroy(go));
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;

            if (musicGO == null)
            {
                musicGO = Instantiate(musicPrefab, transform);
            }
            var audioSource = musicGO.GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = musicGroup;
            audioSource.Play();
        }

        public void PlaySFX(AudioClip clip, Vector3 position, bool loop = false)
        {
            PlayShot(clip, position, loop, sfxGroup);
        }

        private void PlayShot(AudioClip clip, Vector3 position, bool loop, AudioMixerGroup mixerGroup)
        {
            if (clip == null) return;

            var go = audioPool.Get();
            go.transform.position = position;

            var audioSource = go.GetComponent<AudioSource>();
            audioSource.loop = loop;
            audioSource.clip = clip;
            audioSource.outputAudioMixerGroup = mixerGroup;
            audioSource.Play();

            if (!audioSource.loop)
            {
                WaitAudioFinishThenRelease(go).Forget();
            }
        }

        private async UniTaskVoid WaitAudioFinishThenRelease(GameObject go)
        {
            var audioSource = go.GetComponent<AudioSource>();
            await UniTask.WaitForSeconds(audioSource.clip.length);
            audioPool.Release(go);
        }

        private static float MapVolume(float x)
        {
            x = Mathf.Clamp01(x);
            return 20f * Mathf.Log10(0.9999f * x + 0.0001f);
        }

        public float MainVolume
        {
            set => mainGroup.audioMixer.SetFloat("MainVolume", MapVolume(value));
        }

        public float MusicVolume
        {
            set => musicGroup.audioMixer.SetFloat("MusicVolume", MapVolume(value));
        }

        public float DialogVolume
        {
            set => dialogGroup.audioMixer.SetFloat("DialogVolume", MapVolume(value));
        }

        public float SFXVolume
        {
            set => sfxGroup.audioMixer.SetFloat("SFXVolume", MapVolume(value));
        }
    }
}