using UnityEngine;

namespace MyGameNamespace
{

    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "Configs/ResourceConfig")]
    public class ResourceConfig : ScriptableObject
    {
        public string ID;
        public Sprite Avatar;
        public string Name;
    }
}
