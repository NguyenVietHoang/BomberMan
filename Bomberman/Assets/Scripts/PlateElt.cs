using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class PlateElt
{
    //Position of the plate
    public Vector2 position;

    //List of obstacle on this square: wall, d-wall, bomb
    public Obstacle obstacle;
    //List of interactable element on this square: player, pick-up.
    //TODO: This should be a cursor cuz we need to add/remove elt in this list frequently 
    public List<Interactable> interactables;
}
