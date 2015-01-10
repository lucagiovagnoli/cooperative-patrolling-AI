using UnityEngine;
using System.Collections;

public enum modelType {
	kinematic,
	differentialDrive,
	carModel
}

public class VehicleModel {
	
	private modelType model;	
	public float maxVelocity = 2.0f;
	public float movK = 20.0f;
	//	private float theta; 
	private float phi;
	private float L = 0.3f; 
	public GameObject vehicle;

	public VehicleModel(GameObject gameObject, modelType modelPar){
		vehicle = gameObject;
		model = modelPar;
	//	theta = vehicle.transform.rotation.eulerAngles.y;
	}

	public void setModel(modelType mType) {
		model = mType;
	}
	public modelType getModel() {
		return model;
	}
	public Quaternion getCurrentOrientation(){return vehicle.transform.rotation;}

	public void updateModel(Vector3 userInput){
		float omega;
		float v;

		switch (model) {
		case modelType.kinematic: 
			vehicle.rigidbody.useGravity = false;
			vehicle.rigidbody.velocity = userInput*movK*Time.deltaTime; // movement should be a VELOCITY
			//vehicle.transform.rotation = Quaternion.LookRotation(movement);
			break;
		case modelType.differentialDrive: 
			vehicle.rigidbody.useGravity = false;
			omega = 4*userInput.x;
			v = userInput.z;
			//theta += omega*Time.deltaTime;
			vehicle.rigidbody.angularVelocity = new Vector3(0.0f,omega,0.0f);
			//vehicle.rigidbody.velocity = new Vector3(v*Mathf.Sin(theta),0.0f,v*Mathf.Cos(theta));
			vehicle.rigidbody.velocity = v*vehicle.transform.forward*movK*Time.deltaTime;
			break;
		case modelType.carModel: 
			vehicle.rigidbody.useGravity = false;
			phi = userInput.x;
			v = userInput.z;
			omega = v/L * Mathf.Tan(phi);
			//theta += omega*Time.deltaTime;
			vehicle.rigidbody.angularVelocity = new Vector3(0.0f,omega,0.0f);
			//vehicle.rigidbody.velocity = new Vector3 (v*Mathf.Sin(theta), 0.0f, v*Mathf.Cos(theta));
			vehicle.rigidbody.velocity = v*vehicle.transform.forward*movK*Time.deltaTime;
			break;		
		default: 
			break;
		}
	}

	public void UpdateModelTargetedInput(Vector3 direction,float velocity){

		switch (model) {
		case modelType.differentialDrive: 
			if(direction != Vector3.zero){
				vehicle.transform.rotation = Quaternion.LookRotation(direction);
			}
			vehicle.rigidbody.velocity = velocity*vehicle.transform.forward*movK*Time.deltaTime;
			break;
		}
	}

		
	public Vector3 getCurrentVelocity(){return this.vehicle.rigidbody.velocity;}

}
