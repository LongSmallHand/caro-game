using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public GameObject cellPrefab;
    public Transform board;
    public GridLayoutGroup gridLayout;

    public int boardSize;
    public string currentTurn = "X";
    public string[,] matrix;

    public void Start()
    {
        gridLayout.constraintCount = boardSize;
        matrix = new string[boardSize + 1, boardSize + 1];
        createBoard();
    }
    public void createBoard()
    {
        for (int i = 0; i < boardSize; i++)
        {
            for (int j = 0; j < boardSize; j++)
            {
                GameObject cellTransform =  Instantiate(cellPrefab, board);
                Cell cell = cellTransform.GetComponent<Cell>();
                cell.row = i;
                cell.col = j;
                matrix[i, j] = "";
            }
        }
    }
    public bool Check(int row, int col)
    {
        matrix[row, col] = currentTurn;
        bool res = false;
        int c_count = 0;
        int r_count = 0;
        int dl_count = 0;
        int dr_count = 0;

        //Col Check
        for (int i = row - 1; i >= 0; i--)
        {
            if (matrix[i, col] == currentTurn) c_count++;
            else break;
        }
        for (int i = row + 1; i < boardSize; i++)
        {
            if (matrix[i, col] == currentTurn) c_count++;
            else break;
        }
        if(c_count + 1 >= 5) res = true;

        //Row Check
        for (int i = col - 1; i >= 0; i--)
        {
            if (matrix[row, i] == currentTurn) r_count++;
            else break;
        }
        for (int i = col + 1; i < boardSize; i++)
        {
            if (matrix[row, i] == currentTurn)  r_count++;
            else break;
        }
        if (r_count + 1 >= 5) res = true;

        //Diagonal Check: Top Left to Right
        for (int i = col - 1; i >= 0; i--)
        {
            if(row - (col - i) >= 0)
            {
                if (matrix[row - (col - i), i] == currentTurn) dl_count++;
                else break;
            }
            else break;
        }
        for (int i = col + 1; i < boardSize; i++)
        {
            if (matrix[row + (i - col), i] == currentTurn) dl_count++;
            else break;
        }
        if (dl_count + 1 >= 5) res = true;


        //Diagonal Check: Top Right to Left
        for (int i = col + 1; i < boardSize; i++)
        {
            if (row - (i - col) >= 0)
            {
                if (matrix[row - (i - col), i] == currentTurn) dr_count++;
                else break;
            }
            else break;
        }
        for (int i = col - 1; i >= 0; i--)
        {
            if (matrix[row + (col - i), i] == currentTurn) dr_count++;
            else break;
        }
        if (dr_count + 1 >= 5)  res = true;

        return res;
    }
}