using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HeadsUpDisplay : MonoBehaviour
{
    // Instance reference for easy access.
    public static HeadsUpDisplay instance;
    public TMP_Text[] huds;
    public TMP_Text[] combos;
    public Color[] comboColors;
    public int[] comboSizes;

    public static void UpdateDisplay(Player p)
    {
        // Standard HUD
        string text = string.Format("PLAYER {0}\nSCORE: {1}\nHEALTH: {2}\nACTION: {3}\nAMMO: {4}", p.playerID, p.score.ToString("D6"), p.health, ActionName(p.action, p.rounds), p.rounds);
        instance.huds[p.playerID - 1].SetText(text);

        // Combo
        int c = p.combo;
        string comboText = string.Format("COMBO {0}", c);
        Color color = instance.comboColors[Mathf.Clamp(c, 0, instance.comboColors.Length - 1)];
        TMP_Text combohud = instance.combos[p.playerID - 1];
        combohud.fontSize = instance.comboSizes[Mathf.Clamp(c, 0, instance.comboColors.Length - 1)];
        combohud.SetText(comboText);
        combohud.color  = color;
    }

    public static string ActionName(int actionID, int rounds)
    {
        switch(actionID) {
            case 0:
                return "MOVE";
            case 1:
                if(rounds > 0) {
                    return "SHOOT";
                } else {
                    return "RELOAD";
                }
            case 2:
                return "BLOCK";
            case 3:
                return "SPECIAL";
            default:
                return "UNKNOWN";
        }
    }

    
    void Start()
    {
        instance = this;
        foreach(Player p in Player.players) {
            UpdateDisplay(p);
        }
    }

    
}
