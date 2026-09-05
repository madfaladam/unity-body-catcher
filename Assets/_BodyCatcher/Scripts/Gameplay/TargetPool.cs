using System.Collections.Generic;
using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class TargetPool : MonoBehaviour
    {
        [Header("Pool")]
        [SerializeField]
        private int initialPoolSize = 20;

        [SerializeField]
        private bool expandable = true;

        [Header("Target Prefabs")]
        [SerializeField]
        private FallingTarget normalTargetPrefab;

        [SerializeField]
        private FallingTarget bonusTargetPrefab;

        [SerializeField]
        private FallingTarget bombTargetPrefab;

        private readonly Dictionary<
            FallingTarget,
            Queue<FallingTarget>
        > pools = new();

        private readonly HashSet<FallingTarget> activeTargets = new();

        private void Awake()
        {
            CreatePool(normalTargetPrefab);
            CreatePool(bonusTargetPrefab);
            CreatePool(bombTargetPrefab);
        }

        private void CreatePool(FallingTarget prefab)
        {
            if (prefab == null)
            {
                return;
            }

            if (pools.ContainsKey(prefab))
            {
                return;
            }

            Queue<FallingTarget> pool =
                new Queue<FallingTarget>();

            pools.Add(prefab, pool);

            for (int i = 0; i < initialPoolSize; i++)
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
                Instantiate(prefab, transform);

            target.gameObject.SetActive(false);

            target.SetPool(
                this,
                prefab
            );

            return target;
        }

        public FallingTarget Get(
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
                CreatePool(prefab);

                if (!pools.TryGetValue(
                    prefab,
                    out pool))
                {
                    return null;
                }
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
                new FallingTarget[activeTargets.Count];

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