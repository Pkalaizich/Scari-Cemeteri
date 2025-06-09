using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController instance;
    public static GameController Instance { get => instance; }

    public bool Ongoing = false;
    private bool gameEnded=false;

    public UnityEvent PlayerHurt;
    public UnityEvent NewSection;
    public UnityEvent EndGame;

    [SerializeField] private float gameDuration;
    private float timeLeft;
    private int score;
    public int SCORE => score;
    private int currentSection;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        timeLeft = gameDuration;
        score = 0;
        Ongoing = false;
    }
    private void Start()
    {
        currentSection = 1;
        UIController.Instance.UpdateTimer(timeLeft);
        UIController.Instance.UpdateScore(score);
    }

    private void Update()
    {
        if(Ongoing)
        {
            timeLeft -= Time.deltaTime;
            UIController.Instance.UpdateTimer(timeLeft);
            if(timeLeft<((3-currentSection)/3)*gameDuration)
            {
                currentSection++;
                NewSection?.Invoke();
            }
            if(timeLeft<=0)
            {
                GameOver();
            }
        }
        if(Input.GetKeyDown(KeyCode.F11))
        {
            RestartGame();
        }
    }
    public void GameOver()
    {
        EndGame?.Invoke();
        Ongoing = false;
        gameEnded = true;
        UIController.Instance.SetFinalScore(score);
        Cursor.visible= true;
    }

    public void RestartGame()
    {
        StartCoroutine(Restart());
    }
    
    private IEnumerator Restart()
    {
        Ongoing= false;
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }

    public void ChangeScore(int points)
    {
        score += points;
        UIController.Instance.UpdateScore(score);
    }

    public void StartGame()
    {
        EnemyGenerator.Instance.InitTime();
        Ongoing = true;        
    }
}
