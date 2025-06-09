using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;


public class UIController : MonoBehaviour
{
    [SerializeField] private List<GameObject> Hearts;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private GameObject heartsContainer;
    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI score;
    private Player player;

    [Header("Menu UI")]
    [SerializeField] private Button startBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button exitCreditsBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private Button restartGameBtn;
    [SerializeField] private TextMeshProUGUI finalScore;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private GameObject endgamePanel;
    [SerializeField] private GameObject highScorePanel;
    [SerializeField] private Button exitHSPanel;
    [SerializeField] private Button hsBtn;

    #region Singleton
    static private UIController instance;
    static public UIController Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }
    #endregion

    private void Start()
    {
        player = FindObjectOfType<Player>();
        for(int i=0; i<player.MAXHEARTS; i++)
        {            
            Hearts.Add(Instantiate(heartPrefab, heartsContainer.transform));
        }
        startBtn.onClick.AddListener(() =>
        {
            menuPanel.gameObject.SetActive(false);
            GameController.Instance.StartGame();
            Cursor.visible= false;
            AudioManager.Instance.ChangeBackgroundMusic();
        });        
        creditsBtn.onClick.AddListener(() =>
        {
            creditsPanel.gameObject.SetActive(true);
        });
        exitCreditsBtn.onClick.AddListener(() =>
        {
            creditsPanel.gameObject.SetActive(false);
        });
#if !UNITY_WEBGL
        hsBtn.onClick.AddListener(() =>
        {
            highScorePanel.SetActive(true);
        });
#endif
        exitHSPanel.onClick.AddListener(() => 
        {
            highScorePanel.gameObject.SetActive(false); 
        });
        restartGameBtn.onClick.AddListener(() =>
        {
#if !UNITY_WEBGL
            HighScoreManager.instance.NewHighScore(GameController.Instance.SCORE, nameInputField.text);
            restartGameBtn.interactable = false;
#endif
            GameController.Instance.RestartGame();
        });
        nameInputField.onValueChanged.AddListener(delegate
        {
            if (nameInputField.text == "")
            {
                restartGameBtn.interactable = false;
            }
            else
            {
                restartGameBtn.interactable = true;
            }
        });        
        exitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
#if UNITY_WEBGL
        hsBtn.gameObject.SetActive(false);
        nameInputField.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
#endif

    }

   
    public void UpdateHearts(int currentHearts)
    {
        if(currentHearts>=0)
        {
            Image fill = Hearts[currentHearts].GetComponentsInChildren<Image>()[1];
            fill.DOFillAmount(0, 0.5f);
        }
    }

    public void UpdateTimer(float timeLeft)
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft - minutes * 60);
        timer.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void UpdateScore(int newScore)
    {
        score.text = newScore.ToString("0000");        
    }

    public void SetFinalScore(int score)
    {
        finalScore.text = score.ToString("0000");
        endgamePanel.gameObject.SetActive(true);
#if UNITY_WEBGL
        restartGameBtn.interactable = true;
#else
        restartGameBtn.interactable = false;
#endif
    }
}
