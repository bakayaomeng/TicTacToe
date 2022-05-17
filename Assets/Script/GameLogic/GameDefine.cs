using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameDefine
{
    public static int BoardSize = 3;
    public static int VictoryCondition = 3;
}

public enum Role
{
    Attacker,
    Defender,
}

public enum GameModel
{
    Null,
    Pvp,
    Pve
}

public enum PieceState
{
    Empty,
    Attacker,
    Defender,
}
