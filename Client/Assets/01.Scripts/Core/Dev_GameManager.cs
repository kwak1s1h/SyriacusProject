using System.Net.Mime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Packet;

public class Dev_GameManager : MonoBehaviour
{
    public static Dev_GameManager Instance;

    private ManagerUI _managerUI;
    public ManagerUI ManagerUI => _managerUI;

    [SerializeField] private string hostname = "localhost";
    [SerializeField] private int port = 30000;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Multiple GameManager is running!");
        Instance = this;

        _managerUI = transform.Find("ManagerUI").GetComponent<ManagerUI>();

        CreateSocketManager();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        
    }

    private async void CreateSocketManager()
    {
        SocketManager.Instance = gameObject.AddComponent<SocketManager>();
        SocketManager.Instance.Init($"ws://{hostname}:{port}");

        #if UNITY_EDITOR
        SocketManager.Instance.OnDisconnect += () => _managerUI.PopupError("서버 연결에 실패했습니다.", "게임 종료", () => UnityEditor.EditorApplication.isPlaying = false, 0.4f);
        #else
        SocketManager.Instance.OnDisconnect += () => _managerUI.PopupError("서버 연결에 실패했습니다.", "게임 종료", Application.Quit, 0.4f);
        #endif

        await SocketManager.Instance.Connection();
    }
}
