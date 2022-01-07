using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingStageKing : MonoBehaviour
{
	private CircleCollider2D circle;

	public CircleCollider2D Circle { get => circle;  }

	private void Start()
	{
		circle = GetComponent<CircleCollider2D>();
		circle.enabled = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player")
		{
			FindObjectOfType<EndingStageManager>().StartEndingScene();
			circle.enabled = false;
		}
	}
}
