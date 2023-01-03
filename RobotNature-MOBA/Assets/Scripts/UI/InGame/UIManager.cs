using UnityEngine;
using UnityEngine.Animations;

namespace UI.InGame
{
    public partial class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        private ConstraintSource constraintSource;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            Instance = this;
        }
    }
}