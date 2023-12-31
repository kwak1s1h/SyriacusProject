using System;
using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUI : MonoBehaviour
{
    private static LobbyUI _instance;
    public static LobbyUI Instance => _instance;

    private UIDocument _uiDoc;

    private VisualElement _currentContainer;

    private bool _canInput = false;
    public bool CanInput { get => _canInput; set => value = _canInput; }

    private VisualElement _mainContainer;
    public VisualElement MainContainer => _mainContainer;
    [SerializeField] private float _titleMoveHeight;
    private Label _title;
    private Button _playBtn, _settingBtn, _quitBtn;
    private VisualElement _chosePlayMode;
    public VisualElement ChosePlayMode => _chosePlayMode;
    private Button _multiBtn, _singleBtn, _returnBtn;

    private VisualElement _multiModeContainer;
    public VisualElement MultiModeContainer => _multiModeContainer;
    [SerializeField] private VisualTreeAsset _roomUxml;
    private ScrollView _roomList;
    private TextField _nameOption;
    private SliderInt _maxUserOption;
    private Button _closeBtn, _createBtn;

    private VisualElement _roomContainer;
    public VisualElement RoomContainer => _roomContainer;
    [SerializeField] private VisualTreeAsset _userUxml;
    private ScrollView _roomOptionList, _userList;
    private Label _roomTitle;
    private Button _exitBtn, _roomPlayBtn;
    private SliderInt _roomMaxUserOption, _playTimeOption, _targetHpOption;

    private void Awake()
    {
        _uiDoc = GetComponent<UIDocument>();

        if(_instance != null)
        {
            Debug.LogError("Multiple LobbyUI Instance is running, destroy this!");
            Destroy(this);
        }
        _instance = this;
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
        _roomOptionList = _roomContainer.Q<VisualElement>("RoomOption").Q<ScrollView>("ScrollView");
        _userList = _roomContainer.Q<VisualElement>("UserList").Q<ScrollView>("ScrollView");
        _roomTitle = _roomContainer.Q<Label>("Title");
        _roomPlayBtn = _roomContainer.Q<Button>("PlayBtn");
        _roomMaxUserOption = _roomOptionList.Q<SliderInt>("MaxUserOption");
        _playTimeOption = _roomOptionList.Q<SliderInt>("GameTimeOption");
        _targetHpOption = _roomOptionList.Q<SliderInt>("TargetHpOption");
        _quitBtn.RegisterCallback<ClickEvent>(e => {
            QuitRoom quit = new QuitRoom();
            SocketManager.Instance.RegisterSend(MSGID.Quitroom, quit);
            SetCurrentWindow(MultiModeContainer);
        });
        _roomPlayBtn.RegisterCallback<ClickEvent>(e => {
            PlayGameReq req = new PlayGameReq {
                PlayTime = _playTimeOption.value,
                TargetHp = _targetHpOption.value
            };
            SocketManager.Instance.RegisterSend(MSGID.Playgamereq, req);
        });
        _roomMaxUserOption.focusable = false;
        _roomMaxUserOption.SetEnabled(false);
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
            CanInput = false;
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
            {
                RoomListReq req = new RoomListReq();
                SocketManager.Instance.RegisterSend(MSGID.Roomlistreq, req);
                SetCurrentWindow(_multiModeContainer);
            }
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

    #nullable enable
    public void SetCurrentWindow(VisualElement? container)
    {
        if(_currentContainer == container) return;
        _canInput = false;
        _currentContainer.RemoveFromClassList("on");
        _currentContainer = container;
        _currentContainer?.AddToClassList("on");
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
        newRoom.Q<Button>("JoinBtn").RegisterCallback<ClickEvent>(e => {
            JoinRoomReq req = new JoinRoomReq { RoomName = roomName };
            SocketManager.Instance.RegisterSend(MSGID.Joinroomreq, req);
            CanInput = false;
        });
        _roomList.Add(newRoom);
    }

    public void CreateUserElement(string userName, bool isMe = false)
    {
        VisualElement newUser = _userUxml.CloneTree();
        newUser.name = userName;
        newUser.Q<Label>("UserName").text = userName;
        _userList.Add(newUser);
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

    public void InitRoom(string name, int max, bool isOwner = false)
    {
        _roomTitle.text = name;
        _userList.Clear();
        _roomMaxUserOption.value = max;
        SetRoomOwner(isOwner);
        CreateUserElement("나");
    }

    public void DeleteRoomElement(string roomName)
    {
        VisualElement target = _roomList.Q(roomName);
        if(target != null)
            _roomList.Remove(target);
        else
            Debug.LogError($"Cannot find room {roomName}");
    }

    public void DeleteUserElement(string id)
    {
        VisualElement target = _userList.Q(id);
        if(target != null)
            _userList.Remove(target);
        else
            Debug.LogError($"Cannot find user ${id}");
    }

    public void ClearRoomList() => _roomList.Clear();

    public void SetRoomOwner(bool value)
    {
        _roomPlayBtn.SetEnabled(value);
        _playTimeOption.SetEnabled(value);
        _targetHpOption.SetEnabled(value);
    }
}
