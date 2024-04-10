using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.PlayerLoop;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody ball;
    public Text ScoreText;
    public Text Highscore;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;

    private int totalBricks;

    private int currentHighscore;
    private Vector3 paddlePosition = new Vector3(0.0f, 0.7f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        loadHighscore();
        loadScene();
    }

    private void loadScene()
    {
        m_Points = getCurrentHighscore();
        updateScoreText();
        totalBricks = 36;
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
    }
    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                ball.transform.SetParent(null);
                ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                setCurrentHighscore(0);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        else if (totalBricks == 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        setCurrentHighscore(m_Points);
        totalBricks--;
        updateScoreText();
    }

    private void updateScoreText()
    {
        ScoreText.text = $"Score : {m_Points}";
    }
    private void setCurrentHighscore(int score)
    {
        m_Points = score;
        if (SessionManager.Instance != null)
            SessionManager.Instance.currentScore = m_Points;
    }

    private int getCurrentHighscore()
    {
        if (SessionManager.Instance != null)
            return SessionManager.Instance.currentScore;

        return 0;
    }

    public void GameOver()
    {

        SaveHighscore();
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void Exit()
    {
        SaveHighscore();
        SceneManager.LoadScene(0);
    }

    private void loadHighscore()
    {
        Highscore.text = "Highscore: ";
        string path = Application.persistentDataPath + "/highscore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            if (data != null)
            {
                Highscore.text = $"Highscore: {data.PlayerName} : {data.Highscore}";
                Int32.TryParse(data.Highscore, out currentHighscore);
            }
        }
    }

    public void SaveHighscore()
    {
        if (m_Points > currentHighscore)
        {
            PlayerData data = new PlayerData();
            data.PlayerName = (SessionManager.Instance != null && SessionManager.Instance.playerData != null ? SessionManager.Instance.playerData.PlayerName : "None");
            data.Highscore = "" + m_Points;

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(Application.persistentDataPath + "/highscore.json", json);
        }
    }
}
