using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonUtility;

public class AdministratorSetting : MonoBehaviour
{
	[SerializeField] GameObject adminEnter;
	[SerializeField] GameObject secretPanel;
	[SerializeField] Button seacretButton;
	[SerializeField] InputField inputID;
	[SerializeField] InputField inputPW;

	[SerializeField] List<AnyDictionary<SRD, Slider>> sliderLists;
	[SerializeField] List<AnyDictionary<TXT, Text>> textLists;

	enum SRD { Patch, BPower, BSpeed, Speed, }
	enum TXT { Patch, BPower, BSpeed, Speed, }


	private string strID = "asuna";
	private string strPW = "elephant";

	private bool isIDCompleted = false;
	private bool isPWCompleted = false;


	private void Start()
	{
		secretPanel.SetActive(false);
		adminEnter.SetActive(false);

		seacretButton.onClick.AddListener(() => { OnButtonClicked(); });
		inputID.onValueChanged.AddListener((value) => { OnIDInput(value); });
		inputPW.onValueChanged.AddListener((value) => { OnPWInput(value); });

		Utility.GetDICVal(SRD.Patch, sliderLists).onValueChanged.AddListener((value) => { OnPatchSlide(value); });
		Utility.GetDICVal(SRD.BPower, sliderLists).onValueChanged.AddListener((value) => { OnBPowerSlide(value); });
		Utility.GetDICVal(SRD.BSpeed, sliderLists).onValueChanged.AddListener((value) => { OnBSpeedSlide(value); });
		Utility.GetDICVal(SRD.Speed, sliderLists).onValueChanged.AddListener((value) => { OnSpeedSlide(value); });

		Utility.GetDICVal(SRD.Patch, sliderLists).maxValue = ((int)StagePatch.Stage08Completed);
		Utility.GetDICVal(SRD.BPower, sliderLists).maxValue = 1000;
		Utility.GetDICVal(SRD.BSpeed, sliderLists).maxValue = 100;
		Utility.GetDICVal(SRD.Speed, sliderLists).maxValue = 50;
	}

	private void OnObserveFlags()
	{
		if(isIDCompleted && isPWCompleted)
		{
			secretPanel.SetActive(true);
			adminEnter.SetActive(false);
		}
	}

	private void OnButtonClicked()
	{
		adminEnter.SetActive(Utility.FilpFlop(adminEnter.activeSelf));
	}
	private void OnIDInput(string value)
	{
		if (value.Equals(strID)) isIDCompleted = true;
		OnObserveFlags();
	}
	private void OnPWInput(string value)
	{
		if (value.Equals(strPW)) isPWCompleted = true;
		OnObserveFlags();
	}


	private void OnPatchSlide(float value)
	{
		var patch = Utility.GetIntToEnum<StagePatch>(Mathf.RoundToInt(value));
		Utility.GetDICVal(TXT.Patch, textLists).text = "Patch : " + patch.ToString();
		GameManager.instance.Patch = patch;
	}
	private void OnBPowerSlide(float value)
	{
		Utility.GetDICVal(TXT.BPower, textLists).text = "BPower : +" + value.ToString();
		GameManager.instance.AddBPower = value;
	}
	private void OnBSpeedSlide(float value)
	{
		Utility.GetDICVal(TXT.BSpeed, textLists).text = "BSpeed : +" + value.ToString();
		GameManager.instance.AddBSpeed = value;
	}
	private void OnSpeedSlide(float value)
	{
		Utility.GetDICVal(TXT.Speed, textLists).text = "Speed : +" + value.ToString();
		GameManager.instance.AddSpeed = value;
	}
}
