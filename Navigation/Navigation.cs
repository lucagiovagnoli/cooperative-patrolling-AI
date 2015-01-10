using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* Navigation script takes care of getting the vehicleModel from A to B. 
 * For example the list of "B points" can be given by the Astar algorithm */
public class Navigation {
	
	private VehicleModel model;
	private Astar pathFinder = null;
	private LinkedList<Vector3> path = null;
	private Vector3 goalPosition;
	private LinkedList<Vector3>.Enumerator en;
	private bool isPathInProgress = false;

	public Navigation (GameObject oggetto) {
		model = new CarVehicleModel (oggetto.gameObject);
	}

	public void navigateEuclideanTo(Vector3 destination){
		model.updateModel(destination);
	}
	public void stop(){
		model.updateModel(model.getVehicle().transform.position);
	}

	private void computeAstar(Vector3 goalPosition){
		this.goalPosition = goalPosition;
		this.goalPosition.y = model.getVehicle().transform.position.y; // keep closeness-to-the-goal-control in 2D

		/* compute Astar */
		pathFinder = new Astar(MyUtils.precisionAstar,MyUtils.droneRadius,true);
		if(pathFinder.setNewTargetPosition(goalPosition)==true){ //set the target for A* algorithm
			pathFinder.AstarBestPathFrom(model.getVehicle().transform.position);
			path = pathFinder.getSolution();
			en = path.GetEnumerator();
			en.MoveNext();
			isPathInProgress = true;
			//path = Chromosome.allPairPathsMatrix[CHromo,];
		}
		else Debug.Log("Target position unreachable");
		if (path == null) Debug.Log ("Astar failed!");

	}

	public bool navigateAstarTo(Vector3 destination){

		/* If Astar path exists I follow it */
		if(isPathInProgress==true){
			if(model.updateModel (en.Current) == false){ //if false, model is arrived and need to change hop
				//if I arrived to the hop, go to next hop with MoveNext
				if(en.MoveNext()==false) { //if no next element then I am arrived and return true
					isPathInProgress = false; 
					return true;
				}
			}
		}
		else computeAstar(destination);
		return false;
	}
}


