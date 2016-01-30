using UnityEngine;
using System.Collections.Generic;

public class Chemical : MonoBehaviour, System.IEquatable<Chemical>
{
	public string symbol;	
	public Color colour;
	public AudioClip flowSound;

	public bool Equals(Chemical other) {return this.name.Equals(other.name);}

	private Chemical(string symbol, float r, float g, float b)
	{
		this.symbol = symbol; 
		colour = new Color(r, g, b);
	}

}
