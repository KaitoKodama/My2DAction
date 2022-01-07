using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyData", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
	[SerializeField] float maxHealth = 30;
	[SerializeField] float burretPower = 15f;
	[SerializeField] float attackDuration = 2f;

	private float burretSpeed = 100f;
	private float speed = 15f;
	private int burretNum = 10;

	public float MaxHealth { get => maxHealth; }
	public float Speed { get => speed; }
	public float BurretSpeed { get => burretSpeed; }
	public float BurretPower { get => burretPower; }
	public float AttackDuration { get => attackDuration; }
	public int BurretNum { get => burretNum; }
}
