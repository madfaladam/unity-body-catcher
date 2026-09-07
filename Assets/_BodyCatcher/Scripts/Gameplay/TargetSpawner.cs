using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class TargetSpawner : MonoBehaviour
    {
        [Header("Pool")]
        [SerializeField]
        private TargetPool targetPool;

        [Header("Normal Prefabs")]
        [SerializeField]
        private FallingTarget[] normalTargetPrefabs;

        [Header("Bonus Prefabs")]
        [SerializeField]
        private FallingTarget[] bonusTargetPrefabs;

        [Header("Bomb Prefabs")]
        [SerializeField]
        private FallingTarget[] bombTargetPrefabs;

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

            FallingTarget prefab =
                GetRandomTargetPrefab();

            if (prefab == null)
            {
                return;
            }

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

            targetPool.Get(
                prefab,
                position
            );
        }

        private FallingTarget GetRandomTargetPrefab()
        {
            float random =
                Random.value;
            Debug.Log($"Random value: {random}, bombChance: {bombChance}, bonusChance: {bonusChance}");
            if (random < bombChance)
            {
                return GetRandomPrefab(
                    bombTargetPrefabs
                );
            }

            if (random <
                bombChance +
                bonusChance)
            {
                return GetRandomPrefab(
                    bonusTargetPrefabs
                );
            }

            return GetRandomPrefab(
                normalTargetPrefabs
            );
        }

        private FallingTarget GetRandomPrefab(
            FallingTarget[] prefabs
        )
        {
            Debug.Log($"Getting random prefab from array of length: {prefabs?.Length ?? 0}");
            if (prefabs == null ||
                prefabs.Length == 0)
            {
                return null;
            }

            int index =
                Random.Range(
                    0,
                    prefabs.Length
                );
            Debug.Log($"Selected prefab index: {index}");
            return prefabs[index];
        }

        public void ResetSpawner()
        {
            timer = 0f;
        }
    }
}