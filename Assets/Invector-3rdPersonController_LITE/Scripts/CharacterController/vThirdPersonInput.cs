using UnityEngine;
using Unity.Netcode;
using StarterAssets;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : NetworkBehaviour
    {
        public StarterAssetsInputs new_inputs;
        #region Variables       

        [Header("Controller Input")]
        public string horizontalInput = "Horizontal";
        public string verticallInput = "Vertical";
        public KeyCode jumpInput = KeyCode.Space;
        public KeyCode strafeInput = KeyCode.Tab;
        public KeyCode sprintInput = KeyCode.LeftShift;

        [Header("Camera Input")]
        public string rotateCameraXInput = "Mouse X";
        public string rotateCameraYInput = "Mouse Y";

        [HideInInspector] public vThirdPersonController cc;
         public vThirdPersonCamera tpCamera;
         public Camera cameraMain;
         public LayerMask mousecollidermask;
         public Transform debug_transform;
         public Player_Health p_health;

        #endregion

        protected virtual void Start()
        {
            if(!IsOwner)return;
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void Update()
        {
            if(!IsOwner)return;
            if(p_health.Is_alive.Value)
            {

            
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.UpdateAnimator();
             InputHandle();   
             //Was in fixedUpdate before i did keera           
             cc.ControlRotationType();    // update the input methods
           
             
            Vector2 screenCenterPoint = new Vector2(Screen.width/2f,Screen.height/2f);
            Ray ray = cameraMain.ScreenPointToRay(screenCenterPoint);
            if(Physics.Raycast(ray,out RaycastHit hit,999f, mousecollidermask))
            {
                debug_transform.position = hit.point;

                cc.Aim_Spot(hit);
            }               // handle the controller rotation type
            }
        }

        protected virtual void fixedUpdate()
        {
            if(!IsOwner)return;
            if(p_health.Is_alive.Value)
            {
            
            //  cc.ControlRotationType();  
            }
            
            // updates the Animator Parameters
            
        }

        public virtual void OnAnimatorMove()
        {
            if(!IsOwner)return;
            if(p_health.Is_alive.Value)
            {
            cc.ControlAnimatorRootMotion();
            } // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
             if(!IsOwner)return;
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
             if(!IsOwner)return;
             if(p_health.Is_alive.Value)
            {
            MoveInput();
            CameraInput();
            SprintInput();
            StrafeInput();
            JumpInput();
           
            }
        }

        public virtual void MoveInput()
        {
             if(!IsOwner)return;
          //  cc.input.x = Input.GetAxis(horizontalInput);
            //cc.input.z = Input.GetAxis(verticallInput);
            

            cc.input.x = new_inputs.move.x;
             cc.input.z = new_inputs.move.y;
        }

        protected virtual void CameraInput()
        {
             if(!IsOwner)return;
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

           // var Y = Input.GetAxis(rotateCameraYInput);
           // var X = Input.GetAxis(rotateCameraXInput);
           var Y = -new_inputs.look.y;
             var X = new_inputs.look.x;

            tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
             if(!IsOwner)return;
            if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
             if(!IsOwner)return;
            if (new_inputs.sprint)
                cc.Sprint(true);
            else if (!new_inputs.sprint)
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
             if(!IsOwner)return;
            if (new_inputs.jump && JumpConditions())
                cc.Jump();
        }

        #endregion       
    }
}