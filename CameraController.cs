using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	KeyboardInput userKeyInput = new KeyboardInput();
	VehicleModel camera;

	// Use this for initialization
	void Start () {
		camera = new DifferentialVehicleModel(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {

		camera.updateModel(userKeyInput.getOutput());
	}
}
