using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunTemple
{


	public class Door : MonoBehaviour
	{
		public bool IsLocked = false;
		public bool DoorClosed = true;
		public float OpenRotationAmount = 90;
		public float RotationSpeed = 1f;
		public float MaxDistance = 3.0f;
		public string playerTag = "Player";
		private Collider DoorCollider;

		public GameObject Player;
		public Camera Cam;
		private CursorManager cursor;

		Vector3 StartRotation;
		float StartAngle = 0;
		float EndAngle = 0;
		float LerpTime = 1f;
		float CurrentLerpTime = 0;
		bool Rotating = false;


		private bool scriptIsEnabled = true;



		void Start()
		{
			Debug.Log("Playerr");
			Debug.Log(Player);
			StartRotation = transform.localEulerAngles;
			DoorCollider = GetComponent<BoxCollider>();

			if (!DoorCollider)
			{
				Debug.LogWarning(this.GetType().Name + ".cs on " + gameObject.name + "door has no collider", gameObject);
				scriptIsEnabled = false;
				return;
			}
			if (Player == null)
			{
				Player = GameObject.FindGameObjectsWithTag("Player")[0];
			}
			//if (Cam == null)
			//{
			//    Cam = GameObject.FindGameObjectsWithTag("MainCamera")[0];
			//}

			if (!Player)
			{
				Debug.LogWarning(this.GetType().Name + ".cs on " + this.name + ", No object tagged with " + playerTag + " found in Scene", gameObject);
				scriptIsEnabled = false;
				return;
			}


			//if (!Cam) {
			//	Debug.LogWarning (this.GetType ().Name + ", No objects tagged with MainCamera in Scene", gameObject);
			//	scriptIsEnabled = false;
			//}

			cursor = CursorManager.instance;

			if (cursor != null)
			{
				cursor.SetCursorToDefault();
			}
			Debug.Log(scriptIsEnabled);

			//Activate();
		}



		void Update()
		{
			if (Input.GetKeyDown(KeyCode.O))
			{
				if (DoorClosed)
				{
					Open();
				}
				else
				{
					Close();
					Rotate();
				}
			}

			if (Rotating)
			{
				Rotate();
			}

			//Debug.Log(scriptIsEnabled);
			//if (scriptIsEnabled) {
			//	Debug.Log("try to open");
			//	if (Rotating) {
			//		Rotate ();
			//		DoorCollider.enabled = false;
			//	}

			//	if (Input.GetKeyDown (KeyCode.O)) {
			//		Debug.Log("Trying to open");
			//		TryToOpen ();
			//	}


			//	if (cursor != null) {
			//		CursorHint ();
			//	}
			//}


		}




		void TryToOpen()
		{
			if (Mathf.Abs(Vector3.Distance(transform.position, Player.transform.position)) <= MaxDistance)
			{
				if (IsLocked == false)
				{
					Activate();
				}
				//Ray ray = Cam.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));
				//RaycastHit hit;

				//if (DoorCollider.Raycast(ray, out hit, MaxDistance)){					
				//	if (IsLocked == false){
				//		Activate ();
				//	}
				//}
			}
		}



		void CursorHint()
		{
			//if (Mathf.Abs(Vector3.Distance(transform.position, Player.transform.position)) <= MaxDistance){	
			//	Ray ray = Cam.ScreenPointToRay (new Vector3 (Screen.width / 2, Screen.height / 2, 0));
			//	RaycastHit hit;

			//	if (DoorCollider.Raycast (ray, out hit, MaxDistance)) {
			//		if (IsLocked == false) {
			//			cursor.SetCursorToDoor ();
			//		} else if (IsLocked == true) {
			//			cursor.SetCursorToLocked ();
			//		}					
			//	} else {
			//		cursor.SetCursorToDefault ();
			//	}
			//}
		}




		public void Activate()
		{
			Debug.Log(DoorClosed);
			if (DoorClosed)
				Open();
			else
				Close();
		}







		void Rotate()
		{
			//CurrentLerpTime += Time.deltaTime * RotationSpeed;
			//if (CurrentLerpTime > LerpTime)
			//{
			//    CurrentLerpTime = LerpTime;
			//}

			//float _Perc = CurrentLerpTime / LerpTime;

			//float _Angle = CircularLerp.Clerp(StartAngle, EndAngle, _Perc);
			transform.localEulerAngles = new Vector3(transform.eulerAngles.x, OpenRotationAmount, transform.eulerAngles.z);

			//if (CurrentLerpTime == LerpTime) {
			//	Rotating = false;
			//	DoorCollider.enabled = true;
			//}
			Rotating = false;
			DoorCollider.enabled = false;


		}



		void Open()
		{
			Debug.Log("Door Opened");
			DoorCollider.enabled = false;
			DoorClosed = false;
			StartAngle = transform.localEulerAngles.y;
			EndAngle = StartRotation.y + OpenRotationAmount;
			CurrentLerpTime = 0;
			Rotating = true;
		}



		void Close()
		{
			DoorCollider.enabled = false;
			DoorClosed = true;
			StartAngle = transform.localEulerAngles.y;
			EndAngle = transform.localEulerAngles.y - OpenRotationAmount;
			CurrentLerpTime = 0;
			Rotating = true;
		}

	}
}