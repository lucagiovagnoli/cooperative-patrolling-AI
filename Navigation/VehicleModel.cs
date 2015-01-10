using UnityEngine;
using System.Collections;

public enum modelType {
	kinematic,
	differentialDrive,
	carModel
}

public interface VehicleModel{
	bool updateModel(Vector3 to); // returns true if model updated, false otherwise (arrived)
	void updateModel(Vector2 keyboardInput); // update model by keyboardInput
	GameObject getVehicle();
}

public class KinematicVehicleModel:VehicleModel{
	GameObject vehicle;
	private TargetedInputForKinematicModel controller;

	public KinematicVehicleModel(GameObject vehicle){
		this.vehicle = vehicle;
		this.vehicle.rigidbody.useGravity = false;
		controller = new TargetedInputForKinematicModel();
	}

	/* update model using controller for navigating to "to". return true if inProgress*/
	public bool updateModel(Vector3 to){	
		bool inProgress = controller.computeTargetedControl(vehicle.transform.forward,vehicle.transform.position,to);
		TargetedControllerResult res = controller.getOutput();
		vehicle.rigidbody.velocity = Vector3.ClampMagnitude(res.direction,res.velocity)*MyUtils.movementK*Time.deltaTime; 

		return inProgress;
	}
	/* update model with Keyboard Input */
	public void updateModel(Vector2 keyboardInput){}

	public GameObject getVehicle(){return vehicle;}
}

public class DifferentialVehicleModel:VehicleModel {
	
	//	private float theta; 
	public GameObject vehicle;
	private TargetedControllerForDifferentialModel controller;
	
	public DifferentialVehicleModel(GameObject gameObject){
		this.vehicle = gameObject;
		this.vehicle.rigidbody.useGravity = false;
		controller = new TargetedControllerForDifferentialModel();
	//	theta = vehicle.transform.rotation.eulerAngles.y;
	}
	
	/* update model with Targeted Input */
	public bool updateModel(Vector3 to){
		bool inProgress = controller.computeTargetedControl(vehicle.transform.forward,vehicle.transform.position,to);
		TargetedControllerResult res = controller.getOutput();

		vehicle.transform.rotation = Quaternion.LookRotation(res.direction);
		vehicle.rigidbody.velocity = res.velocity*vehicle.transform.forward*MyUtils.movementK * Time.deltaTime;

		return inProgress;
	}

	/* update model with Keyboard Input */
	public void updateModel(Vector2 keyboardInput){
		float omega;
		float v;

		vehicle.rigidbody.useGravity = false;
		omega = 4*keyboardInput.x;
		v = 4*keyboardInput.y;
		//theta += omega*Time.deltaTime;
		vehicle.rigidbody.angularVelocity = new Vector3(0.0f,omega,0.0f);
		//vehicle.rigidbody.velocity = new Vector3(v*Mathf.Sin(theta),0.0f,v*Mathf.Cos(theta));
		vehicle.rigidbody.velocity = v*vehicle.transform.forward*MyUtils.movementK*Time.deltaTime;
	}

	public GameObject getVehicle(){return vehicle;}
}

public class CarVehicleModel:VehicleModel {
	//	private float theta; 
	//private float L = 0.3f; 
	public GameObject vehicle;
	private TargetedControllerForCarlModel controller;

	public CarVehicleModel(GameObject gameObject){
		this.vehicle = gameObject;
		this.vehicle.rigidbody.useGravity = false;
		controller = new TargetedControllerForCarlModel();
		//	theta = vehicle.transform.rotation.eulerAngles.y;
	}

	public bool updateModel(Vector3 to){
		bool inProgress = controller.computeTargetedControl(vehicle.transform.forward,vehicle.transform.position,to);
		TargetedControllerResult res = controller.getOutput();

		vehicle.transform.rotation = Quaternion.LookRotation(res.direction);
		vehicle.rigidbody.velocity = res.velocity*vehicle.transform.forward*MyUtils.movementK * Time.deltaTime;

		return inProgress;
	}

	/* update model with Keyboard Input */
	public void updateModel(Vector2 keyboardInput){
		/*		vehicle.rigidbody.useGravity = false;
			phi = userInput.x;
			v = userInput.z;
			omega = v/L * Mathf.Tan(phi);
			//theta += omega*Time.deltaTime;
			vehicle.rigidbody.angularVelocity = new Vector3(0.0f,omega,0.0f);
			//vehicle.rigidbody.velocity = new Vector3 (v*Mathf.Sin(theta), 0.0f, v*Mathf.Cos(theta));
			vehicle.rigidbody.velocity = v*vehicle.transform.forward*movK*Time.deltaTime;
			break;	*/	
	}


	public GameObject getVehicle(){return vehicle;}
}

