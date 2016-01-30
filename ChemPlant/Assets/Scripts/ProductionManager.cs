using UnityEngine;
using System.Collections.Generic;

/**
Responsible for the win/loss conditions for a level, both setting them in the scene and
checking them when running. Also triggers end-of-level etc.

this needs to query vessel, not have vessel check, because win conditions don't need to be
all in the same vessel!
 */

public class ProductionManager : MonoBehaviour
{
	/**
	 * have these conditions across all your vessels to win the level
	 **/
	public List<ChemPair> winConditions;

	/**
	 * have any of these conditions in any of your reactors to fail the level!
	 */
	public List<ChemPair> loseConditions;

	public string gameOverScene;

	private LinkedList<Vessel> _vessels;	
	private static ProductionManager _instance = null;
	
	public static ProductionManager getInstance() {return _instance;}
	
	void Awake()
	{
		if(_instance != null)
			throw new System.Exception("ProductionManager is a singleton!");
		
		_instance = this;

		_vessels = new LinkedList<Vessel>();
	}

	public void registerVessel(Vessel v) {_vessels.AddLast(v);}

	void Update()
	{
		//any lose condition must be satisfied for loss, in any vessel
		IEnumerator<ChemPair> loseCondition = null;
		bool lost = false;
		IEnumerator<Vessel> v = _vessels.GetEnumerator();
		while(!lost && v.MoveNext())
		{
			IDictionary<Chemical, float> contents = v.Current.getContents();

			IEnumerator<ChemPair> c = loseConditions.GetEnumerator();
			while(!lost == null && c.MoveNext())
			{
				try
				{
					if(contents[c.Current.chemical.GetComponent<Chemical>()] >= c.Current.quantity)
					{
						loseCondition = c;
						lost = true;
					}
				}
				catch(KeyNotFoundException)
				{
					/*ok*/
				}
			}
		}

		if(lost)
		{
			doLoseLevel(loseCondition.Current);
		}
		else
		{
			//all win conditions must be satisfied for win, in any vessel
			int conditionsWon = 0;
			foreach(ChemPair condition in winConditions)
			{
				v = _vessels.GetEnumerator();
				bool conditionWon = false;

				while(conditionsWon < winConditions.Count && v.MoveNext())
				{
					try
					{
						if(v.Current.getContents()[condition.chemical.GetComponent<Chemical>()] >= condition.quantity)
						{
							conditionWon = true;
							conditionsWon++;
						}
					}
					catch(KeyNotFoundException)
					{
						/*ok*/
					}
				}
			}

			if(conditionsWon == winConditions.Count)
			{
				doWinLevel();
			}
		}
	}


	private void doWinLevel()
	{
		//TODO: win the level properly

		Debug.Log("You won!");
	}

	private void doLoseLevel(ChemPair reason)
	{
		//TODO: lose the level properly.

		Debug.Log("You lost! " + reason.chemical.name + " reached " + reason.quantity);
	}
}


/*Matt says:

Yes, ALL objects get destroyed when loading a new scene, even singletons and such. Proper destroyed - memory deallocated. So Awake and Start get called each scene.

for my manager prefab, I need to dump it into each scene/level via the editor.

for changing to another scene, you have each SCENE know which next scene to load. eg a menu scene or whatever. you don't need to use a persistent object.
 */
