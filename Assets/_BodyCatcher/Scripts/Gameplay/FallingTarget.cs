using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class FallingTarget : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        private float fallSpeed = 3f;

        [Header("Score")]
        [SerializeField]
        private int scoreValue = 10;

        private bool isFinished;

        private BodyCatcherGameManager gameManager;

        private void Awake()
        {
            gameManager =
                FindFirstObjectByType<BodyCatcherGameManager>();
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isFinished)
            {
                return;
            }

            if (!other.CompareTag("PlayerHand"))
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

            if (gameManager != null)
            {
                gameManager.TargetCaught(scoreValue);
            }

            Destroy(gameObject);
        }

        private void Miss()
        {
            if (isFinished)
            {
                return;
            }

            isFinished = true;

            if (gameManager != null)
            {
                gameManager.TargetMissed();
            }

            Destroy(gameObject);
        }
    }
}