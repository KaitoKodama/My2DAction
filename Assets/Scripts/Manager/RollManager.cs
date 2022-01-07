using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RollManager : MonoBehaviour
{
    [SerializeField] Slider speedSlider;
    [SerializeField] Text speedText;
    [SerializeField] RectTransform parentCanavs;
    [SerializeField] GameObject rollTextObj;
    [SerializeField] GameObject skipObject;
    private StageNames stage;
    private float duration;

	private void Start()
	{
        skipObject.GetComponent<Button>().onClick.AddListener(() => { OnRollSkipButton(); });
        speedSlider.onValueChanged.AddListener((data) => { OnSpeedSlider(data); });
        speedSlider.maxValue = 1f;

        duration = GameManager.instance.StorySpeed;
        speedSlider.value = duration;
        speedText.text = "表示速度:" + duration.ToString("F") + "/0.01s";
    }


    //------------------------------------------
    // 登録メソッド
    //------------------------------------------
    private void OnRollSkipButton()
	{
        GameManager.instance.OnSceneTransition(stage);
    }
    private void OnSpeedSlider(float value)
	{
        duration = value;
        GameManager.instance.StorySpeed = value;
        speedText.text = "表示速度:" + value.ToString("F") + "/0.01s";
    }


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public void VirtualStart(StageNames expectStage, RollData roll)
	{
        StartCoroutine(RefleshRollSetting(expectStage, roll));
    }


    //------------------------------------------
    // ロール設定
    //------------------------------------------
    private IEnumerator RefleshRollSetting(StageNames stage, RollData roll)
	{
        this.stage = stage;

        var text = rollTextObj.GetComponent<Text>();
        var rect = rollTextObj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -10000);
        text.text = roll.RollMessege;
        yield return null;

        var baseRect = ((parentCanavs.rect.height / 2) + (rect.rect.height / 2));
        Vector2 location = new Vector2(0, baseRect * -1);
        rect.anchoredPosition = location;

        while (true)
		{
            baseRect = ((parentCanavs.rect.height / 2) + (rect.rect.height / 2));
            location.y += duration;
            rollTextObj.transform.localPosition = location;
            yield return new WaitForSeconds(0.01f);
            if (location.y >= baseRect)
			{
                break;
			}
		}

        skipObject.GetComponentInChildren<Text>().text = "クリックしてください";
        skipObject.GetComponent<Image>().DOFade(0.3f, 1f).SetLoops(-1, LoopType.Yoyo);
    }
}