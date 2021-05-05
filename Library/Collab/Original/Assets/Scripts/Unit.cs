using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;

public abstract class Unit : NetworkComponent
{
    public bool CanMove = true;
    public GameBoard Board;
    public GameSquare CurrentSquare;
    
    public override void HandleMessage(string flag, string value)
    {

    }
    public override IEnumerator SlowUpdate()
    {
        CanMove = true;
        yield return new WaitForSeconds(.1f);
    }
    public void SetMove(string direction)
    {
        CanMove = false;
        CurrentSquare.IsOccupied = false;
        GameSquare temp = Board.GetAdjacent(CurrentSquare, direction);
        if (temp != null && temp.IsOccupied == false)
        {
            temp.SetFutureOccupant(this.gameObject);
        }
        else
            CanMove = true;
    }
    public void SetMove(GameSquare square)
    {
        CanMove = false;
        CurrentSquare.IsOccupied = false;
        if (square != null && square.IsOccupied == false)
        {
            square.SetFutureOccupant(this.gameObject);
        }
        else if (square != null)
        {
            CollectItem(square);
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
            float dist = Vector3.Distance(CurrentSquare.transform.position, obj.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearestObj = i;
            }
        }
        if (tag == "Player")
            return objList[nearestObj].GetComponent<Unit>().CurrentSquare;
        else if (tag == "Item")
            return objList[nearestObj].GetComponent<Item>().CurrentSquare;
        else
            return null;
    }
    public GameSquare PathToNearest(GameSquare end)
    {
        
    }
    public abstract void CollectItem(GameSquare square);
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
