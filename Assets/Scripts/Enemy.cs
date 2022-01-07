using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using State = StateMachine<Enemy>.State;

public class Enemy : MonoBehaviour, IEnemyDamager
{
    [SerializeField] AudioClip attackSound;
    [SerializeField] EnemyData data;
    [SerializeField] GameObject deathPrefab;
    [SerializeField] LayerMask groundMask;

    private Transform _transform;
    private Transform targetTransform;
    private PoolManager pool;
    private Rigidbody2D rigid;
    private SpriteRenderer render;
    private AudioSource audioSource;
    private StateMachine<Enemy> stateMachine;

    private Vector2 rayDirection = new Vector2(0, -1);
    private float raycastGroundDistance = 0.25f;
    private float health = 100f;
    private float coolTime = 1f;
    private bool isInitialized = false;

    public void EnemyStart()
	{
        isInitialized = true;
        pool = GetComponent<PoolManager>();
        pool.InitBurretPool(data.BurretNum, data.BurretPower);

        health = data.MaxHealth;
        _transform = gameObject.transform;
        targetTransform = GameObject.FindWithTag("Player").transform;

        rigid = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        stateMachine = new StateMachine<Enemy>(this);
        stateMachine.AddTransition<StateAttack, StateMove>(((int)Event.DoMove));
        stateMachine.AddTransition<StateDamage, StateMove>(((int)Event.DoMove));
        stateMachine.AddTransition<StateMove, StateAttack>(((int)Event.DoAttack));
        stateMachine.AddTransition<StateMove, StateDamage>(((int)Event.DoDamage));
        stateMachine.AddTransition<StateAttack, StateDamage>(((int)Event.DoDamage));
        stateMachine.AddAnyTransition<StateDeath>(((int)Event.DoDeath));
        stateMachine.Start<StateMove>();
    }

    private void Update()
    {
		if (isInitialized)
		{
            stateMachine.Update();
		}
    }
    private void FixedUpdate()
    {
        if (isInitialized)
        {
            stateMachine.FixedUpdate();
        }
    }


    //------------------------------------------
    // 内部共有関数
    //------------------------------------------
    private void FlipRenderer()
	{
        var vec = Mathf.Sign(targetTransform.position.x - _transform.position.x);
        if (vec == 1) render.flipX = false;
        else render.flipX = true;
	}
    private bool IsInAir()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, raycastGroundDistance, groundMask);
        if (hit.collider) return true;
        else return false;
    }
    private Vector2 GetDirection()
    {
        Vector2 force = (targetTransform.position - _transform.position).normalized;
        return force;
    }


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public delegate void OnDeathNotify();
    public OnDeathNotify OnDeathNotifyHandler;

	public EnemyData Data { set => data = value; }


    //------------------------------------------
    // インターフェイス
    //------------------------------------------
    public void OnApplyDamage(float value)
    {
		if (isInitialized)
		{
            health -= value;
            if (health <= 0) stateMachine.Dispatch(((int)Event.DoDeath));
            else stateMachine.Dispatch(((int)Event.DoDamage));
		}
    }


    //------------------------------------------
    // ステートマシン
    //------------------------------------------
    private enum Event : int
    {
        DoMove, DoAttack, DoDamage, DoDeath,
    }
    private class StateMove : State
    {
        float chaseDistance = 20f;
        float attackDistance = 15f;
        float distance = 1000;
		protected override void OnUpdate()
		{
            distance = Vector2.Distance(owner.targetTransform.position, owner._transform.position);
            owner.FlipRenderer();
        }
		protected override void OnFixedUpdate()
        {
            if (distance <= chaseDistance)
            {
                owner.rigid.AddForce(owner.GetDirection() * owner.data.Speed);
                if (distance <= attackDistance)
                {
                    stateMachine.Dispatch(((int)Event.DoAttack));
                }
            }
            else owner.rigid.velocity = Vector2.zero;
        }
    }
    private class StateAttack : State
    {
        float time = 0f;
        protected override void OnEnter(State prevState)
        {
            var target = owner.pool.GetBurretPool();
            if (target != null)
            {
                owner.audioSource.PlayOneShot(owner.attackSound);
                target._transform.position = owner._transform.position;
                target.rigid.velocity = Vector2.zero;
                target.rigid.AddForce(owner.GetDirection() * owner.data.BurretSpeed);
            }
            time = 0f;
        }
		protected override void OnUpdate()
		{
            time += Time.deltaTime;
            if (time >= owner.data.AttackDuration)
			{
                stateMachine.Dispatch(((int)Event.DoMove));
			}
            else owner.FlipRenderer();
        }
		protected override void OnFixedUpdate()
        {
            owner.rigid.AddForce(owner.GetDirection() * owner.data.Speed);
        }
    }
    private class StateDamage : State
    {
        protected override void OnEnter(State prevState)
        {
            float expect = owner.health / owner.data.MaxHealth;
            owner.render.DOColor(new Color(1, expect, expect, 1), owner.coolTime);
            owner._transform.DOShakePosition(owner.coolTime, 0.5f).OnComplete(() =>
            {
                stateMachine.Dispatch(((int)Event.DoMove));
            });
        }
    }
    private class StateDeath : State
    {
        protected override void OnEnter(State prevState)
        {
            owner.rigid.gravityScale = 5f;
        }
		protected override void OnUpdate()
		{
            if (!owner.IsInAir() && owner.enabled) 
			{
                Instantiate(owner.deathPrefab, owner._transform.position, Quaternion.identity);
                owner.OnDeathNotifyHandler?.Invoke();
                owner.gameObject.SetActive(false);
                owner.enabled = false;
			}
		}
	}
}
