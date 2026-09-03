using UnityEngine;

namespace BodyCatcher.Tracking
{
    public class BodyHandVisualizer : MonoBehaviour
    {
        [SerializeField] private MediapipeBodyTracker tracker;
        [SerializeField] private RectTransform leftHand;
        [SerializeField] private RectTransform rightHand;

        private void OnEnable()
        {
            if (tracker != null)
                tracker.OnBodyPoseUpdated += OnBodyPoseUpdated;
        }

        private void OnDisable()
        {
            if (tracker != null)
                tracker.OnBodyPoseUpdated -= OnBodyPoseUpdated;
        }

        private void OnBodyPoseUpdated(BodyPose pose)
        {
            if (!pose.IsValid)
                return;

            UpdateHand(leftHand, pose.LeftWrist);
            UpdateHand(rightHand, pose.RightWrist);
        }

        private void UpdateHand(
            RectTransform hand,
            Vector3 normalizedPosition)
        {
            if (hand == null)
                return;

            hand.anchorMin = normalizedPosition;
            hand.anchorMax = normalizedPosition;
            hand.anchoredPosition = Vector2.zero;
        }
    }
}