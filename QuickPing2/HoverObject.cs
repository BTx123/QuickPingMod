using UnityEngine;
using UnityEngine.Serialization;

namespace QuickPing2
{
    public class HoverObject : MonoBehaviour
    {
        public string Name { get; set; }

        public GameObject Hover;

        public IDestructible Destructible;

        public Vector3 pos;

        public Vector3 center;

        public HoverType type;

        [FormerlySerializedAs("pinable")]
        public bool pinnable;
    }
}
