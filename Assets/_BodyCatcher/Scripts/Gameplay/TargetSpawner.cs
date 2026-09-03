using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class TargetSpawner : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField]
        private FallingTarget targetPrefab;

        [Header("Spawn Area")]
        [SerializeField]
        private float spawnWidth = 7f;

        [SerializeField]
        private float spawnY = 5.5f;

        [Header("Timing")]
        [SerializeField]
        private float spawnInterval = 1.5f;

        private float timer;

        private BodyCatcherGameManager gameManager;

        private void Awake()
        {
            gameManager =
                FindFirstObjectByType<BodyCatcherGameManager>();
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
            if (targetPrefab == null)
            {
                return;
            }

            float x = Random.Range(
                -spawnWidth / 2f,
                spawnWidth / 2f
            );

            Vector3 position = new Vector3(
                x,
                spawnY,
                0f
            );

            Instantiate(
                targetPrefab,
                position,
                Quaternion.identity
            );
        }
    }
}