using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonUtility;
using DG.Tweening;

public class StageManager : MonoBehaviour
{
    [SerializeField] StageData stageData;
    [SerializeField] List<Transform> enemyPositions;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] RollData rollData;
    [SerializeField] StageNames nextStage;
    [SerializeField] StagePatch stagePatch;
    [SerializeField] bool isEndingManager = false;

    private Actor actor;
    private float transitDelay = 2f;
    private int enemyNum;
    private int enemyDeathCount = 0;

	public int EnemyNum { get => enemyNum; }
	public int EnemyDeathCount { get => enemyDeathCount; }


	private void Awake()
	{
        StartCoroutine(OnInitialize());
	}

    private IEnumerator OnInitialize()
	{

        actor = GameObject.FindWithTag("Player").GetComponent<Actor>();
        actor.OnDeathNotifyerHandler = OnActorDeadReciever;

        var enemyData = stageData.GetEnemyData(GameManager.instance.StageLevel); ;
        List<Enemy> enemys = new List<Enemy>();
        enemyNum = enemyPositions.Count;
        foreach (var el in enemyPositions)
        {
            var obj = Instantiate(enemyPrefab, el.position, Quaternion.identity);
            var enemy = obj.GetComponent<Enemy>();
            enemy.Data = enemyData;
            enemy.OnDeathNotifyHandler = OnEnemyDeadReciever;
            enemys.Add(enemy);
        }

        //シーン遷移が完全に完了してからEnemyStart
        yield return new WaitUntil(() => GameManager.instance.IsSceneTransiting);
        foreach(var el in enemys)
		{
            el.EnemyStart();
		}
	}

	private void OnEnemyDeadReciever()
	{
        enemyDeathCount++;
        if(enemyDeathCount >= enemyNum)
		{
            GameManager.instance.Patch = stagePatch;
            GameManager.instance.OnStageCompleted();
            DOVirtual.DelayedCall(transitDelay, () =>
            {
                if (!isEndingManager)
                {
                    actor.enabled = false;
                    GameManager.instance.OnSceneTransition(nextStage, rollData);
                }
				else
				{
                    FindObjectOfType<EndingStageKing>().Circle.enabled = true;
                }
            });
		}
	}
    private void OnActorDeadReciever()
	{
        GameManager.instance.OnStageFailed();
        DOVirtual.DelayedCall(transitDelay, () =>
        {
            GameManager.instance.OnSceneTransition(StageNames.MapScene);
        });
    }
}
