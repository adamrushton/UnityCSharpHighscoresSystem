using UnityEngine;

public class testscore : MonoBehaviour
{

    int score = 50;
    int gameModeNumber = 3; // Game mode 3 has finished
	// Use this for initialization
	void Start()
    {
        PlayerPrefs.SetInt("GameModeNumber", gameModeNumber); // Store game mode number in playerprefs
        PlayerPrefs.SetInt("FinalScore"+gameModeNumber, score);
	}
	
    public void LoadHighscoresScene()
    {
        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("5scores"); // Will change to +gameModeNumber
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }
}
