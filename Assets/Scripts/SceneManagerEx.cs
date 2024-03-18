using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
    public void LoadGame()
    {
        SceneManager.LoadSceneAsync("Game", LoadSceneMode.Single).completed += OnGameLoaded;
    }

    public void LoadLobby()
    {
        SceneManager.LoadSceneAsync("Lobby", LoadSceneMode.Single).completed += OnLobbyLoaded;
    }

    public void OnGameLoaded(AsyncOperation asyncOperation)
    {
        Managers.Game.Init();
    }

    public void OnLobbyLoaded(AsyncOperation asyncOperation)
    {
        Managers.Lobby.Init();
    }
}
