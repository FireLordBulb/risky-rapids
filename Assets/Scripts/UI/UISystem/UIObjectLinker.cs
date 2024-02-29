using UnityEngine;

namespace UISystem
{
    // Only to be used together with LinkableUIObject.
    // Put an instance of this in a LinkableUIObject's corresponding serialized field to link with the instance.
    // When linked this can be used to access the UIObject's GameObject and any attached Components.
    [CreateAssetMenu(fileName = "New UI Object Linker", menuName = "ScriptableObjects/UI Object Linker", order = 1)]
    public sealed class UIObjectLinker : ScriptableObject
    {
        public GameObject GameObject { private set; get; }
        public void SetupLink(LinkableUIObject linkableUIObject)
        {
            GameObject = linkableUIObject.gameObject;
        }
    }
}
