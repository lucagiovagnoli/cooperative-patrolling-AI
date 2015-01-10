using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Astar {

	//public static string[] debugMatrices = new string[] {"* - -\n- - -\n- - -","- * -\n- - -\n- - -","- - *\n- - -\n- - -","- - -\n- - *\n- - -","- - -\n* - -\n- - -","- - -\n- - -\n- - *","- - -\n- - -\n- * -","- - -\n- - -\n* - -"};
	private static float[,] directions = new float[,] {{-1.0f,1.0f},{0.0f,1.0f},{1.0f,1.0f},{1.0f,0.0f},{-1.0f,0.0f},{1.0f,-1.0f},{0.0f,-1.0f},{-1.0f,-1.0f}};
	public static GameObject[] obstacles;

	private Vector3 startingPosition;
	private Vector3 targetPosition;
	private float playerRadius;
	private float precisionAstar; 
	private LinkedList<Vector3> result = null;
	private float solutionCost=0;
	private bool debugDraws = false;

	static Astar(){
		obstacles = GameObject.FindGameObjectsWithTag("obstacle");
	}

	public Astar (float precisionAstar,float playerRadius,bool debugDraws = false){
		this.debugDraws = debugDraws;
		this.precisionAstar = precisionAstar;
		this.playerRadius = playerRadius;
	}

	public bool setNewTargetPosition(Vector3 targetPosition){
		if(isCollision(targetPosition)) return false;
		this.targetPosition = targetPosition;
		return true;
	}
	
	/* return a list of points to go through or null on fail */
	public bool AstarBestPathFrom(Vector3 startPos){

		this.startingPosition = startPos;
		targetPosition.y = startingPosition.y; // movement in 2 D
	
		SortedList<float,SampledPositionAstar> toExploreQueue = new SortedList<float,SampledPositionAstar> ();
		HashSet<Vector3> visited = new HashSet<Vector3> ();
		SampledPositionAstar currentSampledPosition = new SampledPositionAstar (startingPosition,null,0.0f,
		                                                                        (targetPosition - startingPosition).magnitude);

		toExploreQueue.Add (0.0f, currentSampledPosition);
		visited.Add (startingPosition);
		while(toExploreQueue.Count>0){
			currentSampledPosition = toExploreQueue[toExploreQueue.Keys[0]];
			toExploreQueue.RemoveAt(0);

			/* Termination condition*/
			if (MyUtils.near(currentSampledPosition.position,targetPosition,precisionAstar)) {
				buildSolution(currentSampledPosition);
				return true;
			}
			else exploreNeighborhood(currentSampledPosition,visited,toExploreQueue);
		}
		return false;
	}

	private void buildSolution (SampledPositionAstar currentSampledPosition){
		this.result = new LinkedList<Vector3>();
		this.result.AddFirst(currentSampledPosition.position); // add final position, also if same of initial
		this.solutionCost = currentSampledPosition.pastCost; // save cost of solution
		while(currentSampledPosition.lastPosition != null){
			if(debugDraws==true) Debug.DrawLine(currentSampledPosition.position,currentSampledPosition.lastPosition.position, Color.green,15.0f);
			result.AddFirst(currentSampledPosition.position);
			currentSampledPosition = currentSampledPosition.lastPosition;					
		}
	}

	/** Explore neighbourhood **/
	private void exploreNeighborhood(SampledPositionAstar currentSampledPosition, HashSet<Vector3> visited,SortedList<float,SampledPositionAstar> toExploreQueue){
		Vector3 nextPos; nextPos.y = this.startingPosition.y; // altezza del percorso??
		SampledPositionAstar nextPosition;
		for (int i=0; i<8; i++) {
			Vector3 temp = new Vector3(precisionAstar*directions[i,0],0.0f,precisionAstar*directions[i,1]);
			//Vector3 temp0 = new Vector3(directions[i,0],0.0f,directions[i,1]);
			//Vector3 temp = Vector3.ClampMagnitude(temp0,precisionAstar);
			nextPos = currentSampledPosition.position + temp;
			
			if(visited.Contains(nextPos)) continue; //if is visited then continue
			else visited.Add (nextPos); // else mark visited
			if(isCollision(nextPos)) continue; //has collided?

			//create the next Hop
			float newPastCost = (nextPos-currentSampledPosition.position).magnitude + currentSampledPosition.pastCost;			
			float newEstimatedCost = (targetPosition-nextPos).magnitude;
			nextPosition = new SampledPositionAstar(nextPos,currentSampledPosition,newPastCost,newEstimatedCost);
			
			try{	
				toExploreQueue.Add(newPastCost+newEstimatedCost,nextPosition);
				//Debug.Log(debugMatrices[i]+"Added: "+nextPosition.position+ " with cost: "+ nextPosition.getCost());
			}
			catch (ArgumentException e) {	
				//	Debug.Log("Already visited"+e.ToString());
			}
			if(debugDraws==true)Debug.DrawLine(currentSampledPosition.position,nextPosition.position,Color.red,15.0f);
		}
	}

	private bool isCollision(Vector3 nextPos){

		GameObject obstacle;
		int n = obstacles.Length;
		for (int i=0;i<n;i++){ 
			obstacle = obstacles[i];

			// check if the points is closer than playerRadius from any bound of the collider
			if((obstacle.transform.collider.ClosestPointOnBounds(nextPos) - nextPos).magnitude < playerRadius){
				return true;
			}
		}
		return false;
	}

	public LinkedList<Vector3> getSolution(){return result;}
	public float getSolutionCost(){ return solutionCost;}

	public void drawAstar(){
		LinkedList<Vector3>.Enumerator it = result.GetEnumerator();
		Vector3 lastPoint = it.Current;
		while (it.MoveNext()) {
			Debug.DrawLine(lastPoint,it.Current,Color.green,15.0f);
		}
	}
}


