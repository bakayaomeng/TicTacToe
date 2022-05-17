using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public string Name;
    public Role Role;
    private bool _isAI;

    public Player(string name, bool isAI)
    {
        _isAI = isAI;
        Name = name;
    }

    public void BattleStart(Role role)
    {
        Role = role;
    }

    private void Solver()
    {
        
    }
}
