using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour
{
    private static GameUI _instance;
    public static GameUI Instance => _instance;

    private UIDocument _uiDoc;

    private Label _stop;

    private void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("mul gameui");
            Destroy(this);
        }
        _instance = this;
        _uiDoc = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        _stop = _uiDoc.rootVisualElement.Q<Label>("Stop");
    }

    public void SetStop(bool value)
    {
        if(value)
        {
            if(!_stop.ClassListContains("on"))
                _stop.AddToClassList("on");
        }
        else 
        {
            if(_stop.ClassListContains("on"))
                _stop.RemoveFromClassList("on");
        }
    }
}
