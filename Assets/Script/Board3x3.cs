using UnityEngine;
using UnityEngine.UI;

public class Board3x3 : MonoBehaviour
{
    Button[,] buttons = new Button[10, 10];
    private bool BotTurn = false;
    private bool playerTurn = true;
    private bool BotFlag;
    private bool AIFlag;
    private bool endgame = false;

    public Sprite xSprite, oSprite;
    public void Start()
    {
        var cells = GetComponentsInChildren<Button>();
        int n = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                buttons[i, j] = cells[n];
                n++;

                int r = i;
                int c = j;
                buttons[i, j].onClick.AddListener(delegate { OnClickCell(r, c); });
            }
        }
    }
    public void Update()
    {
        if (BotTurn)
        {
            if (BotController.Instance.aiFlag == true)
            {
                AIFlag = true;
                BotFlag = false;
            }
            else if (BotController.Instance.aiFlag == false)
            {
                AIFlag = false;
                BotFlag = true;
            }

            if (BotFlag) BotPlay();
            if (AIFlag) AIPlay();
        }

        if (endgame != false)
        {
            GameObject board = GameObject.FindGameObjectWithTag("Panel");
            BotTurn = false;
            playerTurn = false;
        }
    }
    private void OnClickCell(int r, int c)
    {
        if (playerTurn)
        {
            buttons[r, c].GetComponent<Image>().sprite = xSprite;
            CheckResult(r, c);
            buttons[r, c].enabled = false;
            BotTurn = true;
            playerTurn = false;
        }
    }
    private void BotPlay()
    {
        int col, row;
        while (!IsDraw())
        {
            row = Random.Range(0, 9);
            col = Random.Range(0, 9);
            if (buttons[row, col].GetComponent<Image>().sprite != xSprite && buttons[row, col].GetComponent<Image>().sprite != oSprite)
            {
                buttons[row, col].GetComponent<Image>().sprite = oSprite;
                buttons[row, col].enabled = false;
                CheckResult(row, col);
                BotTurn = false;
                playerTurn = true;
                break;
            }
        }
    }
    #region AI
    private long[] Apoint = new long[7] { 0, 5, 25, 125, 625, 99999, 99999 };
    private long[] Bpoint = new long[7] { 0, 3, 9, 120, 729, 99999, 99999 };
    private int step = 7;
    private void AIPlay()
    {
        int row = Minimax()[0, 0], col = Minimax()[0, 1];
        buttons[row, col].GetComponent<Image>().sprite = oSprite;
        buttons[row, col].enabled = false;
        CheckResult(row, col);
        BotTurn = false;
        playerTurn = true;
    }
    private int[,] Minimax()
    {
        int[,] res = new int[1,2];
        long MaxPoint = 0;
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (buttons[i, j].GetComponent<Image>().sprite != xSprite && buttons[i, j].GetComponent<Image>().sprite != oSprite)
                {
                    long Apoint = ACol(i, j) + ARow(i, j) + ADiaA(i, j) + ADiaB(i, j);
                    long Bpoint = BCol(i, j) + BRow(i, j) + BDiaA(i, j) + BDiaB(i, j);
                    long temp = Apoint > Bpoint ? Apoint : Bpoint;
                    if (MaxPoint < temp)
                    {
                        MaxPoint = temp;
                        res[0, 0] = i;
                        res[0, 1] = j;
                    }
                }
            }
        }
        return res;
    }
    #region A
    private long ACol(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && row + i < 10; i++)
        {
            if (buttons[row + i, col].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row + i, col].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        for (int i = 1; i < step && row - i > 0; i++)
        {
            if (buttons[row - i, col].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row - i, col].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        return  res = Apoint[ally] - Bpoint[enemy] * 3;
    }
    private long ARow(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && col + i < 10; i++)
        {
            if (buttons[row, col + i].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row, col + i].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        for (int i = 1; i < step && col - i > 0; i++)
        {
            if (buttons[row, col - i].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row, col - i].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        return res = Apoint[ally] - Bpoint[enemy] * 3;
    }
    private long ADiaA(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && row + i < 10 && col + i < 10; i++)
        {
            if (buttons[row + i, col + i].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row + i, col + i].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        for (int i = 1; i < step && row - i > 0 && col - i > 0; i++)
        {
            if (buttons[row - i, col - i].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row - i, col - i].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        return res = Apoint[ally] - Bpoint[enemy] * 3;
    }
    private long ADiaB(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && row - i > 0 && col + i < 10; i++)
        {
            if (buttons[row - i, col + i].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row - i, col + i].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        for (int i = 1; i < step && row + i < 10 && col - i > 0; i++)
        {
            if (buttons[row + i, col - i].GetComponent<Image>().sprite == oSprite) ally += 1;
            else if (buttons[row + i, col - i].GetComponent<Image>().sprite == xSprite)
            {
                enemy += 1;
                break;
            }
            else break;
        }
        return res = Apoint[ally] - Bpoint[enemy] * 3;
    }
    #endregion
    
    #region B
    private long BCol(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && row + i < 10; i++)
        {
            if (buttons[row + i, col].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row + i, col].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        for (int i = 1; i < step && row - i > 0; i++)
        {
            if (buttons[row - i, col].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row - i, col].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        return res = Bpoint[enemy];
    }
    private long BRow(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && col + i < 10; i++)
        {
            if (buttons[row, col + i].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row, col + i].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        for (int i = 1; i < step && col - i > 0; i++)
        {
            if (buttons[row, col - i].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row, col - i].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        return res = Bpoint[enemy];
    }
    private long BDiaA(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && row + i < 10 && col + i < 10; i++)
        {
            if (buttons[row + i, col + i].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row + i, col + i].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        for (int i = 1; i < step && row - i > 0 && col - i > 0; i++)
        {
            if (buttons[row - i, col - i].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row - i, col - i].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        return res = Bpoint[enemy];
    }
    private long BDiaB(int row, int col)
    {
        long res = 0;
        int ally = 0, enemy = 0;
        for (int i = 1; i < step && row - i > 0 && col + i < 10; i++)
        {
            if (buttons[row - i, col + i].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row - i, col + i].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        for (int i = 1; i < step && row + i < 10 && col - i > 0; i++)
        {
            if (buttons[row + i, col - i].GetComponent<Image>().sprite == oSprite)
            {
                ally += 1;
                break;
            }
            else if (buttons[row + i, col - i].GetComponent<Image>().sprite == xSprite) enemy += 1;
            else break;
        }
        return res = Bpoint[enemy];
    }
    #endregion
    #endregion

    #region Result
    private void CheckResult(int r, int c)
    {
        if (IsWin(r, c))
        {
            if (BotTurn)
            {
                BotController.Instance.showWinner(0);
            }
            else if (playerTurn)
            {
                BotController.Instance.showWinner(1);
            }
            endgame = true;
        }
        else if (IsDraw())
        {
            BotController.Instance.showWinner(2);
        }
    }
    public bool IsWin(int r, int c)
    {
        Sprite clickedButtonSprite = buttons[r, c].GetComponent<Image>().sprite;
        bool res = false;
        int c_count = 0;
        int r_count = 0;
        int dl_count = 0;
        int dr_count = 0;

        // Checking Column
        for (int i = r - 1; i >= 0; i--)
        {
            if (buttons[i, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite) c_count++;
            else break;
        }
        for (int i = r + 1; i < 10; i++)
        {
            if (buttons[i, c].GetComponentInChildren<Image>().sprite == clickedButtonSprite) c_count++;
            else break;
        }
        if (c_count + 1 >= 5) res = true;

        // Checking Row
        for (int i = c - 1; i >= 0; i--)
        {
            if (buttons[r, i].GetComponentInChildren<Image>().sprite == clickedButtonSprite) r_count++;
            else break;
        }
        for (int i = c + 1; i < 10; i++)
        {
            if (buttons[r, i].GetComponentInChildren<Image>().sprite == clickedButtonSprite) r_count++;
            else break;
        }
        if (r_count + 1 >= 5) res = true;

        //Diagonal Check: Top Left to Right
        for (int i = c - 1; i >= 0; i--)
        {
            if (r - (c - i) >= 0)
            {
                if (buttons[r - (c - i), i].GetComponentInChildren<Image>().sprite == clickedButtonSprite) dl_count++;
                else break;
            }
            else break;
        }
        for (int i = c + 1; i < 10; i++)
        {
            if (r + (i - c) < 10)
            {
                if (buttons[r + (i - c), i].GetComponentInChildren<Image>().sprite == clickedButtonSprite) dl_count++;
                else break;
            }
            else break;
        }
        if (dl_count + 1 >= 5) res = true;

        //Diagonal Check: Top Right to Left
        for (int i = c + 1; i < 10; i++)
        {
            if (r - (i - c) >= 0)
            {
                if (buttons[r - (i - c), i].GetComponentInChildren<Image>().sprite == clickedButtonSprite) dr_count++;
                else break;
            }
            else break;
        }
        for (int i = c - 1; i >= 0; i--)
        {
            if (r + (c - i) < 10)
            {
                if (buttons[r + (c - i), i].GetComponentInChildren<Image>().sprite == clickedButtonSprite) dr_count++;
                else break;
            }
            else break;
        }
        if (dr_count + 1 >= 5) res = true;
        return res;
    }
    private bool IsDraw()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (buttons[i, j].GetComponent<Image>().sprite != xSprite && buttons[i, j].GetComponent<Image>().sprite != oSprite)
                {
                    return false;
                }
            }
        }
        return true;
    }

    #endregion
}
