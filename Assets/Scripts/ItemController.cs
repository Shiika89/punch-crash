using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// アイテムを制御するコンポーネント
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ItemController : MonoBehaviour
{
    /// <summary>移動速度</summary>
    [SerializeField] float m_moveSpeed = 1f;
    /// <summary>移動方向</summary>
    [SerializeField] Vector2 m_moveDirection = Vector2.down;
    PhotonView m_view = default;
    Rigidbody2D m_rb = default;

    void Start()
    {
        m_view = GetComponent<PhotonView>();
        m_rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // オーナー側で移動させる
        if (m_view && m_view.IsMine)
        {
            m_rb.MovePosition((Vector2)this.transform.position + m_moveSpeed * m_moveDirection * Time.deltaTime);

            // 画面外に消えていたら破棄する
            if (transform.position.y < -10f)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Punch")
        {
            var punch = collision.GetComponent<PunchController>();
            punch.Punch += 5;
            Destroy(gameObject);
        }
    }
}
