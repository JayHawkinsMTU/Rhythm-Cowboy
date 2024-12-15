using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMap : MonoBehaviour
{
    static Vector3 unloadPosition = new Vector3(0, -50, 0);
    static float length = 5f;
    public Vector3[] positions; // player postions. positions[0] is the position that player 1 should be.

    // Moves player to their proper starting positions
    public IEnumerator PlayerStart(float length)
    {
        //Disables player and collects starting position
        Vector3[] startingPos = new Vector3[Player.players.Count];
        foreach(Player p in Player.players) {
            startingPos[p.playerID - 1] = p.transform.position;
            p.enabled = false;
        }
        // Moves each player until they're all in position. (which should be at the same time)
        bool inPosition = false;
        while(!inPosition) {
            inPosition = true;
            foreach(Player p in Player.players) {
                Vector3 targetPos = positions[p.playerID - 1];
                Vector3 startPos = startingPos[p.playerID - 1];
                if(p.transform.position != targetPos) {
                    inPosition = false;
                    p.transform.position = Vector3.MoveTowards(p.transform.position, targetPos, Vector3.Distance(startPos, targetPos) * Time.deltaTime / length);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        // Re-enables all players as new round starts
        foreach(Player p in Player.players) {
            p.NewRound();
        }
    }

    // Slides map into view over the time of length
    private IEnumerator LoadMap(float length)
    {
        transform.position = unloadPosition;
        while(transform.position != Vector3.zero) {
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, Vector3.Distance(unloadPosition, Vector3.zero) * Time.deltaTime / length);
            yield return new WaitForEndOfFrame();
        }
    }
    // Slides map out of view and disables over the time of length
    private IEnumerator UnloadMap(float length)
    {
        transform.position = Vector3.zero;
        while(transform.position != Vector3.zero) {
            transform.position = Vector3.MoveTowards(transform.position, unloadPosition, Vector3.Distance(unloadPosition, Vector3.zero) * Time.deltaTime / length);
            yield return new WaitForEndOfFrame();
        }
        Destroy(this.gameObject);
    }

    public void UnloadMap()
    {
        StartCoroutine(UnloadMap(MusicTimeManager.measLength));
    }

    void Start()
    {
        StartCoroutine(PlayerStart(MusicTimeManager.measLength));
        StartCoroutine(LoadMap(MusicTimeManager.measLength));
    }
}
