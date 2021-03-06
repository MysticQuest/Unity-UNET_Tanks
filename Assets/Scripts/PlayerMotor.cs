﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : NetworkBehaviour
{

    public Rigidbody m_rigidbody;
    public Transform m_chassis;
    public Transform m_turret;
    public GameObject buffObj;
    public Buff buff;

    [SyncVar]
    public float m_moveSpeed = 100f;
    [SyncVar]
    public float m_chassisRotateSpeed = 1f;
    [SyncVar]
    public float m_turretRotateSpeed = 3f;

    bool m_canMove = false;

    // public Vector3 m_chassisDirection;
    // public Vector3 m_turretDirection;


    // Use this for initialization
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        // buffObj = GameObject.FindGameObjectWithTag("Buff");
        // buff = buffObj.GetComponent<Buff>();
    }

    public void Enable()
    {
        m_canMove = true;
    }

    public void Disable()
    {
        m_canMove = false;
        m_rigidbody.velocity = Vector3.zero;
    }

    public void MovePlayer(Vector3 dir)
    {
        if (m_canMove)
        {
            Vector3 moveDirection = dir * m_moveSpeed * Time.deltaTime;
            m_rigidbody.velocity = moveDirection;
        }
    }

    public void FaceDirection(Transform xform, Vector3 dir, float rotSpeed)
    {
        if (dir != Vector3.zero && xform != null)
        {
            Quaternion desiredRot = Quaternion.LookRotation(dir);
            xform.rotation = Quaternion.Slerp(xform.rotation, desiredRot, rotSpeed * Time.deltaTime); //angle transition
        }
    }

    public void RotateChassis(Vector3 dir)
    {
        if (m_canMove)
        {
            FaceDirection(m_chassis, dir, m_chassisRotateSpeed);
        }
    }

    public void RotateTurret(Vector3 dir)
    {
        if (m_canMove)
        {
            FaceDirection(m_turret, dir, m_turretRotateSpeed);
        }
    }

    // public IEnumerator BuffSpeed()
    // {


    //     var tempSpeed = m_moveSpeed;
    //     var tempTurret = m_turretRotateSpeed;
    //     var tempRotation = m_chassisRotateSpeed;

    //     m_moveSpeed = buff.buffedSpeed;
    //     m_turretRotateSpeed = buff.buffedTurretRotation;
    //     m_chassisRotateSpeed = buff.buffedRotation;

    //     yield return new WaitForSeconds(buff.m_buffDuration);

    //     m_moveSpeed = tempSpeed;
    //     m_turretRotateSpeed = tempTurret;
    //     m_chassisRotateSpeed = tempRotation;
    // }

}
