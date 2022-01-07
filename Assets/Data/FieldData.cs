using UnityEngine;

[CreateAssetMenu(fileName = "Field", menuName = "ScriptableObject/FieldData")]
public class FieldData : ScriptableObject
{
	[SerializeField] StagePatch requirePatch;
	[SerializeField] StageNames expectStage;
	[SerializeField] RollData rollData;
	[SerializeField] string fieldName;
	[SerializeField] [Multiline(10)] string messege;

	public string Messege { get => messege; }
	public StagePatch RequirePatch { get => requirePatch; }
	public StageNames ExpectStage { get => expectStage; }
	public string FieldName { get => fieldName;  }
	public RollData RollData { get => rollData; }
}
