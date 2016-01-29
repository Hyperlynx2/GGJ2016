using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		_isOpen = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		//TODO: need to check for a mouse button up that's not over a collider. probably appropriate to do it here?
	}

	//called when clicking on a vessel
	public static void startPipe(Vessel source)
	{
		_currentlyConnecting = new Pipe(); //TODO: this should probably be intantiating a pipe gameobject asset instead
		_inputVessel = source;
	}

	//called when un-clicking on a vessel
	public static void endPipe(Vessel end)
	{
		if(end == _inputVessel)
		{
			//TODO: destroy pipe gameobject
		}
		else
		{
			_currentlyConnecting._outputVessel = end;
			_inputVessel.onPipeConnected(_currentlyConnecting);
			_currentlyConnecting = null;
		}
	}

	private static Pipe _currentlyConnecting = null;

	//used  for a callback only
	private static Vessel _inputVessel = null;

	private bool _isOpen;
	private Vessel _outputVessel;

	/*adds chemical to the destination vessel.
	returns the amount of chem left over*/
	public float pushChemical(Chemical chem, float flowRate)
	{
		//TODO:
		return 0;
	}

}
