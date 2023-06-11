using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadingUI : MonoBehaviour
{
    private UIDocument _uiDoc;

    private ProgressBar _progressBar;

    private void Awake()
    {
        _uiDoc = GetComponent<UIDocument>();
        _progressBar = _uiDoc.rootVisualElement.Q<ProgressBar>("LoadingProgress");
    }

    public void SetProgressBar(float value)
    {
        value = Mathf.Clamp(value, 0, 100f);
        _progressBar.value = value;
    }

    public void ShowProgressBar(bool value)
    {
        if(value)
        {
            if(!_progressBar.ClassListContains("on"))
            {
                _progressBar.AddToClassList("on");
            }
        }
        else 
        {
            if(_progressBar.ClassListContains("on"))
            {
                _progressBar.RemoveFromClassList("on");
            }
        }
    }
}
