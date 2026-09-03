using UnityEngine;

namespace BodyCatcher.Tracking
{
    public struct BodyPose
    {
        public bool IsValid;

        public Vector3 Nose;

        public Vector3 LeftShoulder;
        public Vector3 RightShoulder;

        public Vector3 LeftElbow;
        public Vector3 RightElbow;

        public Vector3 LeftWrist;
        public Vector3 RightWrist;

        public Vector3 LeftHip;
        public Vector3 RightHip;

        public Vector3 LeftKnee;
        public Vector3 RightKnee;

        public Vector3 LeftAnkle;
        public Vector3 RightAnkle;
    }
}