using UnityEngine;
using System.Collections;

public class Intruder : MonoBehaviour {

	private MouseInput mouse = new MouseInput(MouseInput.clickType.left);
	private VehicleModel vehicleModel;
	private Vector3 targetPosition;

	void Start(){
		vehicleModel = new CarVehicleModel (this.gameObject);
		targetPosition = gameObject.transform.position;
	}

	void FixedUpdate(){

		if (mouse.isMouseInputReady () == true) {
			targetPosition = mouse.getOutput();
		}
		vehicleModel.updateModel (targetPosition); 
	}

}
