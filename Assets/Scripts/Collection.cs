using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//------------------------------------------
// クラス
//------------------------------------------
[System.Serializable]
public class AnyDictionary<Tkey, Tvalue>
{
	public Tkey key;
	public Tvalue value;

	public AnyDictionary(Tkey key, Tvalue value)
	{
		this.key = key;
		this.value = value;
	}
	public AnyDictionary(KeyValuePair<Tkey, Tvalue> pair)
	{
		this.key = pair.Key;
		this.value = pair.Value;
	}
}
public class ItemWithID
{
	public ItemWithID(ItemData data, int id)
	{
		this.data = data;
		this.id = id;
	}
	public ItemData data;
	public int id;
}


//------------------------------------------
// インターフェイス
//------------------------------------------
interface IActorDamager
{
	void OnApplyDamage(float damage);
}
interface IEnemyDamager
{
	void OnApplyDamage(float damage);
}
interface IApplyItem
{
	void OnApplyItem(ItemData data);
}


//------------------------------------------
// 列挙
//------------------------------------------
public enum StageNames
{
	TitleScene,
	TutorialScene,
	MapScene,
	Stage01,
	Stage02,
	Stage03,
	Stage04,
	Stage05,
	Stage06,
	Stage07,
	Stage08,
	RollScene,
}
public enum StagePatch
{
	None,
	TutorialCompleted,
	Stage01Completed,
	Stage02Completed,
	Stage03Completed,
	Stage04Completed,
	Stage05Completed,
	Stage06Completed,
	Stage07Completed,
	Stage08Completed,
}
public enum StageLevel
{
	Easy,
	Normal,
	Hard,
	Extra,
}
public enum ItemType
{
	ItemHeal,
	ItemPower,
	ItemSpeed,
	ItemStamina,
}


//------------------------------------------
// ユーティリティ
//------------------------------------------
namespace CommonUtility
{
	public static class Utility
	{
		public static TValue GetDICVal<TValue, TKey>(TKey component, List<AnyDictionary<TKey, TValue>> dics)
		{
			foreach (var dic in dics)
			{
				if (dic.key.Equals(component))
				{
					return dic.value;
				}
			}
			return default;
		}
		public static T GetNextEnum<T>(int currentEnum)
		{
			int nextIndex = currentEnum + 1;
			T nextEnum = (T)Enum.ToObject(typeof(T), nextIndex);
			int length = Enum.GetValues(typeof(T)).Length;
			if (nextIndex >= length)
			{
				nextEnum = (T)Enum.ToObject(typeof(T), 0);
			}
			return nextEnum;
		}
		public static T GetIntToEnum<T>(int targetInt)
		{
			T targetEnum = (T)Enum.ToObject(typeof(T), targetInt);
			return targetEnum;
		}
		public static Vector3 GetCirclurVec(float minRadius, float maxRadius)
		{
			Vector3 rndVec = new Vector3();
			rndVec.x = UnityEngine.Random.Range(-1f, 1f);
			rndVec.z = UnityEngine.Random.Range(-1f, 1f);
			rndVec = rndVec.normalized * UnityEngine.Random.Range(minRadius, maxRadius);
			return rndVec;
		}
		public static bool DoOnce(ref bool field, bool accept = false)
		{
			if (field) field = false;
			if (accept) field = true;
			return field;
		}
		public static bool FilpFlop(bool value)
		{
			return !value;
		}
		public static bool Probability(float fPercent)
		{
			float fProbabilityRate = UnityEngine.Random.value * 100.0f;

			if (fPercent == 100.0f && fProbabilityRate == fPercent) return true;
			else if (fProbabilityRate < fPercent) return true;
			else return false;
		}
	}
}