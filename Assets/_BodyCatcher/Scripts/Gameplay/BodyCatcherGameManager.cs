using System;
using UnityEngine;
using BodyCatcher.Tracking;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using BodyCatcher.Audio;
using AudioType = BodyCatcher.Audio.AudioType;

namespace BodyCatcher.Gameplay
{
    public class BodyCatcherGameManager : MonoBehaviour
    {
        [Header("Tracking")]
        [SerializeField]
        private MediapipeBodyTracker bodyTracker;

        [SerializeField]
        private TargetPool targetPool;

        [Header("Body Catch Zones")]
        [SerializeField]
        private Transform headZone;

        [SerializeField]
        private Transform bodyZone;
        [Header("Coordinate")]
        [SerializeField]
        private Camera mainCamera;

        [Header("Game")]
        [SerializeField]
        private int startingLives = 3;

        [Header("Difficulty")]
        [SerializeField]
        private int catchesPerSpeedIncrease = 10;

        [SerializeField]
        private float speedMultiplierPerLevel = 0.15f;

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

        public int ComboMultiplier
        {
            get
            {
                if (Combo >= 10)
                {
                    return 3;
                }

                if (Combo >= 5)
                {
                    return 2;
                }

                return 1;
            }
        }
        public int TotalCatches { get; private set; }

        public int DifficultyLevel
        {
            get
            {
                return TotalCatches /
                    catchesPerSpeedIncrease;
            }
        }

        public float CurrentSpeedMultiplier
        {
            get
            {
                return 1f +
                    DifficultyLevel *
                    speedMultiplierPerLevel;
            }
        }
        private void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            State = GameState.Ready;

            Score = 0;
            Combo = 0;
            TotalCatches = 0;
            Lives = startingLives;
        }
        void Start()
        {
            AudioManager.Instance.PlayBGM(AudioType.BGM_Menu);
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

            UpdateBodyPartPosition(
                headZone,
                pose.Head
            );

            UpdateBodyPartPosition(
                bodyZone,
                pose.Body
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

        private void UpdateBodyPartPosition(
            Transform bodyPartZone,
            Vector3 normalizedPosition
        )
        {
            if (bodyPartZone == null || mainCamera == null)
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

            worldPosition.z = bodyPartZone.position.z;

            bodyPartZone.position = worldPosition;
        }

        // --------------------------------------------------
        // GAMEPLAY
        // --------------------------------------------------

        public void StartGame()
        {
            Score = 0;
            Combo = 0;
            TotalCatches = 0;
            Lives = startingLives;

            State = GameState.Playing;

            OnScoreChanged?.Invoke(Score);
            OnComboChanged?.Invoke(Combo);
            OnLivesChanged?.Invoke(Lives);

            Debug.Log("BODY CATCHER STARTED");
        }

        public void TargetCaught(int baseScore)
        {
            TotalCatches++;

            Combo++;

            int finalScore =
                baseScore * ComboMultiplier;

            Score += finalScore;

            OnScoreChanged?.Invoke(Score);
            OnComboChanged?.Invoke(Combo);

            Debug.Log(
                $"Catch: {TotalCatches} | " +
                $"Difficulty: {DifficultyLevel} | " +
                $"Speed Multiplier: {CurrentSpeedMultiplier:F2}x"
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

        public void TargetBomb()
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
                $"BOMB! Lives={Lives}"
            );

            if (Lives <= 0)
            {
                GameOver();
            }
        }
        public void ResetGame()
        {
            State = GameState.Ready;

            Score = 0;
            Combo = 0;
            TotalCatches = 0;
            Lives = startingLives;

            OnScoreChanged?.Invoke(Score);
            OnComboChanged?.Invoke(Combo);
            OnLivesChanged?.Invoke(Lives);

            Debug.Log("GAME RESET");
        }
        private void GameOver()
        {
            State = GameState.GameOver;

            if (targetPool != null)
            {
                targetPool.ClearAll();
            }

            Debug.Log(
                $"GAME OVER! FINAL SCORE = {Score}"
            );

            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySFX(AudioType.Finish);

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