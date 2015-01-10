using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GeneticSearch {

	private static int ROUNDS = 2; //tournament ROUNDS
	private static double MUTATION_RATE = 0.1f; //mutation probability in %

	private Chromosome[] population;
	private int N=0;
	private Chromosome parent1 = null;
	private Chromosome parent2 = null;
	private Chromosome bestSolSoFar = new Chromosome();
	private HashSet<Chromosome> populationMap = new HashSet<Chromosome>(); 


	public static void setParameters(int rounds,double mutation_rate){
		GeneticSearch.ROUNDS = rounds;
		GeneticSearch.MUTATION_RATE = mutation_rate;
	}

	public GeneticSearch (int Nindividuals)
	{
		createPopulation(Nindividuals);
		bestSolSoFar = population[0]; 	/* initial best solution and best fitness random*/
	}

	private void createPopulation(int N){
		this.N = N;
		population = new Chromosome[N];
		for (int i=0;i<N;i++){
			Chromosome c = new Chromosome();
			c.shuffleChromosome();
			population[i] = c;
			populationMap.Add(c);
			updateBestSolution(c);
		}
	}

	public VRPsolution computeRoutes(int T /*T number of iterations*/){

		Debug.Log("Initial fitness: "+bestSolSoFar.getFitness());
        
        for(int i=0;i<T;i++){
			/* PARENT SELECTION by TOURNAMENT SELECTION */
			parent1 = population[tournamentSelectionBest(ROUNDS)];
			parent2 = population[tournamentSelectionBest(ROUNDS)];

			/* ORDER Crossover - compute OFFSPRING*/
			Chromosome[] children = parent1.orderCrossover(parent2);

			/* MUTATION with MUTATION_RATE probability*/
			if(MyUtils.rnd.NextDouble() < MUTATION_RATE){
				children[0].mutation();
				children[1].mutation();
			}

			/* REPLACEMENT of offspring by TOURNAMENT SELECTION if they are not duplicates and update current best solution */
            
            if(populationMap.Contains(children[0])==false){
				populationMap.Add(children[0]);
				int removed1 = tournamentSelectionWorst(ROUNDS);
				populationMap.Remove(population[removed1]);
				population[removed1] = children[0];
				updateBestSolution(children[0]);   
            }
            if(populationMap.Contains(children[1])==false){
				populationMap.Add(children[1]);
				int removed2 = tournamentSelectionWorst(ROUNDS);
				populationMap.Remove(population[removed2]);
				population[removed2] = children[1];
				updateBestSolution(children[1]);
                
            }
		}

		//debugPopulation();
		Debug.Log("After T="+ T +" iterations. \nBest Fitness: " + bestSolSoFar.getFitness()+"\nFinal Chromosome: "+ bestSolSoFar);

		return bestSolSoFar.buildSolution();

	}

	private void updateBestSolution(Chromosome nuovo){
		if (nuovo.getFitness() < bestSolSoFar.getFitness()) {
			bestSolSoFar = new Chromosome(nuovo);
		}
	}
	
	/* if rounds=0 -> RANDOM INDIVIDUAL SELECTED
	 * if rounds=1 -> 1 DUEL, WINNER SELECTED
	 * if rounds about equal = Npopulation  -> BEST INDIVIDUAL SELECTED */
	private int tournamentSelectionBest(int rounds){

		int duellante;
		int currentWinner = MyUtils.rnd.Next(0,N) ;

		for (int i=0;i<rounds;i++){
			duellante = MyUtils.rnd.Next(0,N);
			if(population[duellante].getFitness() > population[currentWinner].getFitness()) currentWinner = duellante;
		}

		return currentWinner;
	}

	private int tournamentSelectionWorst(int rounds){
		int duellante;
		int currentWinner = MyUtils.rnd.Next(0,N) ;
		
		for (int i=0;i<rounds;i++){
			duellante = MyUtils.rnd.Next(0,N);
			if(population[duellante].getFitness() < population[currentWinner].getFitness()) currentWinner = duellante;
		}
		
		return currentWinner;
	}

	private void debugPopulation(){

		for (int i=0;i<N;i++){
			Debug.Log("Individuo "+i+":"+population[i]);	
		}
	}
}

