using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraController : MonoBehaviour {

	public float CameraDistanceUp { get; set; }
	public float CameraDistanceAway { get; set; }
	public Transform cameraTarget;

	Camera playerCamera;
	float zoomSpeed = 35f;
	float mobileZoomSpeed = .5f;

	void Start() {
		CameraDistanceUp = 7f;
		CameraDistanceAway = 10f;
		playerCamera = GetComponent<Camera>();
		playerCamera.fieldOfView = 50f;
	}

	void Update() {
		// Change to finger pinch?
		/*
		if(Input.GetAxisRaw("Mouse ScrollWheel") != 0) {
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			playerCamera.fieldOfView -= scroll * zoomSpeed;
			playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, 25, 110);
		}
		else if (Input.touchCount == 2) {
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

			playerCamera.fieldOfView += deltaMagnitudeDiff * mobileZoomSpeed;
			playerCamera.fieldOfView = Mathf.Clamp(playerCamera.fieldOfView, 25, 75);
		}*/
		transform.position = new Vector3(cameraTarget.position.x, 
			CameraDistanceUp, cameraTarget.position.z - CameraDistanceAway);
	}

}
