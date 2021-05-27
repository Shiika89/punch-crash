﻿using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(Rigidbody2D))]
public class FighterController : MonoBehaviour
{
    [SerializeField] float m_movePower = 5f;
    Rigidbody2D m_rb = null;
    float m_h, m_v;
    Animator m_anim = null;
    PhotonView m_view = null;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
        m_view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!m_view || !m_view.IsMine) return;      // 自分が生成したものだけ処理する

        m_h = Input.GetAxisRaw("Horizontal");
        m_v = Input.GetAxisRaw("Vertical");

        if (m_anim)
        {
            m_anim.SetBool("Punch", Input.GetButtonDown("Fire1"));
        }
    }

    void FixedUpdate()
    {
        if (!m_view || !m_view.IsMine) return;      // 自分が生成したものだけ処理する

        Vector2 dir = new Vector2(m_h, m_v).normalized;
        m_rb.AddForce(dir * m_movePower, ForceMode2D.Force);

        if (m_rb.velocity != Vector2.zero)
        {
            this.transform.up = m_rb.velocity;
        }
    }

    [PunRPC]
    void Hit(Vector3 attackVector)
    {
        Debug.Log("Hit");
        if (m_view && m_view.IsMine)
        {
            m_rb.AddForce(attackVector, ForceMode2D.Impulse);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("KillZone"))
        {
            if (m_view.IsMine)
            {
                Die();
            }
        }
    }

    void Die()
    {
        Debug.Log("Die");

        RaiseEventOptions raiseEventoptions = new RaiseEventOptions();
        raiseEventoptions.Receivers = ReceiverGroup.All;
        SendOptions sendOptions = new SendOptions();
        PhotonNetwork.RaiseEvent((byte)NetworkEvents.Die, null, raiseEventoptions, sendOptions);

        PhotonNetwork.Destroy(m_view);
    }
}
