using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

/// <summary>
/// ゲームを管理するコンポーネント
/// イベントを受け取ってそれに応じた処理をする
/// </summary>
public class GameManager : MonoBehaviour, IOnEventCallback
{
    /// <summary>障害物となるプレハブの名前</summary>
    [SerializeField] string m_obstaclePrefabName = "Obstacle Prefab";
    /// <summary>障害物を生成する位置</summary>
    [SerializeField] Transform[] m_obstacleSpawnPoints = default;
    /// <summary>障害物を生成する間隔（秒）</summary>
    [SerializeField] float m_generateObstacleInterval = 1f;
    /// <summary>ゲーム中かどうかを判断するフラグ</summary>
    bool m_inGame = false;
    public bool InGame { get { return m_inGame; } }

    /// <summary>障害物生成のためのタイマー</summary>
    float m_generateObstacleTimer;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private void Update()
    {
        // Master Client が障害物を生成する
        if (PhotonNetwork.IsMasterClient && m_inGame)
        {
            m_generateObstacleTimer += Time.deltaTime;

            if (m_generateObstacleTimer > m_generateObstacleInterval)
            {
                m_generateObstacleTimer = 0;
                GenerateObstacle();
            }
        }
    }

    void IOnEventCallback.OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)NetworkEvents.GameStart:
                Debug.Log("Game Start");
                m_inGame = true;
                break;
            case (byte)NetworkEvents.Die:
                Debug.Log("Player " + photonEvent.Sender.ToString() + " died.");
                Debug.Log("Finish Game");   // 現時点では二人プレイなので一人死んだらゲームは終わり。三人以上でプレイできるようにした場合は修正する必要がある。
                FinishGame();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 障害物を生成する
    /// </summary>
    void GenerateObstacle()
    {
        int i = Random.Range(0, m_obstacleSpawnPoints.Length);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(m_obstaclePrefabName, m_obstacleSpawnPoints[i].position, Quaternion.identity);
        }
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    void FinishGame()
    {
        m_inGame = false;

        // Master Client 側から全ての障害物を破棄する
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.FindGameObjectsWithTag("Obstacle").ToList().ForEach(go => PhotonNetwork.Destroy(go));
        }
    }
}

public enum NetworkEvents : byte
{
    GameStart,
    Die,
}
