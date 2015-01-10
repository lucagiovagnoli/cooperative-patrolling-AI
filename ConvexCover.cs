using UnityEngine;
using System;
using System.Collections.Generic;

public class ConvexCover{
				
	private GameObject environment;
	private BoxCollider area;
	private LinkedList<Collider> convexColliders = new LinkedList<Collider>();
	private Vector3[] dir;
	private int index = 0;

	private Vector3 deltaGrid = new Vector3(0,0,MyUtils.droneRadius);
	private Vector3 currentCell;

	public ConvexCover (GameObject environment){

		this.environment = environment;

		dir = new Vector3[]{new Vector3(-MyUtils.droneRadius,0,0),new Vector3(0,0,-MyUtils.droneRadius),
			new Vector3(MyUtils.droneRadius,0,0),new Vector3(0,0,MyUtils.droneRadius)};

		/*set first convex cover*/
		Vector3 cell = Vector3.zero;//findUncoveredCell();
        area = (BoxCollider) environment.gameObject.AddComponent("BoxCollider") as BoxCollider;
		area.center = cell;
		area.size = new Vector3(MyUtils.droneRadius,MyUtils.droneRadius,MyUtils.droneRadius);
		area.isTrigger = true;    

		/* set a list of already seen colliders */
        foreach(GameObject go in Astar.obstacles){
			convexColliders.AddLast(go.collider);
		}

		//currentCell = MyUtils.pavementCollider.bounds.center+MyUtils.pavementCollider.collider.bounds.extents;
		currentCell = new Vector3(0,0.5f,0);
		Debug.Log(currentCell);
		Debug.DrawLine(new Vector3(1f,1f,1f),currentCell,Color.red);
        
    }


	private void computeGrid(){
	
		
	
	}

	private Vector3 getNextCell() {
		Vector3 nextCell = currentCell+deltaGrid;
		Debug.DrawLine(Vector3.zero,nextCell);
		return nextCell;
    }

	private Vector3 findUncoveredCell(){
		currentCell = getNextCell();
		while(true){
			foreach(Collider obstacle in convexColliders){
	            if(obstacle.bounds.Contains(currentCell)){
					break;
				}

			}
			break;
		}
		return currentCell;
	}

	public void cover(){

		if (isExpanded() == true){
			Vector3 cell = findUncoveredCell();
			area = (BoxCollider) environment.gameObject.AddComponent("BoxCollider") as BoxCollider;      
			area.center = cell;
			area.size = new Vector3(MyUtils.droneRadius,MyUtils.droneRadius,MyUtils.droneRadius);
			area.isTrigger = true;      
		}

    }
    


	private bool isExpanded(){

		area.center += dir[index]/2;
		area.size+=dir[index];
		foreach(Collider obstacle in convexColliders){
			if(area.bounds.Intersects(obstacle.bounds)) {
				area.center -= dir[index]/2;
				area.size-=dir[index];
				index++;
				break;
			}
		}
		if(index>3){
			convexColliders.AddFirst(area);
			index=0;
            return true;
        }
       	else return false;
	}
}

