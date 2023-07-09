using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }
    public void TwoPlayerMode()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void BotMode()
    {
        SceneManager.LoadScene("BotScene");
    }
    public void OnlineMode()
    {
        SceneManager.LoadScene("MultiplayerScene");
    }
}
