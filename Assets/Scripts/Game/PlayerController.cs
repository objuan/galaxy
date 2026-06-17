using UnityEngine;

// SwingController.cs — RollCrush
public class PlayerController : MonoBehaviour
{
    public PlayerShip player;
    public float turnSpeed = 10;

    void Start(){
    }

    void Update()
    {
        HandleMouse();
        HandleTouch();
        HandleKey();
    }

    void HandleTouch()
    {
        if (Input.touchCount == 0) return;
        Touch t = Input.GetTouch(0);
       
    }

    void HandleMouse()
    {
  
    }
    void HandleKey()
    {
        if (Input.GetKey(KeyCode.A))
        {
            player.Turn(-turnSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            player.Turn(turnSpeed);
        }

        if (Input.GetKey(KeyCode.W))
        {
            player.Speed(1);
        }
        else
            player.Speed(0);


    }

}