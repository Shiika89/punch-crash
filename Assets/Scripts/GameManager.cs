﻿using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using UnityEngine.UI;

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
    //Gameの開始を伝えるTextを入れる
    [SerializeField] Text m_gameStart;
    //GameSatrtTextのアニメーション
    Animator m_startTextAnim;
    [SerializeField] private Cinemachine.CinemachineImpulseSource m_cameraShake = default;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
    public void Start()
    {
        //m_cameraShake = GameObject.Find("CM VCam").GetComponent<Cinemachine.CinemachineImpulseSource>();
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
                m_startTextAnim = m_gameStart.GetComponent<Animator>();
                m_startTextAnim.SetTrigger("GameStartText");
                break;
            case (byte)NetworkEvents.Die:
                Debug.Log("Player " + photonEvent.Sender.ToString() + " died.");
                Debug.Log("Finish Game");   // 現時点では二人プレイなので一人死んだらゲームは終わり。三人以上でプレイできるようにした場合は修正する必要がある。
                CameraShake();
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
            var go = PhotonNetwork.Instantiate(m_obstaclePrefabName, m_obstacleSpawnPoints[i].position, Quaternion.identity);
            go.transform.Rotate(Vector3.forward, Random.Range(0, 360f));
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
    public void CameraShake()
    {
        m_cameraShake.GenerateImpulse();
    }
}

public enum NetworkEvents : byte
{
    GameStart,
    Die,
}
