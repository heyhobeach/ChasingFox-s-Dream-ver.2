using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 클래스. IUnitController 및 IDamageable 인터페이스를 상속
/// </summary>
public class Player : MonoBehaviour, IUnitController, IDamageable
{
    /// <summary>
    /// 폼을 저장하는 배열
    /// 0 : 인간, 1 : 늑대인간, 2 : 버서커
    /// </summary>
    public PlayerUnit[] forms;

    /// <summary>
    /// 현재 폼을 담는 변수
    /// </summary>
    private PlayerUnit changedForm;

    public int _maxHealth;    
    public int maxHealth { get => _maxHealth; set => _maxHealth = value; }
    private int _health;    
    public int health { get => _health; set => _health = value; }
    private bool _invalidation;
    public bool invalidation { get => _invalidation; set => _invalidation = value; }

    /// <summary>
    /// 폼 체인지 딜레이 시간
    /// </summary>
    public float changeDelay;

    /// <summary>
    /// 늑대인간 폼 유지를 위한 게이지 변수
    /// </summary>
    public float changeGage;

    /// <summary>
    /// 입력 방향을 저장할 변수
    /// </summary>
    private float fixedDir;

    private void Start()
    {
        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        // 인간 상태를 현재 상태로 변경
        changedForm = forms[0]; 
        changedForm.gameObject.SetActive(true);
        health = maxHealth; // 체력 초기화
    }

    public bool Crouch(KeyState crouchKey) => changedForm.Crouch(crouchKey);

    public bool Jump(KeyState jumpKey)
    {
        if(changedForm.UnitState == UnitState.FormChange) return false;
        return changedForm.Jump(jumpKey);
    }

    public bool Move(float dir)
    {
        if(changedForm.UnitState == UnitState.FormChange) return false;
        fixedDir = dir;
        return changedForm.Move(dir);
    }

    public bool Attack(Vector3 clickPos) => changedForm.Attack(clickPos);

    public bool Dash() => changedForm.Dash();

    /// <summary>
    /// 임시
    /// </summary>
    public IEnumerator ChangeDelay()
    {
        yield return new WaitUntil(() => changeGage <= 0);
    }

    public void Death()
    {
        if(changedForm is not Berserker) // 버서커 상태가 아닐 시
        {
            foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
            health = maxHealth; // 체력 초기화
            changedForm = forms[2]; // 상태를 버서커 상태로 변경
            changedForm.gameObject.SetActive(true);
        }
    }

    public bool FormChange()
    {
        if(changedForm.UnitState == UnitState.FormChange || !changedForm.FormChange()) return false; // 대쉬 중이거나 제어가 불가능한 상태일 경우 동작을 수행하지 않음
        StartCoroutine(FormChanging());
        return true;
    }

    private IEnumerator FormChanging()
    {
        changedForm.UnitState = UnitState.FormChange;
        float t = 0;
        while(t <= changeDelay)
        {
            t += Time.unscaledDeltaTime;
            changedForm.Move(fixedDir);
            yield return null;
        }

        foreach(PlayerUnit form in forms) form.gameObject.SetActive(false);
        if(changedForm is Human) changedForm = forms[1]; // 인간 상태일 시 늑대인간으로 변경
        else if(changedForm is Werwolf) changedForm = forms[0]; // 늑대인간 상태일 시 인간으로 변경
        changedForm.gameObject.SetActive(true);
        changedForm.SetVel(fixedDir); // 자연스러운 대쉬 동작을 위한 부분
        changedForm.UnitState = UnitState.Default;
    }

    public bool Reload() => changedForm.Reload();
}