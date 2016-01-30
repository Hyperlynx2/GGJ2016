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
		if(_outputVessel == null)
		{
			Vector3 pos = Input.mousePosition;
			pos.z = -Camera.main.transform.position.z;
			pos = Camera.main.ScreenToWorldPoint(pos);
			pos.z = 0;
			GetComponent<LineRenderer>().SetPosition(1, pos);
		}
	}

	/*called by PipeManager*/
	public void setInputVessel(Vessel v)
	{
		_inputVessel = v;
		GetComponent<LineRenderer>().SetPosition(0, _inputVessel.getPipeOutPoint());
	}

	/*called by PipeManager*/
	public void setOutputVessel(Vessel v)
	{
		_outputVessel = v;

		if(_inputVessel == _outputVessel)
		{
			Destroy(gameObject);
		}
		else
		{
			GetComponent<LineRenderer>().SetPosition(1, _outputVessel.getPipeInPoint());
			_inputVessel.onPipeConnected(this);
		}
	}


	private bool _isOpen;
	private Vessel _outputVessel = null;
	private Vessel _inputVessel = null;

	/*adds chemical to the destination vessel.
	returns the amount of chem left over*/
	public float pushChemical(Chemical chem, float flowRate)
	{
		//TODO:
		return 0;
	}
}
