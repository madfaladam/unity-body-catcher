using UnityEngine;
using DG.Tweening;

namespace BodyCatcher.Animation
{
    public class PanelStartAnim : MonoBehaviour
    {
        [SerializeField] private RectTransform titleRect;
        [SerializeField] private RectTransform startBtnRect;
        
        [SerializeField] private RectTransform decorLeftRect;
        [SerializeField] private RectTransform decorRightRect;


        void OnEnable()
        {
            PlayAnimIn();
        }

        void OnDisable()
        {
            if (titleRect != null)
            {
                titleRect.DOKill();
            }

            if (startBtnRect != null)
            {
                startBtnRect.DOKill();
            }

            if (decorLeftRect != null)
            {
                decorLeftRect.DOKill();
            }

            if (decorRightRect != null)
            {
                decorRightRect.DOKill();
            }
        }
        private void PlayAnimIn()
        {
            if (titleRect != null)
            {
                titleRect.localScale = Vector3.zero;
                titleRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            }

            if (startBtnRect != null)
            {
                startBtnRect.localScale = Vector3.zero;
                startBtnRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).SetDelay(0.2f);
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
        }
    }
}