using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	KeyboardInput userKeyInput = new KeyboardInput();

	void Start () {
	}
	
	void Update () {

		this.rigidbody.velocity = this.transform.forward*200* Input.GetAxis("Mouse ScrollWheel");
		Vector3 newPosition = this.transform.position + 15*userKeyInput.getOutput()*Time.deltaTime;

		if(newPosition.x>ConvexCover.upperLeftCorner.x && newPosition.x<ConvexCover.lowestRightCorner.x &&
		   newPosition.z<ConvexCover.upperLeftCorner.z && newPosition.z>ConvexCover.lowestRightCorner.z){
			this.transform.position = newPosition;
		}

	}
}
