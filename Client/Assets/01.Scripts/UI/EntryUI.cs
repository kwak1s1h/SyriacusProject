using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EntryUI : MonoBehaviour
{
    private UIDocument _uiDoc;
    private VisualElement _backImage;

    private void Awake()
    {
        _uiDoc = GetComponent<UIDocument>();
        _backImage = _uiDoc.rootVisualElement.Q("BackImage");
    }

    private void Start()
    {
        _backImage.AddToClassList("on");
    }

    private void OnDisable()
    {
        if(_backImage != null)
            _backImage.RemoveFromClassList("on");
    }
}
