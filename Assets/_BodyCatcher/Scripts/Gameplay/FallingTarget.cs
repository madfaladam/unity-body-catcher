using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class FallingTarget : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField]
        private TargetType targetType =
            TargetType.Normal;

        [Header("Movement")]
        [SerializeField]
        private float fallSpeed = 3f;

        [Header("Score")]
        [SerializeField]
        private int normalScore = 10;

        [SerializeField]
        private int bonusScore = 25;

        private bool isFinished;

        private BodyCatcherGameManager gameManager;
        private TargetPool targetPool;

        public FallingTarget PoolPrefab
        {
            get;
            private set;
        }

        private void Awake()
        {
            gameManager =
                FindFirstObjectByType<
                    BodyCatcherGameManager>();
        }

        public void SetPool(
            TargetPool pool,
            FallingTarget prefab
        )
        {
            targetPool = pool;
            PoolPrefab = prefab;
        }

        public void ResetTarget()
        {
            isFinished = false;
        }

        private void OnEnable()
        {
            isFinished = false;
        }

        private void Update()
        {
            if (gameManager != null &&
                gameManager.State != GameState.Playing)
            {
                return;
            }

            transform.Translate(
                Vector3.down *
                fallSpeed *
                Time.deltaTime
            );

            if (transform.position.y < -6f)
            {
                Miss();
            }
        }

        private void OnTriggerEnter2D(
            Collider2D other
        )
        {
            if (isFinished)
            {
                return;
            }

            if (!other.CompareTag("PlayerBody"))
            {
                return;
            }

            Catch();
        }

        private void Catch()
        {
            if (isFinished)
            {
                return;
            }

            isFinished = true;

            switch (targetType)
            {
                case TargetType.Normal:

                    if (gameManager != null)
                    {
                        gameManager.TargetCaught(
                            normalScore
                        );
                    }

                    break;

                case TargetType.Bonus:

                    if (gameManager != null)
                    {
                        gameManager.TargetCaught(
                            bonusScore
                        );
                    }

                    break;

                case TargetType.Bomb:

                    if (gameManager != null)
                    {
                        gameManager.TargetBomb();
                    }

                    break;
            }

            ReturnToPool();
        }

        private void Miss()
        {
            if (isFinished)
            {
                return;
            }

            isFinished = true;

            if (targetType != TargetType.Bomb)
            {
                if (gameManager != null)
                {
                    gameManager.TargetMissed();
                }
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (targetPool != null)
            {
                targetPool.ReturnToPool(this);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}