using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

[RequireComponent(typeof(Rigidbody2D))]
public class FighterController : MonoBehaviour
{
    /// <summary>自機の色の候補</summary>
    [SerializeField] Color[] m_playerColor = default;
    /// <summary>移動する時にかける力</summary>
    [SerializeField] float m_movePower = 5f;
    /// <summary>移動キーの入力方向</summary>
    float m_h, m_v;
    Rigidbody2D m_rb = null;
    Animator m_anim = null;
    PhotonView m_view = null;

    public AudioClip clip01;
    public AudioClip clip02;
    public AudioClip clip03;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
        m_view = GetComponent<PhotonView>();
        ChangeColor();
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

    /// <summary>パンチした時の音</summary>
    void PunchAudio()
    {
        AudioSource.PlayClipAtPoint(clip01, transform.position);
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
                AudioSource.PlayClipAtPoint(clip02, transform.position);
                Die();
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (m_view.IsMine)
            {
                AudioSource.PlayClipAtPoint(clip03, transform.position);
            }
        }
    }

    /// <summary>
    /// 死んだときに呼び出す
    /// </summary>
    void Die()
    {
        Debug.Log("Die");

        // イベントを raise する
        RaiseEventOptions raiseEventoptions = new RaiseEventOptions();
        raiseEventoptions.Receivers = ReceiverGroup.All;
        SendOptions sendOptions = new SendOptions();
        PhotonNetwork.RaiseEvent((byte)NetworkEvents.Die, null, raiseEventoptions, sendOptions);

        // オブジェクトを破棄する
        PhotonNetwork.Destroy(m_view);
    }
    void ChangeColor()
    {
        int colorIndex = (m_view.OwnerActorNr - 1) % m_playerColor.Length; 
        GetComponent<SpriteRenderer>().color = m_playerColor[colorIndex];
    }
}
