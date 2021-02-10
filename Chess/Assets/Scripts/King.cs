using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Chessman
{
    public override bool[,] PossibleMove()
    {
        bool[,] r = new bool[8, 8];

        int[] CastleWL = BoardManager.Instance.CastleWhiteLeft; // Castle move check
        int[] CastleBL = BoardManager.Instance.CastleBlackLeft;
        int[] CastleWR = BoardManager.Instance.CastleWhiteRight;
        int[] CastleBR = BoardManager.Instance.CastleBlackRight;


        Chessman c;
        int i, j;

        //topside
        i = CurrentX - 1;
        j = CurrentY + 1;
        if (CurrentY != 7)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 && i < 8)
                {
                    c = BoardManager.Instance.Chessmans[i, j];
                    if (c == null)
                        r[i, j] = true;
                    else if (isWhite != c.isWhite)
                        r[i, j] = true;
                }
                i++;
            }
        }

        //downside
        i = CurrentX - 1;
        j = CurrentY - 1;
        if (CurrentY != 0)
        {
            for (int k = 0; k < 3; k++)
            {
                if (i >= 0 && i < 8)
                {
                    c = BoardManager.Instance.Chessmans[i, j];
                    if (c == null)
                        r[i, j] = true;
                    else if (isWhite != c.isWhite)
                        r[i, j] = true;
                }
                i++;
            }
        }

        //middleLeft
        if (CurrentX != 0)
        {
            c = BoardManager.Instance.Chessmans[CurrentX - 1, CurrentY];
            if (c == null)
                r[CurrentX - 1, CurrentY] = true;
            else if (isWhite != c.isWhite)
                r[CurrentX - 1, CurrentY] = true;
        }

        //middleRight
        if (CurrentX != 7)
        {
            c = BoardManager.Instance.Chessmans[CurrentX + 1, CurrentY];
            if (c == null)
                r[CurrentX + 1, CurrentY] = true;
            else if (isWhite != c.isWhite)
                r[CurrentX + 1, CurrentY] = true;
        }

        //White castle
        //left
        if(CurrentX == 4 && CurrentY == 0)
        {
            if(CastleWL[0] == 2 && CastleWL[1] == 0)
            {
                r[2, 0] = true;
            }
        }
        //right
        if (CurrentX == 4 && CurrentY == 0)
        {
            if (CastleWR[0] == 6 && CastleWR[1] == 0)
            {
                r[6, 0] = true;
            }
        }
        //Black castle
        //left
        if (CurrentX == 4 && CurrentY == 7)
        {
            if (CastleBL[0] == 2 && CastleBL[1] == 7)
            {
                r[2, 7] = true;
            }
        }
        //right
        if (CurrentX == 4 && CurrentY == 7)
        {
            if (CastleBR[0] == 6 && CastleBR[1] == 7)
            {
                r[6, 7] = true;
            }
        }
        return r;
    }
}
