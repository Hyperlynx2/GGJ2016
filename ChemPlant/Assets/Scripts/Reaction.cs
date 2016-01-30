using UnityEngine;
using System.Collections.Generic;

public class Reaction : MonoBehaviour
{
	public float rate;
	public ChemPair[] reagents;
	public ChemPair[] products;

	/*whatever reacts the least number of times defines how many reactions you are able to do. if any react zero
	times you can't do it at all.*/

	public void run(ref IDictionary<Chemical, float> reactorContents, float timeslice)
	{
		try
		{
			/*find out which chemical can react the least number of times, based on the amount in the tank and the
			demands of the formula. That dictates how many reactions can take place.*/
			int leastReactions = 0;
			foreach(ChemPair term in reagents)
			{
				int numReactions = reactorContents[term.chemical]/term.quantity;

				if(numReactions <= leastReactions)
				{
					leastReactions = numReactions;
				}
			}

			if(leastReactions > 0)
			{
				foreach(ChemPair term in reagents)
				{
					float react = Mathf.Min(rate, leastReactions) * timeslice * term.quantity;
					reactorContents[term.chemical] -= react;
				}

				//TODO: bother to check that products can't exceed capacity? have that make the vessel go boom?

				foreach(ChemPair term in products)
				{
					float react = Mathf.Min(rate, leastReactions) * timeslice * term.quantity;
					reactorContents[term.chemical] += react;
				}

			}

		}
		catch(KeyNotFoundException)
		{
			//nothing
		}
	}
}
