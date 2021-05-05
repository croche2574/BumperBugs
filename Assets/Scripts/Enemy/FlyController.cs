using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyController : Unit
{
    //The fly takes a bit from both spider and beetle
    bool moveUp = true;
    bool moveRight = true;
    public bool IsSmart = false;
    public override void HandleMessage(string flag, string value)
    {
        base.HandleMessage(flag, value);
        //The fly's animations aren't the same at all
        //There is no need for an animation controller
        //The only reason it has an "Animations" script attached is so Unit doesn't throw an error
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
                    //Randomize if the fly moves horizontally or vertically for this turn
                    if(Random.Range(0, 2) == 0)
                    {
                        if (moveUp)
                        {
                            SetMove("up");
                            if (Board.GetAdjacent(CurrentSquare, "up") == null)
                                moveUp = !moveUp;
                        }
                        else
                        {
                            SetMove("down");
                            if (Board.GetAdjacent(CurrentSquare, "down") == null)
                                moveUp = !moveUp;
                        }
                    }
                    else
                    {
                        if (moveRight)
                        {
                            SetMove("right");
                            if (Board.GetAdjacent(CurrentSquare, "right") == null)
                                moveRight = !moveRight;
                        }
                        else
                        {
                            SetMove("left");
                            if (Board.GetAdjacent(CurrentSquare, "left") == null)
                                moveRight = !moveRight;
                        }
                    }
                }
                else
                {
                    GameSquare nearest = FindNearest("Collectible");
                    SetMove(PathToNearest(nearest));
                }
            }
            yield return new WaitForSeconds(.1f);
        }
    }

    public override void CollectItem(GameSquare square)
    {

    }
}
