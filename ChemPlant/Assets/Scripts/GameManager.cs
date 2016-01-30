using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

/**
Responsible for the win/loss conditions for a level, both setting them in the scene and
checking them when running. Also triggers end-of-level etc.

this needs to query vessel, not have vessel check, because win conditions don't need to be
all in the same vessel!
 */

public class GameManager : MonoBehaviour
{
	public enum STATE
	{
		BRIEFING,
		PLAYING,
		WON,
		LOST

	};

	private STATE _state;

	private string _briefingText;

	public STATE getState() {return _state;} //TODO: call from Vessel or clickable w/e and prevent input if not playing

	/**
	 * have these conditions across all your vessels to win the level
	 **/
	public List<ChemPair> winConditions;

	/**
	 * have any of these conditions in any of your reactors to fail the level!
	 */
	public List<ChemPair> loseConditions;

	private LinkedList<Vessel> _vessels;	
	private static GameManager _instance = null;
	
	public static GameManager getInstance() {return _instance;}
	
	void Awake()
	{
		if(_instance != null)
			throw new System.Exception("ProductionManager is a singleton!");
		
		_instance = this;

		_vessels = new LinkedList<Vessel>();
	}

	void Start()
	{
		_state = STATE.BRIEFING;
		_briefingText = generateBriefing();
	}

	public void registerVessel(Vessel v) {_vessels.AddLast(v);}

	void Update()
	{
		switch(_state)
		{
		case STATE.WON:
			GetComponentInChildren<Canvas>().enabled = true;
			GetComponentInChildren<Text>().text = "You Won!";
			GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			GetComponentInChildren<Button>().onClick.AddListener(delegate {loadMenuScene();});
			break;

		case STATE.LOST:
			GetComponentInChildren<Canvas>().enabled = true;
			GetComponentInChildren<Text>().text = "You Lost. " + getColouredChemName(_lossReason.chemical) +  " reached " + _lossReason.quantity + ".";
			GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			GetComponentInChildren<Button>().onClick.AddListener(delegate {loadMenuScene();});

			break;
			//TODO: replace with call to the modern UI stuff.

		case STATE.PLAYING:	
			GetComponentInChildren<Canvas>().enabled = false;
			//any lose condition must be satisfied for loss, in any vessel
			IEnumerator<ChemPair> loseCondition = null;
			bool lost = false;
			IEnumerator<Vessel> v = _vessels.GetEnumerator();
			while(!lost && v.MoveNext())
			{
				IDictionary<Chemical, float> contents = v.Current.getContents();

				IEnumerator<ChemPair> c = loseConditions.GetEnumerator();
				while(!lost && c.MoveNext())
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

					while(conditionsWon < winConditions.Count && v.MoveNext())
					{
						try
						{
							if(v.Current.getContents()[condition.chemical.GetComponent<Chemical>()] >= condition.quantity)
							{
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
			break;

		case STATE.BRIEFING:
			GetComponentInChildren<Canvas>().enabled = true;
			GetComponentInChildren<Text>().text = _briefingText;
			GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			GetComponentInChildren<Button>().onClick.AddListener(delegate {resumePlay();});
			break;
		}
	}
	
	public void resumePlay()
	{
		_state = STATE.PLAYING;
	}

	public void loadMenuScene()
	{
		Application.LoadLevel(MainMenu.MENU_SCENE);
	}

	private string getColouredChemName(GameObject chem)
	{
		return "<color=\"#" +chem.GetComponent<Chemical>().colour.ToHexStringRGB() + "\">" + chem.name + "</color>";
	}

	private string generateBriefing()
	{
		string briefing = "<b>Briefing</b>\n<i>Objectives</i>:\n";

		foreach (ChemPair condition in winConditions)
		{
			briefing += "\t•" + condition.quantity + " units of " + getColouredChemName(condition.chemical)  + ".\n";
		}

		//TODO: fail conditions

		//TODO: initial conditions of vessels

		print(briefing);

		return briefing;
	}	

	private void doWinLevel()
	{
		Debug.Log("You won!");
		_state = STATE.WON;
	}

	private ChemPair _lossReason; //TODO: crappy, do beter.
	private void doLoseLevel(ChemPair reason)
	{
		_lossReason = reason;
		Debug.Log("You lost! " + reason.chemical.name + " reached " + reason.quantity);
		_state = STATE.LOST;
	}
}


/*Matt says:

Yes, ALL objects get destroyed when loading a new scene, even singletons and such. Proper destroyed - memory deallocated. So Awake and Start get called each scene.

for my manager prefab, I need to dump it into each scene/level via the editor.

for changing to another scene, you have each SCENE know which next scene to load. eg a menu scene or whatever. you don't need to use a persistent object.
 */
