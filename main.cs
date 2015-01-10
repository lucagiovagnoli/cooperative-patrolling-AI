using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class main : MonoBehaviour {

	private Drone[] drones;
	private Vector3[] customers;
	LinkedList<Vector3> userInputCustomersList = new LinkedList<Vector3>();
	public GameObject original;
	MouseInput mouse = new MouseInput(MouseInput.clickType.right);
	bool isPositioning = false;

	// Use this for initialization
	void Start () {
	
		GameObject[] d = GameObject.FindGameObjectsWithTag("drone");
		this.drones = new Drone[d.Length];
		for (int i=0;i<d.Length;i++){
			drones[i] = d[i].GetComponent<Drone>();
		}

		//TestGA tester = new TestGA ();
		//Debug.Log ("Order crossover test: " + tester.testOrderCrossover ());
		//tester.testFitness (customersPositions,drones);
			
	}

	// Update is called once per frame
	void Update () {


/*		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("obstacle");
		MeshCollider boxCollider = obstacles [1].GetComponent<MeshCollider>();

		Vector3 pt3 = boxCollider.center + boxCollider.extents;
		Debug.DrawLine(pt3 ,new Vector3(0,1,0),Color.red);*/


		if(isPositioning==true && mouse.isMouseInputReady()){
			Vector3 newPatrolPosition = mouse.getOutput();
			newPatrolPosition.y += 0.1f;
			Instantiate(original, newPatrolPosition, original.transform.rotation);
			this.userInputCustomersList.AddLast(newPatrolPosition);
		}
	}


	void OnGUI(){

		GUI.Box (new Rect (10,10,300,120), "Menu");

		if (GUI.Button (new Rect (30, 30, 200, 30), "Set guarding positions")) {
			isPositioning = true;
		}

		if (GUI.Button (new Rect (30, 70, 200, 30), "Start patrolling")) {
			isPositioning = false;

			/*get customers*/
		/*	GameObject[] c = GameObject.FindGameObjectsWithTag("customer");
			this.customers = new Vector3[c.Length];
			for (int i=0;i<c.Length;i++){
				this.customers[i] = c[i].transform.position;
			}
*/
			this.customers = new Vector3[userInputCustomersList.Count];
			userInputCustomersList.CopyTo(this.customers,0);
			VRP VRPsolver = new VRP (drones,this.customers);
			VRPsolver.computeRoutes ();
			//		VRPsolver.getSolution ();

		}
	

	}
}
