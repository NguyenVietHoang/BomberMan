using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bomb : Obstacle
{
    //Delegate only for UI
    public delegate void OnEventEnded();
    public OnEventEnded OnCooldownEnded;

    Vector2 currentPos;
    //Time before explose
    public float cooldown;
    //Power of the bomb
    public int lenght;

    float currentCD;
    bool isCountdown;

    private void Start()
    {
        currentCD = cooldown;
        isCountdown = false;
    }

    //Set the bomb countdown
    public void TriggerCountdown()
    {
        isCountdown = true;
        currentCD = cooldown;
    }

    //Trigger the bomb to explose
    public void TriggerExplose()
    {
        OnCooldownEnded?.Invoke();
        OnCooldownEnded = null;
    }

    private void Update()
    {
        if(isCountdown)
        {
            if(currentCD > 0)
            {
                currentCD -= Time.deltaTime;
            }
            else
            {
                TriggerExplose();
            }
        }
    }

}
