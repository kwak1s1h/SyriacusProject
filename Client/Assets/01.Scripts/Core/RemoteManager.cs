using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;

public class RemoteManager : MonoBehaviour
{
    private static RemoteManager _instance;
    public static RemoteManager Instance => _instance;

    [SerializeField] private RemoteInput _targetPref, _chaserPref;

    private Dictionary<string, RemoteInput> _remotes = new Dictionary<string, RemoteInput>();

    private void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("Multiple RemoteManager is running, Destroy this.");
            Destroy(this);
        }
        _instance = this;
    }

    public void CreateRemote(PlayerType type, string id, Position spawnPos)
    {
        RemoteInput obj;
        Vector3 pos = new Vector3(spawnPos.X, spawnPos.Y, spawnPos.Z);
        if(type == PlayerType.Target)
        {
            obj = Instantiate(_targetPref.gameObject, pos, Quaternion.identity).GetComponent<RemoteInput>();
        }
        else 
        {
            obj = Instantiate(_chaserPref.gameObject, pos, Quaternion.identity).GetComponent<RemoteInput>();
        }
        obj.Type = type;
        obj.Id = id;
        _remotes.Add(id, obj);
    }

    public bool GetRemote(string id, out RemoteInput outValue)
    {
        return _remotes.TryGetValue(id, out outValue);
    }

    public void DeleteRemote(string id)
    {
        RemoteInput obj = _remotes[id];
        _remotes.Remove(id);
        Destroy(obj);
    }
}
