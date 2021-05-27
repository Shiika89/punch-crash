using UnityEngine;
using UnityEngine.UI;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// 鬼ごっこゲームのゲーム状態を制御するコンポーネント
/// 現状では部屋がいっぱいにならないとゲームが始まらない
/// </summary>
public class TagGameManager : MonoBehaviour
{
    /// <summary>画面に文字を表示するための Text</summary>
    [SerializeField] Text m_console = null;
    /// <summary>ゲームが始まっているかを管理するフラグ</summary>
    bool m_isGameStarted = false;

    void Start()
    {
        m_console.text = "Wait for other players...";
    }

    void Update()
    {
        // 入室していて、まだゲームが始まっていない状態で、人数が揃った時
        if (PhotonNetwork.InRoom && !m_isGameStarted
            && PhotonNetwork.CurrentRoom.MaxPlayers == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            // ゲームを開始する
            m_console.text = "Game Start!";
            Invoke("ClearConsole", 1.5f);   // 1.5秒後に表示を消す
            m_isGameStarted = true; // ゲーム開始フラグを立てる

            // マスタークライアントにより、ランダムに鬼を決める
            if (PhotonNetwork.IsMasterClient)
            {
                PlayerController2D[] players = GameObject.FindObjectsOfType<PlayerController2D>();
                PhotonView view = players[Random.Range(0, players.Length)].GetComponent<PhotonView>();
                view.RPC("Tag", RpcTarget.All);
            }
        }
    }

    void ClearConsole()
    {
        m_console.text = "";
    }
}
