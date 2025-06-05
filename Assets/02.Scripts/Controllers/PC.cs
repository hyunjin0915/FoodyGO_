using UnityEngine;
using UnityEngine.InputSystem;

namespace FoodyGo.Controllers
{
    public class PC : MonoBehaviour
    {

        public Vector3 velocity;

        [SerializeField] InputActionReference _moveInputAction;

        private void OnEnable()
        {
            _moveInputAction.action.performed += OnMovePerformed;
            _moveInputAction.action.canceled += OnMoveCanceled;
            _moveInputAction.action.Enable();
        }

        private void OnDisable()
        {
            _moveInputAction.action.performed -= OnMovePerformed;
            _moveInputAction.action.canceled -= OnMoveCanceled;
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            Vector2 input2D = context.ReadValue<Vector2>();
            velocity = new Vector3(input2D.x, 0f, input2D.y);
        }
        
        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            velocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            //if ((velocity)
            //{
                
            //}
        }
    }

}
