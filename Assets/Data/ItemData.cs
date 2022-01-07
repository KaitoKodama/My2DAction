using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName ="ItemData", menuName = "ScriptableObject/ItemData")]
public class ItemData : ScriptableObject
{
	[SerializeField] ItemType itemType;
	[SerializeField] Sprite _sprite;
	[SerializeField] float value;
	[SerializeField] float elapse;

	public ItemType ItemType { get => itemType; }
	public Sprite Sprite { get => _sprite; }
	public float Value { get => value; }
	public float Elapse { get => elapse; }
}
