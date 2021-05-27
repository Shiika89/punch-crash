using System.Linq;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GameManager : MonoBehaviour, IOnEventCallback
{
    [SerializeField] string m_obstaclePrefabName = "Obstacle Prefab";
    [SerializeField] Transform[] m_obstacleSpawnPoints = default;
    [SerializeField] float m_generateObstacleInterval = 1f;
    bool m_inGame = false;
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

    void GenerateObstacle()
    {
        int i = Random.Range(0, m_obstacleSpawnPoints.Length);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(m_obstaclePrefabName, m_obstacleSpawnPoints[i].position, Quaternion.identity);
        }
    }

    void FinishGame()
    {
        m_inGame = false;

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
