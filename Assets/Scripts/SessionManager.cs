using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    public PlayerData playerData;

    public int currentScore;
    
    private void Awake()
    {
        // start of new code
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // end of new code

        playerData = new PlayerData();
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
