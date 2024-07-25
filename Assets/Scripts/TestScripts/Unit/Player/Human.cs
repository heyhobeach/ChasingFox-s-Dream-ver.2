using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 인간 상태 클래스, PlayerUnit 클래스를 상속함
/// </summary>
public class Human : PlayerUnit
{
    public GameObject bullet;

    AudioSource sound;
    public AudioClip soundClip;

    public GameObject userGunsoud;

    /// <summary>
    /// 재장전 시간
    /// </summary>
    public float reloadTime;

    /// <summary>
    /// 총알 피해량
    /// </summary>
    public int bulletDamage;

    /// <summary>
    /// 총알 속도
    /// </summary>
    public float bulletSpeed;
    /// <summary>
    /// 장탄 수
    /// </summary>
    public float magazine;

    /// <summary>
    /// 잔여 탄약 수
    /// </summary>
    public float residualAmmo;

    /// <summary>
    /// 최대 탄약 수
    /// </summary>
    public float maxAmmo;


    /// <summary>
    /// 재장전 진행도
    /// </summary>
    [HideInInspector] private float reloadProgress;

    /// <summary>
    /// 대쉬 코루틴을 저장하는 변수, 대쉬 중 여부 겸용
    /// </summary>
    private Coroutine dashCoroutine;
    private Coroutine reloadCoroutine;

    private float fixedDir = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        var pi = CameraManager.Instance.proCamera2DPointerInfluence;
        pi.MaxHorizontalInfluence = 2.2f;
        pi.MaxVerticalInfluence = 1.2f;
        pi.InfluenceSmoothness = 0.2f;
        CameraManager.Instance.ChangeSize = 5.5f;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        StopDash();
        if(reloadCoroutine != null) StopCoroutine(reloadCoroutine);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        switch(CheckMapType(collision))
        {
            case MapType.Wall:
                SetHorizontalForce(0);
                break;
        }
    }

    protected override void OnCollisionStay2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        switch(CheckMapType(collision))
        {
            case MapType.Wall:
                SetHorizontalForce(0);
                break;
        }
    }

    protected override void Start()
    {
        sound=GetComponent<AudioSource>(); 
        //sound.PlayOneShot(soundClip, 0.3f);
        base.Start();
        residualAmmo = maxAmmo;
    }

    public override bool Attack(Vector3 clickPos)
    {
        Debug.Log("공격");
        shootingAnimationController.AttackAni();
        Debug.Log("문제 이전");
        //sound.GetComponent<AudioSource>().Play();

        Debug.Log("여기 문제");
        if (ControllerChecker() || unitState == UnitState.Dash || unitState == UnitState.Reload || shootingAnimationController.isAttackAni || residualAmmo <= 0) return false;
        base.Attack(clickPos);
        sound.PlayOneShot(soundClip, 0.3f);
        SoundManager.Instance.CoStartBullet(userGunsoud);
        ProCamera2DShake.Instance.Shake("GunShot ShakePreset");
        GameObject _bullet = Instantiate(bullet);//총알을 공격포지션에서 생성함
        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(shootingAnimationController.GetShootPosition(), clickPos, shootingAnimationController.GetShootRotation(), bulletDamage, bulletSpeed, gObj);
        residualAmmo--;
        return true;
    }

    public override bool Move(float dir)
    {
        if(ControllerChecker() || unitState == UnitState.Dash) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        fixedDir = (int)dir; // 대쉬 방향을 저장
        //Anim
        return base.Move(dir);
    }

    public override bool Jump(KeyState jumpKey) => base.Jump(jumpKey);

    public override bool Crouch(KeyState crouchKey)
    {
        if(ControllerChecker() || unitState == UnitState.Dash) return false;
        return base.Crouch(crouchKey);
    }

    public override bool Dash()
    {
        if(unitState != UnitState.Default || dashCoroutine != null) return false; // 조작이 불가능한 상태일 경우 동작을 수행하지 않음
        base.Dash();
        dashCoroutine = StartCoroutine(DashAffterInput());
        return true;
    }

    /// <summary>
    /// 대쉬 동작 중지
    /// </summary>
    public void StopDash()
    {
        if(dashCoroutine != null) StopCoroutine(dashCoroutine);
        invalidation = false;
        dashCoroutine = null;
        unitState = UnitState.Default;
        //여기 false
    }

    /// <summary>
    /// 대쉬 중 동작을 수행
    /// </summary>
    private IEnumerator DashAffterInput()
    {
        unitState = UnitState.Dash;
        Debug.Log("DashTrigger");
        var tempVel = fixedDir == 0 ? spriteRenderer.flipX ? -1 : 1 : Mathf.Sign(fixedDir);
        SetHorizontalVelocity(tempVel);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("Dash"));
        while(anim.GetCurrentAnimatorStateInfo(0).IsName("Dash")) // 대쉬 지속 시간 동안
        {
            SetHorizontalForce(tempVel * movementSpeed * 2f);
            anim.SetFloat("hzForce", -0.5f);
            yield return null;
        }
        StopDash();
    }

    public override bool FormChange()
    {
        if(unitState != UnitState.Default) return false;
        else return base.FormChange();
    }

    public override bool Reload()
    {
        if(reloadCoroutine != null) return false;
        base.Reload();
        reloadCoroutine = StartCoroutine(Reloading());
        base.Reload();
        return true;
    }

    private IEnumerator Reloading()
    {
        // unitState = UnitState.Reload;
        yield return null;
        yield return new WaitUntil(() => !shootingAnimationController.isReloadAni);
        // unitState = UnitState.Default;
        // UIController.Instance.DrawReload(0);
        residualAmmo = maxAmmo;
        reloadCoroutine = null;
    }
}
