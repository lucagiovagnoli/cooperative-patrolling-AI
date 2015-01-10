using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Navigation script takes care of getting the vehicleModel from A to B. 
 * For example the list of "B points" can be given by the Astar algorithm */
public class Navigation {

	public float movK = 3;
	public float distanceToHop = 0.1f;

	private GameObject oggetto;
	private Vector3 goalPosition;
	private VehicleModel vehicleModel;
	private TargetedInput targInput;
	private Astar pathFinder = null;
	private List<Vector3> path = null;

	public Navigation (GameObject oggetto) {
		this.oggetto = oggetto;
		vehicleModel = new VehicleModel (oggetto.gameObject, modelType.kinematic);
		targInput = new TargetedInput (vehicleModel); //start the targeted controller
	}

	public void computeAstar(Vector3 goalPosition){
		this.goalPosition = goalPosition;
		this.goalPosition.y = oggetto.transform.position.y; // keep closeness-to-the-goal-control in 2D

		/* compute Astar */
		pathFinder = new Astar(distanceToHop,0.5f,true);
		if(pathFinder.setNewTargetPosition(goalPosition)==true){ //set the target for A* algorithm
			pathFinder.AstarBestPathFrom(oggetto.transform.position);
			path = pathFinder.getSolution();
			//path = Chromosome.allPairPathsMatrix[CHromo,];
		}
		else Debug.Log("Target position unreachable");
		if (path == null) Debug.Log ("Astar failed!");

	}

	public bool goThere(){

		/* if near the goal stop the walk */
		if (MyUtils.near (oggetto.transform.position, goalPosition, distanceToHop)) {
			targInput.setTarget(oggetto.transform.position); // set target to itself so to STOP the drone
			targInput.computeInputTowardsTargetFrom (oggetto.transform.position);
			vehicleModel.updateModel (movK*targInput.getOutput()); 
			return true;
		}

		/* If Astar path exists I follow it */
		if(path != null && path.Count > 0){
			Vector3 nextHop =  path[0];
			targInput.setTarget (nextHop); // set the target for the movement to the first A* point
			targInput.computeInputTowardsTargetFrom (oggetto.transform.position);
			vehicleModel.updateModel (movK*targInput.getOutput()); 
			if(path.Count>1 && MyUtils.near(oggetto.transform.position,nextHop,distanceToHop)) {
				path.RemoveAt(0); //if I arrived to the hop, remove the next hop
			}
		}
		return false;
	}
}


