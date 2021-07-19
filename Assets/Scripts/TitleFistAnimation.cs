using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleFistAnimation : MonoBehaviour
{
    Animator m_anim;
    [SerializeField] float m_waitTime = 5;

    // Start is called before the first frame update
    void Start()
    {
        m_anim = GetComponent<Animator>();

        StartCoroutine("Fist");
    }

    private IEnumerator Fist()
    {
        yield return new WaitForSeconds(m_waitTime);

        m_anim.enabled = true;
    }
}
