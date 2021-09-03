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
    /// <summary>クリックするとシーンをリロードするボタン</summary>
    [SerializeField] GameObject m_sceneReloadButton = default;
    /// <summary>buttonが画面に出るまでの時間</summary>
    [SerializeField] float m_buttonPopTime = 2f;
    /// <summary>ゲーム中かどうかを判断するフラグ</summary>
    bool m_inGame = false;
    /// <summary>ゲームが終了したことを検知するフラグ</summary>
    bool m_finishFlag = false;
    /// <summary>Playerが死亡したことを検知するフラグ</summary>
    bool m_diePlayer = false;
    /// <summary>時間を覚えとく変数</summary>
    float m_rememberTime = default;
    public bool InGame { get { return m_inGame; } }
    /// <summary>障害物生成のためのタイマー</summary>
    float m_generateObstacleTimer;
    //Gameの開始を伝えるTextを入れる
    [SerializeField] Text m_gameStart;
    //プレイヤーを待っていることを伝えるTextを入れる
    [SerializeField] Text m_waitText;
    //GameSatrtTextのアニメーション
    Animator m_startTextAnim;

    AudioSource m_bgm;

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

        if (m_finishFlag == true)
        {
            m_rememberTime += Time.deltaTime;
            if (m_rememberTime > m_buttonPopTime)
            {
                Debug.Log("ボタンを表示する");
                m_sceneReloadButton.SetActive(true);

                m_rememberTime = 0;
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
                m_startTextAnim = m_gameStart.GetComponent<Animator>();
                m_startTextAnim.SetTrigger("GameStartText");
                PlayBgm();
                break;
            case (byte)NetworkEvents.Die:
                Debug.Log("Player " + photonEvent.Sender.ToString() + " died.");
                Debug.Log("Finish Game");   // 現時点では二人プレイなので一人死んだらゲームは終わり。三人以上でプレイできるようにした場合は修正する必要がある。
                CameraShake();
                FinishGame(photonEvent);
                PlayBgm();
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
        m_finishFlag = true;
        // Master Client 側から全ての障害物を破棄する
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject.FindGameObjectsWithTag("Obstacle").ToList().ForEach(go => PhotonNetwork.Destroy(go));
        }
        if (!m_diePlayer)
        {
            // 勝者を表示する
            if (photonEvent.Sender == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                m_winnerText.text = "You lose!";
            }
            else
            {
                m_winnerText.text = "You win!";
            }
            m_diePlayer = true;
        }               
    }

    private void PlayBgm()
    {
        if (m_inGame)
        {
            m_bgm = GetComponent<AudioSource>();
            m_bgm.Play();
        }
        else
        {
            m_bgm.Stop();
        }
    }

    /// <summary>
    /// シーンをリロードする
    /// ゲーム終了時、クリックした時に呼び出すために作った
    /// </summary>
    public void OnClickPanel()
    {
        SceneManager.LoadScene("Title");
        m_finishFlag = false;
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
