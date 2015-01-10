using UnityEngine;
using System.Collections;

public class TargetedInput{

	Vector3 result = new Vector3(0,0,0);
	private Vector3 targetPosition;
	private VehicleModel model;

	public TargetedInput(VehicleModel model){ 
		this.model = model;
	}
	public void setTarget(Vector3 targetPosition){
		this.targetPosition = targetPosition;
	}

	public void computeInputTowardsTargetFrom(Vector3 startingPosition){

		Vector3 distanceToCover = targetPosition - startingPosition;
		Vector3 neededVelocity;

		switch(model.getModel()){
		case (modelType.kinematic):
			/** CALCULATE NEEDED VELOCITY TOWARDS THE TARGET**/
			Debug.DrawLine (startingPosition, targetPosition, Color.blue, 1.0f);
			neededVelocity = distanceToCover / Time.deltaTime;
			if (neededVelocity.magnitude > model.maxVelocity) result = Vector3.ClampMagnitude(distanceToCover,model.maxVelocity);
			else result = neededVelocity;
			break;
		default:
			break;
		}
	}

	public Vector3 getOutput(){
		return result;
	}
}

public struct DiffControllerResult{
	public Vector3 direction;
	public float velocity;
}

public class TargetedInputDifferentialDrive{

	private	DiffControllerResult result = new DiffControllerResult();
	private Vector3 targetPosition;
	private VehicleModel model;

	public TargetedInputDifferentialDrive(VehicleModel model){ 
		this.model = model;
	}
	public void setTarget(Vector3 targetPosition){
		this.targetPosition = targetPosition;
		this.targetPosition.y = 0;
	}
	
	public void computeInputTowardsTargetFrom(Vector3 startingPosition){

		Vector3 distanceToCover = targetPosition - startingPosition;
		distanceToCover.y = 0;
		if(distanceToCover==Vector3.zero) {
			result.direction = Vector3.zero;
			result.velocity=0.0f;
			return; // if need no movement return
		}

		/* if there's need to rotate, then rotate */
		if(Vector3.Angle(model.vehicle.transform.forward,distanceToCover) != 0.0f){
			result.direction = Vector3.Slerp(model.vehicle.transform.forward, distanceToCover, 0.1f); // interpolates between the two vectors
			result.velocity=0.0f;
			return;
		}
		else{ /* no need to rotate, let's compute the velocity */
			result.direction = Vector3.zero;
			float neededVelocityCar = distanceToCover.magnitude / Time.deltaTime;
			if (neededVelocityCar > model.maxVelocity) result.velocity = model.maxVelocity;
			else result.velocity = neededVelocityCar;
		}
	}
	
	public DiffControllerResult getOutput(){
		return result;
	}
}

public class MouseInput{
	Vector3 target;
	public enum clickType{
		left, 	// 0 is left click 
		right, 	// 1 is right click
		middle 	// 2 is middle click 
	}
	private clickType click;
	public MouseInput(clickType click){ this.click = click;}

	public bool isMouseInputReady(){
		if (Input.GetMouseButtonDown ((int)click)) { 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitPoint; //hitPoint ready to be filled with the info about where the ray hit the collider of the plane
			if (Physics.Raycast(ray, out hitPoint)){
				target = hitPoint.point;
				//Debug.Log ("Pressed mouse key - Coord x,y,z:" + target.x + ","+ target.y +"," + target.z);
				return true;
			}
		}
		return false;
	}

	public Vector3 getOutput(){
		return target;
	}
}

public class KeyboardInput {
	Vector3 input = new Vector3(0,0,0);
	
	public Vector3 getOutput(){
		input.x = Input.GetAxis ("Horizontal");
		input.z = Input.GetAxis ("Vertical");
		return input;
	}
}


