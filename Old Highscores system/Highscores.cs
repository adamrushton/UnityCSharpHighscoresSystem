// Author Adam Rushton
using System;
using System.Collections.Generic;

public class Highscores : UnityEngine.MonoBehaviour {

    enum errorCodes
    {
        FailToLoadScore = 0
    };
 
    // Grouping player data together
    struct Players
    {
        public string username;
        public int points;
    }

    static int TOTAL_SCORES = 5;       // Total number of scores saved
    const int MINIMUM_CHARACTERS = 0;  // Lower bound for the number of characters in a username 
    const int MAXIMUM_CHARACTERS = 18; // Upper bound for the number of characters in a username

    const string SCORE_SAVED_MESSAGE = "The new highscore has been added and saved to the file.";
    const string NEW_SCORE_MESSAGE = "Congratulations, you have a new highscore!";
    const string NO_HIGHSCORE_MESSAGE = "You have not achieved a new highscore.";
    string NAME_TOO_SHORT_MESSAGE = "Username must be more than " + MINIMUM_CHARACTERS + " characters.";
    string NAME_TOO_LONG_MESSAGE = "Username must be less than " + MAXIMUM_CHARACTERS + " characters.";

    // Default names and scores if no file is provided
    string[] defaultNames = { "Sam", "Jim", "Phil", "Bob", "Pam", "Ethan", "Jacob", "Mason", "Liam", "Noah" };
    int[] defaultScores = { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

    public UnityEngine.UI.Text[] scores = new UnityEngine.UI.Text[TOTAL_SCORES]; // All score fields on the display
    public UnityEngine.UI.Text[] names  = new UnityEngine.UI.Text[TOTAL_SCORES]; // All name fields on the display

    public UnityEngine.UI.Text fileName;        // Gets the current game modes file name
    public UnityEngine.UI.Text score;           // Display score
    public UnityEngine.UI.InputField username;  // Get players username

    public UnityEngine.UI.Button addScoreButton;    // Add Score Button
    public UnityEngine.UI.Text status;              // Highscores message
    public UnityEngine.UI.Text displayScoreLabel;   // Score label
    public UnityEngine.UI.Text displayScore;        // Score
    public UnityEngine.UI.InputField usernameInput; // Username

    Players[] entry = new Players[TOTAL_SCORES]; // Grouping together players and scores

    static int amount = 0; // Count number of instances
    int achievedScore = 0; // Update this when game over
    int position = 0;      // Line in the file
    int tempScore = 0;     // Store score temporarily after parsing from file

    void Start()
    {
        // Find Text Components from the interface
        FindTextComponents();

        // Get score from the gamemode
        achievedScore = UnityEngine.PlayerPrefs.GetInt("CurrentScore", 0);
        ToggleFakePanel(false);

        status.text = NO_HIGHSCORE_MESSAGE;
        score.text  = achievedScore.ToString();

        if (achievedScore > entry[TOTAL_SCORES - 1].points)
        {
            status.text = NEW_SCORE_MESSAGE;
            ToggleFakePanel(true); // Show the message
        }
        // Keep track of instances created
        ++amount;
        UnityEngine.Debug.Log("instance " + amount);

        // Load and sort the file
        LoadScoresFromFile(fileName.text);
    }

    // Toggle panel components
    private void ToggleFakePanel(bool show)
    {
        addScoreButton.enabled = show;
        addScoreButton.gameObject.SetActive(show);
        usernameInput.enabled = show;
        usernameInput.gameObject.SetActive(show);
    }

    // Get components from the Unity scene
    private void FindTextComponents()
    {
        addScoreButton    = addScoreButton.GetComponent<UnityEngine.UI.Button>();
        status            = status.GetComponent<UnityEngine.UI.Text>();
        displayScore      = displayScore.GetComponent<UnityEngine.UI.Text>();
        displayScoreLabel = displayScoreLabel.GetComponent<UnityEngine.UI.Text>();
        usernameInput     = usernameInput.GetComponent<UnityEngine.UI.InputField>();

        fileName = fileName.GetComponent<UnityEngine.UI.Text>();
        fileName.enabled = false; // Invisible text view to store the file name for the highscores
        score = score.GetComponent<UnityEngine.UI.Text>();

        for (int i = 0; i < TOTAL_SCORES; i++)
        {
            names[i] = names[i].GetComponent<UnityEngine.UI.Text>();
            scores[i] = scores[i].GetComponent<UnityEngine.UI.Text>();
        }
    }

    // Read names and scores from file using csv as the file structure
    private void LoadScoresFromFile(string fileName)
    {
        var line = "";
        try
        {
            using (var reader = new System.IO.StreamReader(fileName))
            {
                while (position < TOTAL_SCORES)
                {
                    line = reader.ReadLine(); // Read in a line
                    var playerValue = line.Split(','); // Split the line into csv style

                    entry[position].username = playerValue[0].ToString();
                    bool validScore = int.TryParse(playerValue[1], out tempScore);

                    if (validScore)
                    {   // Set score to whatever the score is from the file
                        entry[position].points = tempScore;
                    }
                    else
                    {   // Set score to -1
                        entry[position].points = (int)errorCodes.FailToLoadScore;
                    }
                    ++position;
                }
                reader.Close(); // Close the reader, we are done with it
            }
            SortAndDisplayScores(); // Sort the scores
            SaveScores();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log("Error: " + e.Message);
            DefaultEntries();
            SaveScores();
            LoadScoresFromFile(fileName);
        }
    }

    // Fill in the missing entries
    private void DefaultEntries()
    {
        while (position < TOTAL_SCORES)
        {
            entry[position].username = defaultNames[position];
            entry[position].points = defaultScores[position];
            ++position;
        }
    }
    // Rearrange the order of the scores and display them
    private void SortAndDisplayScores()
    {
        System.Array.Sort<Players>(entry, (a, b) => b.points.CompareTo(a.points)); // Sort them into order (highest score at the top)
        for (int i = 0; i < TOTAL_SCORES; i++)
        {   // Display them here
            names[i].text = entry[i].username;
            scores[i].text = entry[i].points.ToString();
        }
    }

    // Save scores to the file
    private void SaveScores()
    {
        position = 0;
        try
        {
            using (var writer = new System.IO.StreamWriter(fileName.text))
            {
                while (position < TOTAL_SCORES)
                {
                    writer.WriteLine(entry[position].username + "," + entry[position].points);
                    ++position;
                }
                writer.Close(); // Close the writer, we are done with it
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.Log(e.Message);
        }
    }
    // Go back to the main menu
    public void BackButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
    
    // After a game has finished, call this function to get the updated scores table and put it into the structure
    public void UpdateScores()
    {   // update player prefs with the current name and score
        if (achievedScore > entry[TOTAL_SCORES - 1].points && usernameInput.text.Length > MINIMUM_CHARACTERS && 
            usernameInput.text.Length < MAXIMUM_CHARACTERS)
        {   // Replace the highscore in the last position
            entry[TOTAL_SCORES - 1].username = username.text;
            entry[TOTAL_SCORES - 1].points = achievedScore;
            
            ToggleFakePanel(false);
            status.text = SCORE_SAVED_MESSAGE;
            achievedScore = 0;      // Set that score to 0
            UnityEngine.PlayerPrefs.SetInt("CurrentScore", achievedScore); // Update playerprefs score to 0
            SortAndDisplayScores(); // Resort the list.
            SaveScores();           // Save scores to file.
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