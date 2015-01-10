using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Drone script takes care of following the VRP route */
public class Drone : MonoBehaviour {

	private static GameObject[] intruders = null;
	private static float maxRange = 100.0f;
	private readonly static int FREQ_PER_UPDATE = 1;

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
	}

	// Update is called once per frame
	void Update (){
	
		if (isRouteLoaded == true && route.Count>0) { /* if route is loaded then follow route */
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

		for(int i=0;i<FREQ_PER_UPDATE;i++) isIntruderDetected ();
	}

	/* check for intruder by 360° camera analysis of footage 
	* (could be human analysing footage or computer vision tecniques) */
	private void isIntruderDetected(){
		Vector3 origin = this.transform.position; origin.y += 0.3f;
		Vector3 direction = new Vector3 (1,0,1);
		RaycastHit hitPoint; //hitPoint ready to be filled with the info about where the ray hit the collider 
		direction = Quaternion.Euler(0, (d++)%360, 0) * direction;
		direction.Normalize ();
		Ray raggio = new Ray(origin,direction);

		foreach(GameObject intruderCollider in intruders){
			if (intruderCollider.collider.Raycast(raggio,out hitPoint,maxRange)){
				Debug.Log("Intruder Detected at: "+hitPoint.point);		
				Debug.DrawRay(origin,direction*maxRange,Color.red,0.2f);
				/* PERFORM INTRUDER PURSUIT */
			}
			else Debug.DrawRay(origin,direction*maxRange,Color.white,0.2f);
		}
	}

	/* IF INTRUDER DETECTED -> PURSUIT HIM */


}




