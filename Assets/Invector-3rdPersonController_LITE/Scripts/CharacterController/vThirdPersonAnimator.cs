
using Unity.Mathematics;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class vThirdPersonAnimator : vThirdPersonMotor
    {
        #region Variables                

        public const float walkSpeed = 0.5f;
        public const float runningSpeed = 1f;
        public const float sprintSpeed = 1.5f;
        

        #endregion  

        public virtual void UpdateAnimator()
        {
             
            if (animator == null || !animator.enabled) return;
            

            
            
            animator.SetBool(vAnimatorParameters.IsGrounded, isGrounded);
            animator.SetFloat(vAnimatorParameters.GroundDistance, groundDistance);
            
            

            if (isStrafing)
            {
               /// animator.SetFloat(vAnimatorParameters.InputHorizontal, stopMove ? 0 : horizontalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                  if(verticalSpeed > 0.01f)
                  {
              ///  animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, strafeSpeed.animationSmooth, Time.deltaTime);
                  }
                   else
                  {
                //     animator.SetFloat(vAnimatorParameters.InputVertical,0.01f,0f,0f);
               ///      verticalSpeed = 0.01f;
                  }
            }
            else
            {
                  if(verticalSpeed > 0.01f)
                  {
               /// animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
               
                  }
                  else
                  {
                   ///  animator.SetFloat(vAnimatorParameters.InputVertical,0.01f,0f,0f);
                     
                     
                     verticalSpeed = 0.01f;
                  }
            }
            
            if(inputMagnitude > 0.1f)
            {
                
          //  animator.SetFloat(vAnimatorParameters.InputMagnitude, stopMove ? 0f : inputMagnitude, isStrafing ? strafeSpeed.animationSmooth : freeSpeed.animationSmooth, Time.deltaTime);
         if(new_inputs.sprint)
         {
            animator.SetInteger("Run_Speed",2);
         }
         else
         {
            animator.SetInteger("Run_Speed",1);
            

         }
            }
            else
            {
                
               /// animator.SetFloat(vAnimatorParameters.InputMagnitude,0.01f,0f,0f);
                inputMagnitude = 0.01f;
                animator.SetInteger("Run_Speed",0);
            }
          


        }

        public virtual void SetAnimatorMoveSpeed(vMovementSpeed speed)
        {
             
            Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
            verticalSpeed = relativeInput.z;
            horizontalSpeed = relativeInput.x;

            var newInput = new Vector2(verticalSpeed, horizontalSpeed);

            if (speed.walkByDefault)
            {
                float fruit = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? runningSpeed : walkSpeed);
               inputMagnitude = fruit;
                
            }
            else
            {
                float fish = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
                
                if(fish < 0)
                {
                   inputMagnitude = 0;
                }
                else
                {
                    inputMagnitude = fish;
                }

                
            }
        }
    }

    public static partial class vAnimatorParameters
    {
        public static int InputHorizontal = Animator.StringToHash("InputHorizontal");
        public static int InputVertical = Animator.StringToHash("InputVertical");
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static int IsGrounded = Animator.StringToHash("IsGrounded");
        public static int IsStrafing = Animator.StringToHash("IsStrafing");
        public static int IsSprinting = Animator.StringToHash("IsSprinting");
        public static int GroundDistance = Animator.StringToHash("GroundDistance");
    }
}