using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_backGround = default;//”wŒi‚Ìİ’è
    [SerializeField] float m_scrollSpeed = 1.0f;//ƒXƒNƒ[ƒ‹‚·‚é‘¬‚³
    SpriteRenderer m_backgroundSpriteClone;//”wŒi‚ÌƒNƒ[ƒ“‚ğ•Û‘¶‚·‚é•Ï”
    float m_PositionY;//”wŒi‚ÌÀ•W‚ÌÅ‰‚Ì’l
    // Start is called before the first frame update
    void Start()
    {
        m_PositionY = m_backGround.transform.position.y;   // À•W‚Ì‰Šú’l‚ğ•Û‘¶‚µ‚Ä‚¨‚­
        // ”wŒi‰æ‘œ‚ğ•¡»‚µ‚Äã‚É•À‚×‚é
        m_backgroundSpriteClone = Instantiate(m_backGround);
        m_backgroundSpriteClone.transform.Translate(0f, m_backGround.bounds.size.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // ”wŒi‰æ‘œ‚ğƒXƒNƒ[ƒ‹‚·‚é
        m_backGround.transform.Translate(0f, m_scrollSpeed * Time.deltaTime, 0f);
        m_backgroundSpriteClone.transform.Translate(0f, m_scrollSpeed * Time.deltaTime, 0f);

        // ”wŒi‰æ‘œ‚ª‚ ‚é’ö“x‰º‚É~‚è‚½‚çAã‚É–ß‚·
        if (m_backGround.transform.position.y < m_PositionY - m_backGround.bounds.size.y)
        {
            m_backGround.transform.Translate(0, m_backGround.bounds.size.y * 2, 0f);
        }

        // ”wŒi‰æ‘œ‚ÌƒNƒ[ƒ“‚ª‚ ‚é’ö“x‰º‚É~‚è‚½‚çAã‚É–ß‚·
        if (m_backgroundSpriteClone.transform.position.y < m_PositionY - m_backgroundSpriteClone.bounds.size.y)
        {
            m_backgroundSpriteClone.transform.Translate(0, m_backgroundSpriteClone.bounds.size.y * 2, 0f);
        }
    }
}
