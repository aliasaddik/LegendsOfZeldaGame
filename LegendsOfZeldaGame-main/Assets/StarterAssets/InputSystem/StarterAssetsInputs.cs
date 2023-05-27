using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool shoot;
		public bool rune;
		public bool freeze;
		public bool bomb;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			//Debug.Log(value.Get<Vector2>());
			//if(cursorInputForLook)
			//{
			LookInput(value.Get<Vector2>());
			//}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}

		public void OnBomb(InputValue value)
		{
			BombInput(value.isPressed);
		}

		public void OnRune(InputValue value)
		{
			RuneInput(value.isPressed);
		}

		public void OnFreeze(InputValue value)
		{
			FreezeInput(value.isPressed);
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

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}
		public void RuneInput(bool newState)
		{
			rune = newState;
		}

		public void BombInput(bool newRuneState)
		{
			bomb = newRuneState;
			////Debug.Log("Bombing ");
			RunesAbilities_shrine.bombAbility = true;
			RunesAbilities_shrine.freezeAbility = false;
		}

		public void FreezeInput(bool newFreezeState)
		{
			freeze = newFreezeState;
			//Debug.Log("Freezing ");
			RunesAbilities_shrine.freezeAbility = true;
			RunesAbilities_shrine.bombAbility = false;


		}
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}