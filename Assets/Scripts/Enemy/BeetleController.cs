using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleController : Unit
{
    public bool IsDead = false;
    //moveDir = true when moving right, false when moving left
    bool moveDir = true;

    public override void CollectItem(GameSquare square)
    {

    }

    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        if(flag == "DIR")
        {
            //Animation handling
            anim.direction = int.Parse(value);
        }
    }

    public override IEnumerator SlowUpdate()
    {
        base.SlowUpdate();
        while (true)
        {
            if (IsServer && CanMove)
            {
                if (moveDir)
                {
                    SetMove("right");
                    SendUpdate("DIR", "1");
                    if (Board.GetAdjacent(CurrentSquare, "right") == null)
                        moveDir = !moveDir;
                }
                else
                {
                    SetMove("left");
                    SendUpdate("DIR", "3");
                    if (Board.GetAdjacent(CurrentSquare, "left") == null)
                        moveDir = !moveDir;
                }
                    
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
