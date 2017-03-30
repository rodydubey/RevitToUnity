using UnityEngine;
using System.Collections;

public class DemoController : MonoBehaviour {

    [SerializeField]
    private float speed;

    enum MoveDir
    {
        None,
        Up,
        Down,
        Left,
        Right
    }


 
    void Update()
    {
         GetInput();
    }

    void GetInput()
    {
        if (Input.GetAxis("Horizontal") > 0.2f)
        {
            Move(MoveDir.Right);
        }

        if (Input.GetAxis("Horizontal") < -0.2f)
        {
            Move(MoveDir.Left);
        }

        if (Input.GetAxis("Vertical") > 0.2f)
        {
            Move(MoveDir.Up);
        }
        if (Input.GetAxis("Vertical") < -0.2f)
        {
            Move(MoveDir.Down);
        }

    }

    void Move(MoveDir moveDir)
    {
        var movement = Vector3.zero;
        switch (moveDir)
        {
            case MoveDir.Down:
                movement = new Vector3(0, -1f,0);
                break;
            case MoveDir.Up:
                movement = new Vector3(0, 1f,0);
                break;
            case MoveDir.Left:
                movement = new Vector3(-1f, 0, 0);
                break;
            case MoveDir.Right:
                movement = new Vector3(1f, 0, 0);
                break;
            default:
                return;
        }

        transform.Translate(movement*speed * Time.deltaTime);
    }


}

