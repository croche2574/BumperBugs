using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NETWORK_ENGINE;

public class NetPlayer : NetworkComponent
{
    public GameObject LobbyPanel;
    public string PlayerName;
    public InputField NameBox;
    public Button ReadyButton;
    public GameObject SpawnedPlayer;
    public int PlayerNum;
    public GameSquare SpawnSquare;
    public bool IsReady = false;
    public override void HandleMessage(string flag, string value)
    {
        if (flag == "SETNAME")
        {
            PlayerName = value;
            if (IsServer)
                SendUpdate("SETNAME", value);
        }
        if (flag == "READY")
        {
            IsReady = bool.Parse(value);
            if (IsServer)
                SendUpdate("READY", value);
        }
    }
    public void GameStart()
    {
        if (IsClient)
        {
            LobbyPanel.SetActive(false);
            //For each bingo card x in scene
            // x.GameStart()
        }
        if (IsServer)
        {
            SpawnedPlayer = MyCore.NetCreateObject(PlayerNum+6, this.Owner, new Vector3(SpawnSquare.transform.position.x, 0, SpawnSquare.transform.position.z), Quaternion.identity);
            SpawnedPlayer.GetComponent<PlayerController>().SetOptions(PlayerName, PlayerNum);
            SpawnedPlayer.GetComponent<Unit>().CurrentSquare = SpawnSquare;
            SpawnedPlayer.GetComponent<Unit>().CurrentSquare.SetOccupant(SpawnedPlayer.GetComponent<Unit>());
        }
    }
    public override IEnumerator SlowUpdate()
    {
        if (IsLocalPlayer)
            LobbyPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
    }
    public void OnReady()
    {
        SendCommand("READY", "true");
        SendCommand("SETNAME", PlayerName);
        ReadyButton.GetComponentInChildren<Text>().text = "Waiting...";
        ReadyButton.interactable = false;
    }
    public void SetName()
    {
        PlayerName = NameBox.text;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
