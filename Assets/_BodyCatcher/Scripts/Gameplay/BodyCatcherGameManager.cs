using System;
using UnityEngine;
using BodyCatcher.Tracking;
using UnityEngine.SceneManagement;

namespace BodyCatcher.Gameplay
{
    public class BodyCatcherGameManager : MonoBehaviour
    {
        [Header("Tracking")]
        [SerializeField]
        private MediapipeBodyTracker bodyTracker;

        [Header("Hand Catch Zones")]
        [SerializeField]
        private Transform leftHandZone;

        [SerializeField]
        private Transform rightHandZone;

        [Header("Coordinate")]
        [SerializeField]
        private Camera mainCamera;

        [Header("Game")]
        [SerializeField]
        private int startingLives = 3;

        // Public events for UI
        public Action<int> OnScoreChanged;
        public Action<int> OnComboChanged;
        public Action<int> OnLivesChanged;
        public Action OnGameOver;

        public int Score { get; private set; }
        public int Combo { get; private set; }
        public int Lives { get; private set; }

        public GameState State { get; private set; }

        private BodyPose currentPose;

        private readonly object poseLock = new object();

        private bool hasNewPose;

        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            State = GameState.Ready;

            Score = 0;
            Combo = 0;
            Lives = startingLives;
        }

        private void OnEnable()
        {
            if (bodyTracker != null)
            {
                bodyTracker.OnBodyPoseUpdated += OnBodyPoseUpdated;
            }
        }

        private void OnDisable()
        {
            if (bodyTracker != null)
            {
                bodyTracker.OnBodyPoseUpdated -= OnBodyPoseUpdated;
            }
        }

        private void Update()
        {
            UpdateBodyTracking();
        }

        private void UpdateBodyTracking()
        {
            BodyPose pose;

            lock (poseLock)
            {
                if (!hasNewPose)
                {
                    return;
                }

                pose = currentPose;
                hasNewPose = false;
            }

            if (!pose.IsValid)
            {
                return;
            }

            UpdateHandPosition(
                leftHandZone,
                pose.LeftWrist
            );

            UpdateHandPosition(
                rightHandZone,
                pose.RightWrist
            );
        }

        private void OnBodyPoseUpdated(BodyPose pose)
        {
            // MediaPipe callback can run on a worker thread.
            // Do not access Unity API here.

            lock (poseLock)
            {
                currentPose = pose;
                hasNewPose = true;
            }
        }

        private void UpdateHandPosition(
            Transform handZone,
            Vector3 normalizedPosition
        )
        {
            if (handZone == null || mainCamera == null)
            {
                return;
            }

            Vector3 screenPosition = new Vector3(
                normalizedPosition.x * Screen.width,
                normalizedPosition.y * Screen.height,
                Mathf.Abs(mainCamera.transform.position.z)
            );

            Vector3 worldPosition =
                mainCamera.ScreenToWorldPoint(screenPosition);

            worldPosition.z = handZone.position.z;

            handZone.position = worldPosition;
        }

        // --------------------------------------------------
        // GAMEPLAY
        // --------------------------------------------------

        public void StartGame()
        {
            Score = 0;
            Combo = 0;
            Lives = startingLives;

            State = GameState.Playing;

            OnScoreChanged?.Invoke(Score);
            OnComboChanged?.Invoke(Combo);
            OnLivesChanged?.Invoke(Lives);

            Debug.Log("BODY CATCHER STARTED");
        }

        public void TargetCaught(int score)
        {
            if (State != GameState.Playing)
            {
                return;
            }

            Score += score;

            Combo++;

            OnScoreChanged?.Invoke(Score);
            OnComboChanged?.Invoke(Combo);

            Debug.Log(
                $"CAUGHT! Score={Score} Combo={Combo}"
            );
        }

        public void TargetMissed()
        {
            if (State != GameState.Playing)
            {
                return;
            }

            Combo = 0;

            Lives--;

            OnComboChanged?.Invoke(Combo);
            OnLivesChanged?.Invoke(Lives);

            Debug.Log(
                $"MISSED! Lives={Lives}"
            );

            if (Lives <= 0)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            State = GameState.GameOver;

            Debug.Log(
                $"GAME OVER! FINAL SCORE = {Score}"
            );

            OnGameOver?.Invoke();
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }
    }
}