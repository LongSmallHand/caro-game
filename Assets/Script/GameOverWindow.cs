using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverWindow : MonoBehaviour
{
    public Text winner;
    public Button restart;
    public Button quit;

    private void Awake()
    {
        restart.onClick.AddListener(restartGame);
        quit.onClick.AddListener(returnMenu);
    }
    public void setName(string name)
    {
        winner.text = name;
        if(name == "Player X Win") winner.color = Color.red;
        else winner.color = Color.blue;
    }
    public void restartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }
    public void returnMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}
