using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;

public class RemoteMovement : AgentMovement
{
    private RemoteInput _remoteInput;

    private Queue<MoveData> _moveQueue;
    
    protected override void Awake()
    {
        _remoteInput = GetComponent<RemoteInput>();
        _agentAnimation = transform.Find("Visual").GetComponent<AgentAnimation>();
        _charController = GetComponent<CharacterController>();
    }

    public void RegisterMovement(MoveData moveData)
    {
        _moveQueue.Enqueue(moveData);
    }

    protected override void SetMoveVelocity(Vector3 keyInput)
    {
        base.SetMoveVelocity(keyInput);
    }

    protected override void CalculatePlayerMovement()
    {
        base.CalculatePlayerMovement();
    }

    protected override void FixedUpdate()
    {
        
    }
}
