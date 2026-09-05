using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BodyCatcher.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Game Manager")]
        [SerializeField]
        private BodyCatcher.Gameplay.BodyCatcherGameManager gameManager;

        [Header("Panels")]
        [SerializeField]
        private GameObject panelStart;

        [SerializeField]
        private GameObject panelHUD;

        [SerializeField]
        private GameObject panelResult;

        [Header("Start Panel")]
        [SerializeField]
        private TMP_Text titleText;

        [SerializeField]
        private Button startBtn;

        [Header("HUD")]
        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text comboText;

        [SerializeField]
        private TMP_Text multiplierText;

        [SerializeField]
        private TMP_Text livesText;

        [Header("Result Panel")]
        [SerializeField]
        private TMP_Text finalScoreText;

        [SerializeField]
        private Button restartBtn;

        [SerializeField]
        private Button homeBtn;

        private void Awake()
        {
            RegisterButtonListeners();

            ShowStartPanel();
        }

        private void OnEnable()
        {
            if (gameManager == null)
            {
                return;
            }

            gameManager.OnScoreChanged += UpdateScore;
            gameManager.OnComboChanged += UpdateCombo;
            gameManager.OnLivesChanged += UpdateLives;
            gameManager.OnGameOver += ShowResultPanel;
        }

        private void OnDisable()
        {
            if (gameManager == null)
            {
                return;
            }

            gameManager.OnScoreChanged -= UpdateScore;
            gameManager.OnComboChanged -= UpdateCombo;
            gameManager.OnLivesChanged -= UpdateLives;
            gameManager.OnGameOver -= ShowResultPanel;
        }

        private void Start()
        {
            if (gameManager == null)
            {
                Debug.LogError(
                    "GameUI: GameManager is not assigned."
                );

                return;
            }

            UpdateScore(gameManager.Score);
            UpdateCombo(gameManager.Combo);
            UpdateLives(gameManager.Lives);
        }

        // --------------------------------------------------
        // BUTTON LISTENERS
        // --------------------------------------------------

        private void RegisterButtonListeners()
        {
            if (startBtn != null)
            {
                startBtn.onClick.AddListener(
                    OnStartButtonClicked
                );
            }

            if (restartBtn != null)
            {
                restartBtn.onClick.AddListener(
                    OnRestartButtonClicked
                );
            }

            if (homeBtn != null)
            {
                homeBtn.onClick.AddListener(
                    OnHomeButtonClicked
                );
            }
        }

        private void UnregisterButtonListeners()
        {
            if (startBtn != null)
            {
                startBtn.onClick.RemoveListener(
                    OnStartButtonClicked
                );
            }

            if (restartBtn != null)
            {
                restartBtn.onClick.RemoveListener(
                    OnRestartButtonClicked
                );
            }

            if (homeBtn != null)
            {
                homeBtn.onClick.RemoveListener(
                    OnHomeButtonClicked
                );
            }
        }

        private void OnDestroy()
        {
            UnregisterButtonListeners();
        }

        // --------------------------------------------------
        // BUTTON ACTIONS
        // --------------------------------------------------

        private void OnStartButtonClicked()
        {
            if (gameManager == null)
            {
                return;
            }

            gameManager.StartGame();

            ShowHUDPanel();
        }

        private void OnRestartButtonClicked()
        {
            SceneManager.LoadScene(
                SceneManager.GetActiveScene().buildIndex
            );
        }

        private void OnHomeButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.ResetGame();
            }

            ShowStartPanel();
        }

        // --------------------------------------------------
        // PANEL
        // --------------------------------------------------

        private void ShowStartPanel()
        {
            if (panelStart != null)
            {
                panelStart.SetActive(true);
            }

            if (panelHUD != null)
            {
                panelHUD.SetActive(false);
            }

            if (panelResult != null)
            {
                panelResult.SetActive(false);
            }
        }

        private void ShowHUDPanel()
        {
            if (panelStart != null)
            {
                panelStart.SetActive(false);
            }

            if (panelHUD != null)
            {
                panelHUD.SetActive(true);
            }

            if (panelResult != null)
            {
                panelResult.SetActive(false);
            }
        }

        private void ShowResultPanel()
        {
            if (panelStart != null)
            {
                panelStart.SetActive(false);
            }

            if (panelHUD != null)
            {
                panelHUD.SetActive(false);
            }

            if (panelResult != null)
            {
                panelResult.SetActive(true);
            }

            if (finalScoreText != null &&
                gameManager != null)
            {
                finalScoreText.text =
                    $"{gameManager.Score}";
            }
        }

        // --------------------------------------------------
        // HUD
        // --------------------------------------------------

        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text =
                    $"{score}";
            }
        }

        private void UpdateCombo(int combo)
        {
            if (comboText != null)
            {
                comboText.text =
                    $"x{combo}";
            }

            if (multiplierText != null &&
                gameManager != null)
            {
                multiplierText.text =
                    $"x{gameManager.ComboMultiplier}";
            }
        }

        private void UpdateLives(int lives)
        {
            if (livesText != null)
            {
                livesText.text =
                    $"{lives}";
            }
        }
    }
}