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
	
	public ClickManager()
	{
		if(_instance != null)
			throw new Exception("MouseManager is a singleton");
		
		_instance = this;
		_clickables = new LinkedList<BaseClickable>();
		_clickTarget = new Vector2();
		_clickPos = new Vector2();
	}
	
	public static ClickManager getInstance()
	{
		if(_instance == null)
			throw new Exception("MouseManager hasn't been initialised yet. It needs to be attached as a component.");
		
		return _instance;
	}
	
	public void registerClickable(BaseClickable clickable)
	{
		_clickables.AddLast(clickable);
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
	
	private LinkedList<BaseClickable> _clickables;
	private BaseClickable _defaultOnClickedOn;
	private BaseClickable _defaultOnClickRelease;
	
	//to not keep newing these each time:
	private Vector2 _clickTarget;
	private Vector2 _clickPos;
	
	void Update()
	{
		if(Input.GetMouseButtonDown(0)
		|| Input.GetMouseButtonUp(0))
		{
			Vector3 clickPos = Input.mousePosition;
			clickPos.z = Camera.main.transform.position.z;
			clickPos = Camera.main.ScreenToWorldPoint(clickPos);
			
			_clickPos.x = clickPos.x;
			_clickPos.y = clickPos.y;
			
			bool found = false;
			IEnumerator<BaseClickable> i = _clickables.GetEnumerator();
			while(i.MoveNext() && !found)
			{
				_clickTarget.x = i.Current.transform.position.x;
				_clickTarget.y = i.Current.transform.position.y;
				
				_clickTarget += i.Current.clickableOffset;
				
				if ((_clickTarget - _clickPos).magnitude <= i.Current.clickableRadius)
				{
					found = true;
					if(Input.GetMouseButtonDown(0))
						i.Current.onClickedOn();
					else if(Input.GetMouseButtonUp(0))
						i.Current.onClickRelease();
				}
			}
			
			if(!found)
			{
				if(Input.GetMouseButtonDown(0) && _defaultOnClickedOn != null)
					_defaultOnClickedOn.onClickedOn();
				else if(Input.GetMouseButtonUp(0) && _defaultOnClickRelease != null)
					_defaultOnClickRelease.onClickRelease();
			}
			
		}	
	}
}