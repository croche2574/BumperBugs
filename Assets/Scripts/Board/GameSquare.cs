using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NETWORK_ENGINE;
public class GameSquare : NetworkComponent
{
    public GameObject Occupant;
    public List<GameObject> FutureOccupants;
    public bool IsOccupied = false;
    public int Row;
    public int Col;
    public override void HandleMessage(string flag, string value)
    {
        //throw new System.NotImplementedException();
    }
    public override IEnumerator SlowUpdate()
    {
        yield return new WaitForSeconds(1f);
    }
    public void SetOccupant(Item i)
    {
        Occupant = i.gameObject;
        i.CurrentSquare = this;
        IsOccupied = true;
    }
    public void SetOccupant(Unit u)
    {
        Occupant = u.gameObject;
        u.CurrentSquare = this;
        IsOccupied = true;
    }
    public void SetOccupant()
    {
        Occupant = null;
        IsOccupied = false;
    }
    public void SetFutureOccupant(GameObject unit)
    {
        FutureOccupants.Add(unit);
    }
    public void ClearFutureOccupants()
    {
        FutureOccupants.Clear();
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
