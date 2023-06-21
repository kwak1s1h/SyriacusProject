using System;
using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;

public class RemoteInput : AgentInput
{
    [SerializeField] LayerMask targetLayer;

    private Collider _collider;

    private PlayerType _type;
    public PlayerType Type { get => _type; set => _type = value; }

    private float _timer = 0f;
    private bool _isStop = false;

    public string Id;

    protected override void Awake()
    {
        base.Awake();
        _agentAnimation = transform.Find("Visual").GetComponent<AgentAnimation>();
        _collider = GetComponent<Collider>();
    }

    protected override void Update()
    {
        if (GameManager.Instance.Player.Type == PlayerType.Target)
        {
            CheckStopPlayer();
        }
    }

    public void Attack()
    {
        _agentAnimation.SetAttack(true);
    }

    private void CheckStopPlayer()
    {
        Bounds bounds = _collider.bounds;
        Plane[] cameraFrustum = GeometryUtility.CalculateFrustumPlanes(Define.MainCam);

        bool isInCamera = GeometryUtility.TestPlanesAABB(cameraFrustum, bounds);
        
        if (isInCamera)
        {
            if (_timer < 1.5f)
            {
                _timer += Time.deltaTime;
            }
            else
            {
                if(!_isStop)
                {
                    Vector3 cameraPos = Define.MainCam.transform.position;
                    Vector3 myPos = transform.position + Vector3.up * 0.5f;
                    bool ray = Physics.Raycast(cameraPos, myPos - cameraPos, out RaycastHit hit, float.MaxValue, targetLayer);
                    if(hit.collider == _collider)
                    {
                        _isStop = true;
                        SetStop stop = new SetStop { Value = true, Id = this.Id };
                        SocketManager.Instance.RegisterSend(MSGID.Setstop, stop);
                    }
                    else 
                    {
                        _isStop = false;
                        _timer = 0f;
                    }
                }
            }
        }
        else
        {
            if (_isStop)
            {
                // Send Can Move Data
                SetStop stop = new SetStop { Value = false, Id = this.Id };
                SocketManager.Instance.RegisterSend(MSGID.Setstop, stop);
            }
            _isStop = false;
            _timer = 0f;
        }
    }

    public void UpdateMovement(MoveData data)
    {
        transform.position = new Vector3(data.Pos.X, data.Pos.Y, data.Pos.Z);
        transform.eulerAngles = new Vector3(0, data.YRotation, 0);
        _agentAnimation.SetSpeed(data.Speed);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 cameraPos = Define.MainCam.transform.position;
        Vector3 myPos = transform.position + Vector3.up * 0.5f;
        Gizmos.DrawRay(cameraPos, myPos - cameraPos);
        Gizmos.color = Color.white;
    }
#endif
}
