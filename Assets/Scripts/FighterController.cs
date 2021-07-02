using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

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
    GameManager m_gm = null;

    /// <summary>パンチの音</summary>
    [SerializeField] AudioClip clip01;
    /// <summary>壁にぶつかる音</summary>
    [SerializeField] AudioClip clip02;
    /// <summary>飛ばされた音</summary>
    [SerializeField] AudioClip clip03;
    /// <summary>爆発した音</summary>
    [SerializeField] AudioClip clip04;

    // プレイヤー名を表示するアンカーオブジェクトの名前
    string m_playerNameAnchorName = "Name";
    //プレイヤー名を表示するテキスト
    [SerializeField] Text m_nameText = default;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_anim = GetComponent<Animator>();
        m_view = GetComponent<PhotonView>();
        m_gm = FindObjectOfType<GameManager>();
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

        if (m_gm.InGame)
        {
            Vector2 dir = new Vector2(m_h, m_v).normalized;
            m_rb.AddForce(dir * m_movePower, ForceMode2D.Force);
        }

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
                // 飛ばされ死んだ時に音を出す
                AudioSource.PlayClipAtPoint(clip02, transform.position);
                Die();
            }
        }

        if (collision.gameObject.CompareTag("KillWall"))
        {
            if (m_view.IsMine)
            {
                // 壁にぶつかり死んだ時に音を出す
                AudioSource.PlayClipAtPoint(clip04, transform.position);
                Die();
            }
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            if (m_view.IsMine)
            {
                //壁にぶつかった時の音
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
        PlayerName(colorIndex);
    }
    public void PlayerName(int colorIndex)
    {
        if (!m_view || !m_view.IsMine) return;
        
        GameObject nameAnchor = GameObject.Find(m_playerNameAnchorName);
        var playerNameText = Instantiate(m_nameText, nameAnchor.transform);
        playerNameText.color = m_playerColor[colorIndex];
        playerNameText.text = "player" + (colorIndex + 1);
    }
}
