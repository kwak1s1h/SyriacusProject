using System;
using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUI : MonoBehaviour
{
    private UIDocument _uiDoc;

    private VisualElement _currentContainer;

    private bool _canInput = false;
    public bool CanInput { get => _canInput; set => value = _canInput; }

    private VisualElement _mainContainer;
    [SerializeField] private float _titleMoveHeight;
    private Label _title;
    private Button _playBtn, _settingBtn, _quitBtn;

    private VisualElement _chosePlayMode;
    private Button _multiBtn, _singleBtn, _returnBtn;

    private VisualElement _multiModeContainer;
    [SerializeField] private VisualTreeAsset _roomUxml;
    private ScrollView _roomList;
    private TextField _nameOption;
    private SliderInt _maxUserOption;
    private Button _closeBtn, _createBtn;

    private VisualElement _roomContainer;
    [SerializeField] private VisualTreeAsset _userUxml;
    private ScrollView _userList;
    private Label _roomTitle;
    private Button _exitBtn, _roomPlayBtn;

    private void Awake()
    {
        _uiDoc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDoc.rootVisualElement;
        
        _mainContainer = root.Q("MenuContainer");
        MainContainerInit();

        _chosePlayMode = root.Q("ChosePlayMode");
        PlayModeContainerInit();

        _multiModeContainer = root.Q("MultiModeContainer");
        MultiModeContainerInit();

        _roomContainer = root.Q("RoomContainer");
        RoomContainerInit();

        _currentContainer = root.Q(null, "container", "on");
        SetCurrentWindow(_mainContainer);
        _canInput = true;
    }

    private void RoomContainerInit()
    {
        _quitBtn = _roomContainer.Q<Button>("CloseBtn");
        _userList = _roomContainer.Q<ScrollView>("UserList");
        _roomTitle = _roomContainer.Q<Label>("Title");
        _roomPlayBtn = _roomContainer.Q<Button>("PlayBtn");
    }

    private void MultiModeContainerInit()
    {
        _closeBtn = _multiModeContainer.Q<Button>("CloseBtn");
        _createBtn = _multiModeContainer.Q<Button>("CreateBtn");
        _roomList = _multiModeContainer.Q<ScrollView>("ScrollView");
        _nameOption = _multiModeContainer.Q<TextField>("NameOption");
        _maxUserOption = _multiModeContainer.Q<SliderInt>("MaxUserOption");

        _closeBtn.RegisterCallback<ClickEvent>(e => {
            if(!CanInput) return;
            if(_currentContainer == _multiModeContainer)
                SetCurrentWindow(_chosePlayMode);
        });
        _createBtn.RegisterCallback<ClickEvent>(e => {
            if(!CanInput) return;
            string roomName = _nameOption.value;
            int max = _maxUserOption.value;
            CreateRoomReq req = new CreateRoomReq{ MaxUser = max, RoomName = roomName };
            SocketManager.Instance.RegisterSend(MSGID.Createroomreq, req);
        });
    }

    private void PlayModeContainerInit()
    {
        _multiBtn = _chosePlayMode.Q<Button>("MultiPlayBtn");
        _singleBtn = _chosePlayMode.Q<Button>("SinglePlayBtn");
        _returnBtn = _chosePlayMode.Q<Button>("ReturnBtn");

        _multiBtn.RegisterCallback<ClickEvent>(e => {
            if(!CanInput) return;
            if(_currentContainer == _chosePlayMode)
                SetCurrentWindow(_multiModeContainer);
        });
        _returnBtn.RegisterCallback<ClickEvent>(e => {
            if(!CanInput) return;
            if(_currentContainer == _chosePlayMode)
                SetCurrentWindow(_mainContainer);
        });
        _singleBtn.focusable = false;
        _singleBtn.SetEnabled(false);
    }

    private void MainContainerInit()
    {
        _title = _mainContainer.Q<Label>("GameTitle");
        _playBtn = _mainContainer.Q<Button>("PlayBtn");
        _settingBtn = _mainContainer.Q<Button>("SettingBtn");
        _quitBtn = _mainContainer.Q<Button>("QuitBtn");

        _playBtn.RegisterCallback<ClickEvent>(e => {
            if(!CanInput) return;
            if(_currentContainer == _mainContainer)
                SetCurrentWindow(_chosePlayMode);
        });
        _quitBtn.RegisterCallback<ClickEvent>(e => {
            if(!CanInput) return;
            if(_currentContainer == _mainContainer)
                GameManager.Instance.QuitApp();
        });
    }

    private void SetCurrentWindow(VisualElement container)
    {
        if(_currentContainer == container) return;
        _canInput = false;
        _currentContainer.RemoveFromClassList("on");
        _currentContainer = container;
        _currentContainer.AddToClassList("on");
        _canInput = true;
    }

    private void Update()
    {
        TitleMove();
    }

    private void TitleMove()
    {
        if(_mainContainer.ClassListContains("on"))
        {
            Translate translate = new Translate(0, Mathf.Sin(Time.time) * _titleMoveHeight);
            _title.style.translate = new StyleTranslate(translate);
        }
    }

    public void CreateRoomElement(string roomName, int current, int max)
    {
        VisualElement newRoom = _roomUxml.CloneTree();
        newRoom.name = roomName;
        newRoom.Q<Label>("RoomName").text = roomName;
        newRoom.Q<Label>("CurrentUsers").text = $"{current} / {max}";
        _roomList.Add(newRoom);
    }

    public void CreateUserElement(string userName)
    {
        VisualElement newUser = _userUxml.CloneTree();
        newUser.name = userName;
        newUser.Q<Label>("UserName").text = userName;
        _roomList.Add(newUser);
    }

    public void UpdateRoomElement(string roomName, int current)
    {
        VisualElement element = _roomList.Q<VisualElement>(roomName);
        element.Q<Label>("RoomName").text = roomName;
        string max = element.Q<Label>("CurrentUsers").text.Split("/")[1].Trim();
        element.Q<Label>("CurrentUsers").text = $"{current} / {max}";
    }

    public void UpdateUserElement(string userName)
    {
        VisualElement element = _userList.Q<VisualElement>(userName);
        element.Q<Label>("UserName").text = userName;
    }
}
