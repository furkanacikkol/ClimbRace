#pragma warning disable 0649
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace Packages.HifiveInputManager.Scripts.Runtime
{
    public class TapHoldControl : MonoBehaviour
    {
        [SerializeField] private ClimbingSystem climbingSystem;
        [HideInInspector] public bool isButton;
        [HideInInspector] public bool dead;

        #region Serialized Variables

        [SerializeField] private UnityEvent tapAction;
        [SerializeField] private UnityEvent holdAction;
        [SerializeField] private UnityEvent releaseAction;

        #endregion

        #region Control Loop

        private void Update()
        {

            
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                isButton = true;
                dead = climbingSystem.Dead();
                tapAction.Invoke();
            }
#else
            if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
            {
                if (!EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId))
                {
                    isButton = true;
                    dead = false;
                    tapAction.Invoke();
                }
            }
#endif
            if (Input.GetMouseButton(0))
            {
                if (dead) return;
                holdAction.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                releaseAction.Invoke();
                isButton = false;
                dead = false;
            }
        }

        #endregion

        #region Debug

        public void DebugHold()
        {
            Debug.Log("Hold invoked!");
        }

        public void DebugRelease()
        {
            Debug.Log("Release invoked!");
        }

        #endregion
    }
}