using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// ホッケーゲームのパドルを制御するコンポーネント
/// キー入力により左右に動く
/// </summary>
public class PaddleController : MonoBehaviour
{
    /// <summary>動く速さ</summary>
    [SerializeField] float m_moveSpeed = 10f;
    Rigidbody2D m_rb = null;
    PhotonView m_view = null;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (!m_view || !m_view.IsMine) return;      // 自分が生成したものだけ処理する

        Move();
    }

    /// <summary>
    /// 左右にキャラクターを動かす
    /// </summary>
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");

        Vector2 dir = (Vector2.right * h).normalized;
        m_rb.velocity = dir * m_moveSpeed;
    }
}
