using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// ホッケーゲームでゴール地域を制御するコンポーネント。
/// トリガーにパックが触れたらパックを破棄し、ゲームの状態をサーブ状態に変える
/// </summary>
public class KillzoneController : MonoBehaviour
{
    /// <summary>ゲームマネージャー</summary>
    [SerializeField] HockeyGamemanager m_gameManager = null;
    /// <summary>このゴールにパックが触れた時、サーブ権を与えるプレイヤーを指定する</summary>
    [SerializeField] int m_sideOfPlayer = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        PhotonView view = collision.gameObject.GetComponent<PhotonView>();

        if (view && view.IsMine)
        {
            PhotonNetwork.Destroy(view.gameObject);
        }

        m_gameManager.Serve(m_sideOfPlayer);
    }
}
