using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public GameObject GameOverWindow;
    public Sprite xSprite;
    public Sprite oSprite;
    public int row;
    public int col;
    public int endgame = 0;

    private Image image;
    private Button button;
    private Board board;
    private Transform canvas;

    private void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }
    private void Start()
    {
        board = FindObjectOfType<Board>();
        canvas = FindObjectOfType<Canvas>().transform; 
    }
    private void Update()
    {
        if (endgame != 0)
        {
            board.currentTurn = "E";
        }
    }
    public void ChangeImage(string s)
    {
        if (s == "X") image.sprite = xSprite;
        else if (s == "O") image.sprite = oSprite;   
    }
    public void OnClick()
    {
        ChangeImage(board.currentTurn);
        if (board.Check(this.row, this.col))
        {
            endgame += 1;
            Debug.Log(board.currentTurn + " Win");
            GameObject window = Instantiate(GameOverWindow, canvas);
            window.GetComponent<GameOverWindow>().setName("Player " + board.currentTurn + " Win");
        }
        button.enabled = false;
        if (board.currentTurn == "X") board.currentTurn = "O";
        else if (board.currentTurn == "O") board.currentTurn = "X";
    }
}
