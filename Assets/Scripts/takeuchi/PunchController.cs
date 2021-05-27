using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

public class PunchController : MonoBehaviour
{
    [SerializeField] float m_punchPower = 5f;
    PhotonView m_view = null;

    void Start()
    {
        m_view = GetComponent<PhotonView>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_view && m_view.IsMine && collision.gameObject.CompareTag("Player"))
        {
            PhotonView view = collision.gameObject.GetComponent<PhotonView>();
            
            if (view)
            {
                Vector3 dir = view.transform.position - this.transform.parent.position;
                view.RPC("Hit", RpcTarget.Others, dir.normalized * m_punchPower);
            }
        }
    }
}
