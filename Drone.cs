using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Drone script takes care of following the VRP route */
public class Drone : MonoBehaviour {

	private bool intruderDetected = false;
	private Vector3 intruderLastPosition;
	private bool intruderCaptured = false;
	
	private LinkedList<Vector3> route = null;
	private LinkedList<Vector3> reversedRoute = null;
	private LinkedList<Vector3>.Enumerator en;
	private Navigation nav;
	private bool isSegmentInProgress = false;
	private int iteration = 0;

	public void setRoute(LinkedList<Vector3> route){
		this.route = route;
		this.reversedRoute = new LinkedList<Vector3>();
		en = this.route.GetEnumerator();
		en.MoveNext();
	}

	// Use this for initialization
	void Start (){
		this.nav = new Navigation (this.gameObject);
		SphereCollider sc =	gameObject.AddComponent("SphereCollider") as SphereCollider;
		sc.radius = MyUtils.maxRangeScanner;
		sc.isTrigger = true;
	}

	// Update is called once per frame
	void FixedUpdate (){
	
		if(intruderCaptured==true) return;
		if(intruderDetected == true) pursuitIntruder();

		/* if route is loaded then follow route */
		else if (route!=null){
			if (route.Count>1) followRoute();
			else if (route.Count==1) nav.navigateAstarTo(en.Current);
		}
		else nav.stop();
	}

	private void followRoute(){

		/* GO TO customer using Astar */
		if(nav.navigateAstarTo(en.Current) == true){ /* returns true if completed, otherwise moves towards the goal*/
			if(iteration == 0) reversedRoute.AddFirst(en.Current);

			/* ROUTE COMPLETED? -> set enumerator to scan the reversed list and firstRun to false*/
			if(en.MoveNext() == false){ 
				if(iteration%2 == 0) en = reversedRoute.GetEnumerator();
				else en = route.GetEnumerator();
				en.MoveNext();en.MoveNext();
				iteration++;
			}
		}
	}


	private void checkVisibility(Vector3 intruderPosition){
		RaycastHit hitPoint; //hitPoint ready to be filled with the info about where the ray hit the collider 
		Ray raggio = new Ray(this.transform.position+new Vector3(0,0.1f,0),intruderPosition-this.transform.position+new Vector3(0,0.1f,0));
		Debug.DrawRay(this.transform.position+new Vector3(0,0.1f,0),intruderPosition-this.transform.position+new Vector3(0,0.1f,0),Color.red);
		if(Physics.Raycast(raggio,out hitPoint)==true){
			if(hitPoint.collider.tag == "intruder"){
				//Debug.Log("intruder detected");
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
		nav.navigateEuclideanTo(intruderLastPosition);
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
			Debug.Log("Intruder Captured");
			nav.stop();
			intruderCaptured=true;
		}
	}

	void OnTriggerExit(Collider other){
		if(other.tag == "intruder"){
			intruderDetected = false;
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.cyan;
        if(intruderDetected==true)  {
			Gizmos.DrawLine(transform.position, intruderLastPosition);
		}
		Gizmos.DrawSphere (Vector3.zero,150.0f);
    }
    
}




