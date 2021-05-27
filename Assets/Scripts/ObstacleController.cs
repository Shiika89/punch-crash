using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

public class ObstacleController : MonoBehaviour
{
    [SerializeField] float m_moveSpeed = 1f;
    [SerializeField] Vector2 m_moveDirection = Vector2.down;
    PhotonView m_view = default;

    void Start()
    {
        m_view = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (m_view && m_view.IsMine)
        {
            transform.Translate(m_moveSpeed * m_moveDirection * Time.deltaTime);

            if (transform.position.y < - 10f)
            {
                PhotonNetwork.Destroy(this.gameObject);
            }
        }
    }
}
