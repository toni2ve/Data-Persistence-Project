using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.UI;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class MenuUIHandler : MonoBehaviour
{

  public TMP_InputField PlayerNameInput;
  public Button StartButton;

  public TMP_Text HighscoreText;

  void Start()
  {
    LoadHighscore();
    if (SessionManager.Instance != null && SessionManager.Instance.name != null)
    {
      PlayerNameInput.textComponent.text = SessionManager.Instance.name;
    }
  }
  public void StartNew()
  {
    if (SessionManager.Instance != null)
    {
      SessionManager.Instance.playerData.PlayerName = PlayerNameInput.textComponent.text;
      SessionManager.Instance.playerData.Highscore = "0";
    }
    SceneManager.LoadScene(1);
  }
  public void Exit()
  {
#if UNITY_EDITOR
    EditorApplication.ExitPlaymode();
#else
            Application.Quit(); // original code to quit Unity player
#endif
  }

  public void EnableStartButton()
  {
    if (PlayerNameInput != null && PlayerNameInput.textComponent.text != "")
    {
      if (StartButton != null)
      {
        StartButton.interactable = true;
      }
    }
  }

  public void LoadHighscore()
    {
        string path = Application.persistentDataPath + "/highscore.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            if (data != null)
            {
                HighscoreText.text = $"{data.PlayerName} : {data.Highscore}";
            }
        }
    }
}
