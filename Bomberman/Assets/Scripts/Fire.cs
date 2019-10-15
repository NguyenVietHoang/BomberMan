using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : Obstacle
{
    //Delegate for event when fire was triggered
    public delegate void OnEventEnded();
    public OnEventEnded OnCooldownEnded;

    public float cooldown;
    public Vector2 currentPos;
    public ParticleSystem fireParticle;

    public IEnumerator OnFire()
    {
        isActive = true;
        fireParticle.Play();

        yield return new WaitForSeconds(cooldown);

        fireParticle.Stop();
        OnCooldownEnded?.Invoke();
    }
}
