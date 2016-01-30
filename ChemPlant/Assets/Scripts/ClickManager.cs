using UnityEngine;
using System.Collections.Generic;

/**
 * doing this as an input manager class beacuse how can individual collidables cope trigger an onrelease when the mouse isn't
 * over anything?
 */

public class ClickManager : MonoBehaviour
{
	private static ClickManager _instance = null;
	
	public class Exception : System.Exception
	{
		public Exception(string message) : base(message) {}
	}
	public static ClickManager getInstance()
	{
		if(_instance == null)
			throw new Exception("MouseManager hasn't been initialised yet. It needs to be attached as a component.");
		
		return _instance;
	}

	//if the user clicks somewhere and it doesn't hit any of the clickables, call this thing's onClickedOn()
	public void registerDefaultClick(BaseClickable clickable)
	{
		_defaultOnClickedOn = clickable;
	}
	
	//if the user releases the mouse button and it wasn't over any of the clickables, call this thing's onClickRelease()
	public void registerDefaultRelease(BaseClickable clickable)
	{
		_defaultOnClickRelease = clickable;
	}

	private BaseClickable _defaultOnClickedOn;
	private BaseClickable _defaultOnClickRelease;

	void Awake()
	{
		if(_instance != null)
			throw new Exception("MouseManager is a singleton");
		
		_instance = this;
	}

	void Update()
	{
		if(Input.GetMouseButtonDown(0)
		|| Input.GetMouseButtonUp(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();

			if(Physics.Raycast(ray, out hit, 100.0f))
			{
				BaseClickable clickable = hit.collider.GetComponent<BaseClickable>();

				if(clickable != null)
				{
					if(Input.GetMouseButtonDown(0))
						clickable.onClickedOn();
					else if(Input.GetMouseButtonUp(0))
						clickable.onClickRelease();
				}
			}			
			else
			{
				if(Input.GetMouseButtonDown(0) && _defaultOnClickedOn != null)
					_defaultOnClickedOn.onClickedOn();
				else if(Input.GetMouseButtonUp(0) && _defaultOnClickRelease != null)
					_defaultOnClickRelease.onClickRelease();
			}
			
		}	
	}
}