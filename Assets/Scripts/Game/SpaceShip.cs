using NUnit.Framework;

using UnityEngine;

public class SpaceShip : SpaceFieldElement
{
    public float speedMult = 10f;
    public float turnMult = 10f;

    public float speedBust = 4f;
    public float speedNormal = 2f;

    public float speed = 1f;
    public float angle = 0f;

    public float target_angle = 0f;
    public float target_speed= 0f;

    public Vector2 dir = new Vector2(0,1);

    float startY = 0;

    void Start()
    {
        startY = 5;
    }

    private void FixedUpdate()
    {
        float dt = Time.fixedDeltaTime;

        //dir = Quaternion.Euler(0,angle,0) * new  Vector2(0,1); 
        angle = Mathf.LerpAngle(angle, target_angle, dt* turnMult);

        float rad = angle * Mathf.Deg2Rad ;
        dir= new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));

        speed = Mathf.Lerp(speed, target_speed, dt* speedMult);

        fieldPosition += dir * speed * dt;

        transform.position = spaceField.FieldToWorld(fieldPosition);
        transform.position = new Vector3(transform.position.x, startY, transform.position.z);

        transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0,dir.y), Vector3.up);
    }

    public void Turn(float turn)
    {
        target_angle += turn;
        if (target_angle < 0) target_angle += 360;
        if (target_angle > 360) target_angle -= 360;
    }
    public void Boost(bool on)
    {
        target_speed = on? speedBust : speedNormal;
    }

    public void Speed(float speed)
    {
        this.target_speed = Mathf.Clamp(speed,0, 1); 
    }

    public void Set (Vector2 fieldPosition, Vector2 dir)
    {
        this.fieldPosition = fieldPosition;
        this.dir = dir;
    }

}