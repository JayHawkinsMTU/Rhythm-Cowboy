using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInput : MonoBehaviour
{
    // The player that the bot is focusing on.
    Player targetPlayer;
    // This player
    Player me;

    private IEnumerator InputLoop()
    {
        while(true) {
            yield return new WaitForSeconds(Random.Range(0f, MusicTimeManager.noteLength));
            //Debug.Log(targetPlayer.playerID);
            me.BotInput(targetPlayer);
        }
    }

    void Start()
    {
        targetPlayer = Player.players[Random.Range(0, Player.players.Count - 1)];
        Player.players.Add(me);
        StartCoroutine(InputLoop());
    }

    void Awake()
    {
        me = GetComponent<Player>();
        if(me == null) {
            Debug.LogError("BotInput attached to non-player character");
            Destroy(this.gameObject);
        }
    }
}
