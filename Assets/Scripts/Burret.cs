using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burret : MonoBehaviour
{
	[SerializeField] BurretType burretType;
	enum BurretType { ActorBurret, EnemyBurret }

	private float power;
	public float Power { set => power = value; }

	private void OnBecameInvisible()
	{
		this.gameObject.SetActive(false);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (burretType)
		{
			case BurretType.ActorBurret:
				OnReserchComonent<IEnemyDamager>(collision)?.OnApplyDamage(power);
				break;
			case BurretType.EnemyBurret:
				OnReserchComonent<IActorDamager>(collision)?.OnApplyDamage(power);
				break;
		}
	}

	private T OnReserchComonent<T>(Collider2D collider)
	{
		T expect = collider.GetComponent<T>();
		if(expect != null)
		{
			this.gameObject.SetActive(false);
			return expect;
		}
		return default;
	}
}
