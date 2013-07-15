using UnityEngine;
using System.Collections;

[System.Serializable]
public class Facility {

	[System.NonSerialized]Material mat;
	string name;
	[System.NonSerialized]Material secondMat;
	int cost;
	string materialPath;
	string secondMaterialPath;
	int capacity;
	string description;
	
	public Facility(string name, int cost, int capacity, string description, string materialPath, string secondMaterial = null)
	{
		this.name = name;
		this.cost = cost;
		this.mat = Resources.Load(materialPath) as Material;
		this.secondMat = Resources.Load(secondMaterial) as Material;
		this.materialPath = materialPath;
		this.secondMaterialPath = secondMaterial;
		this.capacity = capacity;
		this.description = description;
	}
	
	public bool isLarge()
	{
		return secondMat != null;
	}
	
	public string getName()
	{
		return name;
	}
	
	public int getCost()
	{
		return cost;
	}
	
	public Material getMaterial()
	{
		setMaterial();
		return mat;
	}

	public Material getExtensionMaterial()
	{
		return secondMat;
	}
	
	/*
	 * Note: To be used on Deserialization
	 * */
	public void setMaterial()
	{
		this.mat = Resources.Load(materialPath) as Material;
		this.secondMat = Resources.Load(secondMaterialPath) as Material;
	}
	
	public int getCapacity() {
		return capacity;
	}
	
	public string getDescription() {
		return description;
	}
}
