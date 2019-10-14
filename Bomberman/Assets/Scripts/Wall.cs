using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Obstacle
{
    public bool wallDestroyed;
    public bool canDestroy;
    //TODO: Pickup should be loaded from manager
    public PickUp pickUp;

    public MeshRenderer mesh;

    private void Awake()
    {
        if(wallDestroyed)
        {
            mesh.enabled = false;
        }
        if (pickUp != null && pickUp.enabled)
        {
            pickUp.mesh.enabled = false;
        }
    }

    public void DestroyWall()
    {
        if(canDestroy)
        {
            wallDestroyed = true;
            mesh.enabled = false;
            if(pickUp != null && pickUp.enabled)
            {
                pickUp.mesh.enabled = true;
            }
        }
    }
}
