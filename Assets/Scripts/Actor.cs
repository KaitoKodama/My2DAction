using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CommonUtility;
using State = StateMachine<Actor>.State;

public class Actor : MonoBehaviour, IActorDamager, IApplyItem
{
    [SerializeField] GameObject flyEffect;
    [SerializeField] GameObject healEffect;
    [SerializeField] GameObject deathPrefab;
    [SerializeField] AudioSource subAudioFly;
    [SerializeField] AudioSource subAudioRun;
    [SerializeField] AudioClip buffSound;
    [SerializeField] AudioClip[] attackSounds;
    [SerializeField] AudioClip[] damageSounds;
    [SerializeField] LayerMask groundMask;

    private Transform _transform;
    private Transform cameraTransform;
    private Rigidbody2D rigid;
    private Animator animator;
    private PoolManager pool;
    private AudioSource audioSource;
    private StateMachine<Actor> stateMachine;

    private readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private readonly int VerticalHash = Animator.StringToHash("Vertical");
    private readonly int IsGroundHash = Animator.StringToHash("IsGround");
    private readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private readonly int IsDamageHash = Animator.StringToHash("IsDamage");
    private readonly int IsDeathHash = Animator.StringToHash("IsDeath");

    private Vector2 rayDirection = new Vector2(0, -1);
    private float raycastGroundDistance = 0.25f;

    private float staminaIncreaseSpeed = 10f;
    private float staminaDecreaseSpeed = 25f;
    private float burretSpeed = 100f;
    private float burretPower = 10f;
    private float maxHealth, maxStamina;
    private float horizontal, vertical;
    private float volumingSpeed = 5f;
    private float health = 100f;
    private float stamina = 100f;
    private float speed = 35f;
    private int itemId = 0;


	private void Awake()
	{
        //チートデータ
        speed += GameManager.instance.AddSpeed;
        burretSpeed += GameManager.instance.AddBSpeed;
        burretPower += GameManager.instance.AddBPower;

        //初期化
        maxHealth = health;
        maxStamina = stamina;

        pool = GetComponent<PoolManager>();
        pool.InitBurretPool(10, burretPower);
	}
	void Start()
    {

        //キャッシュ
        cameraTransform = Camera.main.transform;
        _transform = gameObject.transform;
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();

        //ステートマシン
        stateMachine = new StateMachine<Actor>(this);
        stateMachine.AddTransition<StateMove, StateAttack>(((int)Event.DoAttack));
        stateMachine.AddTransition<StateMove, StateDamage>(((int)Event.DoDamage));
        stateMachine.AddTransition<StateMove, StateDeath>(((int)Event.DoDeath));
        stateMachine.AddAnyTransition<StateMove>(((int)Event.DoMove));
        stateMachine.Start<StateMove>();
    }

    private void Update()
    {
        stateMachine.Update();
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }
	private void FixedUpdate()
	{
        stateMachine.FixedUpdate();
	}

	private void OnDisable()
	{
        subAudioFly.volume = 0;
        subAudioRun.volume = 0;
        flyEffect.SetActive(false);
        animator.SetBool(IsGroundHash, true);
        animator.SetFloat(HorizontalHash, 0f);
        animator.SetFloat(VerticalHash, 0f);
    }

	//------------------------------------------
	// 内部共有関数
	//------------------------------------------
	private bool IsInAir()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection, raycastGroundDistance, groundMask);
        if (hit.collider) return false;
        else return true;
    }
    private Vector2 GetDirection()
	{
        //スタミナ、エフェクト
        float value;
        if (Math.Sign(vertical) == 0)
		{
            if (flyEffect.activeSelf) flyEffect.SetActive(false);
            value = stamina + (staminaIncreaseSpeed * Time.deltaTime);
        }
		else
		{
            if (!flyEffect.activeSelf && stamina > 0) flyEffect.SetActive(true);
            value = stamina - (staminaDecreaseSpeed * Time.deltaTime);
        }
        stamina = Mathf.Clamp(value, 0, MaxStamina);

        //付随するオーディオ
        if (stamina > 0)
        {
            float leapedVolume = Mathf.Lerp(subAudioFly.volume, Mathf.Abs(vertical), Time.deltaTime * volumingSpeed);
            subAudioFly.volume = Mathf.Clamp(leapedVolume, 0, 0.5f);
        }
        else subAudioFly.volume = 0;

        //ベクトル算出
        Vector2 force = new Vector2(horizontal, vertical).normalized * speed;
        if (stamina <= 0) force.y = 0;

        return force;
    }
    private void FlipDirection()
	{
        int dir = Math.Sign(horizontal);
        if (dir == -1) _transform.rotation = Quaternion.Euler(0, 180, 0);
        else if (dir == 1) _transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    private AudioClip GetRandomClip(AudioClip[] clips)
	{
        var clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        return clip;
	}


    //------------------------------------------
    // 外部共有関数
    //------------------------------------------
    public delegate void OnBuffBeginNotifyer(ItemWithID item);
    public delegate void OnBuffEndNotifyer(ItemWithID item);
    public delegate void OnDeathNotifyer();
    public OnBuffBeginNotifyer OnBuffBeginNotifyerHandler;
    public OnBuffEndNotifyer OnBuffEndNotifyerHandler;
    public OnDeathNotifyer OnDeathNotifyerHandler;

	public float Health { get => health; }
	public float Stamina { get => stamina; }
	public float MaxHealth { get => maxHealth; }
	public float MaxStamina { get => maxStamina; }


	//------------------------------------------
	// アニメーションイベント
	//------------------------------------------
	public void OnStateMachineExit()
	{
        stateMachine.Dispatch(((int)Event.DoMove));
	}
    public void OnAttack()
	{
        var target = pool.GetBurretPool();
        if (target != null)
        {
            audioSource.PlayOneShot(GetRandomClip(attackSounds));
            target._transform.position = _transform.position;
            target.rigid.velocity = Vector2.zero;
            target.rigid.AddForce(_transform.right * burretSpeed);
        }
    }

    //------------------------------------------
    // インターフェイス
    //------------------------------------------
    public void OnApplyDamage(float value)
	{
		if (enabled)
		{
            health -= value;
            cameraTransform.DOShakePosition(0.8f, 0.6f);
            audioSource.PlayOneShot(GetRandomClip(damageSounds));
            if (health <= 0) stateMachine.Dispatch(((int)Event.DoDeath));
            else stateMachine.Dispatch(((int)Event.DoDamage));
		}
    }

	public void OnApplyItem(ItemData data)
	{
		if (enabled)
		{
            itemId++;
            audioSource.PlayOneShot(buffSound);
            healEffect.SetActive(false);
            healEffect.SetActive(true);
            switch (data.ItemType)
		    {
                case ItemType.ItemHeal:
                    StartCoroutine(OnHealBuffBegin(new ItemWithID(data, itemId)));
                    break;
                case ItemType.ItemPower:
                    StartCoroutine(OnPowerBuffBegin(new ItemWithID(data, itemId)));
                    break;
                case ItemType.ItemSpeed:
                    StartCoroutine(OnSpeedBuffBegin(new ItemWithID(data, itemId)));
                    break;
                case ItemType.ItemStamina:
                    StartCoroutine(OnStaminaBuffBegin(new ItemWithID(data, itemId)));
                    break;
		    }
		}
    }


    //------------------------------------------
    // コルーチン
    //------------------------------------------
    // バフ
    private IEnumerator OnHealBuffBegin(ItemWithID item)
    {
        health = Mathf.Clamp(health + item.data.Value, 0, maxHealth);

        OnBuffBeginNotifyerHandler?.Invoke(item);
        health += item.data.Value;

        yield return new WaitForSeconds(item.data.Elapse);

        OnBuffEndNotifyerHandler?.Invoke(item);
    }
    private IEnumerator OnStaminaBuffBegin(ItemWithID item)
    {
        OnBuffBeginNotifyerHandler?.Invoke(item);
        staminaIncreaseSpeed += item.data.Value;

        yield return new WaitForSeconds(item.data.Elapse);

        staminaIncreaseSpeed -= item.data.Value;
        OnBuffEndNotifyerHandler?.Invoke(item);
    }
    private IEnumerator OnPowerBuffBegin(ItemWithID item)
    {
        OnBuffBeginNotifyerHandler?.Invoke(item);
        burretPower += item.data.Value;

        yield return new WaitForSeconds(item.data.Elapse);

        burretPower -= item.data.Value;
        OnBuffEndNotifyerHandler?.Invoke(item);
    }
    private IEnumerator OnSpeedBuffBegin(ItemWithID item)
    {
        OnBuffBeginNotifyerHandler?.Invoke(item);
        speed += item.data.Value;

        yield return new WaitForSeconds(item.data.Elapse);

        speed -= item.data.Value;
        OnBuffEndNotifyerHandler?.Invoke(item);
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
        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) 
            {
                stateMachine.Dispatch(((int)Event.DoAttack));
            }

            var isInAir = owner.IsInAir();
            if (!isInAir)
			{
                float leapedVolume = Mathf.Lerp(owner.subAudioRun.volume, Mathf.Abs(owner.horizontal), Time.deltaTime * owner.volumingSpeed);
                owner.subAudioRun.volume = Mathf.Clamp(leapedVolume, 0, 0.3f);
            }
            else owner.subAudioRun.volume = 0;
            owner.animator.SetBool(owner.IsGroundHash, !isInAir);
            owner.animator.SetFloat(owner.HorizontalHash, owner.horizontal);
            owner.animator.SetFloat(owner.VerticalHash, owner.vertical);
        }
        protected override void OnFixedUpdate()
		{
            owner.FlipDirection();
            owner.rigid.AddForce(owner.GetDirection());
        }
	}
    private class StateAttack : State
	{
        protected override void OnEnter(State prevState)
        {
            owner.animator.SetBool(owner.IsAttackHash, true);
        }
        protected override void OnFixedUpdate()
        {
            owner.rigid.AddForce(owner.GetDirection());
        }
        protected override void OnExit(State nextState)
		{
            owner.animator.SetBool(owner.IsAttackHash, false);
        }
	}
    private class StateDamage : State
	{
        protected override void OnEnter(State prevState)
        {
            owner.animator.SetBool(owner.IsDamageHash, true);
        }
        protected override void OnExit(State nextState)
        {
            owner.animator.SetBool(owner.IsDamageHash, false);
        }
    }
    private class StateDeath : State
	{
        protected override void OnEnter(State prevState)
        {
            owner.rigid.gravityScale = 5f;
            owner.animator.SetBool(owner.IsDeathHash, true);
        }
		protected override void OnUpdate()
		{
            if (!owner.IsInAir() && owner.enabled) 
			{
                owner.OnDeathNotifyerHandler?.Invoke();
                Instantiate(owner.deathPrefab, owner._transform.position, Quaternion.identity);
                owner.gameObject.SetActive(false);
                owner.enabled = false;
            }
        }
	}
}

