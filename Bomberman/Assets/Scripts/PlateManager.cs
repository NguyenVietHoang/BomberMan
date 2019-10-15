using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateManager : MonoBehaviour
{
    public Wall[] walls;
    PlateElt[] map;
    object _lock = new object();

    public SimplePool bombPool;
    public GameObject bombPrefab;

    public SimplePool firePool;
    public GameObject firePrefab;

    public int mapWidth = 20;
    public int mapHeight = 10;
    // Start is called before the first frame update
    public void Init()
    {
        bombPool.InitPool(15, bombPrefab);
        firePool.InitPool(36, firePrefab);
        GeneratePlateElt();
    }

    void GeneratePlateElt()
    {
        lock(_lock)
        {
            map = new PlateElt[200];
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].isActive = walls[i].mesh.enabled;

                Vector3 wallPos = walls[i].transform.position;
                int x = Mathf.RoundToInt(wallPos.x);
                int y = Mathf.RoundToInt(wallPos.z);
                int mapIndex = GetMapIndex(x, y);
                Debug.Log(walls[i].name+" "+x + "_" + y + "_" + mapIndex);
                map[mapIndex] = new PlateElt
                {
                    position = new Vector3(x, 0.5f, y),
                    obstacle = walls[i],
                    interactables = new List<Interactable>()
                };

                //Add pickup if this wall has one
                if (walls[i].pickUp != null && walls[i].pickUp.gameObject.activeSelf)
                {
                    map[GetMapIndex(x, y)].interactables.Add(walls[i].pickUp);
                }
            }
        }       
    }

    public void SetPlayer(PlayerControl player)
    {
        lock (_lock)
        {
            map[GetMapIndex(player.currentPos.x, player.currentPos.y)].interactables.Add(player);
        }            
    }

    //Convert 2D vector to 1D array index
    public int GetMapIndex(int x, int y)
    {
        return y*mapWidth + x;
    }
    public int GetMapIndex(float x, float y)
    {
        return GetMapIndex(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    public void PlaceBomb(PlayerControl player)
    {
        //The player does not have enough bomb
        if (player.currentBomb <= 0)
            return;

        int mapIndex = GetMapIndex(player.currentPos.x, player.currentPos.y);
        if(!CheckObstacle(mapIndex))
        {
            //Get the bomb
            GameObject newBomb = bombPool.Spawn(bombPool.transform, map[mapIndex].position, Quaternion.identity);
            Bomb bombStat = newBomb.GetComponent<Bomb>();
            bombStat.isActive = true;
            bombStat.cooldown = Const.BOMB_COOLDOWN;
            bombStat.lenght = player.power;
            bombStat.currentPos = player.currentPos;
            player.PlaceBomb(-1);

            lock (_lock)
                map[mapIndex].obstacle = bombStat;

            bombStat.onBombCD = null;
            bombStat.onBombCD = StartCoroutine(bombStat.TriggerCountdown());
            bombStat.OnCooldownEnded += () =>
            {
                bombStat.isActive = false;
                player.PlaceBomb(1);
                if(bombStat.onBombCD != null)
                    StopCoroutine(bombStat.onBombCD);
                bombStat.onBombCD = null;
                ExploseBombEffect(bombStat);

                lock (_lock)
                    map[mapIndex].obstacle = null;

                bombPool.DeSpawn(newBomb);                
            };            
        }        
    }

    public void ExploseBombEffect(Bomb bomb)
    {
        //The boolean that tell us which direction is not blocked
        bool left = true;
        bool right = true;
        bool up = true;
        bool down = true;

        //Create a fire at the bomb position
        int currentMap = GetMapIndex(bomb.currentPos.x, bomb.currentPos.y);
        InitFireObject(currentMap);

        for (int i = 1; i <= bomb.lenght; i++)
        {
            if(left)
            {
                Vector2 newLeft = new Vector2(bomb.currentPos.x - i, bomb.currentPos.y);
                SproudFire(newLeft, ref left);
            }
            if(right)
            {
                Vector2 newRight = new Vector2(bomb.currentPos.x + i, bomb.currentPos.y);
                SproudFire(newRight, ref right);
            }
            if(up)
            {
                Vector2 newUp = new Vector2(bomb.currentPos.x, bomb.currentPos.y + i);
                SproudFire(newUp, ref up);
            }
            if(down)
            {
                Vector2 newDown = new Vector2(bomb.currentPos.x, bomb.currentPos.y - i);
                SproudFire(newDown, ref down);
            }           
        }
    }
    //Create fire in a direction
    void SproudFire(Vector2 dir, ref bool stat)
    {
        int mapIndex = GetMapIndex(dir.x, dir.y);
        Obstacle obs = null;
        lock (_lock)
        {
            obs  = map[mapIndex].obstacle;
            //Destroy all interactable Object
            foreach(var interactable in map[mapIndex].interactables)
            {
                interactable.DestroyObject();
            }
        }

        if (obs != null)
        {
            //Stop spround the fire
            stat = false;
            if (map[mapIndex].obstacle is Wall w)
            {
                if (w.canDestroy)
                {
                    w.mesh.enabled = false;
                    if(CheckPickupExists(mapIndex))
                    {
                        w.pickUp.mesh.enabled = true;
                    }
                    InitFireObject(mapIndex);
                }
            }
            //Trigger another bomb
            else if (map[mapIndex].obstacle is Bomb b && b.isActive)
            {
                b.OnCooldownEnded?.Invoke();
            }
        }
        else//Create a fire at the map index pos
        {
            InitFireObject(mapIndex);
        }
    }
    //Create a fire object
    void InitFireObject(int mapIndex)
    {
        GameObject newFire = firePool.Spawn(firePool.transform, map[mapIndex].position, Quaternion.identity);
        Fire fireStat = newFire.GetComponent<Fire>();
        fireStat.cooldown = Const.FIRE_COOLDOWN;

        lock (_lock)
            map[mapIndex].obstacle = fireStat;

        StartCoroutine(fireStat.OnFire());
        fireStat.OnCooldownEnded += () =>
        {
            fireStat.isActive = false;
            lock (_lock)
                map[mapIndex].obstacle = null;
            firePool.DeSpawn(newFire);
        };
    }

    bool CheckPickupExists(int mapIndex)
    {
        foreach(Interactable i in map[mapIndex].interactables)
        {
            if (i is PickUp)
                return true;
        }
        return false;
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
            lock(_lock)
            {
                if(map[newIndex].obstacle is Fire)
                {
                    player.DestroyObject();
                }
                if(map[newIndex].interactables != null && map[newIndex].interactables.Count > 0)
                {
                    for (int i = 0; i < map[newIndex].interactables.Count; i++)
                    {
                        if (map[newIndex].interactables[i] is PickUp _pickup)
                        {
                            player.PickUp(_pickup);
                            Debug.Log("Pick up: " + _pickup.type.ToString() + " at: " + newIndex);
                            map[newIndex].interactables.Remove(map[newIndex].interactables[i]);
                            break;
                        }
                    }
                }                
            }
           
            yield return new WaitForSeconds((1f / player.speed) / 4);
            //Debug.Log("Finish moving");
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
