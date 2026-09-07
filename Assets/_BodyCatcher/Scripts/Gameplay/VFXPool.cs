using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class VFXPool : MonoBehaviour
    {
        [Header("VFX Root")]
        [SerializeField]
        private Transform vfxRoot;

        [Header("Pool Settings")]
        [SerializeField]
        private int initialPoolSize = 5;

        [SerializeField]
        private bool expandable = true;

        private readonly Dictionary<
            VFXType,
            VFXData
        > vfxDictionary = new();

        private readonly Dictionary<
            VFXType,
            Queue<GameObject>
        > pools = new();

        private readonly Dictionary<
            GameObject,
            VFXType
        > activeVFX = new();

        private void Awake()
        {
            SetupRoot();

            BuildVFXPools();
        }

        // =========================================================
        // ROOT
        // =========================================================

        private void SetupRoot()
        {
            if (vfxRoot != null)
            {
                return;
            }

            GameObject root =
                new GameObject("VFXRoot");

            root.transform.SetParent(
                transform
            );

            vfxRoot =
                root.transform;
        }

        // =========================================================
        // BUILD POOLS
        // =========================================================

        private void BuildVFXPools()
        {
            if (vfxRoot == null)
            {
                return;
            }

            for (int i = 0;
                 i < vfxRoot.childCount;
                 i++)
            {
                Transform child =
                    vfxRoot.GetChild(i);

                if (child == null)
                {
                    continue;
                }

                VFXData data =
                    child.GetComponent<VFXData>();

                if (data == null)
                {
                    Debug.LogWarning(
                        $"VFXPool: {child.name} tidak memiliki VFXData.",
                        child
                    );

                    continue;
                }

                if (data.Prefab == null)
                {
                    Debug.LogWarning(
                        $"VFXPool: Prefab untuk {data.Type} belum di-assign.",
                        child
                    );

                    continue;
                }

                if (vfxDictionary.ContainsKey(
                    data.Type))
                {
                    Debug.LogWarning(
                        $"VFXPool: Duplicate VFXType {data.Type}.",
                        child
                    );

                    continue;
                }

                vfxDictionary.Add(
                    data.Type,
                    data
                );

                Queue<GameObject> pool =
                    new Queue<GameObject>();

                pools.Add(
                    data.Type,
                    pool
                );

                for (int j = 0;
                     j < initialPoolSize;
                     j++)
                {
                    GameObject instance =
                        CreateVFX(data);

                    pool.Enqueue(instance);
                }

                // Template/config object tidak perlu aktif
                child.gameObject.SetActive(false);
            }
        }

        // =========================================================
        // CREATE
        // =========================================================

        private GameObject CreateVFX(
            VFXData data
        )
        {
            GameObject instance =
                Instantiate(
                    data.Prefab,
                    vfxRoot
                );

            instance.SetActive(false);

            return instance;
        }

        // =========================================================
        // PLAY
        // =========================================================

        public void Play(
            VFXType type,
            Vector3 position
        )
        {
            if (!pools.TryGetValue(
                type,
                out Queue<GameObject> pool))
            {
                Debug.LogWarning(
                    $"VFXPool: VFXType {type} tidak ditemukan."
                );

                return;
            }

            GameObject vfx = null;

            if (pool.Count > 0)
            {
                vfx =
                    pool.Dequeue();
            }
            else if (expandable &&
                     vfxDictionary.TryGetValue(
                         type,
                         out VFXData data))
            {
                vfx =
                    CreateVFX(data);
            }

            if (vfx == null)
            {
                return;
            }

            vfx.transform.SetPositionAndRotation(
                position,
                Quaternion.identity
            );

            vfx.SetActive(true);

            activeVFX.Add(
                vfx,
                type
            );

            RestartVFX(vfx);

            StartCoroutine(
                ReturnToPoolWhenFinished(
                    vfx,
                    type
                )
            );
        }

        // =========================================================
        // RESTART VFX
        // =========================================================

        private void RestartVFX(
            GameObject vfx
        )
        {
            ParticleSystem[] particles =
                vfx.GetComponentsInChildren<
                    ParticleSystem
                >(true);

            foreach (ParticleSystem particle in particles)
            {
                particle.Clear();

                particle.Play();
            }
        }

        // =========================================================
        // WAIT
        // =========================================================

        private IEnumerator ReturnToPoolWhenFinished(
            GameObject vfx,
            VFXType type
        )
        {
            if (vfx == null)
            {
                yield break;
            }

            ParticleSystem[] particles =
                vfx.GetComponentsInChildren<
                    ParticleSystem
                >(true);

            if (particles.Length == 0)
            {
                yield return null;

                ReturnToPool(
                    vfx,
                    type
                );

                yield break;
            }

            bool isAlive = true;

            while (isAlive)
            {
                isAlive = false;

                foreach (ParticleSystem particle
                         in particles)
                {
                    if (particle != null &&
                        particle.IsAlive(true))
                    {
                        isAlive = true;

                        break;
                    }
                }

                yield return null;
            }

            ReturnToPool(
                vfx,
                type
            );
        }

        // =========================================================
        // RETURN
        // =========================================================

        private void ReturnToPool(
            GameObject vfx,
            VFXType type
        )
        {
            if (vfx == null)
            {
                return;
            }

            activeVFX.Remove(vfx);

            StopVFX(vfx);

            vfx.SetActive(false);

            if (pools.TryGetValue(
                type,
                out Queue<GameObject> pool))
            {
                pool.Enqueue(vfx);
            }
            else
            {
                Destroy(vfx);
            }
        }

        // =========================================================
        // STOP
        // =========================================================

        private void StopVFX(
            GameObject vfx
        )
        {
            ParticleSystem[] particles =
                vfx.GetComponentsInChildren<
                    ParticleSystem
                >(true);

            foreach (ParticleSystem particle in particles)
            {
                if (particle == null)
                {
                    continue;
                }

                particle.Stop(
                    true,
                    ParticleSystemStopBehavior
                        .StopEmittingAndClear
                );
            }
        }

        // =========================================================
        // CLEAR
        // =========================================================

        public void ClearAll()
        {
            GameObject[] active =
                new GameObject[
                    activeVFX.Count
                ];

            VFXType[] types =
                new VFXType[
                    activeVFX.Count
                ];

            int index = 0;

            foreach (
                KeyValuePair<
                    GameObject,
                    VFXType
                > pair in activeVFX)
            {
                active[index] =
                    pair.Key;

                types[index] =
                    pair.Value;

                index++;
            }

            activeVFX.Clear();

            for (int i = 0;
                 i < active.Length;
                 i++)
            {
                GameObject vfx =
                    active[i];

                if (vfx == null)
                {
                    continue;
                }

                StopVFX(vfx);

                vfx.SetActive(false);

                if (pools.TryGetValue(
                    types[i],
                    out Queue<GameObject> pool))
                {
                    pool.Enqueue(vfx);
                }
            }
        }
    }
}