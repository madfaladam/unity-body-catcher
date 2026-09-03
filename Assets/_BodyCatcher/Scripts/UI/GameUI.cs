using TMPro;
using UnityEngine;

namespace BodyCatcher.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("Game Manager")]
        [SerializeField]
        private BodyCatcher.Gameplay.BodyCatcherGameManager gameManager;

        [Header("HUD")]
        [SerializeField]
        private TMP_Text scoreText;

        [SerializeField]
        private TMP_Text comboText;

        [SerializeField]
        private TMP_Text livesText;

        [Header("Game Over")]
        [SerializeField]
        private GameObject gameOverPanel;

        [SerializeField]
        private TMP_Text finalScoreText;

        private void OnEnable()
        {
            if (gameManager == null)
            {
                return;
            }

            gameManager.OnScoreChanged += UpdateScore;
            gameManager.OnComboChanged += UpdateCombo;
            gameManager.OnLivesChanged += UpdateLives;
            gameManager.OnGameOver += ShowGameOver;
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
            gameManager.OnGameOver -= ShowGameOver;
        }

        private void Start()
        {
            UpdateScore(gameManager.Score);
            UpdateCombo(gameManager.Combo);
            UpdateLives(gameManager.Lives);

            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"SCORE: {score}";
            }
        }

        private void UpdateCombo(int combo)
        {
            if (comboText != null)
            {
                comboText.text = $"COMBO: x{combo}";
            }
        }

        private void UpdateLives(int lives)
        {
            if (livesText != null)
            {
                livesText.text = $"LIVES: {lives}";
            }
        }

        private void ShowGameOver()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }

            if (finalScoreText != null)
            {
                finalScoreText.text =
                    $"FINAL SCORE\n{gameManager.Score}";
            }
        }
    }
}