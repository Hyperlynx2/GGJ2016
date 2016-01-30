using UnityEngine;
using System.Collections.Generic;

public class ReactionManager : MonoBehaviour
{
	public Reaction[] 

	private static ReactionManager _instance = null;

	public ReactionManager()
	{
		if(_instance != null)
			throw new System.Exception("ReactionManager is a singleton!");

		_reactions = new LinkedList<Reaction>();

		_instance = this;
	}

	public ICollection<Reaction> getReactions() {return _reactions;}

	public static ReactionManager getInstance() {return _instance;}
}
