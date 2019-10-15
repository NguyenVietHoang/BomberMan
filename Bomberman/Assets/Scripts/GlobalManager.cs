using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public PlateManager mapControl;
    public PlayerControl player;

    public float gameCountdown = 180f;
    float currentCountdown;
    // Start is called before the first frame update
    void Start()
    {
        player.Init();
        mapControl.Init();
        currentCountdown = gameCountdown;

        //Set player movement Input
        player.input.onLeftKeyPressed += () => 
        {
            if (!player.IsMoving)
            {
                player.LockMvt();
                StartCoroutine(mapControl.MovePlayer(player, new Vector2(-1, 0)));
            }                
        };
        player.input.onRightKeyPressed += () => 
        {
            if (!player.IsMoving)
            {
                player.LockMvt();
                StartCoroutine(mapControl.MovePlayer(player, new Vector2(1, 0)));
            }                
        };
        player.input.onUpKeyPressed += () => 
        {
            if (!player.IsMoving)
            {
                player.LockMvt();
                StartCoroutine(mapControl.MovePlayer(player, new Vector2(0, 1)));
            }
                
        } ;
        player.input.onDownKeyPressed += () => 
        {
            if (!player.IsMoving)
            {
                player.LockMvt();
                StartCoroutine(mapControl.MovePlayer(player, new Vector2(0, -1)));
            }                
        } ;
        player.input.onBombKeyPressed += () => { mapControl.PlaceBomb(player); };

        mapControl.SetPlayer(player);
    }

    public void OnPlayerDeath(PlayerControl _player)
    {
        Debug.Log("Game over, game freeze.");
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentCountdown > 0)
        {
            currentCountdown -= Time.deltaTime;
        }
        else
        {
            Debug.Log("Game Over");
        }
    }
}
