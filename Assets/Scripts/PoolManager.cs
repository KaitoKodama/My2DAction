using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
	public class BurretPool
	{
		internal BurretPool(GameObject obj, Rigidbody2D rigid, float power)
		{
			this.obj = obj;
			this.rigid = rigid;
			this._transform = obj.transform;

			var burret = obj.GetComponent<Burret>();
			burret.Power = power;
		}
		internal GameObject obj;
		internal Rigidbody2D rigid;
		internal Transform _transform;
	}
	[SerializeField] GameObject burretPrefab;
	private List<BurretPool> burretPools;


	public void InitBurretPool(int burretNum, float power)
	{
		burretPools = new List<BurretPool>(burretNum);
		for (int i = 0; i < burretNum; i++)
		{
			var obj = Instantiate(burretPrefab);
			var rigid =  obj.GetComponent<Rigidbody2D>();
			burretPools.Add(new BurretPool(obj, rigid, power));
			obj.SetActive(false);
		}
	}

	public BurretPool GetBurretPool()
	{
		foreach(var el in burretPools)
		{
			if (!el.obj.activeSelf)
			{
				el.obj.SetActive(true);
				return el;
			}
		}
		return null;
	}
}
