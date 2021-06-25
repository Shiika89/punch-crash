using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// 迫りくる壁を制御するコンポーネント
/// </summary>
public class DeathZoneController : MonoBehaviour
{
    /// <summary>移動速度</summary>
    [SerializeField] float m_moveSpeed = 1f;
    /// <summary>移動方向</summary>
    [SerializeField] Vector2 m_moveDirection = Vector2.up;
    /// <summary>時間</summary>
    public float m_countdown = 1f;

    Rigidbody2D m_rb = default;

    bool isFlag = false;

    GameManager m_gm;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_gm = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // オーナー側で移動させる
        if (m_gm.InGame)
        {
            CountDown();
        }
    }

    void CountDown()
    {
        m_countdown -= Time.deltaTime;
        if (m_countdown <= 0)
        {
            if(!isFlag)
            {
                isFlag = true;
                Debug.Log("時間になった");
            }
            m_rb.MovePosition((Vector2)this.transform.position + m_moveSpeed * m_moveDirection * Time.deltaTime);

            m_countdown = 0;
        }
    }
}
