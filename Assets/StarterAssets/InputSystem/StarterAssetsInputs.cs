using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool fpp_mode;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;
		public bool reload;
		public bool change_gun;
		public bool change_sholder;
		public bool buy_button;


		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}
		public void OnFpp(InputValue value)
		{
			fppInput(value.isPressed);
		}
		public void OnBuy(InputValue value)
		{
			BuyInput(value.isPressed);
		}
		public void OnChangegun(InputValue value)
		{
			ChangeInput(value.isPressed);
		}
		public void OnChangeshoulder(InputValue value)
		{
			ShoulderInput(value.isPressed);
		}
		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}
		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}
		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}
		public void fppInput(bool newfppState)
		{
			fpp_mode = newfppState;
		}
		public void BuyInput(bool newBuyState)
		{
			buy_button = newBuyState;
		}
		public void ShoulderInput(bool newShoulderState)
		{
			change_sholder = newShoulderState;
		}
		public void ChangeInput(bool newChangegunState)
		{
			change_gun = newChangegunState;
		}
		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}
		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}
		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			//Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}