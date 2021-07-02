using UnityEngine;

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
    GameManager m_gm;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_gm = FindObjectOfType<GameManager>();
    }

    //void Update()
    //{
    //    if (m_gm.InGame)
    //    {
    //        if (m_countdown > 0)
    //        {
    //            m_countdown -= Time.deltaTime;
    //        }
    //        else
    //        {
    //            m_rb.MovePosition((Vector2)this.transform.position + m_moveSpeed * m_moveDirection * Time.deltaTime);
    //        }
    //    }
    //}
    void FixedUpdate()
    {
        if (m_gm.InGame)
        {
            if (m_countdown > 0)
            {
                m_countdown -= Time.deltaTime;
            }
            else
            {
                m_rb.MovePosition((Vector2)this.transform.position + m_moveSpeed * m_moveDirection * Time.fixedDeltaTime);
            }
        }
    }

}
