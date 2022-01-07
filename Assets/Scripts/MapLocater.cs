using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MapLocater : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
	[SerializeField] FieldData data;

	private MapManager mapManager;
	private RectTransform rect;
	private Tweener tweener;
	private Image image;

	private Color color;
	private float duration = 0.5f;

	private void Start()
	{
		mapManager = GetComponentInParent<MapManager>();
		rect = GetComponent<RectTransform>();
		image = GetComponent<Image>();
		color = image.color;
	}


	//------------------------------------------
	// インターフェイス
	//------------------------------------------
	public void OnPointerDown(PointerEventData eventData)
	{
		mapManager.OnDownNotifyReciever(data);
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		mapManager.OnEnterNotifyReciever(rect, data);
		if (tweener == null)
		{
			tweener = image.DOFade(0.3f, duration).SetLoops(-1, LoopType.Yoyo);
		}
		else tweener.Restart();
	}
	public void OnPointerExit(PointerEventData eventData)
	{
		color.a = 1f;
		image.color = color;
		tweener?.Pause();
	}
}
