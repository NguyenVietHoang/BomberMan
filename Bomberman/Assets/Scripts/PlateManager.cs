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
            walls[i].isActive = walls[i].mesh.enabled;
            
            Vector3 wallPos = walls[i].transform.position;
            int x = Mathf.RoundToInt(wallPos.x);
            int y = Mathf.RoundToInt(wallPos.z);
            map[GetMapIndex(x, y)] = new PlateElt
            {
                position = new Vector3(x, 0.5f,y),
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
        map[GetMapIndex(player.currentPos.x, player.currentPos.y)].interactables.Add(player);
    }

    //Convert 2D vector to 1D array index
    public int GetMapIndex(int x, int y)
    {
        return x + y * mapHeight;
    }
    public int GetMapIndex(float x, float y)
    {
        return GetMapIndex(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    public IEnumerator PlaceBomb(PlayerControl player)
    {
        int mapIndex = GetMapIndex(player.currentPos.x, player.currentPos.y);
        if(!CheckObstacle(mapIndex))
        {
            GameObject newBomb = bombPool.Spawn(bombPool.transform, map[mapIndex].position, Quaternion.identity);
            Bomb bombStat = newBomb.GetComponent<Bomb>();
            bombStat.isActive = true;
            map[mapIndex].obstacle = bombStat;

            yield return new WaitForSeconds(bombStat.cooldown);

            bombStat.isActive = false;
            map[mapIndex].obstacle = null;
            bombStat.TriggerExplose();
            bombPool.DeSpawn(newBomb);
        }        
    }

    public IEnumerator MovePlayer(PlayerControl player,Vector2 newPos)
    {             
        Vector2 currentPos = player.currentPos;
        int currentIndex = GetMapIndex(player.currentPos.x, player.currentPos.y);
        int newIndex = GetMapIndex(player.currentPos.x + newPos.x, player.currentPos.y + newPos.y);
        player.FaceToDir(newPos);
        //Debug.Log("Start Moving: " + newPos.x + "_" + newPos.y + "_"+ currentIndex + "_" + newIndex);
        //Can only move if the direction has no obstacle
        if (!CheckObstacle(newIndex))
        {           
            player.StartMoving(newPos);

            yield return new WaitForSeconds(3 * (1f / player.speed) / 4);
            //When the player is on the 3/4 of the road, we can move him to the new Pos 
            map[currentIndex].interactables.Remove(player);
            map[newIndex].interactables.Add(player);

            yield return new WaitForSeconds((1f / player.speed) / 4);
            Debug.Log("Finish moving");
            player.UnlockMvt();
            player.ForceStop(new Vector3(currentPos.x + newPos.x, player.transform.position.y, currentPos.y + newPos.y));           
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

    //Check if there is an obstacle in this square of map
    bool CheckObstacle(int mapIndex)
    {
        return map[mapIndex].obstacle != null && map[mapIndex].obstacle.isActive;
    }
}
