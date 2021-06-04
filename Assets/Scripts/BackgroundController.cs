using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_backGround = default;//�w�i�̐ݒ�
    [SerializeField] float m_scrollSpeed = 1.0f;//�X�N���[�����鑬��
    SpriteRenderer m_backgroundSpriteClone;//�w�i�̃N���[����ۑ�����ϐ�
    float m_PositionY;//�w�i�̍��W�̍ŏ��̒l
    // Start is called before the first frame update
    void Start()
    {
        m_PositionY = m_backGround.transform.position.y;   // ���W�̏����l��ۑ����Ă���
        // �w�i�摜�𕡐����ď�ɕ��ׂ�
        m_backgroundSpriteClone = Instantiate(m_backGround);
        m_backgroundSpriteClone.transform.Translate(0f, m_backGround.bounds.size.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        // �w�i�摜���X�N���[������
        m_backGround.transform.Translate(0f, m_scrollSpeed * Time.deltaTime, 0f);
        m_backgroundSpriteClone.transform.Translate(0f, m_scrollSpeed * Time.deltaTime, 0f);

        // �w�i�摜��������x���ɍ~�肽��A��ɖ߂�
        if (m_backGround.transform.position.y < m_PositionY - m_backGround.bounds.size.y)
        {
            m_backGround.transform.Translate(0, m_backGround.bounds.size.y * 2, 0f);
        }

        // �w�i�摜�̃N���[����������x���ɍ~�肽��A��ɖ߂�
        if (m_backgroundSpriteClone.transform.position.y < m_PositionY - m_backgroundSpriteClone.bounds.size.y)
        {
            m_backgroundSpriteClone.transform.Translate(0, m_backgroundSpriteClone.bounds.size.y * 2, 0f);
        }
    }
}
