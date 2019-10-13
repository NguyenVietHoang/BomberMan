using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateManager : MonoBehaviour
{
    public Wall[] walls;
    PlateElt[] map;
    public SimplePool bombPool;
    public GameObject bombPrefab;

    public int mapWidth = 20;
    public int mapHeight = 10;
    // Start is called before the first frame update
    public void Init()
    {
        bombPool.InitPool(15, bombPrefab);
        GeneratePlateElt();
    }

    void GeneratePlateElt()
    {
        map = new PlateElt[200];
        for(int i = 0; i < walls.Length; i++)
        {
            Vector3 wallPos = walls[i].transform.position;
            int x = Mathf.RoundToInt(wallPos.x);
            int y = Mathf.RoundToInt(wallPos.z);
            map[GetMapIndex(x, y)] = new PlateElt
            {
                position = new Vector2(x,y),
                obstacle = walls[i],
                interactables = new List<Interactable>()
            };

            //Add pickup if this wall has one
            if (walls[i].pickUp != null && walls[i].pickUp.mesh.enabled)
            {
                map[GetMapIndex(x, y)].interactables.Add(walls[i].pickUp);
            }
        }
    }

    public void SetPlayer(PlayerControl player)
    {
        map[GetMapIndex(Mathf.RoundToInt(player.currentPos.x),
                        Mathf.RoundToInt(player.currentPos.y))].interactables.Add(player);
    }

    //Convert 2D vector to 1D array index
    public int GetMapIndex(int x, int y)
    {
        return x + y * mapHeight;
    }

    public bool PlaceBomb(PlayerControl player)
    {
        return false;
    }

    public IEnumerator MovePlayer(PlayerControl player,Vector2 newPos)
    {             
        Vector2 currentPos = player.currentPos;
        int currentIndex = GetMapIndex(Mathf.RoundToInt(player.currentPos.x),
                            Mathf.RoundToInt(player.currentPos.y));
        int newIndex = GetMapIndex(Mathf.RoundToInt(player.currentPos.x + newPos.x),
                            Mathf.RoundToInt(player.currentPos.y + newPos.y));
        player.FaceToDir(newPos);
        //Debug.Log("Start Moving: " + newPos.x + "_" + newPos.y + "_"+ currentIndex + "_" + newIndex);
        //Can only move if the direction has no obstacle
        if (map[newIndex].obstacle is Wall w && !w.mesh.enabled)
        {           
            player.StartMoving(newPos);

            yield return new WaitForSeconds(1f/player.speed);

            player.ForceStop(new Vector3(currentPos.x + newPos.x, player.transform.position.y, currentPos.y + newPos.y));
            map[currentIndex].interactables.Remove(player);
            map[newIndex].interactables.Add(player);
        }   
        else
        {
            player.UnlockMvt();
        }
    }

    public void BombTrigger(Bomb bomb)
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
