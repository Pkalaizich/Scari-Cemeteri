using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Ghost : MonoBehaviour
{
    [SerializeField] private string m_PoolTag;

    #region private variables
    private bool movable = false;
    private Vector2 movement = Vector2.zero;    
    private Player player;
    private Rigidbody2D rb;
    private float lastSpawnTime;
    private bool firstSpawn = true;
    private bool changingPosition = false;
    private SpriteRenderer sr;
    private CapsuleCollider2D capsuleCollider;
    private Sprite originalSprite;
    private ObjectToPurify asociatedObject;
    #endregion

    
    [FormerlySerializedAs("m_Speed")][SerializeField] private float m_OriginalMaxSpeed;
    private float currentSpeed;
    private float currentMaxSpeed;
    [SerializeField] private float m_TimeToChangePosition;
    [SerializeField] private float m_SpawnDistance;
    [SerializeField] private float m_RespawnDistance;
    [SerializeField] private List<Sprite> m_PurifiedSprites;
    [SerializeField] private int pointsGiven;
    [SerializeField] private SpriteRenderer m_Outline;
    private float currentSpawnDistance;
    [SerializeField] private float m_speedIncrement;

    private void Awake()
    {
        currentSpawnDistance = m_SpawnDistance;
        sr = GetComponent<SpriteRenderer>();
        originalSprite = sr.sprite;
        capsuleCollider= GetComponent<CapsuleCollider2D>();        
    }
    private void Start()
    {
        currentSpeed = m_OriginalMaxSpeed;
        currentMaxSpeed = m_OriginalMaxSpeed;
        rb = GetComponent<Rigidbody2D>();
        player = Player.Instance;
    }

    private void Update()
    {
        if(GameController.Instance.Ongoing)
        {
            movement = (player.transform.position - this.transform.position).normalized;
            if (Time.time - lastSpawnTime > m_TimeToChangePosition && !changingPosition && movable)
            {
                StartCoroutine(StartChange());
            }
        }
    }

    private void FixedUpdate()
    {
        if(movable && GameController.Instance.Ongoing) 
        {
            if (movement.x > 0)
            {
                sr.flipX = false;
                m_Outline.flipX = false;
            }
            else
            {
                sr.flipX = true;
                m_Outline.flipX = true;
            }
            rb.linearVelocity = movement * currentSpeed * Time.deltaTime;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnEnable()
    {
        m_Outline.gameObject.SetActive(true);
        sr.DOFade(0, 0);
        sr.DOFade(1, 0.5f);
        sr.sprite = originalSprite;
        lastSpawnTime= Time.time;
        GameController.Instance.PlayerHurt.AddListener(FreezeAll);
        EnemyGenerator.Instance.EnemiesAmountChanged.AddListener(ChangeSpeedBasedOnEnemies);
        GameController.Instance.EndGame.AddListener(DeactivateOnGameOver);
    }

    public void DeactivateOnGameOver()
    {
        this.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        GameController.Instance.EndGame.RemoveListener(DeactivateOnGameOver);
        GameController.Instance.PlayerHurt.RemoveListener(FreezeAll);
        EnemyGenerator.Instance.EnemiesAmountChanged.RemoveListener(ChangeSpeedBasedOnEnemies);
        movable = false;
        if(!firstSpawn)
        {
            if (ObjectPooler.Instance != null)
                ObjectPooler.Instance.Enqueue(m_PoolTag, this.gameObject);
        }
        else
        {
            firstSpawn=false;
        }
        currentSpawnDistance = m_SpawnDistance;
    }

    public void SetAsociatedObject(ObjectToPurify objToPur)
    {
        asociatedObject = objToPur;
    }

    public void SetOutlineColor(Color color)
    {
        m_Outline.color = color;
    }

    private IEnumerator StartChange()
    {
        asociatedObject.SetPurifiable(false);
        sr.DOFade(0, 0.5f);
        m_Outline.DOFade(0, 0.5f);
        capsuleCollider.enabled = false;
        changingPosition= true;
        movable= false;
        yield return new WaitForSeconds(1f);
        asociatedObject.SetPurifiable(true);
        SetNewPosition();
        sr.DOFade(1, 0.5f);
        m_Outline.DOFade(1, 0.5f);
    }

    public void SetNewPosition()
    {
        int aux1 = Random.Range(-1, 2);
        int aux2 = Random.Range(-1, 2);
        while(aux1==0 && aux2==0)   
        {
            aux1 = Random.Range(-1, 2);
            aux2 = Random.Range(-1, 2);
        }
        Vector3 delta = new Vector3(aux1, aux2,0).normalized * m_SpawnDistance;
        this.transform.position = Player.Instance.transform.position + delta;
        changingPosition = false;
        movable = true;
        lastSpawnTime= Time.time;
        currentSpawnDistance = m_RespawnDistance;
        capsuleCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.LoseHeart();
        }
    }    

    public void changeCurrentMaxSpeed()
    {
        currentMaxSpeed += m_speedIncrement;
    }
    public void FreezeAll()
    {
        StartCoroutine(StartChange());
    }

    public void Purified()
    {
        StartCoroutine(PurifiedAnimation());
    }

    public void ChangeSpeedBasedOnEnemies()
    {
        currentSpeed = currentMaxSpeed * 0.7f + (1- EnemyGenerator.Instance.CURRENT_ENEMIES / 5) * currentMaxSpeed * 0.3f;
    }

    private IEnumerator PurifiedAnimation()
    {
        Sequence purified = DOTween.Sequence();
        purified.SetAutoKill(true);
        capsuleCollider.enabled = false;
        movable = false;
        EnemyGenerator.Instance.SubstractGhost();
        m_Outline.color = Color.white;
        m_Outline.gameObject.SetActive(false);
        for(int i = 0; i<m_PurifiedSprites.Count; i++)
        {
            sr.sprite= m_PurifiedSprites[i];
            yield return new WaitForSeconds(0.3f);
        }
        purified.Append(
            this.transform.DOMoveX(this.transform.position.x + 0.1f,0.05f).SetLoops(10, LoopType.Yoyo))
            .Join(this.transform.DOMoveY(this.transform.position.y + 0.5f, 0.5f)).
            Join(sr.DOFade(0,0.5f));
        yield return new WaitForSeconds(0.5f);
        GameController.Instance.ChangeScore(pointsGiven * EnemyGenerator.Instance.currentPurifiedTombs);
        this.gameObject.SetActive(false);
    }
}
