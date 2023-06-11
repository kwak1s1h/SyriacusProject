using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AgentAnimation : MonoBehaviour
{
    private Animator _animator;

    private int _speedHash = Animator.StringToHash("speed");
    private int _isAttackHash = Animator.StringToHash("is_attack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        _animator.SetFloat(_speedHash, speed);
    }

    public void SetAttack(bool value)
    {
        _animator.SetBool(_isAttackHash, value);
    }
}
