using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// ホッケーゲームのパックを制御するコンポーネント
/// </summary>
public class PackController : MonoBehaviour
{
    /// <summary>初速</summary>
    [SerializeField] float m_speed = 5f;
    /// <summary>サーブしたら true になる</summary>
    bool m_isStarted = false;
    PhotonView m_view = null;
    Rigidbody2D m_rb = null;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_view = GetComponent<PhotonView>();
    }

    void Update()
    {
        // 自分が生成したオブジェクトで、まだサーブしていない状態でスペースキーを押したら
        if (m_view && m_view.IsMine && !m_isStarted && Input.GetButtonUp("Jump"))
        {
            // フラグをたててパックをサーブする
            m_isStarted = true;
            m_rb.velocity = this.transform.up * m_speed;
        }
    }
}
