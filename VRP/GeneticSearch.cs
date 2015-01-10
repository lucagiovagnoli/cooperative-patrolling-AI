using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GeneticSearch {

	private readonly static int ROUNDS = 2; //tournament ROUNDS
	private Chromosome[] population;
	private int N=0;
	private Chromosome parent1 = null;
	private Chromosome parent2 = null;
	private Chromosome bestSolSoFar;

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
		}
		debugPopulation();
	}

	public VRPsolution computeRoutes(int T /*T number of iterations*/){

		for(int i=0;i<T;i++){
			/* PARENT SELECTION by TOURNAMENT SELECTION */
			parent1 = population[tournamentSelectionBest(ROUNDS)];
			parent2 = population[tournamentSelectionBest(ROUNDS)];

			/* ORDER Crossover - compute OFFSPRING*/
			Chromosome[] children = parent1.orderCrossover(parent2);

			/* MUTATION*/
			children[0].mutation();
			children[1].mutation();

			/* REPLACEMENT of offspring by TOURNAMENT SELECTION */
			population[tournamentSelectionWorst(ROUNDS)] = children[0];
			population[tournamentSelectionWorst(ROUNDS)] = children[1];

			/* update current best solution */
			updateBestSolution(children[0]);
			updateBestSolution(children[1]);
		}

		debugPopulation();
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

