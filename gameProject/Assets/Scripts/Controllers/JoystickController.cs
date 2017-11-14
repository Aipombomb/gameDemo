using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoystickController : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

	private Image bgImg;
	private Image joystickImg;
	private Vector3 inputVector;

	private void Start() {
		bgImg = GetComponent<Image> ();
		joystickImg = transform.GetChild (0).GetComponent<Image> ();
	}

	public virtual void OnDrag(PointerEventData ped) {
		Vector2 pos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle (bgImg.rectTransform, 
			   ped.position, ped.pressEventCamera, out pos)) {
			pos.x = (pos.x / bgImg.rectTransform.sizeDelta.x);
			pos.y = (pos.y / bgImg.rectTransform.sizeDelta.y);

			float x = (bgImg.rectTransform.pivot.x == 1) ? pos.x * 2 + 1 : pos.x * 2 - 1;
			float y = (bgImg.rectTransform.pivot.y == 1) ? pos.y * 2 + 1 : pos.y * 2 - 1;

			inputVector = new Vector3 (x, 0, y);
			inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

			// Move Joystick image
			joystickImg.rectTransform.anchoredPosition = 
				new Vector3(inputVector.x * (bgImg.rectTransform.sizeDelta.x/3),
					inputVector.z * (bgImg.rectTransform.sizeDelta.y/3));
		}
	}

	public virtual void OnPointerDown(PointerEventData ped) {
		OnDrag (ped);
	}

	public virtual void OnPointerUp(PointerEventData ped) {
		inputVector = Vector3.zero;
		joystickImg.rectTransform.anchoredPosition = Vector3.zero;
	}

	public float Horizontal() {
		if (inputVector.x != 0) {
			return inputVector.x;
		} else {
			return Input.GetAxis ("Horizontal");
		}
	}
}
