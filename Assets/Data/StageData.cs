using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage", menuName = "ScriptableObject/StageData")]
public class StageData : ScriptableObject
{
	[SerializeField] EnemyData easy;
	[SerializeField] EnemyData normal;
	[SerializeField] EnemyData hard;
	[SerializeField] EnemyData extra;

	public EnemyData GetEnemyData(StageLevel stageLevel)
	{
		switch (stageLevel)
		{
			case StageLevel.Easy:
				return easy;
			case StageLevel.Normal:
				return normal;
			case StageLevel.Hard:
				return hard;
			case StageLevel.Extra:
				return extra;
			default:
				return null;
		}
	}
}
