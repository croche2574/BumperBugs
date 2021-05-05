using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using UnityEngine.UI;

public class ScoreCard : NetworkComponent
{
    public Text PlayerName;
    public Text PlayerScore;
    public int Num;
    public string Name;
    public int Score;
    public override void HandleMessage(string flag, string value)    
    {
        if (flag == "NUM" && IsClient)
            Num = int.Parse(value);
        if (flag == "NAME" && IsClient)
            Name = value;
        if (flag == "SCORE" && IsClient)
            Score = int.Parse(value);
    }
    public override IEnumerator SlowUpdate()
    {
        gameObject.transform.SetParent(GameObject.Find("PlayerScorePanel").transform);
        if (IsLocalPlayer)
        {
            PlayerName.color = Color.green;
        }
        while (true)
        {
            UpdatePanel();
            if (IsServer)
            {
                if (IsDirty)
                {
                    SendUpdate("NUM", Num.ToString());
                    SendUpdate("NAME", Name);
                    SendUpdate("SCORE", Score.ToString());
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
    public void UpdateInfo(int pNetId, string pName, int pScore)
    {
        Num = pNetId;
        Name = pName;
        Score = pScore;
    }
    public void UpdatePanel()
    {
        PlayerName.text = "P" + (Num+1) + ": " + Name;
        PlayerScore.text = Score.ToString();
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
