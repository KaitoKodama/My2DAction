using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonUtility;
using DG.Tweening;
using State = StateMachine<TutorialManager>.State;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] GameObject enemy;
	[SerializeField] GameObject item;
	[SerializeField] CanvasGroup group;
	[SerializeField] CanvasGroup keybordGroup;
	[SerializeField] CanvasGroup mouseGroup;
	[SerializeField] CanvasGroup itemGoup;
	[SerializeField] Text explainText;
	[SerializeField] List<AnyDictionary<IMG, Image>> imageLists;

	private Actor actor;
	private StateMachine<TutorialManager> stateMachine;
	private float fadeDuration = 1f;
	private float textDuration = 1f;

	enum IMG
	{
		Keybord, MouseL, 
	}

	void Start()
    {
		actor = GameObject.FindWithTag("Player").GetComponent<Actor>();

		//デリゲート登録
		actor.OnDeathNotifyerHandler = OnActorDeath;
		enemy.GetComponent<Enemy>().OnDeathNotifyHandler = OnEnemyDeathReciever;
		item.GetComponent<Item>().ItemRecievedNotifyerHandler = OnItemReciever;
		item.SetActive(false);
		enemy.SetActive(false);

		stateMachine = new StateMachine<TutorialManager>(this);
		stateMachine.AddTransition<StateWASD, StateLCkick>(((int)Event.RequireLClick));
		stateMachine.AddTransition<StateLCkick, StateEmpty>(((int)Event.RequireEmptyStamina));
		stateMachine.AddTransition<StateEmpty, StateItemRequire>(((int)Event.RequireGetItem));
		stateMachine.AddTransition<StateItemRequire, StateKillEnemy>(((int)Event.RequireToKillEnemy));
		stateMachine.AddTransition<StateKillEnemy, StateComplete>(((int)Event.CompleteAll));
		stateMachine.Start<StateWASD>();
	}

	private void Update()
	{
		stateMachine.Update();
	}


	//------------------------------------------
	// 内部共有関数
	//------------------------------------------


	//------------------------------------------
	// デリゲート通知
	//------------------------------------------
	private void OnEnemyDeathReciever()
	{
		stateMachine.Dispatch(((int)Event.CompleteAll));
	}
	private void OnItemReciever()
	{
		stateMachine.Dispatch(((int)Event.RequireToKillEnemy));
	}
	private void OnActorDeath()
	{
		GameManager.instance.OnStageFailed();
		GameManager.instance.OnSceneTransition(StageNames.TitleScene);
	}

	//------------------------------------------
	// ステートマシン
	//------------------------------------------
	enum Event : int
	{
		RequireLClick, RequireEmptyStamina, RequireGetItem, RequireToKillEnemy, CompleteAll,
	}
	private class StateWASD : State
	{
		List<AnyDictionary<REQ, bool>> dispatchList = new List<AnyDictionary<REQ, bool>>()
		{
			new AnyDictionary<REQ, bool>(REQ.WKey, false),
			new AnyDictionary<REQ, bool>(REQ.SKey, false),
			new AnyDictionary<REQ, bool>(REQ.AKey, false),
			new AnyDictionary<REQ, bool>(REQ.DKey, false),
		};
		enum REQ { WKey, AKey, SKey, DKey, }

		protected override void OnEnter(State prevState)
		{
			owner.keybordGroup.DOFade(1, owner.fadeDuration);
			owner.explainText.text = "";
			owner.explainText.DOText("WSキーで上下、ADキーで左右に移動できます\nWASDキーで移動を行ってください", owner.textDuration);
			Utility.GetDICVal(IMG.Keybord, owner.imageLists).DOFade(1f, owner.fadeDuration).SetLoops(-1, LoopType.Yoyo);
		}
		protected override void OnUpdate()
		{
			if (DispatchEnabled()) stateMachine.Dispatch(((int)Event.RequireLClick));
			else
			{
				if (Input.GetKeyDown(KeyCode.W)) dispatchList[0].value = true;
				if (Input.GetKeyDown(KeyCode.S)) dispatchList[1].value = true;
				if (Input.GetKeyDown(KeyCode.A)) dispatchList[2].value = true;
				if (Input.GetKeyDown(KeyCode.D)) dispatchList[3].value = true;
			}

		}
		protected override void OnExit(State nextState)
		{
			owner.keybordGroup.DOFade(0, owner.fadeDuration);
		}

		private bool DispatchEnabled()
		{
			int trueNum = 0;
			foreach(var el in dispatchList)
			{
				if (el.value == true) trueNum++;
			}
			if (trueNum >= dispatchList.Count) return true;
			else return false;
		}
	}
	private class StateLCkick : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.mouseGroup.DOFade(1, owner.fadeDuration);
			owner.explainText.text = "";
			owner.explainText.DOText("マウスを左クリックで攻撃ができます、左クリックで攻撃を行ってください", owner.textDuration);
			Utility.GetDICVal(IMG.MouseL, owner.imageLists).DOFade(1f, owner.fadeDuration).SetLoops(-1, LoopType.Yoyo);
		}
		protected override void OnUpdate()
		{
			if (Input.GetMouseButtonDown(0))
			{
				stateMachine.Dispatch(((int)Event.RequireEmptyStamina));
			}
		}
		protected override void OnExit(State nextState)
		{
			owner.mouseGroup.DOFade(0, owner.fadeDuration);
		}
	}
	private class StateEmpty : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.explainText.text = "";
			owner.explainText.DOText("上下移動でスタミナが減ります\n上下移動でスタミナを半分まで減らしてください", owner.textDuration);
		}
		protected override void OnUpdate()
		{
			if (owner.actor.Stamina <= owner.actor.MaxStamina / 2) 
			{
				stateMachine.Dispatch(((int)Event.RequireGetItem));
			}
		}
	}
	private class StateItemRequire : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.item.SetActive(true);
			owner.itemGoup.DOFade(1, owner.fadeDuration);
			owner.explainText.text = "";
			owner.explainText.DOText("オレンジ色の靄がアイテムです、触れると効力を受けられます。\nアイテムに触れてください", owner.textDuration);
		}
		protected override void OnExit(State nextState)
		{
			owner.itemGoup.DOFade(0, owner.fadeDuration);
		}
	}
	private class StateKillEnemy : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.explainText.text = "";
			owner.explainText.DOText("敵の吐く黒い球に当たるとダメージを受けるので\n移動して回避、攻撃して敵を倒してください", owner.textDuration).OnComplete(() =>
			{
				owner.group.DOFade(0, owner.fadeDuration).OnComplete(() =>
				{
					owner.enemy.SetActive(true);
					owner.enemy.GetComponent<Enemy>().EnemyStart();
				});
			});
		}
	}
	private class StateComplete : State
	{
		protected override void OnEnter(State prevState)
		{
			owner.actor.enabled = false;
			GameManager.instance.OnStageCompleted();
			GameManager.instance.Patch = StagePatch.TutorialCompleted;
			DOVirtual.DelayedCall(3f, () =>
			{
				GameManager.instance.OnSceneTransition(StageNames.MapScene);
			});
		}
	}
}
