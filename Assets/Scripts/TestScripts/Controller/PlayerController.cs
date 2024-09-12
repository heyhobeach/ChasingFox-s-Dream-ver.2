using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IBaseController
{
    private IUnitController unitController;
    private bool isKeyDown;

    void Awake() => ((IBaseController)this).AddController();
    void Start() => unitController = GetComponent<IUnitController>();
    void OnDestroy() => ((IBaseController)this).RemoveController();

    public void Controller()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1) && KeyControll()) unitController.FormChange();
        if(Input.GetKeyDown(KeyCode.R) && KeyControll()) unitController.Reload();
        if(Input.GetKeyDown(KeyCode.Mouse0) && KeyControll()) unitController.Attack(ClickPos());
        if(Input.GetKeyDown(KeyCode.Space) && KeyControll()) unitController.Dash();
        
        if (Input.GetKey(KeyCode.S) && KeyControll()) unitController.Crouch(KeyState.KeyDown);//GetKeyDown -> GetKey
        else if(Input.GetKey(KeyCode.S)) unitController.Crouch(KeyState.KeyUp);
        if(Input.GetKeyDown(KeyCode.W) && KeyControll()) unitController.Jump(KeyState.KeyDown);
        else if(Input.GetKey(KeyCode.W)) unitController.Jump(KeyState.KeyStay);
        else if(Input.GetKeyUp(KeyCode.W)) unitController.Jump(KeyState.KeyUp);
        else unitController.Jump(KeyState.None);
        unitController.Move(Vector2.right * Input.GetAxisRaw("Horizontal"));
        isKeyDown = false;
    }

    private bool KeyControll()
    {
        if(isKeyDown) return false;
        isKeyDown = true;
        return true;
    }

    public void Move(float dir) => unitController.Move(Vector2.right * dir);
    public void Attack(float angle) => unitController.Attack(new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle) * 10000, Mathf.Cos(Mathf.Deg2Rad * angle) * 10000));
    public void Reload() => unitController.Reload();

    public Vector3 ClickPos()//클릭한 좌료를 보내주며 현재 공격 클릭시 캐릭터의 바라보는 방향도 변해야한다고 생각해서 필요했던 부분
    {
        var screenPoint = Input.mousePosition;//마우스 위치 가져옴
        screenPoint.z = Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
