// Author Adam Rushton

public class Highscores : UnityEngine.MonoBehaviour {

    enum GameModes
    {
        InvalidGameMode = -1,
        AnimalMasks = 1,
        EmotionMasks,
        EmotionSnap,
        GridEmotion
    } 
    // Grouping player data together
    struct Players
    {
        public string username;
        public int points;
    }

    const int TOTAL_SCORES = 5;        // Total number of scores saved
    const int MINIMUM_CHARACTERS = 0;  // Lower bound for the number of characters in a username 
    const int MAXIMUM_CHARACTERS = 18; // Upper bound for the number of characters in a username

    const string SCORE_SAVED_MESSAGE     = "The new highscore has been added and saved.";
    const string NEW_SCORE_MESSAGE       = "Congratulations, you have a new highscore!";
    const string NO_HIGHSCORE_MESSAGE    = "You have not achieved a new highscore.";
    const string ERROR_LOADING_SCORES    = "Error. The game mode number was not found.";
    static string NAME_TOO_SHORT_MESSAGE = "Username must be more than " + MINIMUM_CHARACTERS + " characters.";
    static string NAME_TOO_LONG_MESSAGE  = "Username must be less than " + MAXIMUM_CHARACTERS + " characters.";

    public UnityEngine.UI.Text[] scores = new UnityEngine.UI.Text[TOTAL_SCORES]; // All score fields on the display
    public UnityEngine.UI.Text[] names  = new UnityEngine.UI.Text[TOTAL_SCORES]; // All name fields on the display

    public UnityEngine.UI.Text score;           // Display score
    public UnityEngine.UI.InputField username;  // Get players username

    public UnityEngine.UI.Button addScoreButton;    // Add Score Button
    public UnityEngine.UI.Text status;              // Highscores message
    public UnityEngine.UI.Text displayScoreLabel;   // Score label
    public UnityEngine.UI.Text displayScore;        // Score
    public UnityEngine.UI.InputField usernameInput; // Username

    Players[] entry = new Players[TOTAL_SCORES]; // Grouping together players and scores

    static int amount  = 0;  // Count number of instances
    int achievedScore  = 0;  // Update this when game over
    int gameModeNumber = -1; // Get the game mode that is currently being used, load from playerprefs

    void Start()
    {
        // Find Text Components from the interface
        FindTextComponents();
        gameModeNumber = UnityEngine.PlayerPrefs.GetInt("GameModeNumber", (int)GameModes.InvalidGameMode);

        if (gameModeNumber == (int)GameModes.InvalidGameMode)
        {
            UnityEngine.Debug.Log("Invalid game mode number.");
            status.text = ERROR_LOADING_SCORES;
        }
        else
        {
            // Get score from the gamemode that was just played FinalScore1, FinalScore2, FinalScore3, FinalScore4
            achievedScore = UnityEngine.PlayerPrefs.GetInt("FinalScore" + gameModeNumber, 0);
            EnterScoreInterface(false);

            status.text = NO_HIGHSCORE_MESSAGE;
            score.text = achievedScore.ToString();

            // Keep track of instances created
            ++amount;
            UnityEngine.Debug.Log("instance " + amount);

            // Load scores
            LoadScores();

            // Check if theres a new highscore
            if (achievedScore > entry[entry.Length - 1].points)
            {
                status.text = NEW_SCORE_MESSAGE; // Set status text to new score to enter message
                EnterScoreInterface(true);       // Show the message
            }
        }
    }

    // Toggle panel components
    private void EnterScoreInterface(bool show)
    {
        addScoreButton.enabled = show;
        addScoreButton.gameObject.SetActive(show);
        usernameInput.enabled = show;
        usernameInput.gameObject.SetActive(show);
    }

    // Get components from the Unity scene
    private void FindTextComponents()
    {
        try
        {
            addScoreButton    = addScoreButton.GetComponent<UnityEngine.UI.Button>();
            status            = status.GetComponent<UnityEngine.UI.Text>();
            displayScore      = displayScore.GetComponent<UnityEngine.UI.Text>();
            displayScoreLabel = displayScoreLabel.GetComponent<UnityEngine.UI.Text>();
            usernameInput     = usernameInput.GetComponent<UnityEngine.UI.InputField>();
            score             = score.GetComponent<UnityEngine.UI.Text>();

            for (int i = 0; i < TOTAL_SCORES; i++)
            {
                names[i] = names[i].GetComponent<UnityEngine.UI.Text>();
                scores[i] = scores[i].GetComponent<UnityEngine.UI.Text>();
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }

    // Read names and scores from playerprefs

    private void LoadScores()
    {
        for (int i = 0; i < entry.Length; i++)
        {
            entry[i].username = UnityEngine.PlayerPrefs.GetString("GameMode"+ gameModeNumber +"_name" + i, "empty");
            entry[i].points = UnityEngine.PlayerPrefs.GetInt("GameMode" + gameModeNumber + "_score" + i, 0);
        }
        DisplayScores();
        if (!Sorted(entry))
        {
            Sort();          // Sort scores
            DisplayScores(); // Display scores
            SaveScores();    // Save to playerprefs
        }
    }

    // Check to see if the scores list is already sorted
    private bool Sorted(Players[] entry)
    {
        for (int i = entry.Length - 2; i >= 0; i--)
        {
            if (entry[i].points < entry[i + 1].points)
            {
                return false; // Is not sorted
            }
        }
        return true; // If it has not been set to false, return true as it is already sorted
    }

    // Sort the array of players
    private void Sort()
    {
        System.Array.Sort<Players>(entry, (a, b) => b.points.CompareTo(a.points)); // Sort them into order (highest score at the top)
    }

    // Display the scores
    private void DisplayScores()
    {
        for (int i = 0; i < entry.Length; i++)
        {   // Display them here
            names[i].text = entry[i].username;
            scores[i].text = entry[i].points.ToString();
        }
    }
    // Save scores to playerprefs
    private void SaveScores()
    {   // Save all scores to playerprefs
        for (int i = 0; i < entry.Length; i++)
        {
            UnityEngine.PlayerPrefs.SetString("GameMode"+gameModeNumber+"_name" + i, entry[i].username);
            UnityEngine.PlayerPrefs.SetInt   ("GameMode"+gameModeNumber+"_score"+ i, entry[i].points);
        }
    }
    // Go back to the main menu
    public void BackButton()
    {
        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }
    
    // After a game has finished, call this function to get the updated scores table and put it into the structure
    public void UpdateScores()
    {   // update player prefs with the current name and score
        if (achievedScore > entry[entry.Length - 1].points && usernameInput.text.Length > MINIMUM_CHARACTERS && 
            usernameInput.text.Length < MAXIMUM_CHARACTERS)
        {   // Replace the highscore in the last position
            entry[entry.Length - 1].username = username.text;
            entry[entry.Length - 1].points = achievedScore;

            EnterScoreInterface(false);
            status.text = SCORE_SAVED_MESSAGE;
            achievedScore = 0;      // Set that score to 0
            UnityEngine.PlayerPrefs.SetInt("FinalScore"+gameModeNumber, achievedScore); // Update playerprefs score to 0 for that particular gamemode
            DisplayScores();
            if (!Sorted(entry))
            {
                Sort();          // Resort the list.
                DisplayScores(); // Display new order
                SaveScores();    // Save scores to playerprefs.
            }
        }
        else if (usernameInput.text.Length <= MINIMUM_CHARACTERS)
        {
            status.text = NAME_TOO_SHORT_MESSAGE;
        }
        else if (usernameInput.text.Length >= MAXIMUM_CHARACTERS)
        {
            status.text = NAME_TOO_LONG_MESSAGE;
        }
    }
}