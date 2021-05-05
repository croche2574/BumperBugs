using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using NETWORK_ENGINE;
using System;
public class NetGameManager : NetworkComponent
{
    public bool GameReady = false;
    public bool GameRunning = false;
    public NetPlayer[] NetPlayers;
    public PlayerController[] PlayerControllers;
    public GameBoard Board;
    public GameSquare[] StartSpaces;
    public GameObject ScorePanel;
    public GameObject GameOverPanel;
    public int MaxEnemies;
    public int MaxItems = 10;
    public float TurnInterval = 20;
    public int RoundTimer = 120;
    public Text WinText;
    public Text PointsText;
    public Text BerriesText;
    public Text BumpText;
    public Text BumpedText;
    public Text MovesText;
    public Text FinalScoreText;
    public GameObject CountdownPanel;
    public Text CountdownText;
    public override void HandleMessage(string flag, string value)
    {
        if (flag == "GAMESTART")
        {
            GameReady = true;
            ScorePanel.SetActive(true);
            if (IsClient)
            {
                NetPlayers = GameObject.FindObjectsOfType<NetPlayer>();

                foreach (NetPlayer x in NetPlayers)
                {
                    x.GameStart();
                }
            }
        }
        if (flag == "DISPWIN" && IsClient)
        {
            Debug.Log("Is winner");
            EndGame();
        }
        if (flag == "TIMER" && IsClient)
        {
            CountdownPanel.SetActive(true);
            RoundTimer = int.Parse(value);
            UpdateTimerDisp();
        }
    }
    void UpdateTimerDisp()
    {
        TimeSpan time = TimeSpan.FromSeconds(RoundTimer);
        CountdownText.text = "Time: " + time.ToString(@"mm\:ss");
    }
    public override IEnumerator SlowUpdate()
    {
        if (IsServer)
        {
            Board.GenBoard();
            StartSpaces = new GameSquare[] { Board.Squares[0, 0], Board.Squares[0, Board.Columns - 1], Board.Squares[Board.Rows - 1, 0], Board.Squares[Board.Rows - 1, Board.Columns - 1] };
            while (!GameReady)
            {
                // See if all the players are ready.
                //If not wait
                bool testReady = true;
                NetPlayers = GameObject.FindObjectsOfType<NetPlayer>();
                if (NetPlayers.Length > 1)
                {
                    foreach (NetPlayer c in NetPlayers)
                    {
                        if (!c.IsReady)
                        {
                            testReady = false;
                            break;
                        }
                    }
                    if (testReady)
                    {
                        GameReady = true;
                        ScorePanel.SetActive(true);
                        Debug.Log("gamestart");
                        SendUpdate("GAMESTART", "1");
                        int playerCounter = 0;
                        foreach (NetPlayer x in NetPlayers)
                        {
                            x.PlayerNum = playerCounter;
                            x.SpawnSquare = StartSpaces[playerCounter];
                            x.GameStart();
                            playerCounter++;
                        }
                        PlayerControllers = GameObject.FindObjectsOfType<PlayerController>();
                        MaxEnemies = NetPlayers.Count() + 1;
                        GameRunning = true;
                    }
                }
                yield return new WaitForSeconds(.2f);
                //Set Game Ready to true//send game start.
            }
            StartCoroutine("GameTimer");
            CountdownPanel.SetActive(true);
            while (GameRunning)
            {
                Debug.Log("New Turn");
                for (int j = 0; j < Board.Columns; j++)
                {
                    for (int i = 0; i < Board.Rows; i++)
                    {
                        if (IsServer)
                            CheckMoves(i, j);
                    }
                }
                PopulateBoard();
                foreach (PlayerController player in PlayerControllers)
                {
                    player.UpdateUI();
                    if (player.IsDead)
                        player.Respawn();
                }
                yield return new WaitForSeconds(TurnInterval);
            }
        }
        foreach (PlayerController player in PlayerControllers)
        {
            player.UpdateUI();
        }
    }
    IEnumerator GameTimer()
    {
        while (RoundTimer > 0)
        {
            RoundTimer --;
            UpdateTimerDisp();
            SendUpdate("TIMER", RoundTimer.ToString());
            yield return new WaitForSeconds(1);
        }
        EndGame();
        yield return new WaitForSeconds(1);
        SendUpdate("DISPWIN", "0");
    }
    void EndGame()
    {
        //Do End Stuff
        GameRunning = false;
        Debug.Log("Game Over");
        GameOverPanel.SetActive(true);
        if (IsServer)
        {
            int winner = CalculateWinner();
            DisplayWinner(winner);
            PlayerControllers[winner].IsWinner = true;
        }
        else if (IsClient)
        {
            PlayerControllers = GameObject.FindObjectsOfType<PlayerController>();
            for (int i = 0; i < PlayerControllers.Count(); i++)
            {
                Debug.Log(PlayerControllers[i].IsWinner);
                if (PlayerControllers[i].IsWinner)
                    DisplayWinner(i);
            }
        }
    }
    int CalculateWinner()
    {
        List<int> finalScores = new List<int>();
        foreach (PlayerController player in PlayerControllers)
        {
            finalScores.Add(player.CalculateFinalScore());
        }
        return finalScores.IndexOf(finalScores.Max());
    }
    void DisplayWinner(int playerNum)
    {
        Debug.Log("Display");
        string fmt = "00";
        PlayerController player = PlayerControllers[playerNum];
        WinText.text = "Player " + (player.PlayerNum+1) + " Won!";
        PointsText.text = player.Score.ToString(fmt);
        BerriesText.text = player.BerryCounter.ToString(fmt) + " (+" + ((int)(player.BerryCounter/5)).ToString("0") + ")";
        BumpText.text = player.BumpCounter.ToString(fmt) + " (+" + ((int)(player.BumpCounter/5)).ToString("0") + ")";
        BumpedText.text = player.BumpedCounter.ToString(fmt) + " (-" + ((int)(player.BumpedCounter/5)).ToString("0") + ")";
        MovesText.text = player.MoveCounter.ToString(fmt) + " (+" + ((int)(player.MoveCounter/5)).ToString("0") + ")";
        FinalScoreText.text = player.FinalScore.ToString(fmt);
    }
    public void DisconnectGame()
    {
        StartCoroutine(MyCore.Disconnect(0, true));
    }
    void PopulateBoard()
    {
        Item[] curItems = GameObject.FindObjectsOfType<Item>();
        GameObject[] curEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        //Item Spawning
        int difference = MaxItems - curItems.Count();
        if (difference != 0)
        {
            for (int i = 0; i < difference; i++)
            {
                Debug.Log("Spawn Item");
                SpawnEntity(Board.SelectRandomEmpty(), UnityEngine.Random.Range(10, 13));
            }
        }
        //Enemy Spawning
        difference = MaxEnemies - curEnemies.Count();
        if (difference != 0)
        {
            for (int i = 0; i < difference; i++)
            {
                int selection = UnityEngine.Random.Range(3, 6);
                if (selection == 4 || selection == 5)
                {
                    int temp = UnityEngine.Random.Range(0, 5);
                    if (temp == 0 || temp == 1)
                        SpawnEntity(Board.SelectRandomEmpty(), selection, true);
                    else
                        SpawnEntity(Board.SelectRandomEmpty(), selection);
                }
                else
                    SpawnEntity(Board.SelectRandomEmpty(), selection);
            }
        }
    }
    void SpawnEntity(GameSquare square, int PrefabID, bool IsSmart = false)
    {
        //Small change made to this line of code because the fly wouldn't be oriented correctly
        GameObject temp = MyCore.NetCreateObject(PrefabID, this.Owner, square.transform.position, Quaternion.Euler(new Vector3(0, 180, 0)));
        if (temp.GetComponent<Unit>())
            square.SetOccupant(temp.GetComponent<Unit>());
        else if (temp.GetComponent<Item>())
            square.SetOccupant(temp.GetComponent<Item>());
    }
    void CheckMoves(int row, int col)
    {
        int selectedOccupant = 0;
        bool bumped = false;
        if (Board.Squares[row, col].FutureOccupants.Count != 0)
        {
            if (Board.Squares[row, col].FutureOccupants.Count == 1)
            {
                bumped = false;
                selectedOccupant = 0;
            }
            else
            {
                int playerCount = 0;
                int enemyCount = 0;
                foreach (GameObject o in Board.Squares[row, col].FutureOccupants)
                {
                    if (o.tag == "Enemy")
                        enemyCount++;
                    else if (o.tag == "Player")
                        playerCount++;
                }
                if (enemyCount == 0)
                {
                    for (int i = 0; i < Board.Squares[row, col].FutureOccupants.Count; i++)
                    {
                        bumped = true;
                        selectedOccupant = i;
                        if (selectedOccupant == 0)
                            Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<PlayerController>().BumpCounter++;
                        else
                            Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<PlayerController>().BumpedCounter++;
                    }
                }
                else if (playerCount == 0)
                {
                    bumped = false;
                    selectedOccupant = 0;
                    for (int i = 1; i < Board.Squares[row, col].FutureOccupants.Count; i++)
                    {
                        Board.Squares[row, col].FutureOccupants[i].GetComponent<Unit>().FinalizeMove(Board.Squares[row, col], true);
                    }
                }
                else
                {
                    bool firstEnemy = false;
                    for (int i = 1; i < Board.Squares[row, col].FutureOccupants.Count; i++)
                    {
                        if (Board.Squares[row, col].FutureOccupants[i].gameObject.tag == "Player")
                        {
                            Board.Squares[row, col].FutureOccupants[i].GetComponent<PlayerController>().BumpedCounter++;
                            Board.Squares[row, col].FutureOccupants[i].GetComponent<PlayerController>().FinalizeMove(Board.Squares[row, col], true);
                        }    
                        else
                        {
                            if (firstEnemy == false)
                            {
                                bumped = false;
                                firstEnemy = true;
                                selectedOccupant = i;
                            }
                        }
                    }
                }
            }
            if (Board.Squares[row, col].Occupant && Board.Squares[row, col].Occupant.tag == "Collectible" && !bumped)
            {
                MyCore.NetDestroyObject(Board.Squares[row, col].Occupant.GetComponent<Item>().NetId);
                Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<Unit>().CollectItem(Board.Squares[row, col]);
                if (Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<PlayerController>())
                    Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<PlayerController>().BerryCounter++;
            }
            if (Board.Squares[row, col].IsOccupied && Board.Squares[row, col].Occupant.GetComponent<PlayerController>() && !bumped)
                Board.Squares[row, col].Occupant.GetComponent<PlayerController>().StartRespawn();
            Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<Unit>().FinalizeMove(Board.Squares[row, col], bumped);
            if (Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<PlayerController>() && !bumped)
                Board.Squares[row, col].FutureOccupants[selectedOccupant].GetComponent<PlayerController>().MoveCounter++;

            foreach (GameObject unit in Board.Squares[row, col].FutureOccupants)
                unit.GetComponent<Unit>().CanMove = true;
            Board.Squares[row, col].FutureOccupants.Clear();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Board = GameObject.FindObjectOfType<GameBoard>();
    }
    // Update is called once per frame
    void Update()
    {

    }
}