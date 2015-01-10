using UnityEngine;
using System;
using System.Collections.Generic;

public class ConvexCover{

	/* DEBUG PURPOSES (to show convex covers) */
	private GameObject currentCube;
	private LinkedList<GameObject> debugCubes = new LinkedList<GameObject>();
	private Color[] colorList = new Color[] {Color.red,Color.cyan,Color.green,Color.magenta,Color.yellow};
	private int currentColor = 0;

	/* real convex covers variables*/
	private GameObject environment;
	private LinkedList<Collider> convexColliders = new LinkedList<Collider>();
	private BoxCollider area;
	private Vector3[] dir;
	private int index = 0;

	/*grid variables */
	private Vector3[,] grid;
	public static Vector3 upperLeftCorner = new Vector3(-12.5f,0.2f,16.4f);
	public static Vector3 lowestRightCorner = new Vector3(12.2f,0.2f,-8.2f);
    private int xGridSize;
	private int zGridSize;
	private int gridSize;
	private bool[] insideObstacles;

	private int currentIndex =0;
	private Vector3 currentCell;

	public ConvexCover (GameObject environment){

		this.environment = environment;

		dir = new Vector3[]{new Vector3(MyUtils.droneRadius,0,0),new Vector3(-MyUtils.droneRadius,0,0),
			new Vector3(0,0,MyUtils.droneRadius),
			new Vector3(0,0,-MyUtils.droneRadius)};

		computeGrid();
		currentCell = upperLeftCorner;
		currentIndex =0;

        /*set first convex cover*/
		if(existsUncoveredCell()){
			createNewConvexArea();    
        }				

		foreach(GameObject go in Astar.obstacles){
			convexColliders.AddLast(go.collider);
		}

    }

	private void computeGrid(){

		xGridSize = (int) Mathf.Floor((lowestRightCorner.x-upperLeftCorner.x)/MyUtils.droneRadius);
		zGridSize = (int) Mathf.Floor((upperLeftCorner.z-lowestRightCorner.z)/MyUtils.droneRadius);
		gridSize = zGridSize*xGridSize;
		grid = new Vector3[zGridSize,xGridSize];
		insideObstacles = new bool[gridSize];

		Vector3 lastPoint = upperLeftCorner;
		for(int i =0;i<zGridSize;i++,lastPoint.z-=MyUtils.droneRadius){
			for(int z=0;z<xGridSize;z++,lastPoint.x+=MyUtils.droneRadius){
				insideObstacles[i*xGridSize+z]=false;
				foreach(GameObject go in Astar.obstacles){
					if(MyUtils.near(lastPoint,go.collider.ClosestPointOnBounds(lastPoint),MyUtils.droneRadius*2)==true) 
						insideObstacles[i*xGridSize+z] = true;
				}
				grid[i,z] = lastPoint;
		    }
			lastPoint.x = upperLeftCorner.x;
		}
	}

	private void updateCurrentCell() {
		currentCell = grid[currentIndex/xGridSize,currentIndex%xGridSize];
		currentIndex++;
		currentIndex = currentIndex%gridSize;        
    }

	private bool existsUncoveredCell(){

		int cnt=0;
		bool inside;

		/*when cnt>gridSize*0.9, 90% of the cells are covered. 90% is used because of little 
		 * errors at the boundaries of the covers */
		while(cnt<gridSize-1){ 
			if(insideObstacles[currentIndex]==true){
				updateCurrentCell();
				cnt++;
                continue;				
            }

			/*check if cell inside one of the new convex cover colliders*/
			inside = false;
			foreach(Collider obstacle in convexColliders){
		   		 if(MyUtils.near(currentCell,obstacle.ClosestPointOnBounds(currentCell),MyUtils.droneRadius*2)==true){
					insideObstacles[currentIndex]=true;
					inside = true;
					updateCurrentCell();
                    cnt++;
					break;
				}
			}
			if(inside==false) return true;
		}
		return false; //if here it means that cnt>gridSize*0.9, then no uncovered cells exist
	}


    
	/* expanding in one diretion is obtained moving along that direction 0.5 unit 
	 * and expanding along the same direction for 1 unit*/
	private void expandAreaAtDirection(){
		/*RANGE CHECK*/
		area.center += dir[index]/2; //always move in a new direction
		if(index<2) area.size+=dir[0]; 
		if(index>=2) area.size+=dir[2]; 

		currentCube.transform.position =area.center; //always move in a new direction
		currentCube.transform.localScale = area.size;
    }

	private void oneStepBeforeExpand(){
		area.center -= dir[index]/2; //always move in a new direction
		if(index<2) area.size-=dir[0]; 
		if(index>=2) area.size-=dir[2];

		currentCube.transform.position =area.center; //always move in a new direction
		currentCube.transform.localScale = area.size;
        
    }


	private bool isExpanded(){

		expandAreaAtDirection();
		foreach(Collider obstacle in convexColliders){
			if(area.bounds.Intersects(obstacle.bounds)) {
				oneStepBeforeExpand();
				index++;
				break;
			}
		}
		if(index>3){
			/* set up a list of convex colliders*/;
			convexColliders.AddFirst(area);
			debugCubes.AddLast(currentCube);
			index=0;
            return true;
        }
       	else return false;
	}

	private void createNewConvexArea(){
		area = (BoxCollider) environment.gameObject.AddComponent("BoxCollider") as BoxCollider;
		area.center = currentCell;
		area.size = new Vector3(MyUtils.droneRadius,MyUtils.droneRadius,MyUtils.droneRadius);
		area.isTrigger = true;    

		currentCube = (GameObject) GameObject.CreatePrimitive(PrimitiveType.Cube);
		currentCube.renderer.material.color = colorList[currentColor];currentColor++;currentColor%=colorList.Length;
		currentCube.transform.position = currentCell;
    }
    
    public bool cover(){
		if (isExpanded() == true){
			if(existsUncoveredCell()){
				createNewConvexArea();
				return false;
			}
			else return true;
		}
		return false;
	}

	public LinkedList<Collider> getSolution(){

		foreach(GameObject go in Astar.obstacles){
			convexColliders.RemoveLast();
		}
		foreach(GameObject go in debugCubes) GameObject.Destroy(go);
		return convexColliders;
	}
}

