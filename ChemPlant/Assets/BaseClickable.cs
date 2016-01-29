using UnityEngine;
using System.Collections.Generic;

/**
 * doing this as an input manager class beacuse how can individual collidables cope trigger an onrelease when the mouse isn't
 * over anything?
 * /

public class MouseManager : MonoBehaviour
{
	private static MouseManager _instance = null;

	public class Exception : System.Exception
	{
		public Exception(string message) : base(message) {}
	}

	public MouseManager()
	{
		if(_instance != null)
			throw new Exception("MouseManager is a singleton");

		_instance = this;
		_clickables = new LinkedList<BaseClickable>();
		_clickTarget = new Vector2();
		_clickPos = new Vector2();
	}

	public static MouseManager getInstance()
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
			
				if ((_clickTarget - clickPos).magnitude <= i.Current.clickableRadius)
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

/*TODO: the idea here is that a vessel is a clickable. on click, call pipe.startPipe. on release, call pipe.endpipe.

 pipe itself is the default onclickrelease, to tell it to stop drawing itself if the user released over nothing.*/

public abstract class BaseClickable : MonoBehaviour
{
	public abstract void onClickedOn();
	public abstract void onClickRelease();

	public float clickableRadius;
	public Vector2 clickableOffset; //TODO: or a collider, or whatever. kind of really needs to be collider if I want non-circular clickables...

	//derived class needs to call the appropriate register methods.
}
