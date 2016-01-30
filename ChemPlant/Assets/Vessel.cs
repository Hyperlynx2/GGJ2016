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

public class Vessel : BaseClickable
{
	public float capacity;

	/*maps chemical to quantity of that chemical*/
	private IDictionary<Chemical, float> _contents = new Dictionary<Chemical, float>();

	private ICollection<Pipe> _outputs = new LinkedList<Pipe>();

	// Use this for initialization
	void Start()
	{
		ClickManager.getInstance().registerClickable(this);
	}
	
	// Update is called once per frame
	void Update()
	{
		foreach(Reaction reaction in Reaction.getReactions())
		{
			reaction.run(_contents, Time.deltaTime);
		}
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

	//adds chemical to the contents, as much as there's room, and returns the excess.
	public float addChemical(Chemical chem, float inputQuantity)
	{
		float excess = 0;

		float totalQuantity = 0;
		foreach(float quantity in _contents.Values)
		{
			totalQuantity += quantity;
		}

		float capacityRemaining = capacity - totalQuantity;

		if(capacityRemaining < inputQuantity)
		{
			excess = inputQuantity - capacityRemaining;
			inputQuantity -= capacityRemaining;
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
