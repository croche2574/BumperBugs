using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : Unit
{
    public string PlayerName;
    public int PlayerNum;
    public bool IsDead = false;
    public int Score = 0;
    public int BerryCounter;
    public int BumpCounter;
    public int BumpedCounter;
    public int MoveCounter;
    public int FinalScore;
    public bool IsActive = true;
    public bool IsWinner = false;

    //Added a light to the players to signal which player you are
    public Light thisIsYou;
    public ScoreCard ScoreCard;
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        if (flag == "SETNAME")
        {
            PlayerName = value;
            if (IsServer)
            {
                SendUpdate("SETNAME", PlayerName);
            }
        }
        if (flag == "SETSCORE")
        {
            Score = int.Parse(value);
            if (IsServer)
                SendUpdate("SETSCORE", Score.ToString());
        }
        if (flag == "SETNUM")
        {
            PlayerNum = int.Parse(value);
        }
        if (flag == "VERTICAL")
        {
            if (CanMove)
            {
                if (float.Parse(value) > 0)
                {
                    base.SetMove("up");
                    //The SendUpdates here will make sure players face the direction they just moved in
                    //This is probably best done at a different timing -- as is now, players who input first will have their moves
                    //telegraphed to the other players, giving the early player a disadvantage
                    //This actually needed to be done for both client and server for all animations to work properly
                    anim.direction = 0;
                    SendUpdate("DIR", "0");
                }
                else if (float.Parse(value) < 0)
                {
                    base.SetMove("down");
                    anim.direction = 2;
                    SendUpdate("DIR", "2");
                }
            }
        }
        if (flag == "HORIZONTAL")
        {
            if (CanMove)
            {
                if (float.Parse(value) > 0)
                {
                    base.SetMove("right");
                    anim.direction = 1;
                    SendUpdate("DIR", "1");
                }
                else if (float.Parse(value) < 0)
                {
                    base.SetMove("left");
                    anim.direction = 3;
                    SendUpdate("DIR", "3");
                }
            }
        }
        if(flag == "DIR")
        {
            anim.direction = int.Parse(value);
        }
        if (flag == "BERRYCTR")
        {
            BerryCounter = int.Parse(value);
        }
        if (flag == "BUMPCTR")
        {
            BumpCounter = int.Parse(value);
        }
        if (flag == "BUMPEDCTR")
        {
            BumpedCounter = int.Parse(value);
        }
        if (flag == "MOVECTR")
        {
            MoveCounter = int.Parse(value);
        }
        if (flag == "FINALSCORE")
        {
            FinalScore = int.Parse(value);
        }
        if (flag  == "WINNER")
        {
            IsWinner = bool.Parse(value);
        }
    }
    public void UpdateUI()
    {
        ScoreCard.UpdateInfo(PlayerNum, PlayerName, Score);
    }
    public void StartRespawn()
    {
        StartCoroutine("Respawn");
    }
    public IEnumerator Respawn()
    {
        bool waiting = true;
        Score -= 10;
        if (Score < 0)
            Score = 0;
        GameSquare spawn = GameObject.FindObjectOfType<NetGameManager>().StartSpaces[PlayerNum];
        BumpedCounter++;
        CurrentSquare.SetOccupant();
        this.transform.position = new Vector3(0, -3, 0);
        while (waiting)
        {
            if (!spawn.IsOccupied && spawn.FutureOccupants.Count == 0)
            {
                FinalizeMove(spawn, false);
                break;
            }
            else
                yield return new WaitForSeconds(1);

        }
        
        IsDead = false;
    }
    public override IEnumerator SlowUpdate()
    {
        base.SlowUpdate();
        if (IsServer)
        {
            ScoreCard = MyCore.NetCreateObject(13, this.Owner, Vector3.zero, Quaternion.identity).GetComponent<ScoreCard>();
        }
        while (true)
        {
            while (!IsActive)
            {
                yield return new WaitForSeconds(1);
            }
            if (IsLocalPlayer)
            {
                SendCommand("VERTICAL", (Input.GetAxis("Vertical")).ToString());
                SendCommand("HORIZONTAL", (Input.GetAxis("Horizontal")).ToString());
            }
            else
            {
                thisIsYou.enabled = false;
            }
            if (IsServer)
            {
                if (IsDirty)
                {
                    SendUpdate("SETNAME", PlayerName);
                    SendUpdate("SETSCORE", Score.ToString());
                    SendUpdate("SETNUM", PlayerNum.ToString());
                    SendUpdate("BERRYCTR", BerryCounter.ToString());
                    SendUpdate("BUMPCTR", BumpCounter.ToString());
                    SendUpdate("BUMPEDCTR", BumpedCounter.ToString());
                    SendUpdate("MOVECTR", MoveCounter.ToString());
                    SendUpdate("FINALSCORE", FinalScore.ToString());
                    SendUpdate("WINNER", IsWinner.ToString());
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    public override void CollectItem(GameSquare square)
    {
        square.Occupant.GetComponent<Item>().Bite.Play();
        Score += square.Occupant.GetComponent<Item>().Value;
    }
    public void SetOptions(string Name, int Num)
    {
        PlayerName = Name;
        PlayerNum = Num;
    }
    public int CalculateFinalScore()
    {
        int tempScore = Score;
        tempScore += (int)(BerryCounter/5);
        tempScore += (int)(BumpCounter/5);
        tempScore -= (int)(BumpedCounter/5);
        tempScore += (int)(MoveCounter/5);
        FinalScore = tempScore;

        return FinalScore;
    }
    public override void Start()
    {
        base.Start();
        if (IsServer)
        {
            
        }    
    }
}
