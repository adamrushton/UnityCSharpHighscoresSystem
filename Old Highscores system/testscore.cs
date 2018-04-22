using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscore : MonoBehaviour
{

    int score = 50;
	// Use this for initialization
	void Start ()
    {
        PlayerPrefs.SetInt("CurrentScore", score);
        Debug.Log(score);
	}
	
    public void LoadHighscoresScene()
    {
        PlayerPrefs.SetInt("CurrentScore", score);
        UnityEngine.SceneManagement.SceneManager.LoadScene("vuforiascene");
    }
}
