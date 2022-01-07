using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TOwner>
{
	public abstract class State
	{
		protected StateMachine<TOwner> StateMachine => stateMachine;
		internal StateMachine<TOwner> stateMachine;

		internal Dictionary<int, State> transition = new Dictionary<int, State>();

		protected TOwner owner => stateMachine.Owner;

		//Enter
		internal void Enter(State prevState)
		{
			OnEnter(prevState);
		}
		protected virtual void OnEnter(State prevState) { }

		//Update
		internal void Update()
		{
			OnUpdate();
		}
		protected virtual void OnUpdate() { }

		//FixedUpdate
		internal void FixedUpdate()
		{
			OnFixedUpdate();
		}
		protected virtual void OnFixedUpdate() { }

		//Exit
		internal void Exit(State nextState)
		{
			OnExit(nextState);
		}
		protected virtual void OnExit(State nextState) { }
	}

	public sealed class AnyState : State { }

	public TOwner Owner { get; }

	public State CurrentState { get; private set; }

	private LinkedList<State> states = new LinkedList<State>();

	//ステートマシンの初期化
	public StateMachine(TOwner owner)
	{
		Owner = owner;
	}

	//ステートを追加する(ジェネリック)
	public T Add<T>() where T : State, new()
	{
		var state = new T();
		state.stateMachine = this;
		states.AddLast(state);
		return state;
	}

	//特定のステートを取得なければ生成
	public T GetOrAddState<T>() where T : State, new()
	{
		foreach(var state in states)
		{
			if(state is T result)
			{
				return result;
			}
		}
		return Add<T>();
	}

	//遷移を定義する
	public void AddTransition<TFrom, TTo>(int eventId)
		where TFrom : State, new()
		where TTo : State, new()
	{
		var from = GetOrAddState<TFrom>();
		if (from.transition.ContainsKey(eventId))
		{
			throw new System.ArgumentException($"ステート{nameof(TFrom)}に対してイベントID{eventId.ToString()}の遷移は定義済みです");
		}
		var to = GetOrAddState<TTo>();
		from.transition.Add(eventId, to);
	}

	//どのステートからでも特定のステートへ遷移できるイベントを追加する
	public void AddAnyTransition<TTo>(int evenId) where TTo : State, new()
	{
		AddTransition<AnyState, TTo>(evenId);
	}

	//ステートマシンの実行を開始する
	public void Start<TFirst>() where TFirst : State, new()
	{
		Start(GetOrAddState<TFirst>());
	}

	public void Start(State firstState)
	{
		CurrentState = firstState;
		CurrentState.Enter(null);
	}
	public void Update()
	{
		CurrentState.Update();
	}
	public void FixedUpdate()
	{
		CurrentState.FixedUpdate();
	}

	//イベントを発行する
	public void Dispatch(int eventId)
	{
		State to;
		if(!CurrentState.transition.TryGetValue(eventId, out to))
		{
			if(!GetOrAddState<AnyState>().transition.TryGetValue(eventId, out to))
			{
				return;
			}
		}
		Change(to);
	}

	//ステートを変更する
	private void Change(State nextState)
	{
		CurrentState.Exit(nextState);
		nextState.Enter(CurrentState);
		CurrentState = nextState;
	}
}
