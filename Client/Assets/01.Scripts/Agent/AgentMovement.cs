using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using static Define;

[RequireComponent(typeof(AgentInput), typeof(CharacterController))]
public class AgentMovement : MonoBehaviour
{
    private AgentInput _agentInput;
    private AgentAnimation _agentAnimation;
    private CharacterController _charController;

    [Range(0f, 2f), SerializeField] private float _axisSpeedX = 1f;
    [Range(0f, 0.1f), SerializeField] private float _axisSpeedY = 0.05f;

    private Cinemachine3rdPersonFollow _followComponent;

    private Vector3 _movementVelocity;
    private float _verticalVelocity;
    private float _gravity = -9.8f;
    [SerializeField] private float _moveSpeed = 1f, _zoomSmooth = 0.5f;

    private bool _canMove = true;
    public bool CanMove { get => _canMove; set => _canMove = value; }

    private void Awake()
    {
        _agentInput = GetComponent<AgentInput>();
        _agentAnimation = transform.Find("Visual").GetComponent<AgentAnimation>();
        _charController = GetComponent<CharacterController>();
        _followComponent = Define.CmVCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        _agentInput.OnMousePosInput += SetCameraViewPosition;
        _agentInput.OnMousePosInput += SetPlayerRotation;
        _agentInput.OnMovementInput += SetMoveVelocity;
        _agentInput.OnMouseWheelScroll += SetCameraDistance;
    }

    private void SetCameraDistance(Vector2 mouseDelta)
    {
        _followComponent.CameraDistance = Mathf.Clamp(_followComponent.CameraDistance + -mouseDelta.y * _zoomSmooth, 2.25f, 3f);
    }

    private void SetMoveVelocity(Vector3 keyInput)
    {
        _movementVelocity = keyInput;
        _agentAnimation.SetSpeed(keyInput.sqrMagnitude);
    }

    public void StopImmediately()
    {
        _movementVelocity = Vector3.zero;
        _agentAnimation.SetSpeed(0f);
    }

    private void SetPlayerRotation(Vector2 mouseInput)
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles + Vector3.up * mouseInput.y * _axisSpeedX);
    }

    private void SetCameraViewPosition(Vector2 mouseInput)
    {
        _followComponent.ShoulderOffset.y = Mathf.Clamp(_followComponent.ShoulderOffset.y + mouseInput.x * _axisSpeedY, 0.2f, 4f);
    }

    private void FixedUpdate()
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

    private void CalculatePlayerMovement()
    {
        _movementVelocity.Normalize();
        _movementVelocity *= _moveSpeed * Time.deltaTime;
    }
}
