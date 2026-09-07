using UnityEngine;

namespace BodyCatcher.Gameplay
{
    public class VFXData : MonoBehaviour
    {
        [Header("VFX")]
        [SerializeField]
        private VFXType type;

        [SerializeField]
        private GameObject prefab;

        public VFXType Type => type;

        public GameObject Prefab => prefab;
    }
}