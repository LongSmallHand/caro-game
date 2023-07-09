using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotController : MonoBehaviour
{
    public static BotController Instance;
    public Sprite tick;
    public Transform playPanel;
    public GameObject board;
    public GameObject aiBox;
    public GameObject result;
    public Text winner;
    public bool aiFlag;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    public void Start()
    {
        createBoard();
    }
    private void createBoard()
    {
        Instantiate(board, playPanel);
    }
    public void ReturnMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void PlayAgain()
    {
        SceneManager.LoadScene("BotScene");
    }
    public void showWinner(int win)
    {
        if(win == 0)
        {
            result.SetActive(true);
            winner.text = "COMPUTER WIN";
        }
        else if (win == 1)
        {
            result.SetActive(true);
            winner.text = "PLAYER WIN";
        }
        else if (win == 2)
        {
            result.SetActive(true);
            winner.text = "GAME DRAW";
        }
    }
    public void changeAI()
    {
        if (aiBox.GetComponent<Image>().sprite != tick)
        {
            aiBox.GetComponent<Image>().sprite = tick;
            aiFlag = true;
        }
        else
        {
            aiBox.GetComponent<Image>().sprite = null;
            aiFlag = false;
        }
    }
}
