using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement {



	// Use this for initialization
	public override void Start () {
        base.Start();
	}

    // Update is called once per frame
    public override void Update () {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        base.Update();
	}
}
