using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput
{
    // This is basically a class to put all input bindings in one place because I suck at Unity's input system and refuse to learn it.
    // This method is hella monotonous but it makes things easier if changes need to be made.

    // Keycode bindings
    public static KeyCode p1moveup = KeyCode.W,
    p1moveleft = KeyCode.A,
    p1movedown = KeyCode.S,
    p1moveright = KeyCode.D,
    p1shootreload = KeyCode.E,
    p1block = KeyCode.Q,
    p1special = KeyCode.R,
    p2moveup = KeyCode.UpArrow,
    p2moveleft = KeyCode.LeftArrow,
    p2movedown = KeyCode.DownArrow,
    p2moveright = KeyCode.RightArrow,
    p2shootreload = KeyCode.RightShift,
    p2block = KeyCode.Slash,
    p2special = KeyCode.RightControl;

    public static bool MoveUp(int id = 1)
    {
        if(id == 1) return Input.GetKeyDown(p1moveup);
        else return Input.GetKeyDown(p2moveup);
    }
    public static bool MoveLeft(int id = 1)
    {
        if(id == 1) return Input.GetKeyDown(p1moveleft);
        else return Input.GetKeyDown(p2moveleft);
    }
    public static bool MoveDown(int id = 1) {
        if(id == 1) return Input.GetKeyDown(p1movedown);
        else return Input.GetKeyDown(p2movedown);
    }
    public static bool MoveRight(int id = 1)
    {
        if(id == 1) return Input.GetKeyDown(p1moveright);
        else return Input.GetKeyDown(p2moveright);
    }
    public static bool ShootReload(int id = 1)
    {
        if(id == 1) return Input.GetKeyDown(p1shootreload);
        else return Input.GetKeyDown(p2shootreload);
    }
    public static bool Block(int id = 1)
    {
        if(id == 1) return Input.GetKeyDown(p1block);
        else return Input.GetKeyDown(p2block);
    }
    public static bool Special(int id = 1)
    {
        if(id == 1) return Input.GetKeyDown(p1special);
        else return Input.GetKeyDown(p2special);
    }
}
