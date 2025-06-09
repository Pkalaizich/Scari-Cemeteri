using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{
    private static Player instance;
    public static Player Instance { get => instance; }
    [SerializeField] private float speed;
    [SerializeField] private int maxHearts;
    public int MAXHEARTS => maxHearts;
    private int currentHearts;
    private Rigidbody2D rb;
    public Vector2 movement;
    [SerializeField] private Light2D playerLight;
    //private Animator animator;
    private bool walking = false;
    private SpriteRenderer sr;

    [SerializeField] private List<Sprite> sprites;

    //public Transform pointsSpawn;

   

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        currentHearts = maxHearts;
        //animator = GetComponent<Animator>();
    }

    private void Start()
    {
        GameController.Instance.EndGame.AddListener(() => AudioManager.Instance.StepStatus(false));
    }
    void Update()
    {
        if (GameController.Instance.Ongoing)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            //ANIMATION
            if (movement == Vector2.zero)
            {
                sr.sprite = sprites[3];
                if(walking)
                {
                    AudioManager.Instance.StepStatus(false);
                    walking = false;
                }                
            }
            else
            {
                if (!walking)
                {
                    AudioManager.Instance.StepStatus(true);
                    walking = true;
                }
                if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
                {
                    if (movement.x < 0)
                    {
                        sr.sprite = sprites[0];
                    }
                    else
                    {
                        sr.sprite = sprites[1];
                    }
                }
                else
                {
                    if (movement.y < 0)
                    {
                        sr.sprite = sprites[3];
                    }
                    else
                    {
                        sr.sprite = sprites[2];
                    }
                }
            }
        }
        else
        {
            movement = Vector2.zero;            
        }
        sr.sortingOrder = -Mathf.RoundToInt(transform.position.y);
    }

    public void LoseHeart()
    {
        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, 0.5f, 0.25f);
        DOTween.To(() => playerLight.intensity, x => playerLight.intensity = x, 1.5f, 0.5f).SetDelay(0.25f);
        GameController.Instance.PlayerHurt?.Invoke();
        sr.DOFade(0.1f, 0.25f);
        sr.DOFade(1f, 0.5f).SetDelay(0.25f);
        currentHearts-=1;
        UIController.Instance.UpdateHearts(currentHearts);
        if(currentHearts == 0)
        {
            GameController.Instance.GameOver();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * speed * Time.deltaTime;        
    }    


}
