using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void Signal_UI__OnClickPiece(int x, int y);

public delegate void Signal_Battle__Start(bool value);

public delegate void Signal_Battle__BoardChange(PieceState[,] chessBoard);

public delegate void Signal_Battle__Play(GameModel model);

public delegate void Signal_Battle__RePlay();

public delegate void Signal_Battle__GameEnd(Player victory);