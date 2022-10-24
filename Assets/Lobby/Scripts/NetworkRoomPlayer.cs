using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class NetworkRoomPlayer : NetworkBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
    [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
    [SerializeField] private Button startGameButton = null;

    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;
    [SyncVar(hook = nameof(HandleNameChanged))]
    public string DisplayName = "Loading...";

    public override void OnStartAuthority() {
        CmdSetDisplayName(PlayerDataManager.StrGet("Username"));
        this.lobbyUI.SetActive(true);
    }

    public override void OnStartClient()
    {
        NetworkManagerLobby.Instance.RoomPlayers.Add(this);
        Debug.Log("Client started on Room Player");
        UpdateDisplay();
    }

    public override void OnStopClient()
    {
        NetworkManagerLobby.Instance.RoomPlayers.Remove(this);
        UpdateDisplay();
    }

    public void HandleReadyStatusChanged(bool oldVal, bool newVal) => UpdateDisplay();
    public void HandleNameChanged(string oldVal, string newVal) => UpdateDisplay();

    private void UpdateDisplay() {
        for(int i = 0; i < this.playerNameTexts.Length; i++) {
            this.playerNameTexts[i].text = "Waiting for player...";
            this.playerReadyTexts[i].text = string.Empty;
        }

        for(int i = 0; i < NetworkManagerLobby.Instance.RoomPlayers.Count; i++) {
            this.playerNameTexts[i].text = NetworkManagerLobby.Instance.RoomPlayers[i].DisplayName;
            this.playerReadyTexts[i].text = NetworkManagerLobby.Instance.RoomPlayers[i].IsReady ? "Ready" : "Not ready";
        }

        if (!hasAuthority) {
            foreach(var player in NetworkManagerLobby.Instance.RoomPlayers) {
                if (player.hasAuthority) {
                    player.UpdateDisplay();
                    break;
                }
            }
            return;
        }
    }

    public void HandleReadyToStart(bool readyToStart) {
        this.startGameButton.interactable = readyToStart;
    }

    [Command]
    private void CmdSetDisplayName(string displayName) {
        DisplayName = displayName;
    }

    [Command]
    public void CmdReadyUp() {
        IsReady = !IsReady;
        NetworkManagerLobby.Instance.NotifyPlayersOfReadyState();
    }

    [Command]
    public void CmdStartGame() {
        NetworkManagerLobby.Instance.StartGame();
    }

}