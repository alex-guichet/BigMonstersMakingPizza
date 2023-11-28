using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public enum SceneName
{
    MainMenuScene,
    GameScene,
    LoadingScene,
    LobbyScene,
    CharacterSelectionScene
}

public static class Loader
{
    public static event EventHandler OnSceneLoaded;
    private static SceneName _sceneTarget;
    
    public static void LoadScene(SceneName sceneTarget)
    {
        _sceneTarget = sceneTarget;
        SceneManager.LoadScene(SceneName.LoadingScene.ToString());
    }
    
    public static void LoadNetworkScene(SceneName sceneTarget)
    {
        _sceneTarget = sceneTarget;
        KitchenGameMultiplier.Instance.SetLoaderTargetScene();
        NetworkManager.Singleton.SceneManager.LoadScene(sceneTarget.ToString(), LoadSceneMode.Single);
    }
    
    public static void LoadCallback()
    {
        //SceneManager.LoadScene(_sceneTarget.ToString());
        CoroutineRunner.Instance.StartCoroutine(LoadAsyncScene());
    }
    
    static IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneTarget.ToString());

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        OnSceneLoaded?.Invoke(null, EventArgs.Empty);
    }

    public static SceneName GetSceneTarget()
    {
        return _sceneTarget;
    }
    
    public static void SetSceneTarget(SceneName sceneName)
    {
        _sceneTarget = sceneName;
    }

    public static int GetIndexOfSceneName(SceneName sceneName)
    {
        return Array.IndexOf(Enum.GetValues(sceneName.GetType()), sceneName);
    }
    
    public static SceneName GetSceneNameFromIndex(int sceneNameIndex)
    {
        return (SceneName)(Enum.GetValues(typeof(SceneName))).GetValue(sceneNameIndex);
    }

}
