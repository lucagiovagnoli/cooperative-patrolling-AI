using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class VRPsolution{

	private class VRPelement{
		public Drone drone;
		public LinkedList<Vector3> route;

		public VRPelement(Drone drone, LinkedList<Vector3> route){
			this.drone = drone;
			this.route = route;
		}

		/* toString */
		public override string ToString(){
			string str = "Drone: " + drone + "\n Route:";
			foreach (Vector3 v in route)
				str+=v;
			return str;
		}
	}

	private VRPelement[] solution = null;
	private int Ndrones;
	private int indexFilled = 0;

	/* Constructor */
	public VRPsolution(int Ndrones){
		this.Ndrones = Ndrones;
		this.solution = new VRPelement[Ndrones];
		this.indexFilled = 0;
	}

	public void addDrone(Drone drone, LinkedList<Vector3> route){
		this.solution [indexFilled] = new VRPelement (drone,route);			
		this.indexFilled++;
	}

	public void comunicateRoutesToDrones(){
		foreach (VRPelement e in solution) {
			e.drone.setRoute(e.route);		
		}
	}

	public void debugSolution(){
		Vector3 lastPoint;
		foreach(VRPelement e in solution){
			lastPoint = e.drone.transform.position;
			foreach (Vector3 point in e.route){
				Debug.DrawLine(lastPoint,point,Color.yellow,400.0f);
				lastPoint=point;
			}
		}
	}

	/* toString */
	public override string ToString(){
		string str = "Sol \n";
		foreach (VRPelement e in solution)
			str +=  e.ToString() + "\n";
		return str;
	}
	
}

public class VRP {

	private Drone[] drones;
	private Vector3[] customers;
	private VRPsolution solution;

	
	public VRP(Drone[] drones, Vector3[] customers){

		this.drones = drones;
		this.customers = customers;

		/* Initialize Chromosome static variables 
		 * (mapping between coding of solution and real points) */
		Chromosome.initChromosome (customers, drones);
	}

	public void computeRoutes(){

		//RandomSearch randomSearcher = new RandomSearch ();
		//solution = randomSearcher.computeRoutes (1000);

		GeneticSearch geneticSearch = new GeneticSearch (100);
		solution = geneticSearch.computeRoutes (10000);
		solution.debugSolution ();
	}

	public VRPsolution getSolution(){return solution;}

}
