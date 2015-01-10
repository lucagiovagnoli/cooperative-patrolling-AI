//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Chromosome : IComparable<Chromosome>{

	/* These need to be set by initChromosome before creating Chromosomes !!*/
	public static Vector3[] customersPositions=null;
	public static Vector3[] dronesPositions=null;
	public static int N=-1,M=-1;
	public static int Ngenes;	
	private static Drone[] drones=null;
	private static Astar [,] allPairAstarMatrix;
	private static float [,] allPairDistancesMatrix;
	public static List<Vector3> [,] allPairPathsMatrix;

	private int[] genes;
	private double fitness;

	/* CALL init BEFORE ANYTHING ELSE */
	public static void initChromosome(Vector3[] customersPositions, Drone[] drones){
		Chromosome.drones = drones;
		Chromosome.customersPositions = customersPositions;
		/* save the Vector3 positions of the drones for easy use*/
		Chromosome.dronesPositions = new Vector3[drones.Length];
		for (int i=0; i<drones.Length; i++) {
			Chromosome.dronesPositions[i] = drones[i].gameObject.transform.position;
		}
		Chromosome.N = customersPositions.Length;
		Chromosome.M = drones.Length;
		Chromosome.Ngenes = N+M;
		computeAllPairDistancesAndPaths ();
	}

	/* Computes all pairs distances between drones and customers in order 
	 * to be used later by the computing fitness function */
	private static void computeAllPairDistancesAndPaths(){

		allPairDistancesMatrix = new float[Ngenes,Ngenes];
		allPairPathsMatrix = new List<Vector3>[Ngenes,Ngenes];

		Astar pathFinder = new Astar(0.1f,0.5f);
		Vector3 pt1, pt2;

		for (int i=0;i<Ngenes;i++) {
			allPairDistancesMatrix[i,i] = 0.0f;
			for (int z=i+1;z<Ngenes;z++) {
				pt1 = positionAtIndex(i);
				pt2 = positionAtIndex(z);
				if(pathFinder.setNewTargetPosition(pt2)==true){
					float solutionCost = 0;
					pathFinder.AstarBestPathFrom(pt1);
					List<Vector3> path = pathFinder.getSolution();
					allPairDistancesMatrix[i,z] = pathFinder.getSolutionCost();
					allPairDistancesMatrix[z,i] = pathFinder.getSolutionCost();
					allPairPathsMatrix[i,z] = path;
					allPairPathsMatrix[z,i] = path;
				}
			}
		}
	/*	String str = "";
		for (int i=0; i<Ngenes; i++) {
			for (int z=0; z<Ngenes; z++) {
				str+=allPairDistancesMatrix[i,z]+" ";
			}
			str+="\n";
		}
		Debug.Log (str);*/
	}

	/*  Constructor with array initialized by index values.
	 *  N tasks and M robots */
	public Chromosome (){
		if (customersPositions==null || drones == null || dronesPositions ==null ) 
			throw new ArgumentException ("Error creating chromosome, drones or customers positions not set.");
		if (M ==-1 || N==-1)
			throw new ArgumentException ("Error creating chromosome, M or N not set");

		genes = new int[Ngenes];
		for (int i=0; i<Ngenes; i++) {
			genes [i] = i;	
		}
	}

	/* Constructor by already-existing array */
	public Chromosome (int[] vett){
		if (customersPositions==null || drones == null || dronesPositions ==null ) 
			throw new ArgumentException ("Error creating chromosome, drones or customers positions not set.");
		if (vett.Length != M + N)
			throw new ArgumentException ("Error creating chromosome, length of vector:"+vett.Length+" given differs from M+N:"+M+","+N);
		this.genes = vett;
		Ngenes = vett.Length;
	}

	/* Copy Constructor */
	public Chromosome(Chromosome other){
		this.genes = new int[Chromosome.Ngenes];
		for (int i=0; i<Chromosome.Ngenes; i++) {
			this.genes [i] = other.genes[i];	
		}
		this.fitness = other.fitness;
	}

	public void shuffleChromosome(){
		MyUtils.FisherYatesShuffle (genes);
	}

	/* Pair distances between point computed by Astar
	 * Fitness equal to TOTAL length walked by drones */
	public double computeFitnessAstar(){

		int lastPoint,currentPoint;
		double tot = 0;
		int i;
		//		Debug.Log ("Computing fitness: "+this);
		
		/* Set last position to last gene, start compute distance "from" it */
		lastPoint = genes [Ngenes - 1];

		/* I have to consider to be back on last gene to compute distance "to" it*/
		for (i=0; i<Ngenes; i++) {
			if(isDrone(genes[i])) {
				lastPoint = genes[i];
				//				Debug.Log("Change drone: "+lastPosition);
			}
			else{ // case of customer position
				currentPoint = genes[i]; 
				//				Debug.Log("Customer: "+currentPosition);
				tot += allPairDistancesMatrix[lastPoint,currentPoint];
				//				Debug.Log("distanza: "+(currentPosition-lastPosition).magnitude);
				lastPoint = currentPoint;
			}
		}
		
		return tot;
	}

	/* Euclidean compute Fitness
	 * Fitness equal to TOTAL length walked by drones */
	public double computeFitnessEuclidean(){

		Vector3 lastPosition,currentPosition;
		double tot = 0;
		int i;
//		Debug.Log ("Computing fitness: "+this);

		/* Set last position to last gene, start compute distance "from" it */
		if(isDrone(genes[Ngenes-1])) lastPosition = dronesPositions[genes[Ngenes-1]-N];
		else lastPosition = customersPositions[genes[Ngenes-1]];

		/* I have to consider to be back on last gene to compute distance "to" it*/
		for (i=0; i<Ngenes; i++) {
			if(isDrone(genes[i])) {
				lastPosition = dronesPositions[genes[i]-N];
//				Debug.Log("Change drone: "+lastPosition);
			}
			else{ // case of customer position
				currentPosition = customersPositions[genes[i]]; 
//				Debug.Log("Customer: "+currentPosition);
				tot += (currentPosition-lastPosition).magnitude;
//				Debug.Log("distanza: "+(currentPosition-lastPosition).magnitude);
				lastPosition = currentPosition;
			}
		}

		return tot;
	}

	public VRPsolution buildSolution(){
		
		VRPsolution solution = new VRPsolution(drones.Length);
		Drone currentDrone = null;
		LinkedList<Vector3> route = null;
		int i,c;

		/* Scan till the first drone */
		for (i=0; i<Ngenes; i++) {
			if(isDrone(genes[i])) {
				/* first drone, first solution */
				currentDrone = drones[genes[i]-N];
				route = new LinkedList<Vector3>();
				i++; // start from first customer
				break;
			}
		}

		for (/* using i from last iteration */ c=0; c<Ngenes-1 /*first drone scanned already*/; c++, i++,i=i%Ngenes) {
			if(isDrone(genes[i])){ /* it is a drone */
				solution.addDrone(currentDrone,route);
				currentDrone = drones[genes[i]-N];
				route = new LinkedList<Vector3>();
			}
			else{ /* it is a customer */
				route.AddLast(customersPositions[genes[i]]);
			}
		}
		solution.addDrone(currentDrone,route);
		return solution;
	}

	private static bool isDrone(int x){
		if (x >= N)	return true;
		return false;
	}

	public static Vector3 positionAtIndex(int index){
		if (isDrone (index)) return dronesPositions[index-N];
		else return customersPositions[index];
	}

	public int[] getGenes(){return genes;}
	public int getSize(){return Ngenes;}


	/* OX - Order Crossover */
	public Chromosome[] orderCrossover(Chromosome other, int cutPar1 = -1, int cutPar2 = -1){

		Chromosome[] res = new Chromosome[2];
		int[] offspring1 = new int[Ngenes];
		int[] offspring2 = new int[Ngenes];
		int[] visited1 = new int[Ngenes];
		int[] visited2 = new int[Ngenes];
		int i,cut1,cut2;

		if (cutPar1 == -1) {
			cut1 = MyUtils.rnd.Next (0, genes.Length);
		} else {
			cut1 = cutPar1;
		}

		if (cutPar2 == -1) {
			cut2 = MyUtils.rnd.Next (0, genes.Length);
		} else {
			cut2 = cutPar2;
		}

		Array.Clear(visited1, 0, Ngenes);
		Array.Clear(visited2, 0, Ngenes);

		/* no need to go on, single cut is useless */
		if (cut1 == cut2) {
			res[0] = this;
			res[1] = other;
			return res; 
		}
		/* cut1 must precedes cut2, if cut2<cut1 I need to invert them */
		if (cut2 < cut1) { 		
			int temp = cut2;
			cut2 = cut1;
			cut1 = temp;
		}

		/* Copy the central part between cuts in the offsprings */
		for(i=cut1; i<=cut2;i++){
			offspring1[i] = genes[i];
			visited1[genes[i]] = 1;
			offspring2[i] = other.genes[i];
			visited2[other.genes[i]] = 1;
		}

/*		Debug.Log ("cut1: "+cut1);
		Debug.Log ("cut2: "+cut2);
		
		string str = "";
		for (i=0; i<Ngenes; i++) {
			str += + offspring1[i]+" ";
		}
		Debug.Log(str);
		str = "";
		for (i=0; i<Ngenes; i++) {
			str += + offspring2[i]+" ";
		}
		Debug.Log(str); */


		/* Order crossover */
		int i1 = cut2 + 1;
		for(i=cut2+1; i1!=cut1 ; i++){
			i = i%Ngenes;
			i1 = i1%Ngenes;
			if(visited1[other.genes[i]]==0) {
				visited1[other.genes[i]] = 1;
				offspring1[i1] = other.genes[i];
				i1++;
			}
		}
		for (i1 = cut2 + 1,i=cut2+1; i1!=cut1; i++) {
			i = i%Ngenes;
			i1 = i1%Ngenes;
			if(visited2[genes[i]]==0) {
				visited2[genes[i]] = 1;
				offspring2[i1] = genes[i];
				i1++;
			}
		}

		res [0] = new Chromosome (offspring1);
		res [1] = new Chromosome (offspring2);
		return res;
	}

//	public void inversion?(){}


	/* Fitness COMPARATOR */
	public int CompareTo(Chromosome other){
		if(this.fitness > other.fitness) return 1;
		if(this.fitness == other.fitness) return 0;
		return -1;
	}

	/* OPERATOR OVERLOADING */
	public static bool operator ==(Chromosome a, Chromosome b)
	{
		// If both are null, or both are same instance, return true.
		if (System.Object.ReferenceEquals(a, b)) return true;
		
		// If one is null, but not both, return false.
		if (((object)a == null) || ((object)b == null)) return false;
		
		// My equality:
		for (int i=0; i<a.genes.Length; i++) {
			if (a.genes [i] != b.genes [i]) return false;
		}
		return true;
	}
	
	public static bool operator !=(Chromosome a, Chromosome b){
		return !(a == b);
	}


	/* ToString */
	public override string ToString(){

		string res = "";
		for (int i=0; i<Ngenes; i++) {
			res += + genes[i]+" ";
		}
		return res;
	}

}

