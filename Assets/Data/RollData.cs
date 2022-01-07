using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Roll", menuName = "ScriptableObject/RollData")]
public class RollData : ScriptableObject
{
	[SerializeField][Multiline(20)] string rollMessege;
	public string RollMessege { get => rollMessege; }
}
