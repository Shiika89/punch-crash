using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// Tag ゲームのプレイヤーを制御するコンポーネント
/// ダッシュした直後はしばらくダッシュできない
/// 鬼が移った後はしばらく鬼を移すことができない
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class PlayerController2D : MonoBehaviour
{
    /// <summary>動く力</summary>
    [SerializeField] float m_movePower = 10f;
    /// <summary>ダッシュ力</summary>
    [SerializeField] float m_dashPower = 50f;
    /// <summary>一度ダッシュした後、次にダッシュできるまでの秒数</summary>
    [SerializeField] float m_dashPeriod = 3f;
    /// <summary>鬼の時の色</summary>
    [SerializeField] Color m_taggedColor = Color.red;
    /// <summary>鬼ではない時の色</summary>
    [SerializeField] Color m_normalColor = Color.white;
    /// <summary>鬼を移された直後、鬼を移せない猶予期間（秒）</summary>
    [SerializeField] float m_gracePeriod = 1.5f;

    Rigidbody2D m_rb = null;
    PhotonView m_view = null;
    SpriteRenderer m_sprite = null;
    /// <summary>ダッシュの間隔を計るためのタイマー</summary>
    float m_dashTimer = 0f;
    /// <summary>猶予期間を計るためのタイマー</summary>
    float m_graceTimer = 0f;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_view = GetComponent<PhotonView>();
        m_dashTimer = m_dashPeriod;
        m_sprite = GetComponent<SpriteRenderer>();
        m_sprite.color = m_normalColor;
    }

    void Update()
    {
        if (!m_view || !m_view.IsMine) return;      // 自分が生成したものだけ処理する

        Move();
        Rotate();

        // ダッシュの処理
        if (m_dashTimer < m_dashPeriod)
        {
            m_dashTimer += Time.deltaTime;
        }
        else if (Input.GetButtonDown("Jump"))
        {
            // ダッシュタイマーが溜まった時に Jump ボタンを押すとダッシュする
            Dash();
            m_dashTimer = 0f;
        }

        // 猶予期間のタイマー処理
        if (m_graceTimer < m_gracePeriod)
        {
            m_graceTimer += Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 自分が生成したオブジェクトが鬼であり、かつ猶予期間でなく衝突相手がプレイヤーである時
        if (m_view && m_view.IsMine && m_sprite.color.Equals(m_taggedColor)
            && m_graceTimer >= m_gracePeriod && collision.gameObject.CompareTag("Player"))
        {
            // 衝突相手を鬼にする
            PhotonView view = collision.gameObject.GetComponent<PhotonView>();

            if (view)
            {
                view.RPC("Tag", RpcTarget.All);
            }

            // 自分を鬼ではなくする
            m_view.RPC("Release", RpcTarget.All);
        }
    }

    /// <summary>
    /// ダッシュする
    /// </summary>
    void Dash()
    {
        m_rb.AddForce(this.transform.up * m_dashPower, ForceMode2D.Impulse);
    }

    /// <summary>
    /// プレイヤーを進行方向に向ける
    /// </summary>
    void Rotate()
    {
        this.transform.up = m_rb.velocity;
    }

    /// <summary>
    /// 上下左右にキャラクターを動かす
    /// </summary>
    void Move()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        Vector2 dir = (Vector2.up * v + Vector2.right * h).normalized;

        if (dir != Vector2.zero)
        {
            m_rb.AddForce(dir * m_movePower, ForceMode2D.Force);
        }
    }

    /// <summary>
    /// つかまった時に呼び出す
    /// </summary>
    [PunRPC]
    public void Tag()
    {
        ChangeColor(m_taggedColor.r, m_taggedColor.g, m_taggedColor.b, m_taggedColor.a);
    }

    /// <summary>
    /// 鬼でなくなる時に呼び出す
    /// </summary>
    [PunRPC]
    public void Release()
    {
        ChangeColor(m_normalColor.r, m_normalColor.g, m_normalColor.b, m_normalColor.a);
    }

    /// <summary>
    /// プレイヤーの色を変える
    /// </summary>
    /// <param name="r">red</param>
    /// <param name="g">green</param>
    /// <param name="b">blue</param>
    /// <param name="a">alpha</param>
    void ChangeColor(float r, float g, float b, float a)
    {
        m_sprite.color = new Color(r, g, b, a);
        m_graceTimer = 0f;  // これは Tag() に移してもよいかもしれない
    }
}
