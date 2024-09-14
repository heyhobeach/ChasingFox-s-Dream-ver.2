using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;

/// <summary>
/// 유닛의 기본적인 동작을 정의하는 추상 클래스
/// MonoBehaviour, IUnitController를 상속함
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public abstract class UnitBase : MonoBehaviour, IUnitController
{
    /// <summary>
    /// 중력 상수
    /// </summary>
    public const float gravity = -9.81f;

    /// <summary>
    /// 이동속도
    /// </summary>
    public float movementSpeed;

    /// <summary>
    /// 이동 가속도
    /// </summary>
    public float accelerate;

    /// <summary>
    /// 대시 지속시간
    /// </summary>
    public float dashDuration;

    /// <summary>
    /// 점프 유지력
    /// </summary>
    public float jumpForce;

    /// <summary>
    /// 점프 추진력
    /// </summary>
    public float jumpImpulse;

    /// <summary>
    /// 점프 유지 시간
    /// </summary>
    public float jumpTime;

    /// <summary>
    /// 유닛상태 열거형
    /// </summary>
    protected UnitState unitState;

    /// <summary>
    /// 유닛의 현재 상태를 가져옴
    /// </summary>
    public UnitState UnitState { get => unitState; set => unitState = value; }

    protected float boxOffsetX;
    /// <summary>
    /// 유닛 콜라이더 가로 크기
    /// </summary>
    protected float boxSizeX;

    /// <summary>
    /// 유닛 콜라이더 가로 크기를 가져옴
    /// </summary>
    public float BoxSizeX { get; private set; }

    protected float boxOffsetY;
    /// <summary>
    /// 유닛 콜라이더 세로 크기
    /// </summary>
    protected float boxSizeY;

    /// <summary>
    /// 유닛 콜라이더 세로 크기를 가져옴
    /// </summary>
    public float BoxSizeY { get; private set; }
    
    /// <summary>
    /// 수평 힘
    /// </summary>
    protected float hzForce;

    /// <summary>
    /// 수직 힘
    /// </summary>
    protected float vcForce;

    /// <summary>
    /// 수평 및 수직 힘
    /// </summary>
    public Vector2 Force { get { return new Vector2(hzForce, vcForce); } private set {  } }

    /// <summary>
    /// 바닥 체크
    /// </summary>
    protected bool isGrounded;

    protected SpriteRenderer spriteRenderer;

    [HideInInspector] public Animator anim;

    private bool longRangeUnit;
    [HideInInspector] public ShootingAnimationController shootingAnimationController;
    public Action onDeath;
    public Action onEnable;
    public Action onDisable;
    private SpriteResolver spriteResolver;

    public void Init() => Start();

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteResolver = GetComponent<SpriteResolver>();
        anim = GetComponent<Animator>();
        // 콜라이더 크기(절반) 계산
        var ect = gameObject.activeSelf;
        gameObject.SetActive(true);
        boxSizeX = gameObject.GetComponent<Collider2D>().bounds.extents.x;
        boxSizeY = gameObject.GetComponent<Collider2D>().bounds.extents.y;
        boxOffsetX = gameObject.GetComponent<Collider2D>().offset.x;
        boxOffsetY = gameObject.GetComponent<Collider2D>().offset.y;
        gameObject.SetActive(ect);
        
        unitState = UnitState.Default;
        anim.SetFloat("dashMultiplier", dashDuration > 0 ? 1/dashDuration : 1);

        shootingAnimationController = GetComponent<ShootingAnimationController>();
        if(shootingAnimationController != null) longRangeUnit = true;
    }
    protected virtual void Update()
    {
        // 힘의 방향에 따라 이미지를 좌우 반전
        if(hzForce < -0.1f) spriteRenderer.flipX = true;
        else if(hzForce > 0.1f) spriteRenderer.flipX = false;
        if(!isGrounded) anim.SetBool("isAir", true);
        else anim.SetBool("isAir", false);
        if(Mathf.Abs(vcForce) > 0.2f) anim.SetFloat("vcForce", vcForce);
        else anim.SetFloat("vcForce", 0);
        if(Mathf.Abs(hzForce) < 0.1f || Physics2D.Raycast(transform.position, Vector2.right*Mathf.Sign(hzForce), boxSizeX*1.1f, 1<<LayerMask.NameToLayer("Map")))
        {
            anim.SetFloat("hzForce", 0);
            anim.SetBool("isRun", false);
        }
        else
        {
            anim.SetFloat("hzForce", Mathf.Sign(hzForce) * (Mathf.Clamp(hzForce / movementSpeed, -1, 1) - fixDir * 1.5f));
            anim.SetBool("isRun", true);
        }
        anim.SetFloat("dashMultiplier", dashDuration > 0 ? 1/dashDuration : 1);
    }

    private void LateUpdate()
    {
        if(spriteResolver.enabled) spriteResolver.ResolveSpriteToSpriteRenderer();
    }
    
    protected virtual void OnEnable() => onEnable?.Invoke();

    /// <summary>
    /// 폼체인지 시 초기화 해야할 작업을 수행
    /// </summary>
    protected virtual void OnDisable()
    {
        unitState = UnitState.Default;
        onDisable?.Invoke();
        if(anim) anim.SetBool("isDeath", false);
    }

    public virtual bool Attack(Vector3 clickPos)
    {
        anim.SetTrigger("attack");
        if(!longRangeUnit) return false;
        shootingAnimationController.AttackAni();
        shootingAnimationController.Shoot();
        return true;
    }

    public virtual bool Dash()
    {
        if(longRangeUnit) shootingAnimationController.NomalAni();
        anim.SetTrigger("dash");
        return true;
    }

    public abstract bool Jump(KeyState jumpKey);

    public abstract bool Crouch(KeyState crouchKey);

    private float fixDir;
    public virtual bool Move(Vector2 dir)
    {
        fixDir = dir.x;
        return true;
    }

    public virtual bool FormChange()
    {
        anim.SetBool("formChange", true);
        unitState = UnitState.FormChange;
        return true;
    }

    public virtual bool Reload()
    {
        if(!longRangeUnit) return false;
        anim.SetTrigger("reload");
        shootingAnimationController.AttackAni();
        shootingAnimationController.Reload();
        return true;
    }
    
    public void Death()
    {
        anim.SetTrigger("death");
        anim.SetBool("isDeath", true);
        if(longRangeUnit) shootingAnimationController.NomalAni();
        unitState = UnitState.Death;
        if(onDeath != null) onDeath.Invoke();
    }

    /// <summary>
    /// 현재 플레이어 유닛의 제어 가능 여부 확인
    /// </summary>
    /// <returns>플레이어 유닛이 제어 불가능한 상태일 시 true를 반환</returns>
    protected bool ControllerChecker() => ControllerChecker(this);

    /// <summary>
    /// 주어진 플레이어 유닛의 제어 가능 여부 확인
    /// </summary>
    /// <param name="playerUnit">확인할 플레이어 유닛</param>
    /// <returns>플레이어 유닛이 제어 불가능한 상태일 시 true를 반환</returns>
    public static bool ControllerChecker(UnitBase unitBase)
    {
        var unitState = unitBase.UnitState;
        switch(unitState)
        {
            case UnitState.KnockBack:
            case UnitState.Stiffen:
            case UnitState.Stiffen_er:
            case UnitState.Death:
            case UnitState.Pause:
            return true;
            default: return false;
        }
    }

    /// <summary>
    /// 특정 위치로부터 유닛의 방향 계산
    /// </summary>
    /// <param name="tr">확인할 위치</param>
    /// <returns>유닛의 방향을 나타내는 값 (-1 or 1)</returns>
    protected int CheckDir(Vector3 tr)
    {
        int check = (int)Mathf.Sign(tr.x - this.gameObject.transform.position.x);//오브젝트와 유저의 값에 따라 보내줌
        return check;
    }

    /// <summary>
    /// 지정된 두 벡터 사이의 각도 계산
    /// </summary>
    /// <param name="from">시작 벡터</param>
    /// <param name="to">목표 벡터</param>
    /// <param name="dir">방향 벡터</param>
    /// <returns>시작 벡터에서 목표 벡터로의 회전 각도</returns>
    protected Vector3 GetSignedAngle(Vector2 from, Vector2 to, out Vector2 dir)
    {
        dir = (to - from).normalized; // 시작 벡터에서 목표 벡터까지의 방향 계산
        return new Vector3(0, 0, Vector3.SignedAngle(transform.right, dir, transform.forward)); // 유닛 기준 뱡향 벡터의 각도 계산 및 반환
    }

    public void OnResolver() => spriteResolver.enabled = true;
    public void OffResolver() => spriteResolver.enabled = false;
}
