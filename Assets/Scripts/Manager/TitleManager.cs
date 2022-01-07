using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonUtility;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
	[SerializeField] RollData rollData;
	[SerializeField] AudioClip clickSound;
	[SerializeField] List<AnyDictionary<TXT, Text>> textLists;
	[SerializeField] List<AnyDictionary<BTN, Button>> buttonLists;
	[SerializeField] List<AnyDictionary<OBJ, GameObject>> objectLists;

	private AudioSource audioSource;

	enum BTN
	{
		StartButton, SettingButton, 
		LevelButton, PolicyButton, ResetButton,
		LevelEasyButton, LevelNormalButton, LevelHardButton, LevelExtraButton, InitConformButton,
	}
	enum TXT
	{
		TapRequireText, MyLevelText, ResetCompleteText, 
		InitCompletedText,
	}
	enum OBJ{ SettingPanelObj, PolicyScrollObj, LevelPanelObj, InitPanelObj, }


	private void Start()
	{
		audioSource = GetComponent<AudioSource>();

		//テキストリクエスト
		Utility.GetDICVal(TXT.TapRequireText, textLists).DOFade(0.3f, 1f).SetLoops(-1, LoopType.Yoyo);
		Utility.GetDICVal(TXT.MyLevelText, textLists).text = "難易度 : " + GameManager.instance.StageLevel.ToString();

		//ボタン役割登録
		Utility.GetDICVal(BTN.StartButton, buttonLists).onClick.AddListener(() => { OnStartButton(); });
		Utility.GetDICVal(BTN.SettingButton, buttonLists).onClick.AddListener(() => { OnSettingButton(); });

		Utility.GetDICVal(BTN.LevelButton, buttonLists).onClick.AddListener(() => { OnLevelButton(); });
		Utility.GetDICVal(BTN.PolicyButton, buttonLists).onClick.AddListener(() => { OnPolicyButton(); });
		Utility.GetDICVal(BTN.ResetButton, buttonLists).onClick.AddListener(() => { OnResetButton(); });

		Utility.GetDICVal(BTN.LevelEasyButton, buttonLists).onClick.AddListener(() => { OnLevelSwitchButton(((int)StageLevel.Easy)); });
		Utility.GetDICVal(BTN.LevelNormalButton, buttonLists).onClick.AddListener(() => { OnLevelSwitchButton(((int)StageLevel.Normal)); });
		Utility.GetDICVal(BTN.LevelHardButton, buttonLists).onClick.AddListener(() => { OnLevelSwitchButton(((int)StageLevel.Hard)); });
		Utility.GetDICVal(BTN.LevelExtraButton, buttonLists).onClick.AddListener(() => { OnLevelSwitchButton(((int)StageLevel.Extra)); });
		Utility.GetDICVal(BTN.InitConformButton, buttonLists).onClick.AddListener(() => { OnInitComformButton(); });
	}

	//------------------------------------------
	// トップボタン
	//------------------------------------------
	private void OnStartButton()
	{
		audioSource.PlayOneShot(clickSound);
		var manager = GameManager.instance;
		var patch = manager.Patch;

		if (patch < StagePatch.TutorialCompleted)
		{
			manager.OnSceneTransition(StageNames.TutorialScene, rollData);
		}
		else
		{
			manager.OnSceneTransition(StageNames.MapScene);
		}
	}
	private void OnSettingButton()
	{
		audioSource.PlayOneShot(clickSound);
		var target = Utility.GetDICVal(OBJ.SettingPanelObj, objectLists);
		target.SetActive(Utility.FilpFlop(target.activeSelf));
	}


	//------------------------------------------
	// セカンドボタン
	//------------------------------------------
	private void OnLevelButton()
	{
		audioSource.PlayOneShot(clickSound);
		Utility.GetDICVal(OBJ.PolicyScrollObj, objectLists).SetActive(false);
		Utility.GetDICVal(OBJ.InitPanelObj, objectLists).SetActive(false);
		var target = Utility.GetDICVal(OBJ.LevelPanelObj, objectLists);
		target.SetActive(Utility.FilpFlop(target.activeSelf));
	}
	private void OnPolicyButton()
	{
		audioSource.PlayOneShot(clickSound);
		Utility.GetDICVal(OBJ.LevelPanelObj, objectLists).SetActive(false);
		Utility.GetDICVal(OBJ.InitPanelObj, objectLists).SetActive(false);
		var target = Utility.GetDICVal(OBJ.PolicyScrollObj, objectLists);
		target.SetActive(Utility.FilpFlop(target.activeSelf));
	}
	private void OnResetButton()
	{
		audioSource.PlayOneShot(clickSound);
		Utility.GetDICVal(OBJ.PolicyScrollObj, objectLists).SetActive(false);
		Utility.GetDICVal(OBJ.LevelPanelObj, objectLists).SetActive(false);
		var target = Utility.GetDICVal(OBJ.InitPanelObj, objectLists);
		target.SetActive(Utility.FilpFlop(target.activeSelf));
	}


	//------------------------------------------
	// サードボタン
	//------------------------------------------
	private void OnLevelSwitchButton(int index)
	{
		audioSource.PlayOneShot(clickSound);
		StageLevel level =  Utility.GetIntToEnum<StageLevel>(index);
		GameManager.instance.StageLevel = level;
		Utility.GetDICVal(TXT.MyLevelText, textLists).text = "難易度 : " + level.ToString();
	}
	private void OnInitComformButton()
	{
		audioSource.PlayOneShot(clickSound);
		Utility.GetDICVal(TXT.InitCompletedText, textLists).text = "正常に初期化されました";
		GameManager.instance.OnResetSaveData();
	}
}