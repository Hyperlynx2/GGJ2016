using UnityEngine;
using System.Collections.Generic;

public class ReactionManager : MonoBehaviour
{
	public Reaction[] reactions;

	private static ReactionManager _instance = null;
	
	public static ReactionManager getInstance() {return _instance;}

	void Start()
	{
		if(_instance != null)
			throw new System.Exception("ReactionManager is a singleton!");
		
		_instance = this;
	}
}
