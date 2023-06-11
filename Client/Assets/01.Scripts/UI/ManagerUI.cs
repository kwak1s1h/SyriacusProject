using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ManagerUI : MonoBehaviour
{
    private UIDocument _uiDoc;

    // Error Popup
    private VisualElement _errorWindow;
    private Button _errorPopupCloseBtn;
    private Label _errorPopupTitle;

    private void Awake()
    {
        _uiDoc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        VisualElement root = _uiDoc.rootVisualElement;

        // Error Popup
        _errorWindow = root.Q<VisualElement>("ErrorWindow");
        _errorPopupCloseBtn = _errorWindow.Q<Button>("CloseBtn");
        _errorPopupTitle = _errorWindow.Q<Label>("ErrorText");
    }

    public void PopupError(string title, string btn, Action closeCallback = null, float delay = 0f)
    {
        _errorPopupTitle.text = title;
        _errorPopupCloseBtn.text = btn;
        _errorPopupCloseBtn.RegisterCallback<ClickEvent>((ev) => {
            _errorWindow.RemoveFromClassList("on");
            if(delay >= 0f)
            {
                StartCoroutine(DelayCoroutine(closeCallback, delay));
            }
            else closeCallback?.Invoke();
        });

        _errorWindow.AddToClassList("on");
    }

    private IEnumerator DelayCoroutine(Action closeCallback, float delay)
    {
        yield return new WaitForSeconds(delay);
        closeCallback?.Invoke();
    }
}
