using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Chessman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        //upLeft
        KnightMove(CurrentX - 1, CurrentY + 2, ref r);
        //upRight
        KnightMove(CurrentX + 1, CurrentY + 2, ref r);
        //RightUp
        KnightMove(CurrentX + 2, CurrentY + 1, ref r);
        //RightDown
        KnightMove(CurrentX + 2, CurrentY - 1, ref r);
        //LeftUp
        KnightMove(CurrentX - 2, CurrentY + 1, ref r);
        //LeftDown
        KnightMove(CurrentX - 2, CurrentY - 1, ref r);
        //downLeft
        KnightMove(CurrentX - 1, CurrentY - 2, ref r);
        //downRight
        KnightMove(CurrentX + 1, CurrentY - 2, ref r);
        return r;
    }

    public void KnightMove(int x, int y,ref bool[,] r)
    {
        Chessman c;
        if(x >= 0 && x < 8 && y >= 0 && y<8)
        {
            c = BoardManager.Instance.Chessmans[x, y];
            if (c == null)
                r[x, y] = true;
            else if (isWhite != c.isWhite)
                r[x, y] = true;
        }
    }
}
