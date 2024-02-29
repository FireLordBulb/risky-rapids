using UnityEngine;

namespace UISystem
{
    // Put on root UI prefab to initialize LinkableUIObjects that don't start active (which is almost all of them).
    // Inactive LinkableUIObjects don't call Awake() when loaded so have to be initialized by this script.
    public sealed class LinkableUIObjectInitializer : MonoBehaviour
    {
        private void Awake()
        {
            LinkableUIObject[] linkedUIObjects = GetComponentsInChildren<LinkableUIObject>(true);
            foreach (LinkableUIObject linkableUIObject in linkedUIObjects)
            {
                linkableUIObject.Initialize();
            }
        }
    }
}