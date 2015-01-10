using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ComparerByPosition : IComparer<SampledPositionAstar> {
	public int Compare(SampledPositionAstar x, SampledPositionAstar y) 	{ 
		return (int) Mathf.FloorToInt((x.position-y.position).magnitude); 
	} 
} 
public class ComparerByCost : IComparer<SampledPositionAstar> {
	public  int Compare(SampledPositionAstar x, SampledPositionAstar y) 	{ 
		return Mathf.FloorToInt(x.cost-y.cost);
	} 
}

public class SampledPositionAstar:IComparable<SampledPositionAstar>{

	public Vector3 position;
	public SampledPositionAstar lastPosition;
	public float pastCost;
	public float estimatedCost;
	public float cost;
	public Vector3 lastMovement;

	public SampledPositionAstar (Vector3 myPos,SampledPositionAstar lastPos,float pastCost,float estCost){
		this.position=myPos;
		this.lastPosition=lastPos;
		this.pastCost=pastCost;
		this.estimatedCost=estCost;
		this.cost = pastCost+estimatedCost;
	}

	public int CompareTo(SampledPositionAstar other){
		return Mathf.FloorToInt(this.cost-other.cost);
	}
}

