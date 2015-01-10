using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class myGUI : MonoBehaviour {

	public GUIText infoText;
	public GameObject original; // customer to be duplicated in order to create the positions 

	private Drone[] drones;
	private Vector3[] customers;
	private LinkedList<Collider> userInputCustomersList = new LinkedList<Collider>();

	private MouseInput mouse = new MouseInput(MouseInput.clickType.middle);

	private	ConvexCover convexCover;
    private bool isPositioning = false;
	private bool isAutomaticConvexCover = false; 

	void Start () {

        /* get drones by tag */
		GameObject[] d = GameObject.FindGameObjectsWithTag("drone");
		this.drones = new Drone[d.Length];
		for (int i=0;i<d.Length;i++){
			this.drones[i] = d[i].GetComponent<Drone>();
		}
	
		convexCover = new ConvexCover (this.gameObject);
           
		//	Tester tester = new Tester (new Vector3[5],drones);
		//	tester.testEquality();
		//Debug.Log ("Order crossover test: " + tester.testOrderCrossover ());
		//tester.testFitness (customersPositions,drones);
	}

	// Update is called once per frame
	void Update () {

		/*	GameObject[] c = GameObject.FindGameObjectsWithTag("customer");
		this.customers = new Vector3[c.Length];
		for (int i=0;i<c.Length;i++){
			this.customers[i] = c[i].transform.position;
		}*/

        if(isAutomaticConvexCover==true){
			if(convexCover.cover()==true) {
				LinkedList<Collider> convexColliders = convexCover.getSolution();
				foreach(Collider c in convexColliders){
					this.userInputCustomersList.AddLast(((GameObject) Instantiate(original, c.bounds.center, original.transform.rotation)).collider);
                }
				isAutomaticConvexCover = false;
			}
		}

        if(isPositioning==true && mouse.isMouseInputReady()){
			Vector3 newPatrolPosition = mouse.getOutput();
			newPatrolPosition.y += 0.2f;
			this.userInputCustomersList.AddLast(((GameObject) Instantiate(original, newPatrolPosition, original.transform.rotation)).collider);
		}
	}

	void OnGUI(){

		GUI.Box (new Rect (10,10,300,150), "Menu");

		if (GUI.Button (new Rect (30, 30, 200, 30), "Compute convex covers")) {
			foreach(Collider go in this.userInputCustomersList){
				Destroy(go); // destroy old customers
			}
			userInputCustomersList = new LinkedList<Collider>();
			infoText.text = "Calculating Convex Covers";
            isAutomaticConvexCover = true;
        }
        
        if (GUI.Button (new Rect (30, 70, 200, 30), "Set covers manually")) {
			foreach(Collider go in this.userInputCustomersList){
				Destroy(go); // destroy old customers
			}
			userInputCustomersList = new LinkedList<Collider>();
			infoText.text = "Set guarding positions by middle mouse click";
			isPositioning = true;
		}

		if (GUI.Button (new Rect (30, 110, 200, 30), "Start patrolling")) {
			isPositioning = false;

			/*get customers*/
			this.customers = new Vector3[userInputCustomersList.Count];
			int i=0;
			foreach(Collider go in this.userInputCustomersList){
                this.customers[i] = go.transform.position;
				i++;
			}
             
			/* computer VRP */
			infoText.text = "Compute VRP..";
            VRP VRPsolver = new VRP (drones,this.customers);
			VRPsolver.computeRoutes ();
			VRPsolution sol = VRPsolver.getSolution ();
			sol.comunicateRoutesToDrones();
			infoText.text = "Routes communicated to drones.";
		}
	}
}
