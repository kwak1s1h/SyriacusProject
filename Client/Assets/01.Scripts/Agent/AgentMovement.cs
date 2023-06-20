using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static Define;

[RequireComponent(typeof(AgentInput), typeof(CharacterController))]
public class AgentMovement : MonoBehaviour
{
    protected AgentInput _agentInput;
    protected AgentAnimation _agentAnimation;
    protected CharacterController _charController;

    [Range(0f, 2f), SerializeField] protected float _axisSpeedX = 1f;
    [Range(0f, 0.1f), SerializeField] protected float _axisSpeedY = 0.05f;

    #nullable enable
    protected Cinemachine3rdPersonFollow? _followComponent;
    #nullable disable

    protected Vector3 _movementVelocity;
    protected float _verticalVelocity;
    protected float _gravity = -9.8f;
    [SerializeField] protected float _moveSpeed = 1f, _zoomSmooth = 0.5f;

    protected bool _canMove = true;
    public bool CanMove { get => _canMove; set => _canMove = value; }

    protected virtual void Awake()
    {
        _agentInput = GetComponent<AgentInput>();
        _agentAnimation = transform.Find("Visual").GetComponent<AgentAnimation>();
        _charController = GetComponent<CharacterController>();
        _followComponent = Define.CmVCam?.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        _agentInput.OnMousePosInput += SetCameraViewPosition;
        _agentInput.OnMousePosInput += SetPlayerRotation;
        _agentInput.OnMovementInput += SetMoveVelocity;
        _agentInput.OnMouseWheelScroll += SetCameraDistance;
    }

    public void SetCamera(CinemachineVirtualCamera cmVCam)
    {
        _followComponent = cmVCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        cmVCam.Follow = transform;
        cmVCam.LookAt = transform.Find("CameraLook");
    }

    protected virtual void SetCameraDistance(Vector2 mouseDelta)
    {
        if(_followComponent != null)
        _followComponent.CameraDistance = Mathf.Clamp(_followComponent.CameraDistance + -mouseDelta.y * _zoomSmooth, 2.25f, 3f);
    }

    protected virtual void SetMoveVelocity(Vector3 keyInput)
    {
        _movementVelocity = keyInput;
        _agentAnimation.SetSpeed(keyInput.sqrMagnitude);
    }

    public virtual void StopImmediately()
    {
        _movementVelocity = Vector3.zero;
        _agentAnimation.SetSpeed(0f);
    }

    protected virtual void SetPlayerRotation(Vector2 mouseInput)
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * mouseInput.y * _axisSpeedX);
    }

    protected virtual void SetCameraViewPosition(Vector2 mouseInput)
    {
        if(_followComponent != null)
        _followComponent.ShoulderOffset.y = Mathf.Clamp(_followComponent.ShoulderOffset.y + mouseInput.x * _axisSpeedY, 0.2f, 4f);
    }

    protected virtual void FixedUpdate()
    {
        CalculatePlayerMovement();
        if (_charController.isGrounded == false)
        {
            _verticalVelocity = _gravity * Time.fixedDeltaTime;
        }
        else
        {
            _verticalVelocity = _gravity * 0.3f * Time.fixedDeltaTime;
        }

        Vector3 move = transform.rotation * _movementVelocity + _verticalVelocity * Vector3.up;
        _charController.Move(move);
    }

    protected virtual void CalculatePlayerMovement()
    {
        _movementVelocity.Normalize();
        _movementVelocity *= _moveSpeed * Time.deltaTime;
    }
}
