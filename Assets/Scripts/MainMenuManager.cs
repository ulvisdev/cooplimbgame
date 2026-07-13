using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadSceneAsync("PlayerSetup");
    }

    public void Quit()
    {
        Application.Quit();
    }
}