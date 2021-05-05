using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : Unit
{
    //**** THIS CODE WAS LITERALLY COPIED FROM BEETLECONTROLLER WITH MINOR TWEAKS ****
    //**** IF THERE ARE ANY ERRORS WITH BEETLECONTROLLER, THERE WILL BE ISSUES WITH THIS ****

    public bool IsDead = false;
    //moveDir = true when moving up, false when moving down
    bool moveDir = true;
    public bool IsSmart = false;

    public override void CollectItem(GameSquare square)
    {

    }

    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        if (flag == "DIR")
        {
            //Animation handling
            anim.direction = int.Parse(value);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        base.SlowUpdate();
        while (true)
        {
            if (IsServer && CanMove)
            {
                if (!IsSmart)
                {
                    if (moveDir)
                    {
                        SetMove("up");
                        SendUpdate("DIR", "0");
                        if (Board.GetAdjacent(CurrentSquare, "up") == null)
                            moveDir = !moveDir;
                    }
                    else
                    {
                        SetMove("down");
                        SendUpdate("DIR", "2");
                        if (Board.GetAdjacent(CurrentSquare, "down") == null)
                            moveDir = !moveDir;
                    }
                }
                else
                {
                    GameSquare nearest = FindNearest("Player");
                    SetMove(PathToNearest(nearest));
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
