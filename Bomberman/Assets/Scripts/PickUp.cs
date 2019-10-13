using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : Interactable
{
    public enum PickUpType
    {
        PowerUp,
        SpeedUp,
        BombUp
    }

    public PickUpType type;
    public MeshRenderer mesh;
    public Material powerUpMat;
    public Material speedUpMat;
    public Material bombUpMat;

    private void Start()
    {
        switch (type)
        {
            case PickUpType.PowerUp:
                mesh.material = powerUpMat;
                break;
            case PickUpType.SpeedUp:
                mesh.material = speedUpMat;
                break;
            case PickUpType.BombUp:
                mesh.material = bombUpMat;
                break;
            default:
                break;
        }
    }

    public override void DestroyObject()
    {
        mesh.enabled = false;
    }
}
