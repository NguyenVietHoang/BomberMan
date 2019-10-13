using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{
    public PlateManager mapControl;
    public PlayerControl player;

    // Start is called before the first frame update
    void Start()
    {
        player.Init();
        mapControl.Init();

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
        player.input.onBombKeyPressed += () => mapControl.PlaceBomb(player);

        mapControl.SetPlayer(player);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
