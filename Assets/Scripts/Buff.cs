﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(PlayerShoot))]
[RequireComponent(typeof(Bullet))]

public class Buff : NetworkBehaviour
{
    public PlayerShoot m_pShoot;
    public PlayerMotor m_pMotor;
    public PlayerHealth m_pHealth;

    Collider m_collider;
    public GameObject m_buff;
    public ParticleSystem m_buffFX;

    GameObject buffedPlayer;

    public FXbuffOnPlayer FXscript;

    public int m_buffCD;
    public int m_buffDuration;

    public int buffedBulletSpeed;

    public int buffedShotsPerBurst;

    public float buffedBulletLifetime;
    public int buffedBulletBounces;

    public float buffedSpeed;
    public float buffedRotation;
    public float buffedTurretRotation;


    // Use this for initialization
    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.enabled = false;
        m_buffFX.Stop();

        StartCoroutine("EnableBuff");
    }

    IEnumerator EnableBuff()
    {
        yield return new WaitForSeconds(m_buffCD);
        m_collider.enabled = true;
        m_buffFX.Play();

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            GameObject buffedChild = collision.gameObject;
            buffedPlayer = buffedChild.transform.root.gameObject;

            m_pHealth = buffedPlayer.GetComponent<PlayerHealth>();
            m_pShoot = buffedPlayer.GetComponent<PlayerShoot>();
            m_pMotor = buffedPlayer.GetComponent<PlayerMotor>();

            m_collider.enabled = false;
            m_buffFX.Stop();

            BuffPlayer();
            StartCoroutine("EnableBuff");
        }
    }

    void BuffPlayer()
    {
        StartCoroutine("PlayerBuffEffect");
        CmdBuffPicker();
        PlayerBuffEffect();
    }

    void PlayerBuffEffect()
    {
        GameObject buffEffect = Instantiate(m_buff, buffedPlayer.transform.position, buffedPlayer.transform.rotation) as GameObject;
        buffEffect.transform.parent = buffedPlayer.transform;
        Vector3 temp = new Vector3(0, 0.5f, 0);
        buffEffect.transform.position += temp;
        FXscript = buffEffect.GetComponent<FXbuffOnPlayer>();

        Destroy(buffEffect, m_buffDuration);
    }


    [Command]
    void CmdBuffPicker()
    {
        int actions = Random.Range(0, 5);
        switch (actions)
        {
            case 0:
                StartCoroutine("BuffBullet");
                break;

            case 1:
                StartCoroutine("BuffRegen");
                break;

            case 2:
                StartCoroutine("BuffBulletLife");
                break;

            case 3:
                StartCoroutine("BuffBurst");
                break;

            case 4:
                StartCoroutine("BuffSpeed");
                break;

            default:
                Debug.Log("DEFAULT");
                break;

        }
    }


    IEnumerator BuffRegen()
    {
        Debug.Log("buff regen");
        FXscript.SetRegenC();

        yield return new WaitForSeconds(2f);
        m_pHealth.m_currentHealth += 1;
        yield return new WaitForSeconds(3f);
        m_pHealth.m_currentHealth += 1;
        yield return new WaitForSeconds(3f);
        m_pHealth.m_currentHealth += 1;
    }

    IEnumerator BuffBulletLife()
    {
        Debug.Log("buff bullet life");
        FXscript.SetBulletLifeC();

        var tempBounce = m_pShoot.m_bBounces;
        var tempLifetime = m_pShoot.m_bLifetime;

        m_pShoot.m_bBounces = buffedBulletBounces;
        m_pShoot.m_bLifetime = buffedBulletLifetime;

        yield return new WaitForSeconds(m_buffDuration);

        m_pShoot.m_bBounces = tempBounce;
        m_pShoot.m_bLifetime = tempLifetime;
    }

    IEnumerator BuffBurst()
    {
        Debug.Log("buff burst");
        FXscript.SetBurstC();

        var tempBurst = m_pShoot.m_shotsPerBurst;
        m_pShoot.m_shotsPerBurst = buffedShotsPerBurst;
        yield return new WaitForSeconds(m_buffDuration);
        m_pShoot.m_shotsPerBurst = tempBurst;
    }

    IEnumerator BuffSpeed()
    {
        Debug.Log("buff speed");
        FXscript.SetSpeedC();

        var tempSpeed = m_pMotor.m_moveSpeed;
        var tempTurret = m_pMotor.m_turretRotateSpeed;
        var tempRotation = m_pMotor.m_chassisRotateSpeed;

        m_pMotor.m_moveSpeed = buffedSpeed;
        m_pMotor.m_turretRotateSpeed = buffedTurretRotation;
        m_pMotor.m_chassisRotateSpeed = buffedRotation;

        yield return new WaitForSeconds(m_buffDuration);

        m_pMotor.m_moveSpeed = tempSpeed;
        m_pMotor.m_turretRotateSpeed = tempTurret;
        m_pMotor.m_chassisRotateSpeed = tempRotation;
    }

    IEnumerator BuffBullet()
    {
        Debug.Log("buff bullet");
        FXscript.SetBSpeedC();

        var tempBSpeed = m_pShoot.m_bSpeed;
        m_pShoot.m_bSpeed = buffedBulletSpeed;
        yield return new WaitForSeconds(m_buffDuration);
        m_pShoot.m_bSpeed = tempBSpeed;
    }
}
