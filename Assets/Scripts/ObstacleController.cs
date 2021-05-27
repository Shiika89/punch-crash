using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// 障害物を制御するコンポーネント
/// </summary>
public class ObstacleController : MonoBehaviour
{
    /// <summary>移動速度</summary>
    [SerializeField] float m_moveSpeed = 1f;
    /// <summary>移動方向</summary>
    [SerializeField] Vector2 m_moveDirection = Vector2.down;
    PhotonView m_view = default;

    void Start()
    {
        m_view = GetComponent<PhotonView>();
    }

    void Update()
    {
        // オーナー側で移動させる
        if (m_view && m_view.IsMine)
        {
            transform.Translate(m_moveSpeed * m_moveDirection * Time.deltaTime);

            // 画面外に消えていたら破棄する
            if (transform.position.y < - 10f)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}
