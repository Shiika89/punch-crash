using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_backGround = default;//背景の設定
    [SerializeField] float m_scrollSpeed = 1.0f;//スクロールする速さ
    SpriteRenderer m_backgroundSpriteClone;//背景のクローンを保存する変数
    float m_PositionY;//背景の座標の最初の値
    // Start is called before the first frame update
    void Start()
    {
        m_PositionY = m_backGround.transform.position.y;   // 座標の初期値を保存しておく
        // 背景画像を複製して上に並べる
        m_backgroundSpriteClone = Instantiate(m_backGround);
        m_backgroundSpriteClone.transform.Translate(0f, m_backGround.bounds.size.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // 背景画像をスクロールする
        m_backGround.transform.Translate(0f, m_scrollSpeed * Time.deltaTime, 0f);
        m_backgroundSpriteClone.transform.Translate(0f, m_scrollSpeed * Time.deltaTime, 0f);

        // 背景画像がある程度下に降りたら、上に戻す
        if (m_backGround.transform.position.y < m_PositionY - m_backGround.bounds.size.y)
        {
            m_backGround.transform.Translate(0, m_backGround.bounds.size.y * 2, 0f);
        }

        // 背景画像のクローンがある程度下に降りたら、上に戻す
        if (m_backgroundSpriteClone.transform.position.y < m_PositionY - m_backgroundSpriteClone.bounds.size.y)
        {
            m_backgroundSpriteClone.transform.Translate(0, m_backgroundSpriteClone.bounds.size.y * 2, 0f);
        }
    }
}
