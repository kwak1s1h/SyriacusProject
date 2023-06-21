using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AgentAnimation : MonoBehaviour
{
    private Animator _animator;
    private AgentMovement _agentMovement;

    public PlayerType Type;

    private int _speedHash = Animator.StringToHash("speed");
    private int _isAttackHash = Animator.StringToHash("is_attack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agentMovement = GetComponentInParent<AgentMovement>();
    }

    public void SetSpeed(float speed)
    {
        _animator.SetFloat(_speedHash, speed);
        
    }

    public void SetAttack(bool value)
    {
        _animator.SetBool(_isAttackHash, value);
        if(_agentMovement != null)
            _agentMovement.CanMove = false;   
    }

    public void OnAnimationEnd()
    {
        SetAttack(false);
        if(_agentMovement != null)
            _agentMovement.CanMove = true;
    }
}
