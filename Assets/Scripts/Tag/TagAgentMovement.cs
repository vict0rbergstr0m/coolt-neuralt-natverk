using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagAgentMovement : MonoBehaviour
{
    [SerializeField] public Rigidbody rigid;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float moveForce = 1000.0f;
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private float rotateForce = 100.0f;
    [SerializeField] private float rotateDamping = 0.5f;
    private Vector3 movementDir;
    public void SetTargetMovement(Vector3 dir)
    {
        movementDir = dir;
    }

    [SerializeField]private float targetAngleVel;
    public void setTargetAngleVel(float angleVel)
    {
        targetAngleVel = angleVel;
        targetAngleVel=targetAngleVel%360;
    }

    private void FixedUpdate() 
    {
        movementForce(movementDir, moveSpeed);
        angleTorque(targetAngleVel, rotateSpeed);
    }

    void movementForce(Vector3 dir, float moveSpeed)
    {
        Vector3 relativeVel = transform.TransformDirection(dir)*moveSpeed;
        Vector3 force = (relativeVel - rigid.velocity)*moveForce;
        rigid.AddForce(force*Time.fixedDeltaTime);
    }

    void angleTorque(float angleVel, float rotateSpeed)
    {
        float targetVel = angleVel*rotateSpeed;
        float torque = (targetVel-rigid.angularVelocity.y)*rotateForce;
        float damping = -rigid.angularVelocity.y*rotateDamping*rotateForce;
        rigid.AddTorque(Vector3.up * (torque+damping)*Time.fixedDeltaTime);
    }


}
