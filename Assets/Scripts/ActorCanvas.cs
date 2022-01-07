using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonUtility;

public class ActorCanvas : MonoBehaviour
{
	[SerializeField] GameObject itemPrefsb;
	[SerializeField] Transform itemParent;
	[SerializeField] Button homeButton;
	[SerializeField] Slider healthSlider;
	[SerializeField] Slider staminaSlider;
	[SerializeField] Text leastText;

	private List<AnyDictionary<ItemWithID, GameObject>> itemLists = new List<AnyDictionary<ItemWithID, GameObject>>();
	private Actor actor;
	private StageManager stageManager;
	private float slideSpeed = 10f;

	private void Start()
	{
		stageManager = FindObjectOfType<StageManager>();
		actor = FindObjectOfType<Actor>();
		actor.OnBuffBeginNotifyerHandler = OnBuffBegin;
		actor.OnBuffEndNotifyerHandler = OnBuffEnd;

		healthSlider.maxValue = actor.MaxHealth;
		staminaSlider.maxValue = actor.MaxStamina;
		homeButton.onClick.AddListener(() => { OnHomeButton(); });

	}

	private void Update()
	{
		if (actor.enabled)
		{
			float health = Mathf.Lerp(healthSlider.value, actor.Health, Time.deltaTime * slideSpeed);
			float stamina = Mathf.Lerp(staminaSlider.value, actor.Stamina, Time.deltaTime * slideSpeed);
			healthSlider.value = Mathf.Clamp(health, 0, actor.MaxHealth);
			staminaSlider.value = Mathf.Clamp(stamina, 0, actor.MaxStamina);
		}
		else
		{
			healthSlider.value = Mathf.Lerp(healthSlider.value, 0, Time.deltaTime * slideSpeed);
			staminaSlider.value = Mathf.Lerp(staminaSlider.value, 0, Time.deltaTime * slideSpeed);
		}

		if (stageManager != null)
		{
			leastText.text = stageManager.EnemyDeathCount + "/" + stageManager.EnemyNum;
		}
	}


	//------------------------------------------
	// バフ
	//------------------------------------------
	private void OnBuffBegin(ItemWithID item)
	{
		var obj = Instantiate(itemPrefsb, itemParent);
		obj.GetComponent<Image>().sprite = item.data.Sprite;
		itemLists.Add(new AnyDictionary<ItemWithID, GameObject>(item, obj));
	}
	private void OnBuffEnd(ItemWithID item)
	{
		Utility.GetDICVal(item, itemLists).SetActive(false);
	}


	//------------------------------------------
	// ボタン
	//------------------------------------------
	private void OnHomeButton()
	{
		GameManager.instance.OnSceneTransition(StageNames.MapScene);
	}
}
