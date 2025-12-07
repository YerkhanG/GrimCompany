using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadSceneAsync("Lobby");
    public void LoadMap() => SceneManager.LoadSceneAsync("Map Scene");
    public void LoadCombat() => SceneManager.LoadSceneAsync("Combat Scene");
    public void QuitGame() => Application.Quit();
}