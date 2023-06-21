using System;
using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;

public class AgentInput : MonoBehaviour
{
    public event Action<Vector3> OnMovementInput;
    public event Action<Vector2> OnMousePosInput;
    public event Action<Vector2> OnMouseWheelScroll;
    public event Action OnAttackKeyDownInput;

    protected AgentAnimation _agentAnimation;
    [SerializeField] LayerMask _targetLayer;

    protected virtual void Awake()
    {
        _agentAnimation = GetComponentInChildren<AgentAnimation>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected virtual void Update()
    {
        UpdateMovementInput();
        UpdateMousePosInput();
        UpdateMouseWheelScroll();
        UpdateKeyDownInput();
    }

    protected void UpdateKeyDownInput()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(GameManager.Instance.Player.Type == Packet.PlayerType.Chaser)
            {
                _agentAnimation.SetAttack(true);

                bool result = Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out RaycastHit hit, 1f, _targetLayer);
                Attack res = new Attack{Success = result};
                SocketManager.Instance.RegisterSend(MSGID.Attack, res);
            }
        }
    }

    protected void UpdateMouseWheelScroll()
    {
        if(Input.mouseScrollDelta != Vector2.zero)
            OnMouseWheelScroll?.Invoke(Input.mouseScrollDelta);
    }

    protected void UpdateMousePosInput()
    {
        float x = Input.GetAxis("Mouse Y");
        float y = Input.GetAxis("Mouse X");
        OnMousePosInput?.Invoke(new Vector2(x, y));
    }

    protected void UpdateMovementInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        OnMovementInput?.Invoke(new Vector3(x, 0, z));
    }
}
