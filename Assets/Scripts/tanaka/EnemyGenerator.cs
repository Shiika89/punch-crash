using UnityEngine;
// Photon 用の名前空間を参照する
using Photon.Pun;

/// <summary>
/// 敵を生成するコンポーネント
/// </summary>
public class EnemyGenerator : MonoBehaviour
{
    /// <summary>敵として生成するプレハブの名前</summary>
    [SerializeField] string m_enemyResourceName = "PrefabResourceName";
    /// <summary>敵を生成する間隔（秒）</summary>
    [SerializeField] float m_interval = 1f;
    float m_timer = 0f;

    void Update()
    {
        // マスタークライアント側でのみ敵を生成する
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;
        
        m_timer += Time.deltaTime;

        if (m_timer > m_interval)
        {
            m_timer = 0;
            PhotonNetwork.Instantiate(m_enemyResourceName, this.transform.position, Quaternion.identity);
        }
    }
}
