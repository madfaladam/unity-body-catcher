using UnityEngine;
using DG.Tweening;

namespace BodyCatcher.Animation
{
    public class PanelResultAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform frameRect;
        
        [SerializeField] private RectTransform decorLeftRect;
        [SerializeField] private RectTransform decorRightRect;

        [SerializeField] private RectTransform decorLeftRect2;
        [SerializeField] private RectTransform decorRightRect2;

        void OnEnable()
        {
            PlayAnimIn();
        }

        void OnDisable()
        {
            if (frameRect != null)
            {
                frameRect.DOKill();
            }

            if (decorLeftRect != null)
            {
                decorLeftRect.DOKill();
            }

            if (decorRightRect != null)
            {
                decorRightRect.DOKill();
            }
            if (decorLeftRect2 != null)
            {
                decorLeftRect2.DOKill();
            }
        }
        private void PlayAnimIn()
        {
            if (frameRect != null)
            {
                frameRect.localScale = Vector3.zero;
                frameRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }

            if (decorLeftRect != null)
            {
                decorLeftRect.localScale = Vector3.one;
                decorLeftRect.DOScale(Vector3.one*.95f, 4f).SetEase(Ease.InOutSine).SetDelay(0.1f).SetLoops(-1, LoopType.Yoyo);
            }

            if (decorRightRect != null)
            {
                decorRightRect.localScale = Vector3.one;
                decorRightRect.DOScale(Vector3.one*.95f, 4f).SetEase(Ease.InOutSine).SetDelay(0.1f).SetLoops(-1, LoopType.Yoyo);
            }

            if (decorLeftRect2 != null)
            {
                decorLeftRect2.anchoredPosition = new Vector2(decorLeftRect2.anchoredPosition.x, -600f);
                decorLeftRect2.DOAnchorPosY(0f, 1f).SetEase(Ease.OutSine).SetDelay(0.15f).OnComplete(
                    () => {
                        decorLeftRect2.DOAnchorPosY(-20f, 4f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    });
            }

            if (decorRightRect2 != null)
            {
                decorRightRect2.anchoredPosition = new Vector2(decorRightRect2.anchoredPosition.x, -600f);
                decorRightRect2.DOAnchorPosY(0f, 1f).SetEase(Ease.OutSine).SetDelay(0.3f).OnComplete(
                    () => {
                        decorRightRect2.DOAnchorPosY(-20f, 4f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
                    });
            }
        }
    }
}