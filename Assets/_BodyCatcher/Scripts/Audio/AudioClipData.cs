using DG.Tweening;
using UnityEngine;

namespace BodyCatcher.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioClipData : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField]
        private AudioType type;

        [SerializeField]
        private AudioClip clip;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField]
        private float volume = 1f;

        [Range(0.5f, 2f)]
        [SerializeField]
        private float pitch = 1f;

        private AudioSource audioSource;

        public AudioType Type => type;

        public AudioClip Clip => clip;

        public float Volume => volume;

        public float Pitch => pitch;

        public AudioSource Source => audioSource;

        private void Awake()
        {
            audioSource =
                GetComponent<AudioSource>();

            audioSource.playOnAwake = false;
        }

        public void Play()
        {
            if (audioSource == null ||
                clip == null)
            {
                return;
            }

            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = pitch;

            audioSource.Play();
        }

        public void PlayOneShot()
        {
            if (audioSource == null ||
                clip == null)
            {
                return;
            }

            audioSource.pitch = pitch;

            audioSource.PlayOneShot(
                clip,
                volume
            );
        }

        public void Stop()
        {
            if (audioSource == null)
            {
                return;
            }

            audioSource.DOFade(0f, 0.5f).OnComplete(() => {
                audioSource.Stop();
            });
        }

        public bool IsPlaying()
        {
            return audioSource != null &&
                   audioSource.isPlaying;
        }
    }
}