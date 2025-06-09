using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField] private string _GhostPoolTag;
    [SerializeField] private string _ObjectPoolTag;
    [SerializeField] private List<Transform> _SpawnPoints;
    private List<int> availableSpawnPoints = new List<int>();
    private List<int> occupiedSpawnsIndex = new List<int>();

    [SerializeField] private List<Color> lightColors= new List<Color>();
    [SerializeField] private float m_InitialTimeBetweenSpawn;
    private float lastSpawnTime;
    private float currentEnemies;
    public float CURRENT_ENEMIES =>currentEnemies;
    private float currentTombs;
    public int currentPurifiedTombs;

    public UnityEvent EnemiesAmountChanged;

    #region Singleton
    static private EnemyGenerator instance;
    static public EnemyGenerator Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
        for(int i =0;i<_SpawnPoints.Count;i++) 
        {
            availableSpawnPoints.Add(i);
        }
        //lastSpawnTime= Time.time;
    }
    #endregion

    private void Update()
    {
        if(GameController.Instance.Ongoing)
        {
            if (Time.time - lastSpawnTime > m_InitialTimeBetweenSpawn)
            {
                lastSpawnTime = Time.time;
                if (currentTombs < 5)
                {
                    SpawnGhost();
                }
            }
        }       
    }

    public void InitTime()
    {
        lastSpawnTime= Time.time;
    }
    public void SpawnGhost()
    {
        if(availableSpawnPoints.Count > 0) 
        {
            currentEnemies++;            
            currentTombs++;
            GameObject ghost = ObjectPooler.Instance.SpawnFromPool(_GhostPoolTag);
            Ghost ghostScript = ghost.GetComponent<Ghost>();
            GameObject objectToPurify = ObjectPooler.Instance.SpawnFromPool(_ObjectPoolTag);
            ObjectToPurify purifyScript = objectToPurify.GetComponent<ObjectToPurify>();
            int auxIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnIndex = availableSpawnPoints[auxIndex];
            occupiedSpawnsIndex.Add(availableSpawnPoints[auxIndex]);
            availableSpawnPoints.RemoveAt(auxIndex);
            objectToPurify.transform.position = _SpawnPoints[spawnIndex].transform.position;
            purifyScript.SetGhost(ghostScript);
            ghostScript.SetAsociatedObject(purifyScript);
            purifyScript.SetIndex(spawnIndex);
            ghostScript.SetNewPosition();
            ghostScript.SetOutlineColor(lightColors[spawnIndex]);
            Tombs.Instance.SetTombColor(spawnIndex, lightColors[spawnIndex]);
            EnemiesAmountChanged?.Invoke();
        }
    }

    public void SubstractGhost()
    {
        currentEnemies--;
        EnemiesAmountChanged?.Invoke();
    }

    public void SubstractTomb()
    {
        currentTombs--;
    }

    public void ReleaseIndex(int index)
    {
        int aux = occupiedSpawnsIndex.IndexOf(index);
        availableSpawnPoints.Add(occupiedSpawnsIndex[aux]);
        occupiedSpawnsIndex.RemoveAt(aux);
    }
}
