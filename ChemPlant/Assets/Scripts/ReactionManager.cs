using UnityEngine;
using System.Collections.Generic;

public class ReactionManager : MonoBehaviour
{
	private ICollection<Reaction> _reactions;

	private static ReactionManager _instance = null;

	public ReactionManager()
	{
		if(_instance != null)
			throw new System.Exception("ReactionManager is a singleton!");

		_reactions = new LinkedList<Reaction>();
		_reactions.Add(new BurnHydrogen());

		_instance = this;
	}

	public ICollection<Reaction> getReactions() {return _reactions;}

	public static ReactionManager getInstance() {return _instance;}
}
