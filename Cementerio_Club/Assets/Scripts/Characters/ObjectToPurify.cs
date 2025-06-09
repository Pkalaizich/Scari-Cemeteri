using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class ObjectToPurify : MonoBehaviour
{
    [SerializeField] private string m_PoolTag;
    [SerializeField] private float m_TimeToPurify;
    public float purifyTimer=0;
    private bool beingPurified = false;
    private bool firstSpawn = true;
    [SerializeField] private Ghost asociatedGhost;
    private CircleCollider2D cirColl;
    [SerializeField] private Image m_RadialIndicator;
    [SerializeField] private float postPurifyTime;
    private int currentIndex;
    private bool purifiable;

    private void Awake()
    {
        cirColl= GetComponent<CircleCollider2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        AudioManager.Instance.PlaySFX(0);
        beingPurified = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        beingPurified= false;
        AudioManager.Instance.StopPurification();
    }

    public void SetGhost(Ghost ghost)
    {
        asociatedGhost = ghost;
    }

    private void Update()
    {
        if(beingPurified && GameController.Instance.Ongoing && purifiable)
        {
            purifyTimer+= Time.deltaTime;            
            if(purifyTimer > m_TimeToPurify)
            {
                AudioManager.Instance.PlaySFX(1);
                EnemyGenerator.Instance.currentPurifiedTombs++;
                Tombs.Instance.SetAnimation(currentIndex, 2);
                m_RadialIndicator.fillAmount = 0;
                //this.gameObject.SetActive(false);
                Tombs.Instance.DeactivateOutline(currentIndex);
                asociatedGhost.Purified();
                cirColl.enabled= false;
                StartCoroutine(StayPurifiedSomeTime());
                return;
            }
            m_RadialIndicator.fillAmount = 1f - (purifyTimer / m_TimeToPurify);
        }
    }

    private void OnDisable()
    {
        cirColl.enabled = false;
        if (!firstSpawn)
        {
            purifyTimer = 0;
            beingPurified = false;
            if (ObjectPooler.Instance != null)
                ObjectPooler.Instance.Enqueue(m_PoolTag, this.gameObject);
            if (EnemyGenerator.Instance != null)
                EnemyGenerator.Instance.ReleaseIndex(currentIndex);
        }
        else
        {
            firstSpawn= false;
        }
    }

    private void OnEnable()
    {
        cirColl.enabled = true;
        SetPurifiable(true);
        m_RadialIndicator.fillAmount = 0;
    }

    public void SetIndex(int index)
    {
        currentIndex = index;
        Tombs.Instance.SetAnimation(currentIndex, 1);
    }

    public void SetPurifiable(bool status)
    {
        purifiable= status;
    }

    private IEnumerator StayPurifiedSomeTime()
    {
        yield return new WaitForSeconds(postPurifyTime);
        this.gameObject.SetActive(false);
        EnemyGenerator.Instance.SubstractTomb();
        Tombs.Instance.SetAnimation(currentIndex, 0);
        EnemyGenerator.Instance.currentPurifiedTombs--;
    }
}
