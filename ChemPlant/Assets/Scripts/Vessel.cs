using UnityEngine;
using System.Collections.Generic;

/*a vessel is any thing that stores reactant. 

 map of reactant to quantity.

 one output, multiple inputs.

 output, when active, sends all of the contents to the input of the next reactor.

 what about more than one output, for splitting?.... probably not going to have time for that to matter.

 ok. so, for the time being, a reactor does something to its reactants under certain conditions:

*automatically
*when the player presses the button on the reactor
*when the player turns the valve

 store references to the vessels you're linking TO. that is, the vessel your output will go to. create those links
 when connecting pipe, break those links when disconnecting pipe.

 later on, being fancy, could store those pipes as actual game entities rather than just cosmetics, but for now just reference what
 you're connecting to.

 Actually, they do have to be entities because they have to be "on" or not, for whether to send the chemicals through or not.  */

[System.Serializable]
public struct ChemPair
{
	public GameObject chemical;
	public float quantity;
}

public class Vessel : BaseClickable
{
	public float capacity;
	public List<ChemPair> startingChemicals = new List<ChemPair>();

	/*maps chemical to quantity of that chemical*/
	private IDictionary<Chemical, float> _contents = new Dictionary<Chemical, float>();

	private ICollection<Pipe> _outputs = new LinkedList<Pipe>();

	// Use this for initialization
	void Start()
	{
		foreach(ChemPair chem in startingChemicals)
		{
			_contents.Add(chem.chemical.GetComponent<Chemical>(), chem.quantity);
		}
	}

	private Dictionary<Chemical, float> _changes = new Dictionary<Chemical, float>();

	// Update is called once per frame
	void Update()
	{
		string logMessage = "<" + gameObject.name + ">\n";

		Color colour = GetComponent<Renderer>().material.color;

		colour.r = 0;
		colour.g = 0;
		colour.b = 0;

		float totalQuantity = getQuantity();
		_changes.Clear();
		foreach(Chemical chem in _contents.Keys)
		{
			_changes.Add(chem, 0);
			logMessage += "\t" + chem.name + ": " + _contents[chem] + '\n';

			foreach(Pipe pipe in _outputs)
			{
				float quantityPiped = pipe.pushChemical(chem, Mathf.Min(10, _contents[chem]));

				_changes[chem] -= quantityPiped;
			}

			colour.r += chem.colour.r * _contents[chem]/totalQuantity;
			colour.g += chem.colour.g * _contents[chem]/totalQuantity;
			colour.b += chem.colour.b * _contents[chem]/totalQuantity;
		}

		foreach(KeyValuePair<Chemical, float> change in _changes)
		{
			_contents[change.Key] += change.Value;
			if(_contents[change.Key] <= 0)
				_contents.Remove(change.Key);
		}


		GetComponent<Renderer>().material.color = colour;

		foreach(Reaction reaction in ReactionManager.getInstance().reactions)
		{
			reaction.run(ref _contents, Time.deltaTime);
		}

		logMessage += "</" + gameObject.name + ">\n";

		print (logMessage);
	}

	/*for rendering pipes.

	 ideally this would offset each output pipe, but for now just the one please*/
	public Vector3 getPipeOutPoint()
	{
		return transform.position;
	}

	public Vector3 getPipeInPoint()
	{
		return transform.position;
	}

	public override void onClickedOn()
	{
		PipeManager.getInstance().startPipe(this);
	}

	public override void onClickRelease()
	{
		PipeManager.getInstance().endPipe(this);
	}

	public void onPipeConnected(Pipe pipe)
	{
		_outputs.Add(pipe);
	}

	public float getQuantity()
	{
		float totalQuantity = 0;
		foreach(float quantity in _contents.Values)
		{
			totalQuantity += quantity;
		}

		return totalQuantity;
	}

	//adds chemical to the contents, as much as there's room, and returns the excess.
	public float addChemical(Chemical chem, float inputQuantity)
	{
		float excess = 0;

		float capacityRemaining = capacity - getQuantity();

		if(capacityRemaining < inputQuantity)
		{
			excess = inputQuantity - capacityRemaining;
			inputQuantity = capacityRemaining;
		}

		if(_contents.ContainsKey(chem))
		{
			_contents[chem] += inputQuantity;
		}
		else
		{
			_contents.Add(chem, inputQuantity);
		}

		return excess;

	}

	
}
