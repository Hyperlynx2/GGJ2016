using UnityEngine;
using System.Collections;

public class PipeManager : BaseClickable
{
	public GameObject pipeAsset; //put the Pipe script on this
	
	private static PipeManager _instance = null;
	
	public static PipeManager getInstance() {return _instance;}
	
	void Start()
	{
		if(_instance != null)
			throw new System.Exception("PipeManager is a singleton!");
		
		_instance = this;
		
		ClickManager.getInstance().registerDefaultRelease(this);
	}
	
	//called when clicking on a vessel
	public void startPipe(Vessel source)
	{
		_currentlyConnecting = Instantiate(pipeAsset).GetComponent<Pipe>();
		_currentlyConnecting.setInputVessel(source);
	}
	
	//called when un-clicking on a vessel
	public void endPipe(Vessel end)
	{
		if(_currentlyConnecting != null)
			_currentlyConnecting.setOutputVessel(end);

		_currentlyConnecting = null;
	}
	
	//default click handling:
	public override void onClickedOn() {/*nothing*/}
	public override void onClickRelease()
	{
		if(_currentlyConnecting != null)
		{
			Destroy(_currentlyConnecting.gameObject);
			_currentlyConnecting = null;
		}
	}
	
	private Pipe _currentlyConnecting = null;
}