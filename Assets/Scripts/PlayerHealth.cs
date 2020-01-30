﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour
{

    [SyncVar(hook = "UpdateHealthBar")]
    public float m_currentHealth;

    public float m_maxHealth = 3;

    [SyncVar]
    public bool m_isDead = false;

    public GameObject m_deathPrefab;

    public RectTransform m_healthBar;

    public PlayerManager m_lastAttacker;

    // Use this for initialization
    void Start()
    {
        Reset();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateHealthBar(float value)
    {
        if (m_healthBar != null)
        {
            m_healthBar.sizeDelta = new Vector2(value / m_maxHealth * 150f, m_healthBar.sizeDelta.y);
        }
    }

    public void Damage(float damage, PlayerManager pc = null)
    {
        if (!isServer)
        {
            return;
        }
        if (pc != null && pc != this.GetComponent<PlayerManager>())
        {
            m_lastAttacker = pc;
        }

        m_currentHealth -= damage;

        if (m_currentHealth <= 0 && !m_isDead)
        {
            if (m_lastAttacker != null)
            {
                m_lastAttacker.m_score++;
                m_lastAttacker = null;
            }
            GameManager.Instance.UpdateScoreBoard();
            m_isDead = true;
            RpcDie();
        }
    }

    [ClientRpc]
    void RpcDie()
    {
        if (m_deathPrefab)
        {
            GameObject deathFx = Instantiate(m_deathPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity) as GameObject;
            GameObject.Destroy(deathFx, 3f);
        }

        SetActiveState(false);
        gameObject.SendMessage("Respawn");
    }

    void SetActiveState(bool state)
    {
        foreach (Collider c in GetComponentsInChildren<Collider>())
        {
            c.enabled = state;
        }
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = state;
        }
        foreach (Canvas c in GetComponentsInChildren<Canvas>())
        {
            c.enabled = state;
        }
    }

    public void Reset()
    {
        m_currentHealth = m_maxHealth;
        SetActiveState(true);
        m_isDead = false;
    }
}
