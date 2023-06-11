using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class DevelopUI : MonoBehaviour
{
    private UIDocument _uiDoc;

    // Socket Info
    private VisualElement _socketInfo;
    private Label _connectionText, _userCount;
    private VisualElement _connectionStatus;
    [SerializeField] private Texture2D _connectIcon, _disconnectIcon;

    // Pause Window
    private VisualElement _pauseWindow;
    private Label _pauseText;

    private void Awake()
    {
        _uiDoc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDoc.rootVisualElement;

        _socketInfo = root.Q("SocketInfo");

        _userCount = _socketInfo.Q<Label>("UserCount");

        _connectionText = _socketInfo.Q<Label>(null, "connection-text");
        _connectionStatus = _socketInfo.Q("Status", "connection-status");
    }

    private void Start()
    {
        SocketManager.Instance.OnConnect += () => SetConnection(true);
        SocketManager.Instance.OnDisconnect += () => SetConnection(false);
    }

    private void SetConnection(bool value)
    {
        _connectionText.text = value ? "Connect" : "Disconnect";
        _connectionStatus.style.backgroundImage = new StyleBackground(value ? _connectIcon : _disconnectIcon);
        if(!value) _userCount.text = "? / ?";
    }
}
