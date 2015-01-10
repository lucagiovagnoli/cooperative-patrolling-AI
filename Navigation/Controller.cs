using UnityEngine;
using System.Collections;

public struct TargetedControllerResult{
	public Vector3 direction;
	public float velocity;
}

public interface TargetedController{
	bool computeTargetedControl(Vector3 currentOrientation,Vector3 startingPosition, Vector3 targetPosition);
	TargetedControllerResult getOutput();
}

public class TargetedInputForKinematicModel:TargetedController{
	
	private TargetedControllerResult result = new TargetedControllerResult();

	public bool computeTargetedControl(Vector3 currentOrientation, Vector3 startingPosition, Vector3 targetPosition){

		Vector3 distanceToCover = targetPosition - startingPosition; distanceToCover.y =0;
		result.direction = distanceToCover;
		if(distanceToCover.sqrMagnitude< MyUtils.epsilonMinDistance) {
			result.velocity = 0.0f;
			return false;
		}

		/** CALCULATE NEEDED VELOCITY TOWARDS THE TARGET**/
		float neededVelocity = distanceToCover.sqrMagnitude / Time.deltaTime;
		result.velocity = MyUtils.myClampVelocity(neededVelocity);
		return true;

	}
	public TargetedControllerResult getOutput() {return result;}

}

public class TargetedControllerForDifferentialModel:TargetedController{

	private	TargetedControllerResult result = new TargetedControllerResult();

	public bool computeTargetedControl(Vector3 currentOrientation,Vector3 startingPosition,Vector3 targetPosition){

		Vector3 distanceToCover = targetPosition - startingPosition;
		distanceToCover.y = 0;

		/* if need no movement return */
		if(distanceToCover.sqrMagnitude < MyUtils.epsilonMinDistance) {
			result.direction = currentOrientation;
			result.velocity=0.0f;
			return false; 
		}

		/* if there's need to rotate, then rotate and return */
		if(Vector3.Angle(currentOrientation,distanceToCover) > MyUtils.epsilonMinDistance){
			result.direction = Vector3.Slerp(currentOrientation, distanceToCover, MyUtils.slerpRotationSpeed); // interpolates between the two vectors
			result.velocity=0.0f;
			return true;
		}
		/* no need to rotate, let's compute the velocity */
		result.direction = currentOrientation;
		float neededVelocityCar = distanceToCover.sqrMagnitude / Time.deltaTime;
		result.velocity = MyUtils.myClampVelocity(neededVelocityCar);
		return true;
	}
	
	public TargetedControllerResult getOutput() {return result;}
}

public class TargetedControllerForCarlModel:TargetedController{
	
	private	TargetedControllerResult result = new TargetedControllerResult();

	public bool computeTargetedControl(Vector3 currentOrientation,Vector3 startingPosition,Vector3 targetPosition){
		
		Vector3 distanceToCover = targetPosition - startingPosition;
		distanceToCover.y = 0;

		/* if need no movement return */
		if(distanceToCover.sqrMagnitude < MyUtils.epsilonMinDistance) {
			result.direction = currentOrientation; //no rotation
			result.velocity = 0.0f; //no velocity
			return false;
		}

		/* rotate only while speeding */
		result.direction = Vector3.Slerp(currentOrientation, distanceToCover, MyUtils.slerpRotationSpeed); // interpolates between the two vectors
		float neededVelocityCar = distanceToCover.sqrMagnitude / Time.deltaTime;
		result.velocity = MyUtils.myClampVelocity(neededVelocityCar);
		return true;
	}
	
	public TargetedControllerResult getOutput() {return result;}
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
			if (MyUtils.pavementCollider.Raycast(ray, out hitPoint,Mathf.Infinity)){
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
	Vector2 input = new Vector2(0,0);
	
	public Vector2 getOutput(){
		input.x = Input.GetAxis ("Horizontal");
		input.y = Input.GetAxis ("Vertical");
		return input;
	}
}


