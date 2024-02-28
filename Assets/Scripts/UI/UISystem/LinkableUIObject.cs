using UnityEngine;

namespace UISystem
{
    // Put on any UI GameObject that should be referencable from anywhere, regardless of prefab/scene structure.
    // The main intended uses are:
    // 1. To allow buttons located on an object in one prefab to activate a UIPanel in another
    // prefab without relying on a reference in a scene or composed prefab.
    // 2. To give game logic an easy way to access UI to update which panel is active or text or other information.

    // The class is sealed since it should not be extended with additional logic.
    // Put all additional logic in separate MonoBehaviors that you access using GetComponentByType<> on the UIObjectLinker's GameObject reference.
    public sealed class LinkableUIObject : MonoBehaviour
    {
        [SerializeField] private UIObjectLinker linker;
        private bool isInitialized;

        private void Awake()
        {
            Initialize();
        }
        public void Initialize()
        {
            if (isInitialized)
            {
                return;
            }
            linker.SetupLink(this);
            isInitialized = true;
        }
    }
}