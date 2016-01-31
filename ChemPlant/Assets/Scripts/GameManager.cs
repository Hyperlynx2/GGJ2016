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
		REFERENCE,
		PLAYING,
		WON,
		LOST

	};

	private STATE _state;

	private string _briefingText;
	private string _referenceText;

	public STATE getState() {return _state;} //TODO: call from Vessel or clickable w/e and prevent input if not playing

	/**
	 * have these conditions across all your vessels to win the level
	 **/
	public List<ChemPair> winConditions;

	/**
	 * have any of these conditions in any of your reactors to fail the level!
	 */
	public List<ChemPair> loseConditions;

	public AudioClip loseSound;
	public AudioClip winSound;

	private LinkedList<Vessel> _vessels = new LinkedList<Vessel>();	
	private static GameManager _instance = null;
	
	public static GameManager getInstance() {return _instance;}

	public Canvas outGameCanvas;
	public Canvas inGameCanvas;



	void Awake()
	{
		if(_instance != null)
			throw new System.Exception("ProductionManager is a singleton!");
		
		_instance = this;

		_vessels.Clear();
	}

	void Start()
	{
		_state = STATE.BRIEFING;
		_referenceText = generateReferenceText();
	}

	public void registerVessel(Vessel v)
	{
		_vessels.AddLast(v);
		_briefingText = generateBriefing();
	}

	void Update()
	{
		switch(_state)
		{
		case STATE.WON:
			inGameCanvas.enabled = false;
			outGameCanvas.enabled = true;
			outGameCanvas.GetComponentInChildren<Text>().text = "You Won!";
			outGameCanvas.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			outGameCanvas.GetComponentInChildren<Button>().onClick.AddListener(delegate {loadMenuScene();});
			break;

		case STATE.LOST:
			inGameCanvas.enabled = false;
			outGameCanvas.enabled = true;
			outGameCanvas.GetComponentInChildren<Text>().text = "You Lost. " + getColouredChemName(_lossReason.chemical) +  " reached " + _lossReason.quantity + ".";
			outGameCanvas.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			outGameCanvas.GetComponentInChildren<Button>().onClick.AddListener(delegate {loadMenuScene();});

			break;

		case STATE.PLAYING:	
			outGameCanvas.enabled = false;
			inGameCanvas.enabled = true;
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
			inGameCanvas.enabled = false;
			outGameCanvas.enabled = true;
			outGameCanvas.GetComponentInChildren<Text>().text = _briefingText;
			outGameCanvas.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			outGameCanvas.GetComponentInChildren<Button>().onClick.AddListener(delegate {resumePlay();});
			break;
		case STATE.REFERENCE:
			inGameCanvas.enabled = false;
			outGameCanvas.enabled = true;
			outGameCanvas.GetComponentInChildren<Text>().text = _referenceText;
			outGameCanvas.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
			outGameCanvas.GetComponentInChildren<Button>().onClick.AddListener(delegate {resumePlay();});
			break;
		}
	}
	
	public void resumePlay()
	{
		_state = STATE.PLAYING;
	}

	public void openBriefing()
	{
		_state = STATE.BRIEFING;
	}

	public void openReference()
	{
		_state = STATE.REFERENCE;
	}

	public void loadMenuScene()
	{
		Application.LoadLevel(MainMenu.MENU_SCENE);
	}

	private string getColouredChemName(Chemical chem)
	{
		return "<color=\"#" +chem.colour.ToHexStringRGB() + "\">" + chem.gameObject.name + "</color>";
	}

	private string getColouredChemName(GameObject chem)
	{
		return getColouredChemName(chem.GetComponent<Chemical>());
	}

	private string getColouredChemSymbol(Chemical chem)
	{
		return "<color=\"#" +chem.colour.ToHexStringRGB() + "\">" + chem.symbol + "</color>";
	}
	
	private string getColouredChemSymbol(GameObject chem)
	{
		return getColouredChemSymbol(chem.GetComponent<Chemical>());
	}


	private string generateReferenceText()
	{
		string reference = "";

		foreach(Reaction r in ReactionManager.getInstance().reactions)
		{
			foreach(ChemPair reagent in r.GetComponent<Reaction>().reagents)
			{
				reference += (reagent.quantity > 0?((int)reagent.quantity).ToString():"") + getColouredChemSymbol(reagent.chemical) + " ";
			}

			reference += "→ ";

			foreach(ChemPair product in r.GetComponent<Reaction>().products)
			{
				reference += (product.quantity > 0?((int)product.quantity).ToString():"") + getColouredChemSymbol(product.chemical) + " ";
			}
			reference += "\n";
		}

		return reference;
	}

	private string generateBriefing()
	{
		string briefing = "<b>Briefing</b>\n<i>Objectives</i>:\n";

		foreach (ChemPair condition in winConditions)
		{
			briefing += "\t•" + condition.quantity + " units of " + getColouredChemName(condition.chemical)  + ".\n";
		}

		briefing += "\n<i>Initial setup</i>:\n";
		int vesselNum = 1;
		foreach(Vessel v in _vessels)
		{
			briefing += "\tVessel #" + vesselNum + ", " + v.capacity + " units capacity:\n";

			if(v.getQuantity() == 0)
			{
				briefing += "\t\t•empty.\n";
			}
			else
			{
				foreach(KeyValuePair<Chemical, float> p in v.getContents())
				{
					briefing += "\t\t•" + p.Value + " units of " + getColouredChemName(p.Key) + ".\n";
				}
			}

			vesselNum++;
		}

		briefing += "\n<i>Hazards</i>:\n";

		if(loseConditions.Count == 0)
		{
			briefing += "\t•none.";
		}
		else
		{
			foreach (ChemPair condition in loseConditions)
			{
				briefing += "\t•" + condition.quantity + " units of " + getColouredChemName(condition.chemical)  + ".\n";
			}
		}

		return briefing;
	}	

	private void doWinLevel()
	{
		Debug.Log("You won!");
		AudioManager.getInstance().playOnce(winSound);
		_state = STATE.WON;
	}

	private ChemPair _lossReason; //TODO: crappy, do beter.
	private void doLoseLevel(ChemPair reason)
	{
		AudioManager.getInstance().playOnce(loseSound);
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
