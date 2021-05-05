using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
using System;
public class GameBoard : MonoBehaviour
{
    public NetworkCore MyCore;
    public int Rows = 10;
    public int Columns = 10;
    public float GapWidth = 3;
    public float Offset = -13.5f;
    public GameSquare[,] Squares;

    // Start is called before the first frame update
    void Start()
    {
        MyCore = GameObject.FindObjectOfType<NetworkCore>();
    }
    public void GenBoard()
    {
        Debug.Log("Generating Board");
        Squares = new GameSquare[Rows,Columns];
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                GameObject temp = MyCore.NetCreateObject(2, GameObject.FindObjectOfType<NetGameManager>().Owner, new Vector3(j * GapWidth + Offset, 0, i * GapWidth + Offset), Quaternion.identity);
                Squares[i,j] = temp.GetComponent<GameSquare>();
                Squares[i,j].Row = i;
                Squares[i,j].Col = j;
            }
        }
    }
    public void ResetBoard()
    {
        for (int i = 0; i < Rows; i++)
        {
            for (int j = 0; j < Columns; j++)
            {
                MyCore.NetDestroyObject(Squares[i,j].NetId);
                Squares[i,j] = null;
            }
        }
        GenBoard();
    }
    public GameSquare GetAdjacent(GameSquare cur, string direction)
    {
        try
        {
            switch (direction)
            {
                case "up":
                    return Squares[cur.Row+1, cur.Col];
                case "down":
                    return Squares[cur.Row-1, cur.Col];
                case "left":
                    return Squares[cur.Row, cur.Col-1];
                case "right":
                    return Squares[cur.Row, cur.Col+1];
                default:
                    return null;
            }
        }
        catch (IndexOutOfRangeException e)
        {
            Debug.Log("Invalid Move");
            return null;
        }
    }
    public GameSquare SelectRandomEmpty()
    {
        GameSquare temp;
        while (true)
        {
            temp = Squares[UnityEngine.Random.Range(0,9),UnityEngine.Random.Range(0,9)];
            if (!temp.IsOccupied)
                break;
        }
        return temp;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
