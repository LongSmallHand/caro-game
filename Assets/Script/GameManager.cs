using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public NetworkVariable<int> currentTurn = new NetworkVariable<int>(0);
    public static GameManager Instance;
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
    private async void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (clientId) =>
        {
            Debug.Log("Client with id " + clientId + " joined");
            if (NetworkManager.Singleton.IsHost && NetworkManager.Singleton.ConnectedClients.Count == 2)
            {
                SpwanBoard();        
            }
        };

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    [SerializeField] private GameObject boardPrefab;
    private GameObject newBoard;
    private void SpwanBoard()
    {
        newBoard = Instantiate(boardPrefab);
        newBoard.GetComponent<NetworkObject>().Spawn();
    }

    [SerializeField] private TextMeshProUGUI joinCodeText;
    public async void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            joinCodeText.text = joinCode;


            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }

    }
    [SerializeField] private TMP_InputField joinCodeInput;

    public async void StartClient()
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCodeInput.text);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
    }
                catch (RelayServiceException e)
                {
                    Debug.Log(e);
                }

    }
    [SerializeField] private GameObject gameEndPanel;
    [SerializeField] private TextMeshProUGUI msgText;

    public void ShowMsg(string msg)
    {
        if (msg.Equals("won"))
        {
            msgText.text = "You Won";
            gameEndPanel.SetActive(true);
            ShowOpponentMsg("You Lose");
        }
        else if (msg.Equals("draw"))
        {
            msgText.text = "Game Draw";
            gameEndPanel.SetActive(true);
            ShowOpponentMsg("Game Draw");
        }
    }
    //Use Rpc to show Message at opponent side
    private void ShowOpponentMsg(string msg)
    {
        if (IsHost) OpponentMsgClientRpc(msg);
        else OpponentMsgServerRpc(msg);
    }

    [ClientRpc]
    private void OpponentMsgClientRpc(string msg)
    {
        if (IsHost) return;
        msgText.text = msg;
        gameEndPanel.SetActive(true);
    }
    [ServerRpc(RequireOwnership = false)]
    private void OpponentMsgServerRpc(string msg)
    {
        msgText.text = msg;
        gameEndPanel.SetActive(true);
    }

    public void Restart()
    {
        if (!IsHost)
        {
            RestartServerRpc();
            gameEndPanel.SetActive(false);
        }
        else
        {
            Destroy(newBoard);
            SpwanBoard();
            RestartClientRpc();
        }

    }

    [ServerRpc(RequireOwnership = false)]
    private void RestartServerRpc()
    {
        Destroy(newBoard);
        SpwanBoard();
        gameEndPanel.SetActive(false);
        currentTurn.Value = 0;
    }

    [ClientRpc]
    private void RestartClientRpc()
    {
        gameEndPanel.SetActive(false);
        currentTurn.Value = 0;
    }
    public void ReturnMenu()
    {
        SceneManager.LoadScene("StartScene");
    }
}
