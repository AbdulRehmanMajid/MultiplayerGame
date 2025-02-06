using UnityEngine;

namespace StarterAssets
{
    public class UICanvasControllerInput : MonoBehaviour
    {
        public bool sprint_enabled = false;
        bool has_pressed_sprint_once;
       

        [Header("Output")]
        public StarterAssetsInputs starterAssetsInputs;

        public void VirtualMoveInput(Vector2 virtualMoveDirection)
        {
            starterAssetsInputs.MoveInput(virtualMoveDirection);
        }

        public void VirtualLookInput(Vector2 virtualLookDirection)
        {
            starterAssetsInputs.LookInput(virtualLookDirection);
        }

        public void VirtualJumpInput(bool virtualJumpState)
        {
            starterAssetsInputs.JumpInput(virtualJumpState);
        }

        public void VirtualSprintInput(bool state)
        {
           if(state = true && !has_pressed_sprint_once)
           {
                has_pressed_sprint_once = true;
                sprint_enabled = !sprint_enabled;
            
            
           }
           if(!state && has_pressed_sprint_once)
           {
            has_pressed_sprint_once = false;
           }
            
            
        }
        void Update()
        {
            starterAssetsInputs.SprintInput(sprint_enabled);
        }
        public void VirtualFireInput(bool VirtualFireInput)
        {
             starterAssetsInputs.ShootInput(VirtualFireInput);

        }
        public void VirtualBuyInput(bool VirtualBuyInput)
        {
             starterAssetsInputs.BuyInput(VirtualBuyInput);
        }
        public void VirtualReloadInput(bool VirtualReloadInput)
        {
             starterAssetsInputs.ReloadInput(VirtualReloadInput);
        }
        public void VirutalChangeGunInput(bool VirutalChangeGunInput)
        {
             starterAssetsInputs.ChangeInput(VirutalChangeGunInput);
        }
        public void VirutalFppMode(bool virtualFppmode)
        {
            starterAssetsInputs.fppInput(virtualFppmode);
        }
        public void VirutalAim(bool state)
        {
            starterAssetsInputs.AimInput(state);
        }
        
    }

}
