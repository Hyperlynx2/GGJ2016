using UnityEngine;
using System.Collections.Generic;
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
