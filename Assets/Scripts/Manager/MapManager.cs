using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CommonUtility;

public class MapManager : MonoBehaviour
{
	[SerializeField] AudioClip hoverSound;
	[SerializeField] AudioClip conformSound;
	[SerializeField] AudioClip denySound;
	[SerializeField] Text fieldText;
	[SerializeField] GameObject storyTarget;
	[SerializeField] GameObject locateTarget;
	[SerializeField] GameObject downArrow;
	private AudioSource audioSource;
	private float duration = 0.5f;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		locateTarget.SetActive(false);
		downArrow.transform.DOLocalMoveY(downArrow.transform.localPosition.y + 10, duration).SetLoops(-1, LoopType.Yoyo);

		if (GameManager.instance.Patch >= StagePatch.Stage08Completed) storyTarget.SetActive(true);
		else storyTarget.SetActive(false);
	}


	public void OnEnterNotifyReciever(RectTransform rect, FieldData data)
	{
		audioSource.PlayOneShot(hoverSound);
		if (!locateTarget.activeSelf) locateTarget.SetActive(true);
		locateTarget.transform.DOMove(rect.position, duration);

		if (((int)GameManager.instance.Patch) >= ((int)data.RequirePatch)) 
		{
			fieldText.text = data.FieldName + "\n" + data.Messege;
		}
		else
		{
			fieldText.text = data.FieldName + "\nここにはまだ入れないようだ";
		}
	}
	public void OnDownNotifyReciever(FieldData data)
	{
		if (((int)GameManager.instance.Patch) >= ((int)data.RequirePatch))
		{
			audioSource.PlayOneShot(conformSound);
			GameManager.instance.OnSceneTransition(data.ExpectStage, data.RollData);
		}
		else audioSource.PlayOneShot(denySound);
	}
}
