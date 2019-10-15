using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Interactable
{
    public GlobalManager manager;

    [HideInInspector]
    public Vector2 currentPos;
    Vector3 targetPos;
    public float speed = 1f;
    public int bombNB = 1;
    public int power = 1;

    public bool IsMoving { get; private set; }
    public PlayerInput input;
    public Animator anim;
    
    public int currentBomb { get; private set; }
    // Start is called before the first frame update
    public void Init()
    {
        targetPos = transform.position;
        currentPos = new Vector2(targetPos.x, targetPos.z);
        IsMoving = false;
        currentBomb = bombNB;
    }

    public void LockMvt()
    {
        IsMoving = true;
    }
    public void UnlockMvt()
    {
        IsMoving = false;
    }
    public void FaceToDir(Vector2 newPos)
    {
        //Face the player to the new direction
        transform.forward = new Vector3(newPos.x, 0, newPos.y);
    }
    
    //Set new target position for moving toward
    public void StartMoving(Vector2 newPos)
    {
        //Debug.Log("Start Moving");
        IsMoving = true;
        anim.SetBool("Walking", true);
        Vector3 curPos = transform.position;
        targetPos = new Vector3(curPos.x + newPos.x, curPos.y, curPos.z + newPos.y);       
    }
    //Called by the manager to sync the player movement with the other
    public void ForceStop(Vector3 newPos)
    {
        IsMoving = false;
        transform.position = newPos;
        currentPos = new Vector2(newPos.x, newPos.z);
        anim.SetBool("Walking", false);
    }

    //Call this to check the bomb number
    public void PlaceBomb(int value)
    {
        currentBomb += value;
        currentBomb = Mathf.Clamp(currentBomb, 0, bombNB);
    }
    //Affect pickup to player
    public void PickUp(PickUp pickup)
    {
        //Debug.Log("Pickup");
        switch (pickup.type)
        {
            case global::PickUp.PickUpType.PowerUp:
                if(power < Const.MAX_PLAYER_POWER)
                {
                    power+=Mathf.RoundToInt(pickup.value);
                }
                break;
            case global::PickUp.PickUpType.SpeedUp:
                if (speed < Const.MAX_PLAYER_SPEED)
                {
                    speed += pickup.value;
                }
                break;
            case global::PickUp.PickUpType.BombUp:
                if (bombNB < Const.MAX_PLAYER_BOMB)
                {
                    currentBomb += Mathf.RoundToInt(pickup.value);
                    bombNB += Mathf.RoundToInt(pickup.value);
                }
                break;
            default:
                break;
        }

        pickup.DestroyObject();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsMoving)
        {
            if(Vector3.Distance(transform.position, targetPos) > 0.01f)
            {
                float step = Time.deltaTime * speed;
                transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            }
            //else
            //{
            //    IsMoving = false;
            //    transform.position = targetPos;
            //}
        }
    }

    public override void DestroyObject()
    {
        base.DestroyObject();
        manager.OnPlayerDeath(this);
    }
}
