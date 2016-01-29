using UnityEngine;
using System.Collections.Generic;

//TODO: get this to work so that I can do it in the unity editor.

public abstract class Reaction
{
	public static ICollection<Reaction> getReactions() {return _reactions;}

	private static LinkedList<Reaction> _reactions;

	public abstract void run(IDictionary<Chemical, float> contents, float timeslice);

	protected Reaction()
	{
		_reactions.AddLast(this);
	}
}


public class BurnHydrogen : Reaction
{
	public const float RATE = 100;

	public void run(IDictionary<Chemical, float> contents, float timeslice)
	{
		try
		{
			float hydrogen = contents[Chemical.HYDROGEN];
			float oxygen = contents[Chemical.OXYGEN];

			float wannaBurnO2 = 1 * timeslice * RATE;
			float wannaBurnH = 2 * timeslice * RATE;

			//nooo wrong, it's allowed to burn less than the full rate/second if there isn't that much in the tank
			if(wannaBurnO2 <= oxygen
			&& wannaBurnH <= hydrogen)
			{
				contents[Chemical.HYDROGEN] -= wannaBurnH;
				contents[Chemical.OXYGEN] -= wannaBurnO2;

				if(contents.ContainsKey(Chemical.WATER))
				{
					contents.Add(Chemical.WATER, wannaBurnO2);
				}
			}

		}
		catch(KeyNotFoundException e)
		{
			//nothing
		}
	}
}