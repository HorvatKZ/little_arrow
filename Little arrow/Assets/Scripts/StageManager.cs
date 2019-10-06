using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public GameObject Player;
    int Enemies = 0;
    public int[] enemyNumbers;
    public GameObject[] EnemyPrefabs;
    float EnemyMaxRange;
    public float LevelRadius;
    public float EnemyMinRange;
    int[] spawnChances;

    int wave = 0;

    public Text Task;
    public Text Wave;
    public Transform WaveAnchor;
    int EnemyLeft;

    public Animator waveAnim;
    public float safeTime;

    public GameObject gameOverScr;
    public Text gameOverScore;
    public Animator gameOverAnim;

    private void Start()
    {
        spawnChances = new int[6];
        NewWave();
    }

    public void OneDie()
    {
        --EnemyLeft;
        Task.text = EnemyLeft.ToString();

        waveAnim.enabled = false;
        if (EnemyLeft == 0)
        {
            wave++;
            NewWave();
        }

        WaveAnchor.localScale = new Vector3((float)EnemyLeft / Enemies, 1, 1);
    }

    Vector3 Rand()
    {
        float randx = Random.Range(-EnemyMaxRange, EnemyMaxRange);
        float randy = Random.Range(-EnemyMaxRange, EnemyMaxRange);
        Vector3 result = new Vector3(randx, randy, 0);
        while ((Player.transform.position - result).magnitude < EnemyMinRange)
        {
            randx = Random.Range(-EnemyMaxRange, EnemyMaxRange);
            randy = Random.Range(-EnemyMaxRange, EnemyMaxRange);
            result = new Vector3(randx, randy, 0);
        }
        return result;
    }

    int RandType(int[] spawnChances)
    {
        int n = spawnChances.Length;
        int sum = 0;
        foreach (int x in spawnChances)
        {
            sum += x;
        }
        int rand = Random.Range(0, sum);
        sum = 0;
        int i = 0;
        while (i < n && sum <= rand)
        {
            sum += spawnChances[i];
            ++i;
        }
        return --i;
    }

    void NewWave()
    {
        //Spawn Chances
        spawnChances[0] = wave * wave + 4;
        spawnChances[1] = wave * wave + wave +1;
        spawnChances[2] = spawnChances[1] / 2;
        spawnChances[3] = spawnChances[2] * 2 / 3;
        if (wave > 2) {
            spawnChances[4] = (wave - 3) * (wave - 3) + 1;
        }
        else
        {
            spawnChances[4] = 0;
        }
        spawnChances[5] = spawnChances[4] / 2;

        //Enemies
        if (wave < enemyNumbers.Length)
        {
            Enemies = enemyNumbers[enemyNumbers.Length - wave - 1];
        }
        else
        {
            Enemies = enemyNumbers[0];
        }

        //Other stuffs
        float maxRange = EnemyMinRange + Enemies / 4;
        EnemyMaxRange = maxRange > LevelRadius ? LevelRadius : maxRange;
        Wave.text = "W A V E  " + (wave + 1).ToString();
        waveAnim.enabled = true;
        waveAnim.SetBool("waveChanging", true);
        Invoke("Spawn", safeTime);
    }

    void Spawn()
    {
        for (int i = 0; i < Enemies; ++i)
        {
            Vector3 pos = Rand();
            int type = RandType(spawnChances);
            GameObject enemy = Instantiate(EnemyPrefabs[type], pos, transform.rotation);
        }
        EnemyLeft = Enemies;
        Task.text = EnemyLeft.ToString();
        WaveAnchor.localScale = new Vector3(1, 1, 1);
        waveAnim.SetBool("waveChanging", false);
    }

    public void GameOver(int XP)
    {
        gameOverScr.SetActive(true);
        gameOverScore.text = "S C O R E : " + XP.ToString();
        gameOverAnim.SetBool("GameOver", true);
    }
}
