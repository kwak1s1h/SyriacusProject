using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentInput : MonoBehaviour
{
    public event Action<Vector3> OnMovementInput;
    public event Action<Vector2> OnMousePosInput;
    public event Action<Vector2> OnMouseWheelScroll;
    public event Action OnAttackKeyDownInput;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdateMovementInput();
        UpdateMousePosInput();
        UpdateMouseWheelScroll();
        UpdateKeyDownInput();
    }

    private void UpdateKeyDownInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnAttackKeyDownInput?.Invoke();
        }
    }

    private void UpdateMouseWheelScroll()
    {
        if(Input.mouseScrollDelta != Vector2.zero)
            OnMouseWheelScroll?.Invoke(Input.mouseScrollDelta);
    }

    private void UpdateMousePosInput()
    {
        float x = Input.GetAxis("Mouse Y");
        float y = Input.GetAxis("Mouse X");
        OnMousePosInput?.Invoke(new Vector2(x, y));
    }

    private void UpdateMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        OnMovementInput?.Invoke(new Vector3(x, 0, z));
    }
}
