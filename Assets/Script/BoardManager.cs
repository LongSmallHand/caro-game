using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class BoardManager : NetworkBehaviour
{
    Button[,] buttons = new Button[10, 10];
    public override void OnNetworkSpawn()
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
                buttons[i, j].onClick.AddListener(delegate{ OnClickCell(r, c); });
            }
        }
    }
    [SerializeField] public Sprite xSprite, oSprite;
    private void OnClickCell(int r, int c)
    {
        if (NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 0)
        {
            buttons[r, c].GetComponent<Image>().sprite = xSprite;
            buttons[r, c].enabled = false;
            GameManager.Instance.currentTurn.Value = 1;
            CheckResult(r, c);
            ChangeSpriteClientRpc(r, c);
        }
        else if (!NetworkManager.Singleton.IsHost && GameManager.Instance.currentTurn.Value == 1)
        {
            buttons[r, c].GetComponent<Image>().sprite = oSprite;
            buttons[r, c].enabled = false;
            CheckResult(r, c);
            ChangeSpriteServerRpc(r, c);
        }
    }

    [ClientRpc]
    private void ChangeSpriteClientRpc(int r, int c)
    {
        buttons[r, c].GetComponent<Image>().sprite = xSprite;
        buttons[r, c].enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeSpriteServerRpc(int r, int c)
    {
        buttons[r, c].GetComponent<Image>().sprite = oSprite;
        buttons[r, c].enabled = false;
        GameManager.Instance.currentTurn.Value = 0;
    }

    private void CheckResult(int r, int c)
    {
        if (IsWon(r, c))
        {
            GameManager.Instance.ShowMsg("won");
            GameManager.Instance.currentTurn.Value = 2;
        }
        else
        {
            if (IsDraw())
            {
                GameManager.Instance.ShowMsg("draw");
                GameManager.Instance.currentTurn.Value = 2;
            }
        }
    }
    public bool IsWon(int r, int c)
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
            if(r + (i - c) < 10)
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
            if (r + (c - i) < 10){
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
}