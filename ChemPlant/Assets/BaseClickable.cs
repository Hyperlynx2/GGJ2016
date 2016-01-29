using UnityEngine;
using System.Collections;

public abstract class BaseClickable : MonoBehaviour
{
	public float clickableRadius;
	public Vector2 clickableOffset;

	//to not keep newing these each time:
	private Vector2 _clickTarget = new Vector2();
	private Vector2 _clickPos = new Vector2();

	// Update is called once per frame
	void Update()
	{
		Vector3 clickPos = Input.mousePosition;
		clickPos.z = Camera.main.transform.position.z;
		clickPos = Camera.main.ScreenToWorldPoint(clickPos);

		_clickPos.x = clickPos.x;
		_clickPos.y = clickPos.y;

		_clickTarget.x = transform.position.x;
		_clickTarget.y = transform.position.y;

		_clickTarget += clickableOffset;

		if ((_clickTarget - clickPos).magnitude <= clickableRadius)
		{
			onClickedOn();
		}

	}

	abstract void onClickedOn();
}
