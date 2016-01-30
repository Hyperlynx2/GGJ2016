using UnityEngine;
using System.Collections.Generic;

public class Chemical : System.IEquatable<Chemical>
{
	public string name;
	public Color colour;

	public bool Equals(Chemical other) {return this.name.Equals(other.name);}

	private Chemical(string name, float r, float g, float b)
	{
		this.name = name; 
		colour = new Color(r, g, b);
	}


	public static Chemical HYDROGEN = new Chemical("Hydrogen", 255, 0, 0);
	public static Chemical OXYGEN = new Chemical("Hydrogen", 0, 0, 255);
	public static Chemical WATER = new Chemical("Water", 0, 100, 255);
}
