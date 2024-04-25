using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private IUnitController unitController;

    void Start() => unitController = GetComponent<IUnitController>();

    void Update()
    {
        unitController.Move(Input.GetAxisRaw("Horizontal"));
        
        if(Input.GetKeyDown(KeyCode.Space)) unitController.Jump(KeyState.KeyDown);
        else if(Input.GetKey(KeyCode.Space)) unitController.Jump(KeyState.KeyStay);
        else if(Input.GetKeyUp(KeyCode.Space)) unitController.Jump(KeyState.KeyUp);
        else unitController.Jump(KeyState.None);

        if(Input.GetKey(KeyCode.S)) unitController.Crouch(KeyState.KeyStay);
        else if(Input.GetKeyUp(KeyCode.S)) unitController.Crouch(KeyState.KeyUp);

        if(Input.GetKeyDown(KeyCode.Mouse0)) unitController.Attack(ClickPos());

        if(Input.GetKeyDown(KeyCode.Mouse1)) unitController.Dash();
    }

    public Vector3 ClickPos()//클릭한 좌료를 보내주며 현재 공격 클릭시 캐릭터의 바라보는 방향도 변해야한다고 생각해서 필요했던 부분
    {
        var screenPoint = Input.mousePosition;//마우스 위치 가져옴
        screenPoint.z = Camera.main.transform.position.z;
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
