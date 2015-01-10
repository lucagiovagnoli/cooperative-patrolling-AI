using UnityEngine;
using System.Collections;

public class LeaderScript : MonoBehaviour {

	public modelType model;
	private VehicleModel vehicleModel;
	private KeyboardInput userInput;
	private TargetedInputDifferentialDrive targInput;
	MouseInput mouse = new MouseInput(MouseInput.clickType.left);
	
	void Start(){
		userInput = new KeyboardInput (); 
		vehicleModel = new VehicleModel (this.gameObject, model);
		targInput = new TargetedInputDifferentialDrive (vehicleModel); //start the targeted controller
		targInput.setTarget (this.gameObject.transform.position);
	}

	void FixedUpdate(){
	/*	Vector3 u = userInput.getOutput ();
		vehicleModel.updateModel (u); */

		if (mouse.isMouseInputReady () == true) {
			targInput.setTarget (mouse.getOutput ()); // set the target for the movement to the first A* point
		}
		targInput.computeInputTowardsTargetFrom (this.gameObject.transform.position);
		DiffControllerResult result = targInput.getOutput();
		vehicleModel.UpdateModelTargetedInput (result.direction,result.velocity); 
	}


}
