using System.Collections.Generic;
using UnityEngine;

namespace BodyCatcher.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance
        {
            get;
            private set;
        }

        [Header("Audio Roots")]
        [SerializeField]
        private Transform bgmRoot;

        [SerializeField]
        private Transform sfxRoot;

        [Header("Volume")]
        [Range(0f, 1f)]
        [SerializeField]
        private float bgmVolume = 0.5f;

        [Range(0f, 1f)]
        [SerializeField]
        private float sfxVolume = 1f;

        private readonly Dictionary<
            AudioType,
            AudioClipData
        > audioDictionary = new();

        private AudioClipData currentBGM;

        private void Awake()
        {
            if (Instance != null &&
                Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);

            SetupRoots();

            SetupAudioClipData();

            BuildAudioDictionary();
        }

        // =========================================================
        // ROOT
        // =========================================================

        private void SetupRoots()
        {
            if (bgmRoot == null)
            {
                GameObject root =
                    new GameObject("BGMRoot");

                root.transform.SetParent(transform);

                bgmRoot =
                    root.transform;
            }

            if (sfxRoot == null)
            {
                GameObject root =
                    new GameObject("SFXRoot");

                root.transform.SetParent(transform);

                sfxRoot =
                    root.transform;
            }
        }

        // =========================================================
        // AUDIO DATA
        // =========================================================

        private void SetupAudioClipData()
        {
            SetupRoot(
                bgmRoot,
                true
            );

            SetupRoot(
                sfxRoot,
                false
            );
        }

        private void SetupRoot(
            Transform root,
            bool isBGM
        )
        {
            if (root == null)
            {
                return;
            }

            for (int i = 0;
                 i < root.childCount;
                 i++)
            {
                Transform child =
                    root.GetChild(i);

                if (child == null)
                {
                    continue;
                }

                AudioClipData AudioClipData =
                    child.GetComponent<AudioClipData>();

                if (AudioClipData == null)
                {
                    AudioClipData =
                        child.gameObject.AddComponent<AudioClipData>();
                }

                AudioSource source =
                    child.GetComponent<AudioSource>();

                if (source == null)
                {
                    source =
                        child.gameObject.AddComponent<AudioSource>();
                }

                source.playOnAwake = false;

                source.loop = isBGM;

                source.volume =
                    isBGM
                        ? bgmVolume
                        : sfxVolume;
            }
        }

        // =========================================================
        // DICTIONARY
        // =========================================================

        private void BuildAudioDictionary()
        {
            audioDictionary.Clear();

            AudioClipData[] AudioClipData =
                GetComponentsInChildren<AudioClipData>(
                    true
                );

            foreach (AudioClipData data in AudioClipData)
            {
                if (data == null)
                {
                    continue;
                }

                if (data.Clip == null)
                {
                    Debug.LogWarning(
                        $"AudioManager: AudioClip kosong pada {data.gameObject.name}",
                        data
                    );

                    continue;
                }

                if (audioDictionary.ContainsKey(
                    data.Type))
                {
                    Debug.LogWarning(
                        $"AudioManager: Duplicate AudioType: {data.Type}",
                        data
                    );

                    continue;
                }

                audioDictionary.Add(
                    data.Type,
                    data
                );
            }

            Debug.Log(
                $"AudioManager: Loaded {audioDictionary.Count} audio."
            );
        }

        // =========================================================
        // BGM
        // =========================================================

        public void PlayBGM(AudioType type)
        {
            if (!TryGetAudio(
                type,
                out AudioClipData data))
            {
                return;
            }

            if (currentBGM != null)
            {
                currentBGM.Stop();
            }

            currentBGM = data;

            AudioSource source =
                data.Source;

            source.loop = true;

            source.volume =
                bgmVolume *
                data.Volume;

            source.pitch =
                data.Pitch;

            source.clip =
                data.Clip;

            source.Play();
        }

        public void StopBGM()
        {
            if (currentBGM == null)
            {
                return;
            }

            currentBGM.Stop();

            currentBGM = null;
        }

        public void PauseBGM()
        {
            if (currentBGM == null)
            {
                return;
            }

            currentBGM.Source.Pause();
        }

        public void ResumeBGM()
        {
            if (currentBGM == null)
            {
                return;
            }

            currentBGM.Source.UnPause();
        }

        // =========================================================
        // SFX
        // =========================================================

        public void PlaySFX(AudioType type)
        {
            if (!TryGetAudio(
                type,
                out AudioClipData data))
            {
                return;
            }

            AudioSource source =
                data.Source;

            source.volume =
                sfxVolume *
                data.Volume;

            source.pitch =
                data.Pitch;

            source.PlayOneShot(
                data.Clip
            );
        }

        // =========================================================
        // LOOKUP
        // =========================================================

        private bool TryGetAudio(
            AudioType type,
            out AudioClipData data
        )
        {
            if (audioDictionary.TryGetValue(
                type,
                out data))
            {
                return true;
            }

            Debug.LogWarning(
                $"AudioManager: Audio tidak ditemukan: {type}"
            );

            data = null;

            return false;
        }

        // =========================================================
        // VOLUME
        // =========================================================

        public void SetBGMVolume(float volume)
        {
            bgmVolume =
                Mathf.Clamp01(volume);

            if (currentBGM != null)
            {
                currentBGM.Source.volume =
                    bgmVolume *
                    currentBGM.Volume;
            }
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume =
                Mathf.Clamp01(volume);
        }
    }
}