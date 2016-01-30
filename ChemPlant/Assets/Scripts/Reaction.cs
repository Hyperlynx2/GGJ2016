using UnityEngine;
using System.Collections.Generic;

//TODO: get this to work so that I can do it in the unity editor.

public abstract class Reaction
{
	public abstract void run(ref IDictionary<Chemical, float> reactorContents, float timeslice);
}

public class BurnHydrogen : Reaction
{
	public const float RATE = 100;

	public override void run(ref IDictionary<Chemical, float> reactorContents, float timeslice)
	{
		try
		{
			float hydrogen = reactorContents[Chemical.HYDROGEN];
			float oxygen = reactorContents[Chemical.OXYGEN];

			float wannaBurnO2 = timeslice * RATE;
			if(wannaBurnO2 > oxygen)
				wannaBurnO2 = oxygen;

			float wannaBurnH = wannaBurnO2 * 2;
			if(wannaBurnH > hydrogen)
			{
				wannaBurnH = hydrogen;
				wannaBurnO2 = wannaBurnH /2;
			}

			if(wannaBurnH > 0
			&& wannaBurnO2 > 0)
			{
				reactorContents[Chemical.HYDROGEN] -= wannaBurnH;
				reactorContents[Chemical.OXYGEN] -= wannaBurnO2;

				if(!reactorContents.ContainsKey(Chemical.WATER))
				{
					reactorContents.Add(Chemical.WATER, wannaBurnO2);
				}
				else
				{
					reactorContents[Chemical.WATER] += wannaBurnO2;
				}
			}

		}
		catch(KeyNotFoundException e)
		{
			//nothing
		}
	}
}
