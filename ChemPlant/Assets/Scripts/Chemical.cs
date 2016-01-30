using UnityEngine;
using System.Collections.Generic;

public class Chemical : MonoBehaviour, System.IEquatable<Chemical>
{
	public string name;	
	public Color colour;

	public bool Equals(Chemical other) {return this.name.Equals(other.name);}

	private Chemical(string name, float r, float g, float b)
	{
		this.name = name; 
		colour = new Color(r, g, b);
	}

}
