using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public static class Define
{
    private static CinemachineVirtualCamera _cmVCam;
    #nullable enable
    public static CinemachineVirtualCamera? CmVCam 
    {
        get 
        {
            if(_cmVCam == null)
                _cmVCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
            return _cmVCam;
        }
    }
    #nullable disable

    private static Camera _mainCam;
    public static Camera MainCam
    {
        get 
        {
            if(_mainCam == null)
                _mainCam = GameObject.FindObjectOfType<Camera>();
            return _mainCam;
        }
    }
}
