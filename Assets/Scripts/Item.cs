using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
	[SerializeField] ItemData data;
	[SerializeField] bool handlerActivate = false;

	public delegate void ItemRecievedNotifyer();
	public ItemRecievedNotifyer ItemRecievedNotifyerHandler;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		var target = collision.gameObject.GetComponent<IApplyItem>();
		if(target != null)
		{
			target.OnApplyItem(data);
			this.gameObject.SetActive(false);
			if (handlerActivate) ItemRecievedNotifyerHandler?.Invoke();
		}
	}
}
