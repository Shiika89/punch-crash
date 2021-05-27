using UnityEngine;
using UnityEngine.UI;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// ホッケーゲームのゲームを管理するコンポーネント。
/// 二人プレイを前提としている。三人以上で同時にプレイするようにしたい場合は設計し直す必要がある。
/// </summary>
public class HockeyGamemanager : MonoBehaviour
{
    /// <summary>パックのプレハブ名</summary>
    [SerializeField] string m_packPrefabName = "ResourcePrefabName";
    /// <summary>パックを出現させる場所</summary>
    [SerializeField] Transform m_packSpawnPoint = null;
    /// <summary>画面に文字を表示するための Text</summary>
    [SerializeField] Text m_console = null;
    /// <summary>ゲームの状態</summary>
    HockeyGameState m_state = HockeyGameState.Initializing;

    void Start()
    {
        m_console.text = "Wait...";
    }

    void Update()
    {
        // 入室したら処理を始める
        if (PhotonNetwork.InRoom)
        {
            /* ゲーム状態によって処理を変える。
             * この処理は各自ローカルで処理しているが、ネットワークとしては「イベント」で処理をしたい。
             * Photon Event を使って作り直すとよいでしょう。
             */
            switch (m_state)
            {
                
                case HockeyGameState.Initializing:  // 初期化前
                    if (PhotonNetwork.CurrentRoom.PlayerCount > 1)  // 二人揃ったら
                    {
                        m_state = HockeyGameState.Player2Serve; // 二人目のプレイヤーのサーブから始める
                    }
                    break;
                case HockeyGameState.Player1Serve:
                    SpawnPack(1);
                    m_state = HockeyGameState.InGame;
                    break;
                case HockeyGameState.Player2Serve:
                    SpawnPack(2);
                    m_state = HockeyGameState.InGame;
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// パックを出現させる
    /// </summary>
    /// <param name="actorNumber">サーブするプレイヤーの ActorNumber</param>
    void SpawnPack(int actorNumber)
    {
        int myActorNumber = PhotonNetwork.LocalPlayer.ActorNumber;

        // パックが飛ぶ方向をランダムに決める
        float x = Random.Range(-1f, 1f);
        float y = myActorNumber == 1 ? -1 : 1;  // 1P がサーブする時は下に、2P がサーブする時は上にサーブする
        m_packSpawnPoint.up = new Vector3(x, y, 0f);

        if (myActorNumber == actorNumber)
        {
            // パックを生成する
            PhotonNetwork.Instantiate(m_packPrefabName, m_packSpawnPoint.position, m_packSpawnPoint.rotation);
            // 1.5秒間メッセージを表示する
            m_console.text = "Press SPACE to serve...";
            Invoke("ClearConsole", 1.5f);
        }
        else
        {
            ClearConsole();
        }
    }

    /// <summary>
    /// 表示されている文字を消す
    /// </summary>
    void ClearConsole()
    {
        m_console.text = "";
    }

    /// <summary>
    /// サーブ権を設定する
    /// </summary>
    /// <param name="actorNumber">サーブ権を与えるプレイヤーの ActorNumber</param>
    public void Serve(int actorNumber)
    {
        if (actorNumber == 1)
        {
            m_state = HockeyGameState.Player1Serve;
        }
        else
        {
            m_state = HockeyGameState.Player2Serve;
        }
    }
}

public enum HockeyGameState
{
    Initializing,
    Player2Serve,
    Player1Serve,
    InGame,
}