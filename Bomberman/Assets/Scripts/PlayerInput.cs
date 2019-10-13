﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public delegate void OnKeyPressed();
    public OnKeyPressed onLeftKeyPressed;
    public OnKeyPressed onRightKeyPressed;
    public OnKeyPressed onUpKeyPressed;
    public OnKeyPressed onDownKeyPressed;
    public OnKeyPressed onBombKeyPressed;

    public KeyCode left = KeyCode.A;
    public KeyCode right = KeyCode.D;
    public KeyCode up = KeyCode.W;
    public KeyCode down = KeyCode.S;
    public KeyCode bomb = KeyCode.Space;

    // Start is called before the first frame update
    void Start()
    {
        //TODO: Import Input from the Input Manager
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(left))
        {
            onLeftKeyPressed?.Invoke();
            //Debug.Log("Press Left");
        }
        if (Input.GetKey(right))
        {
            onRightKeyPressed?.Invoke();
            //Debug.Log("Press Right");
        }
        if (Input.GetKey(up))
        {
            onUpKeyPressed?.Invoke();
            //Debug.Log("Press Up");
        }
        if (Input.GetKey(down))
        {
            onDownKeyPressed?.Invoke();
            //Debug.Log("Press Down");
        }
        if (Input.GetKey(bomb))
        {
            onBombKeyPressed?.Invoke();
            //Debug.Log("Press Bomb");
        }
    }
}
