using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class PlayerController : MonoBehaviour {

	Rigidbody playerCollider;
	GameObject mainObject;
	float movementSpeed = 6f;
	//Rigidbody rigidBodyPlayer;
	public JoystickController joystick;

	private CharacterController characterController;
	private float gravity = 18f;
	private float jumpForce = 10f;
	private float verticalVelocity;
	private Vector3 moveVector;
	private Vector3 direction;
	private Vector3 lastMove;
	private bool jumpButtonPressed;

	// Use this for initialization
	void Start () {
		playerCollider = GetComponent<Rigidbody> ();
		characterController = GetComponent<CharacterController>();
	}

	void Update() {
		Move();
	}

	void Move() {
		moveVector = Vector3.zero;
		direction = Vector3.zero;
		direction.x = joystick.Horizontal();

		if (direction.magnitude > 1)
			direction.Normalize ();

		moveVector.x = direction.x;
		moveVector.z = 0; // not moving in z axis for now

		if (characterController.isGrounded) {
			verticalVelocity = -gravity * Time.deltaTime;
			if (jumpButtonPressed || Input.GetKeyDown(KeyCode.Space)) {
				verticalVelocity = jumpForce;
			}
		}
		else {
			verticalVelocity -= gravity * Time.deltaTime; 
			moveVector = lastMove;
		}

		moveVector.y = 0;
		moveVector.Normalize();
		moveVector *= movementSpeed;
		moveVector.y = verticalVelocity; // This is what's causing the player to jump when moving towards enemy
		characterController.Move(moveVector * Time.deltaTime);
		lastMove = moveVector;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit) {
		//hit.normal.y is similar to the slope
		if (!characterController.isGrounded && hit.normal.y < 0.1f) {
			if (jumpButtonPressed || Input.GetKeyDown (KeyCode.Space)) {
				verticalVelocity = jumpForce;
				moveVector = hit.normal * movementSpeed;
			}
		}
	}
		
	public void OnJumpButton() {
		jumpButtonPressed = true;
		Invoke ("ReleaseJumpButton", .01f);
	}

	private void ReleaseJumpButton() {
		jumpButtonPressed = false;
		Debug.Log ("Released");
	}
}
