using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    /// <summary>勝者を表示する Text</summary>
    [SerializeField] Text m_winnerText = default;
    /// <summary>クリックするとシーンをリロードするパネル</summary>
    [SerializeField] GameObject m_sceneReloadPanel  = default;
    /// <summary>ゲーム中かどうかを判断するフラグ</summary>
    bool m_inGame = false;
    /// <summary>ゲーム終了処理中だけ立つフラグ</summary>
    bool m_FinishGame = false;

    public bool InGame { get { return m_inGame; } }

    /// <summary>障害物生成のためのタイマー</summary>
    float m_generateObstacleTimer;
    //Gameの開始を伝えるTextを入れる
    [SerializeField] Text m_gameStart;
    //プレイヤーを待っていることを伝えるTextを入れる
    [SerializeField] Text m_waitText;
    //GameSatrtTextのアニメーション
    Animator m_startTextAnim;

    [SerializeField] private Cinemachine.CinemachineImpulseSource m_cameraShake;
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

        if (m_inGame && m_waitText)
        {
            Destroy(m_waitText);
        }
    }

    void IOnEventCallback.OnEvent(ExitGames.Client.Photon.EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case (byte)NetworkEvents.GameStart:
                Debug.Log("Game Start");
                m_inGame = true;
                m_startTextAnim = m_gameStart.GetComponent<Animator>();
                m_startTextAnim.SetTrigger("GameStartText");
                break;
            case (byte)NetworkEvents.Die:
                Debug.Log("Player " + photonEvent.Sender.ToString() + " died.");
                Debug.Log("Finish Game");   // 現時点では二人プレイなので一人死んだらゲームは終わり。三人以上でプレイできるようにした場合は修正する必要がある。
                CameraShake();
                FinishGame(photonEvent);
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
            var go = PhotonNetwork.Instantiate(m_obstaclePrefabName, m_obstacleSpawnPoints[i].position, Quaternion.identity);
            go.transform.Rotate(Vector3.forward, Random.Range(0, 360f));
        }
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    void FinishGame(ExitGames.Client.Photon.EventData photonEvent)
    {
        m_inGame = false;
        m_FinishGame = true;

        // Master Client 側から全ての障害物を破棄する
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.FindGameObjectsWithTag("Obstacle").ToList().ForEach(go => PhotonNetwork.Destroy(go));
        }

        // 勝者を表示する
        if (photonEvent.Sender == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            m_winnerText.text = "You lose!";
        }
        else
        {
            m_winnerText.text = "You win!";
        }

        m_sceneReloadPanel.SetActive(true);
    }

    /// <summary>
    /// シーンをリロードする
    /// ゲーム終了時、クリックした時に呼び出すために作った
    /// </summary>
    public void OnClickPanel()
    {
        m_FinishGame = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void CameraShake()
    {
        if (m_cameraShake)
        {
            m_cameraShake.GenerateImpulse();
        }
    }
}

public enum NetworkEvents : byte
{
    GameStart,
    Die,
}
