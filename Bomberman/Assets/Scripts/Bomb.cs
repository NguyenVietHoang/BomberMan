using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Bomb : Obstacle
{
    //Delegate for event when bomb was triggered
    public delegate void OnEventEnded();
    public OnEventEnded OnCooldownEnded;
    public Coroutine onBombCD;

    public Vector2 currentPos;
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
    public IEnumerator TriggerCountdown()
    {
        isCountdown = true;
        
        yield return new WaitForSeconds(cooldown);

        //Debug.Log("On Bomb CD");
        TriggerExplose();       
    }

    //Trigger the bomb to explose
    public void TriggerExplose()
    {
        //Debug.Log("Trigger explose.");
        isCountdown = false;

        //Trigger Event
        OnCooldownEnded?.Invoke();
        OnCooldownEnded = null;
    }

    private void Update()
    {
        
    }

}
