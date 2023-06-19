using System;
using System.Collections;
using System.Collections.Generic;
using Packet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private ManagerUI _managerUI;
    private LoadingUI _loadingUI;

    [SerializeField] private string hostname = "localhost";
    [SerializeField] private int port = 30000;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("Multiple GameManager is running!");
        Instance = this;

        _managerUI = transform.Find("ManagerUI").GetComponent<ManagerUI>();
        _loadingUI = transform.Find("LoadingUI").GetComponent<LoadingUI>();

        CreateSocketManager();

        DontDestroyOnLoad(gameObject);
    }

    private async void CreateSocketManager()
    {
        SocketManager.Instance = gameObject.AddComponent<SocketManager>();
        SocketManager.Instance.Init($"ws://{hostname}:{port}");

        SocketManager.Instance.OnDisconnect += () => _managerUI.PopupError("서버 연결에 실패했습니다.", "게임 종료", QuitApp, 0.4f);

        await SocketManager.Instance.Connection();
        LoadSceneAsync("Lobby");
    }

    public void LoadSceneAsync(string sceneName, bool waitServer = false, LoadSceneMode mode = LoadSceneMode.Single)
    {
        var operation = SceneManager.LoadSceneAsync(sceneName, mode);
        StartCoroutine(SceneLoadRoutine(operation, waitServer));
    }
    public void LoadSceneAsync(int sceneIdx, bool waitServer = false, LoadSceneMode mode = LoadSceneMode.Single)
    {
        var operation = SceneManager.LoadSceneAsync(sceneIdx, mode);
        StartCoroutine(SceneLoadRoutine(operation, waitServer));
    }

    private IEnumerator SceneLoadRoutine(AsyncOperation operation, bool waitServer)
    {
        operation.allowSceneActivation = false;
        _loadingUI.ShowProgressBar(true);
        while(!operation.isDone)
        {
            _loadingUI.SetProgressBar(Mathf.MoveTowards(operation.progress * 100f, 100f, Time.deltaTime));
            if(operation.progress >= 0.9f) 
            {
                yield return new WaitForSeconds(0.75f);
                if(waitServer)
                {
                    SocketManager.Instance.RegisterSend(MSGID.Sceneready, new SceneReady());
                    yield return new WaitUntil(() => SocketManager.Instance.LoadSceneAllow);
                }
                else operation.allowSceneActivation = true;
            }
            yield return null;
        }
        _loadingUI.ShowProgressBar(false);
    }

    public void PopupError(string title, string button, Action closeCallback = null, float delay = 0f)
    {
        _managerUI.PopupError(title, button, closeCallback, delay);
    }

    public void QuitApp()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
