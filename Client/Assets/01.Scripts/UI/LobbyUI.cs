using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyUI : MonoBehaviour
{
    private UIDocument _uiDoc;

    private VisualElement _currentContainer;

    private VisualElement _mainContainer;
    [SerializeField] private float _titleMoveHeight;
    private Label _title;
    private Button _playBtn, _settingBtn, _quitBtn;

    private VisualElement _chosePlayMode;
    private Button _multiBtn, _singleBtn, _returnBtn;

    private VisualElement _multiModeContainer;
    private Button _closeBtn;

    private void Awake()
    {
        _uiDoc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDoc.rootVisualElement;
        
        _mainContainer = root.Q("MenuContainer");
        _title = _mainContainer.Q<Label>("GameTitle");
        _playBtn = _mainContainer.Q<Button>("PlayBtn");
        _settingBtn = _mainContainer.Q<Button>("SettingBtn");
        _quitBtn = _mainContainer.Q<Button>("QuitBtn");
        MainContainerInit();

        _chosePlayMode = root.Q("ChosePlayMode");
        _multiBtn = _chosePlayMode.Q<Button>("MultiPlayBtn");
        _singleBtn = _chosePlayMode.Q<Button>("SinglePlayBtn");
        _returnBtn = _chosePlayMode.Q<Button>("ReturnBtn");
        PlayModeContainerInit();

        _multiModeContainer = root.Q("MultiModeContainer");
        _closeBtn = _multiModeContainer.Q<Button>("CloseBtn");
        MultiModeContainerInit();

        _currentContainer = root.Q(null, "container", "on");
        SetCurrentWindow(_mainContainer);
    }

    private void MultiModeContainerInit()
    {
        _closeBtn.RegisterCallback<ClickEvent>(e => {
            if(_currentContainer == _multiModeContainer)
                SetCurrentWindow(_chosePlayMode);
        });
    }

    private void PlayModeContainerInit()
    {
        _multiBtn.RegisterCallback<ClickEvent>(e => {
            if(_currentContainer == _chosePlayMode)
                SetCurrentWindow(_multiModeContainer);
        });
        _returnBtn.RegisterCallback<ClickEvent>(e => {
            if(_currentContainer == _chosePlayMode)
                SetCurrentWindow(_mainContainer);
        });
        _singleBtn.focusable = false;
        _singleBtn.SetEnabled(false);
    }

    private void MainContainerInit()
    {
        _playBtn.RegisterCallback<ClickEvent>(e => {
            if(_currentContainer == _mainContainer)
                SetCurrentWindow(_chosePlayMode);
        });
        _quitBtn.RegisterCallback<ClickEvent>(e => {
            if(_currentContainer == _mainContainer)
                GameManager.Instance.QuitApp();
        });
    }

    private void SetCurrentWindow(VisualElement container)
    {
        if(_currentContainer == container) return;
        _currentContainer.RemoveFromClassList("on");
        _currentContainer = container;
        _currentContainer.AddToClassList("on");
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
}
