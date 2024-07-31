using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Unity.FPS.Gameplay
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;

        [Tooltip("Additional sensitivity multiplier for WebGL")]
        public float WebglLookSensitivityMultiplier = 0.25f;

        [Tooltip("Limit to consider an input when using a trigger on a controller")]
        public float TriggerAxisThreshold = 0.4f;

        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;

        GameFlowManager m_GameFlowManager;
        PlayerCharacterController m_PlayerCharacterController;
        bool m_FireInputWasHeld;

        private Vector3 _move;
        private Vector2 _look;

        private bool _actualJumpDown;

        private bool _jumpDown
        {
            get
            {
                if (!_actualJumpDown) return false;
                _actualJumpDown = false;
                return true;
            }
            set => _actualJumpDown = value;
        }

        private bool _jumpHeld;
        private bool _fireStart;
        private bool _fireEnd;
        private bool _fireProcess;
        private bool _aimHeld;
        private bool _sprintHeld;
        private bool _crouchStart;
        private bool _crouchEnd;
        private bool _reload;
        private int _weaponSwitchInput;
        private int _weaponSwitchAxisInput;

        void Start()
        {
            m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerCharacterController, PlayerInputHandler>(
                m_PlayerCharacterController, this, gameObject);
            m_GameFlowManager = FindObjectOfType<GameFlowManager>();
            DebugUtility.HandleErrorIfNullFindObject<GameFlowManager, PlayerInputHandler>(m_GameFlowManager, this);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            m_FireInputWasHeld = GetFireInputHeld();
        }

        public bool CanProcessInput()
        {
            // Cursor.lockState == CursorLockMode.Locked && 
            if (m_GameFlowManager == null) return false;
            return !m_GameFlowManager.GameIsEnding;
        }

        public void OnMoveEvent(InputAction.CallbackContext ctx)
        {
            var move = ctx.ReadValue<Vector2>();
            _move = new Vector3(move.x, 0f, move.y);
        }    
        
        public void OnLookEvent(InputAction.CallbackContext ctx)
        {
            var look = ctx.ReadValue<Vector2>();
            if (InvertYAxis)
                look.y *= -1;
            look *= LookSensitivity;
            look *= Time.deltaTime;
            _look = look;
        }   
        
        public void OnJumpEvent(InputAction.CallbackContext ctx)
        { 
            if(ctx.started) _jumpDown = true;
            _jumpHeld = ctx.performed;
        }       
        
        public void OnFireEvent(InputAction.CallbackContext ctx)
        {
            _fireStart = ctx.started;
            _fireEnd = ctx.canceled;
            _fireProcess = ctx.performed;
        }    
        
        public void OnAimEvent(InputAction.CallbackContext ctx)
        {
            _aimHeld = ctx.performed;
        }   
        
        public void OnSprintEvent(InputAction.CallbackContext ctx)
        {
            _sprintHeld = ctx.performed;
        }    
        
        public void OnCrouchEvent(InputAction.CallbackContext ctx)
        {
            _crouchStart = ctx.started;
            _crouchEnd = ctx.canceled;
        }  
        
        public void OnReloadEvent(InputAction.CallbackContext ctx)
        {
            _reload = ctx.started;
        }    
        
        public void OnSwitchWeaponAxisEvent(InputAction.CallbackContext ctx)
        {
            var x = Mathf.RoundToInt(ctx.ReadValue<Vector2>().y);
            _weaponSwitchAxisInput = x != 0 ? Mathf.RoundToInt(Mathf.Sign(x)) : 0;
        }   
        
        public void OnSwitchWeaponNextEvent(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
            {
                _weaponSwitchInput = 0;
                return;
            }
            
            _weaponSwitchInput = 1;
        }         
        
        public void OnSwitchWeaponPreviousEvent(InputAction.CallbackContext ctx)
        {
            if (!ctx.started)
            {
                _weaponSwitchInput = 0;
                return;
            }
            
            _weaponSwitchInput = -1;
        } 

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                return _move;
            }

            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return !CanProcessInput() ? 0f : _look.x;
        }

        public float GetLookInputsVertical()
        {
            return !CanProcessInput() ? 0f : _look.y;
        }  

        public bool GetJumpInputDown()
        {
            return CanProcessInput() && _jumpDown;
        }

        public bool GetJumpInputHeld()
        {
            return CanProcessInput() && _jumpHeld;
        }

        public bool GetFireInputDown()
        {
            return CanProcessInput() && _fireStart;
        }

        public bool GetFireInputReleased()
        {
            return CanProcessInput() && _fireEnd;
        }

        public bool GetFireInputHeld()
        {
            return CanProcessInput() && _fireProcess;
        }

        public bool GetAimInputHeld()
        {
            return CanProcessInput() && _aimHeld;
        }

        public bool GetSprintInputHeld()
        {
            return CanProcessInput() && _sprintHeld; 
        }

        public bool GetCrouchInputDown()
        {
            return CanProcessInput() && _crouchStart; 
        }

        public bool GetCrouchInputReleased()
        {
            return CanProcessInput() && _crouchEnd; 
        }

        public bool GetReloadButtonDown()
        {
            return CanProcessInput() && _reload; 
        }

        public int GetSwitchWeaponInput()
        {
            if (!CanProcessInput()) return 0;
            
            if (_weaponSwitchInput != 0)
            {
                return _weaponSwitchInput;
            }

            if (_weaponSwitchAxisInput != 0)
            {
                return _weaponSwitchAxisInput;
            }

            return 0;
        }

        public int GetSelectWeaponInput()
        {
            // if (CanProcessInput())
            // {
            //     if (Input.GetKeyDown(KeyCode.Alpha1))
            //         return 1;
            //     else if (Input.GetKeyDown(KeyCode.Alpha2))
            //         return 2;
            //     else if (Input.GetKeyDown(KeyCode.Alpha3))
            //         return 3;
            //     else if (Input.GetKeyDown(KeyCode.Alpha4))
            //         return 4;
            //     else if (Input.GetKeyDown(KeyCode.Alpha5))
            //         return 5;
            //     else if (Input.GetKeyDown(KeyCode.Alpha6))
            //         return 6;
            //     else if (Input.GetKeyDown(KeyCode.Alpha7))
            //         return 7;
            //     else if (Input.GetKeyDown(KeyCode.Alpha8))
            //         return 8;
            //     else if (Input.GetKeyDown(KeyCode.Alpha9))
            //         return 9;
            //     else
            //         return 0;
            // }

            return 0;
        } 
    }
}