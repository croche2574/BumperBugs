using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using System;

public abstract class Unit : NetworkComponent
{
    public bool CanMove = true;
    public GameBoard Board;
    public GameSquare CurrentSquare;
    public Animations anim;
    
    public override void HandleMessage(string flag, string value)
    {
        //Philip Neff -- Gonna try using this for animations to other clients
        //As it is currently, the bump animation only plays for the current client's character
        if(flag == "BUMP")
        {
            if (IsServer)
            {
                SendUpdate("BUMP", "0");
            }
            if (IsClient)
            {
                anim.bumped = true;
            }
        }
    }
    public override IEnumerator SlowUpdate()
    {
        CanMove = true;
        yield return new WaitForSeconds(.1f);
    }
    public void SetMove(string direction)
    {
        GameSquare square = Board.GetAdjacent(CurrentSquare, direction);
        if (square != null)
        {
            CanMove = false;
            CurrentSquare.IsOccupied = false;
            square.SetFutureOccupant(this.gameObject);
        }
    }
    public void SetMove(GameSquare square)
    {
        if (square != null)
        {
            CanMove = false;
            CurrentSquare.IsOccupied = false;
            square.SetFutureOccupant(this.gameObject);
        }
    }
    public void FinalizeMove(GameSquare square, bool bumped)
    {
        Debug.Log("Finalizing Move");
        if (!bumped)
        {
            CurrentSquare.SetOccupant();
            square.SetOccupant(this);
            gameObject.transform.position = CurrentSquare.transform.position;
        }
        else
        {
            SendCommand("BUMP", "0");
            anim.bumped = true;
        }
        CanMove = true;
    }
    public GameSquare FindNearest(string tag)
    {
        GameObject[] objList;
        float minDist = int.MaxValue;
        int nearestObj = 0;
        objList = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < objList.Length; i++)
        {
            float dist = Vector3.Distance(CurrentSquare.transform.position, objList[i].transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestObj = i;
            }
        }
        if (tag == "Player")
            return objList[nearestObj].GetComponent<Unit>().CurrentSquare;
        else if (tag == "Collectible")
            return objList[nearestObj].GetComponent<Item>().CurrentSquare;
        else
            return null;
    }
    public GameSquare PathToNearest(GameSquare end)
    {
        int rowDiff = CurrentSquare.Row - end.Row;
        int colDiff = CurrentSquare.Col - end.Col;

        return Board.Squares[CurrentSquare.Row - Math.Sign(rowDiff), CurrentSquare.Col - Math.Sign(colDiff)];
    }
    public abstract void CollectItem(GameSquare square);
    // Start is called before the first frame update
    public virtual void Start()
    {
        Board = GameObject.FindObjectOfType<GameBoard>();
        anim = this.gameObject.GetComponent<Animations>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
