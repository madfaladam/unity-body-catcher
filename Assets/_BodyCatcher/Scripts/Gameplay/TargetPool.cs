using System.Collections.Generic;
using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class TargetPool : MonoBehaviour
    {
        [Header("Pool Settings")]
        [SerializeField]
        private int initialPoolSize = 2;

        [SerializeField]
        private bool expandable = true;

        [Header("Normal Prefabs")]
        [SerializeField]
        private FallingTarget[] normalTargetPrefabs;

        [Header("Bonus Prefabs")]
        [SerializeField]
        private FallingTarget[] bonusTargetPrefabs;

        [Header("Bomb Prefabs")]
        [SerializeField]
        private FallingTarget[] bombTargetPrefabs;

        private readonly Dictionary<
            FallingTarget,
            Queue<FallingTarget>
        > pools = new();

        private readonly HashSet<
            FallingTarget
        > activeTargets = new();

        private void Awake()
        {
            CreatePools(normalTargetPrefabs);
            CreatePools(bonusTargetPrefabs);
            CreatePools(bombTargetPrefabs);
        }

        private void CreatePools(
            FallingTarget[] prefabs
        )
        {
            if (prefabs == null)
            {
                return;
            }

            foreach (FallingTarget prefab in prefabs)
            {
                if (prefab == null)
                {
                    continue;
                }

                CreatePool(prefab);
            }
        }

        private void CreatePool(
            FallingTarget prefab
        )
        {
            if (pools.ContainsKey(prefab))
            {
                return;
            }

            Queue<FallingTarget> pool =
                new Queue<FallingTarget>();

            pools.Add(
                prefab,
                pool
            );

            for (int i = 0;
                 i < initialPoolSize;
                 i++)
            {
                FallingTarget target =
                    CreateTarget(prefab);

                pool.Enqueue(target);
            }
        }

        private FallingTarget CreateTarget(
            FallingTarget prefab
        )
        {
            FallingTarget target =
                Instantiate(
                    prefab,
                    transform
                );

            target.gameObject.SetActive(false);

            target.SetPool(
                this,
                prefab
            );

            return target;
        }

        public FallingTarget GetRandomTarget(
            TargetType type,
            Vector3 position
        )
        {
            FallingTarget prefab =
                GetRandomPrefab(type);

            if (prefab == null)
            {
                return null;
            }

            return Get(
                prefab,
                position
            );
        }

        private FallingTarget GetRandomPrefab(
            TargetType type
        )
        {
            FallingTarget[] prefabs =
                GetPrefabs(type);

            if (prefabs == null ||
                prefabs.Length == 0)
            {
                Debug.LogWarning(
                    $"TargetPool: Tidak ada prefab untuk {type}."
                );

                return null;
            }

            int index =
                Random.Range(
                    0,
                    prefabs.Length
                );

            return prefabs[index];
        }

        private FallingTarget[] GetPrefabs(
            TargetType type
        )
        {
            switch (type)
            {
                case TargetType.Normal:
                    return normalTargetPrefabs;

                case TargetType.Bonus:
                    return bonusTargetPrefabs;

                case TargetType.Bomb:
                    return bombTargetPrefabs;

                default:
                    return null;
            }
        }

        private FallingTarget Get(
            FallingTarget prefab,
            Vector3 position
        )
        {
            if (prefab == null)
            {
                return null;
            }

            if (!pools.TryGetValue(
                prefab,
                out Queue<FallingTarget> pool))
            {
                Debug.LogWarning(
                    $"TargetPool: Prefab {prefab.name} belum terdaftar."
                );

                return null;
            }

            FallingTarget target = null;

            if (pool.Count > 0)
            {
                target = pool.Dequeue();
            }
            else if (expandable)
            {
                target = CreateTarget(prefab);
            }

            if (target == null)
            {
                return null;
            }

            target.transform.SetPositionAndRotation(
                position,
                Quaternion.identity
            );

            target.gameObject.SetActive(true);

            target.ResetTarget();

            activeTargets.Add(target);

            return target;
        }

        public void ReturnToPool(
            FallingTarget target
        )
        {
            if (target == null)
            {
                return;
            }

            activeTargets.Remove(target);

            target.gameObject.SetActive(false);

            FallingTarget prefab =
                target.PoolPrefab;

            if (prefab == null)
            {
                Destroy(target.gameObject);
                return;
            }

            if (!pools.TryGetValue(
                prefab,
                out Queue<FallingTarget> pool))
            {
                Destroy(target.gameObject);
                return;
            }

            pool.Enqueue(target);
        }

        public void ClearAll()
        {
            FallingTarget[] active =
                new FallingTarget[
                    activeTargets.Count
                ];

            activeTargets.CopyTo(active);

            activeTargets.Clear();

            foreach (FallingTarget target in active)
            {
                if (target == null)
                {
                    continue;
                }

                target.gameObject.SetActive(false);

                if (target.PoolPrefab != null &&
                    pools.TryGetValue(
                        target.PoolPrefab,
                        out Queue<FallingTarget> pool))
                {
                    pool.Enqueue(target);
                }
            }
        }
    }
}