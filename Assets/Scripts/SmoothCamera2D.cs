using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera2D : MonoBehaviour
{
    [SerializeField] Vector2 offset = new Vector2(0, 1.5f);
    private Transform _transform;
    private Transform targetTransform;
    private float speed = 10f;

    void Start()
    {
        _transform = gameObject.transform;
        targetTransform = GameObject.FindWithTag("Player").transform;
    }

	private void FixedUpdate()
	{
        float x = Mathf.Lerp(_transform.position.x, targetTransform.position.x, Time.deltaTime * speed);
        float y = Mathf.Lerp(_transform.position.y, targetTransform.position.y, Time.deltaTime * speed);
        _transform.position = new Vector3(x + offset.x, y + offset.y, -10f);
	}
}
