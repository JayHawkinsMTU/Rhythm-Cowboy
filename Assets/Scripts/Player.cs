/**
 * Handles user actions sent at the end of every measure
 * and other logic pertaining to the player GameObject.
 * Jay Hawkins
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static List<Player> players = new();
    public static int playersLeft = 0;
    public static int maxRounds = 1;
    public static int maxLife = 1;
    // Mappings of actions to numbers
    public const int MOVE = 0,
    SHOOT_RELOAD = 1,
    BLOCK = 2,
    SPECIAL = 3;

    // Score values
    public const int KILL_SCORE = 100,
    BLOCK_SCORE = 10,
    MAX_SCORE = 999999,
    INPUT_LIMIT = 2;

    // Instance (player specific) attributes
    public int playerID = 1;
    public char direction = 'N';
    public int action = Player.BLOCK;
    public int score = 0;
    public int health = 1;
    public int rounds = 1;
    public bool blocking;
    public float actionTime = -1; // Time of earliest action input this measure
    private SpriteRenderer spi;
    public Sprite[] sprites;
    public GameObject bullet; // Bullet projectile to spawn
    private Vector3 prevPos; // Position to return to once hitting a wall
    public GameObject shield;
    public bool timeBonus; // Bonus move for good timing
    public int inputThisBeat = 0; // Number of inputs on the current beat. Used to prevent spamming
    public int combo = 0; // # of time bonuses the player has in a row.

    // Audio stuff
    AudioSource audioSource;
    public AudioClip stepClip, shootClip, blockEquipClip, blockUseClip, hurtClip, cantMoveClip, turnClip, reloadClip;

    // Bot input
    BotInput bot;

    // Used by MusicTimeManager to activate actions of all players
    public static void SendActions(bool bonus = false)
    {
        foreach(Player p in Player.players) {
            if(!bonus || p.timeBonus) {
                p.inputThisBeat = 0;
                p.DoAction();
            }
        }
    }

    // Determines action, then executes said action
    public void DoAction()
    {
        inputThisBeat = 0;
        if(!enabled) return;
        // Grant player the time bonus if the are between 1/8th note before the deadline and the deadline. Actions CANNOT be late!
        if(actionTime >= MusicTimeManager.nextTime - MusicTimeManager.grace) {
            timeBonus = true;
            combo++;
        } else {
            timeBonus = false;
            combo = 0;
        }
        Unblock();
        switch(action) {
            case MOVE:
                MoveAction();
                break;
            case SHOOT_RELOAD:
                if(rounds < 1) {
                    ReloadAction();
                } else {
                    ShootAction();
                }
                break;
            case BLOCK:
                BlockAction();
                break;
            case SPECIAL:
                // TODO: NOT YET IMPLEMENTED
                break;
            default:
                Debug.LogError("INVALID ACTION AT DoAction() FROM PLAYER: " + playerID);
                break;
        }
        action = BLOCK;
        actionTime = -1;
        HeadsUpDisplay.UpdateDisplay(this);
    }

    public void NewRound()
    {
        playersLeft = players.Count;
        enabled = true;
        health = maxLife;
        rounds = maxRounds;
        action = BLOCK;
        spi.color = Color.white;
        prevPos = transform.position;
        HeadsUpDisplay.UpdateDisplay(this);
    }

    private void SetActionTime()
    {
        if(inputThisBeat >= INPUT_LIMIT) return;
        // Time of most recent action input
        actionTime = MusicTimeManager.time;
        // Increments inputThisBeat in order to prevent spamming, limited by INPUT_LIMIT
        inputThisBeat++;
    }

    // Moves player 1 unit in the direction they're facing
    private void MoveAction()
    {
        // Updates prevPos in case a wall is hit
        prevPos = transform.position;
        Vector3 delta = Vector3.zero;
        switch(direction) {
            case 'N':
                delta = Vector3.up;
                break;
            case 'W':
                delta = Vector3.left;
                break;
            case 'S':
                delta = Vector3.down;
                break;
            case 'E':
                delta = Vector3.right;
                break;
        }
        // Volume lowered because the clip is louder than others
        audioSource.PlayOneShot(stepClip, 0.25f);
        // Moves player to new position in the length of a 16th note
        StartCoroutine(MoveAnimation(transform.position + delta, MusicTimeManager.noteLength / 4f)); 
    }

    // Moves player over time instead of instantly
    private IEnumerator MoveAnimation(Vector3 newPos, float length)
    {
        while(transform.position != newPos) {
            transform.position = Vector3.MoveTowards(transform.position, newPos, Time.deltaTime / length);
            yield return new WaitForEndOfFrame();
        }
    }

    // Reloads to the max amount of allowable rounds
    private void ReloadAction()
    {
        rounds = maxRounds;
        audioSource.PlayOneShot(reloadClip);
    }

    // Shoots bullet and decrements rounds
    private void ShootAction()
    {
        rounds--;
        audioSource.PlayOneShot(shootClip);
        Instantiate(bullet).GetComponent<Bullet>().Shoot(this);
    }

    // Sets blocking to true. blocking should only absorb 1 bullet
    private void BlockAction()
    {
        blocking = true;
        audioSource.PlayOneShot(blockEquipClip);
        shield.SetActive(true);
    }

    // Disables shield
    private void Unblock()
    {
        blocking = false;
        shield.SetActive(false);
    }

    // Reverts back to prevPos
    public void Revert()
    {
        StopAllCoroutines(); // Prevents animation from persisting
        transform.position = prevPos;
        audioSource.PlayOneShot(cantMoveClip);
    }
    private void ChangeDirection(char dir)
    {
        SetActionTime();
        action = MOVE;
        direction = dir;
        switch(dir) {
            case 'N':
                spi.sprite = sprites[0];
                break;
            case 'W':
                spi.sprite = sprites[1];
                break;
            case 'S':
                spi.sprite = sprites[2];
                break;
            case 'E':
                spi.sprite = sprites[3];
                break;
            default:
                break;
        }
        HeadsUpDisplay.UpdateDisplay(this);
    }

    public void BotInput(Player target)
    {
        Vector2 diff = target.transform.position - transform.position;
        void ShootOrBlock()
        {
            float random = Random.Range(0f, 1f);
            // 50/50 block or shoot/reload
            if(random < .5f) {
                action = SHOOT_RELOAD;
            } else {
                action = BLOCK;
            }
        }

        void RandomMove()
        {
            float random = Random.Range(0f, 1f);

            bool horizontal = false; // Marks the priority of movement
            if(random < .5f) {
                horizontal = true;
            }

            if(horizontal) {
                if(diff.x < 0) {
                    ChangeDirection('E');
                } else {
                    ChangeDirection('W');
                }
            }
        }

        // Look at target if on the same x file and either block or shoot
        if(diff.x <= 0.25f) {
            // If target is higher, look north
            if(diff.y > 0) {
                ChangeDirection('N');
            } else {
                ChangeDirection('S');
            }
            
            ShootOrBlock();
        } else if(diff.y <= 0.25f) {
            // If target is to left, look west
            if(diff.x < 0) {
                ChangeDirection('W');
            } else {
                ChangeDirection('E');
            }

            ShootOrBlock();
        } else {
            RandomMove();
        }
        

    }

    // Checks player input and updates action
    private void HandleInput()
    {
        if(GameInput.MoveUp(playerID)) {
            ChangeDirection('N');
        } else if(GameInput.MoveLeft(playerID)) {
            ChangeDirection('W');
        } else if(GameInput.MoveDown(playerID)) {
            ChangeDirection('S');
        } else if(GameInput.MoveRight(playerID)) {
            ChangeDirection('E');
        } else if(GameInput.ShootReload(playerID)) {
            SetActionTime();
            action = SHOOT_RELOAD;
            HeadsUpDisplay.UpdateDisplay(this);
        } else if(GameInput.Block(playerID)) {
            SetActionTime();
            action = BLOCK;
            HeadsUpDisplay.UpdateDisplay(this);
        } else if(GameInput.Special()) {
            SetActionTime();
            action = SPECIAL;
            HeadsUpDisplay.UpdateDisplay(this);
        }
    }

    private void Die(GameObject by)
    {
        playersLeft--;
        enabled = false;
        spi.color = new Color(.5f, .5f, .5f);
        // If shot by bullet, give player points unless player shot self. We don't encourage suicide.
        if(by.TryGetComponent<Bullet>(out Bullet b) && b.shotBy != this) {
            b.shotBy.score += KILL_SCORE;
        } else {
            score -= Mathf.Clamp(score - KILL_SCORE, 0, MAX_SCORE);
        }

        // Loads next map on last player standing
        if(playersLeft < 2) {
            MapLoader.instance.NextMap();
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if(!enabled) return;
        if(coll.gameObject.TryGetComponent<ObjProperties>(out ObjProperties props)) {
            if(props.wall) {
                Revert();
            }
            if(props.hazard) {
                if(blocking) {
                    Unblock();
                    audioSource.PlayOneShot(blockUseClip);
                    score += BLOCK_SCORE;
                } else {
                    health--;
                    audioSource.PlayOneShot(hurtClip);
                    if(health < 1) {
                        Die(coll.gameObject);
                    }
                    
                }
                
            }
            if(props.fragile) {
                Destroy(coll.gameObject);
            }
        }
        HeadsUpDisplay.UpdateDisplay(this);
    }

    void Awake()
    {
        // Bots are added after all human players
        bot = GetComponent<BotInput>();
        players.Add(this);
        playersLeft++;
        spi = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        prevPos = transform.position;
        Unblock();
    }

    void Update()
    {
        if(bot == null) {
            HandleInput();
        }
    }    
}
