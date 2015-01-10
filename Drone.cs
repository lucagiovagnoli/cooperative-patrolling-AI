using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Drone script takes care of following the VRP route */
public class Drone : MonoBehaviour {

	private static GameObject[] intruders = null;

	private bool intruderDetected = false;
	private Vector3 intruderLastPosition;
	private bool intruderCaptured = false;

	private int d = 0;
	private LinkedList<Vector3> route; 
	private Navigation nav;
	private bool isSegmentInProgress = false;
	private bool isRouteLoaded = false;
	
	public void setRoute(LinkedList<Vector3> route){
		this.route = route;
		isRouteLoaded = true;
	}

	// Use this for initialization
	void Start (){
		this.nav = new Navigation (this.gameObject);
		intruders = GameObject.FindGameObjectsWithTag("intruder");
		SphereCollider sc =	gameObject.AddComponent("SphereCollider") as SphereCollider;
		sc.radius = MyUtils.maxRangeScanner;
		sc.isTrigger = true;
	}

	// Update is called once per frame
	void FixedUpdate (){
	
		if(intruderCaptured==true)return;
		if(intruderDetected == true) pursuitIntruder();

		else if (isRouteLoaded == true && route.Count>0) { /* if route is loaded then follow route */
			if(isSegmentInProgress==false){ /* if route still to be started, compute Astar */
				nav.computeAstar(route.First.Value);
				isSegmentInProgress = true;
			}

			/* GO TO customer using Astar */
			if(nav.goThere()==true){ /* returns true if completed, otherwise moves towards the goal*/
				route.RemoveFirst(); 
				if(route.Count==0){ /* if route is completed, init everthing to false again*/
					isSegmentInProgress = false;
					isRouteLoaded = false;
				}
				isSegmentInProgress = false;
			}
		}
		else nav.stop();
	}

	private void checkVisibility(Vector3 intruderPosition){
		Debug.Log("visibility check");
		RaycastHit hitPoint; //hitPoint ready to be filled with the info about where the ray hit the collider 
		Ray raggio = new Ray(this.transform.position+new Vector3(0,0.1f,0),intruderPosition-this.transform.position+new Vector3(0,0.1f,0));
		Debug.DrawRay(this.transform.position+new Vector3(0,0.1f,0),intruderPosition-this.transform.position+new Vector3(0,0.1f,0),Color.red);
		if(Physics.Raycast(raggio,out hitPoint)==true){
			if(hitPoint.collider.tag == "intruder"){
				Debug.Log("intruder detected");
				this.intruderDetected = true;
				this.intruderLastPosition = intruderPosition;
			}
			else this.intruderDetected = false;
		}
	}

	private void setOutOfLineOfSight(){
		this.intruderDetected = false;
	}

	/* IF INTRUDER DETECTED -> PURSUIT HIM */
	private void pursuitIntruder(){
		nav.navigateFromATo(intruderLastPosition);
	}

	/* when a drone is within range of detection, need to check if it is also visible */
	void OnTriggerStay(Collider other){
		if(other.tag == "intruder"){
			checkVisibility(other.transform.position);
		}
	}
	
	/* if intruder gets into drone or viceversa, the intruder has been captured and the drone stops.*/
	void OnTriggerEnter(Collider other){
		/* check if collided with the intruder and if it was already 
		 * detected because it means we got into the second collider */
		if(other.tag=="intruder" && intruderDetected==true) { 
			Debug.Log("Intruder Captured TRIGGER");
			nav.stop();
			intruderCaptured=true;
		}
	}

	void OnTriggerExit(Collider other){
		if(other.tag == "intruder"){
			intruderDetected = false;
		}
	}

}




