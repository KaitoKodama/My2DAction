using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonUtility;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
	//------------------------------------------
	// Unityランタイム
	//------------------------------------------
	void Awake()
	{
		OnActivateSigleton();
	}
	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		InitTransitPanel();
		OnSaveDataRefleshment();
	}

	//------------------------------------------
	// シングルトン
	//------------------------------------------
	static public GameManager instance;
	private void OnActivateSigleton()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}

	//------------------------------------------
	// シーン遷移
	//------------------------------------------
	[SerializeField] GameObject rollRequestPanel;
	[SerializeField] RectTransform parentRect;
	[SerializeField] RectTransform fadeRect;
	[SerializeField] Button rollAcceptButton;
	[SerializeField] Button rollCancelButton;
	[SerializeField] Image loadingCircle;
	[SerializeField] AudioClip completedSound;
	[SerializeField] AudioClip failedSound;
	private AudioSource audioSource;
	private float fadeSpeed = 1f;
	private bool isSceneTransiting = false;
	enum RollState { None, Accepted, Canceled, }
	private RollState rollState = RollState.None;

	private void InitTransitPanel()
	{
		rollAcceptButton.onClick.AddListener(OnRollAccepeButton);
		rollCancelButton.onClick.AddListener(OnRollCancelButton);
		loadingCircle.transform.DOLocalRotate(new Vector3(0, 0, -365),2f).
			SetEase(Ease.Linear).SetRelative().SetLoops(-1, LoopType.Incremental);
	}
	public bool IsSceneTransiting { get => isSceneTransiting; }
	public void OnSceneTransition(StageNames nextStage, RollData rollData = null)
	{
		if (!isSceneTransiting)
		{
			isSceneTransiting = true;
			StartCoroutine(SceneTransition(nextStage, rollData));
		}
	}
	private IEnumerator SceneTransition(StageNames nextStage, RollData rollData)
	{
		if (rollData != null)
		{
			rollRequestPanel.SetActive(true);
			yield return new WaitUntil(() => rollState != RollState.None);
			rollRequestPanel.SetActive(false);
		}


		fadeRect.transform.DOLocalMoveX(0, fadeSpeed);
		yield return new WaitForSeconds(fadeSpeed + 1.5f);

		var loadScene = nextStage.ToString();
		if (rollState == RollState.Accepted) loadScene = StageNames.RollScene.ToString();
		SceneManager.LoadScene(loadScene);
		yield return new WaitForSeconds(1.5f);


		fadeRect.transform.DOLocalMoveX(fadeRect.rect.width * -1, fadeSpeed);
		yield return new WaitForSeconds(fadeSpeed);

		if (rollState == RollState.Accepted)
		{
			RollManager manager = FindObjectOfType<RollManager>();
			manager.VirtualStart(nextStage, rollData);
		}

		rollState = RollState.None;
		isSceneTransiting = false;
	}

	private void OnRollAccepeButton()
	{
		rollState = RollState.Accepted;
	}
	private void OnRollCancelButton()
	{
		rollState = RollState.Canceled;
	}

	//------------------------------------------
	// ミッション結果
	//------------------------------------------
	public void OnStageFailed()
	{
		audioSource.PlayOneShot(failedSound);
	}
	public void OnStageCompleted()
	{
		audioSource.PlayOneShot(completedSound);
	}

	//------------------------------------------
	// セーブデータ管理
	//------------------------------------------
	private StagePatch patch;
	private StageLevel stageLevel;
	private float storySpeed = 0.4f;
	private string stagePatchKey = "stage_patch_key";
	private string stageLevelKey = "stage_level_key";
	private string storySpeedKey = "story_speed_key";

	private void OnSaveDataRefleshment()
	{
		if (PlayerPrefs.HasKey(stagePatchKey))
		{
			int index = PlayerPrefs.GetInt(stagePatchKey);
			patch = Utility.GetIntToEnum<StagePatch>(index);
		}
		else PlayerPrefs.SetInt(stagePatchKey, ((int)StagePatch.None));

		if (PlayerPrefs.HasKey(stageLevelKey)) stageLevel = Utility.GetIntToEnum<StageLevel>(PlayerPrefs.GetInt(stageLevelKey));
		else PlayerPrefs.SetInt(stageLevelKey, ((int)StageLevel.Normal));

		if (PlayerPrefs.HasKey(storySpeedKey)) storySpeed = PlayerPrefs.GetFloat(storySpeedKey);
		else PlayerPrefs.SetFloat(storySpeedKey, storySpeed);

		PlayerPrefs.Save();
	}

	public StagePatch Patch
	{
		get { return patch; }
		set
		{
			if (((int)value) > ((int)patch))
			{
				patch = value;
				PlayerPrefs.SetInt(stagePatchKey, ((int)value));
				PlayerPrefs.Save();
			}
		}
	}
	public StageLevel StageLevel 
	{
		get { return stageLevel; }
		set
		{
			stageLevel = value;
			PlayerPrefs.SetInt(stageLevelKey, ((int)value));
			PlayerPrefs.Save();
		}
	}

	public float StorySpeed
	{
		get { return storySpeed; }
		set
		{
			storySpeed = value;
			PlayerPrefs.SetInt(storySpeedKey, ((int)value));
			PlayerPrefs.Save();
		}
	}
	public void OnResetSaveData()
	{
		PlayerPrefs.SetInt(stagePatchKey, ((int)StagePatch.None));
		PlayerPrefs.SetInt(stageLevelKey, ((int)StageLevel.Normal));
		PlayerPrefs.Save();

		patch = StagePatch.None;
		stageLevel = StageLevel.Easy;
	}

	//------------------------------------------
	// マスター
	//------------------------------------------
	private float addBPower = 0;
	private float addBSpeed = 0;
	private float addSpeed = 0;

	public float AddSpeed { get => addSpeed; set => addSpeed = value; }
	public float AddBSpeed { get => addBSpeed; set => addBSpeed = value; }
	public float AddBPower { get => addBPower; set => addBPower = value; }
}
