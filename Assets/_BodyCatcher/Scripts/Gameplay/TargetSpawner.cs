using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class TargetSpawner : MonoBehaviour
    {
        [Header("Pool")]
        [SerializeField]
        private TargetPool targetPool;

        [Header("Spawn Area")]
        [SerializeField]
        private float spawnWidth = 7f;

        [SerializeField]
        private float spawnY = 5.5f;

        [Header("Timing")]
        [SerializeField]
        private float spawnInterval = 1.5f;

        [Header("Spawn Chance")]
        [Range(0f, 1f)]
        [SerializeField]
        private float bombChance = 0.10f;

        [Range(0f, 1f)]
        [SerializeField]
        private float bonusChance = 0.10f;

        private float timer;

        private BodyCatcherGameManager gameManager;

        private void Awake()
        {
            gameManager =
                FindFirstObjectByType<
                    BodyCatcherGameManager>();
        }

        private void Update()
        {
            if (gameManager == null)
            {
                return;
            }

            if (gameManager.State != GameState.Playing)
            {
                return;
            }

            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer = 0f;

                SpawnTarget();
            }
        }

        private void SpawnTarget()
        {
            if (targetPool == null)
            {
                return;
            }

            TargetType targetType =
                GetRandomTargetType();

            float x =
                Random.Range(
                    -spawnWidth / 2f,
                    spawnWidth / 2f
                );

            Vector3 position =
                new Vector3(
                    x,
                    spawnY,
                    0f
                );

            targetPool.GetRandomTarget(
                targetType,
                position
            );
        }

        private TargetType GetRandomTargetType()
        {
            float random =
                Random.value;

            if (random < bombChance)
            {
                return TargetType.Bomb;
            }

            if (random <
                bombChance +
                bonusChance)
            {
                return TargetType.Bonus;
            }

            return TargetType.Normal;
        }

        public void ResetSpawner()
        {
            timer = 0f;
        }
    }
}