using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif
public class MenuUIHandler : MonoBehaviour
{
    public UIDocument uiDocument;
    private Label bestScore;
    private TextField playerNameInput;
    private Button startButton;
    private Button quitButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bestScore = uiDocument.rootVisualElement.Q<Label>("BestScore");

        playerNameInput = uiDocument.rootVisualElement.Q<TextField>("PlayerInput");
        playerNameInput.value = DataPersistent.Instance.currentPlayerName;
        playerNameInput.isDelayed = true;
        playerNameInput.RegisterValueChangedCallback(OnNameInputChange);
        playerNameInput.RegisterCallback<KeyDownEvent>(NameInput, TrickleDown.TrickleDown);

        startButton = uiDocument.rootVisualElement.Q<Button>("Start");
        startButton.clicked += StartGame;

        quitButton = uiDocument.rootVisualElement.Q<Button>("Quit");
        quitButton.clicked += Exit;
    }

    private void OnNameInputChange(ChangeEvent<string> evt)
    {
        DataPersistent.Instance.currentPlayerName = playerNameInput.value;
    }

    private void NameInput(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            UpdateBestScore();
            evt.StopPropagation();
        }
    }

    private void UpdateBestScore()
    {
        int? score = DataPersistent.Instance.GetPlayerByName(DataPersistent.Instance.currentPlayerName)?.highScore;
        if (score != null)
        {
            bestScore.text = "Best Score: "+  DataPersistent.Instance.currentPlayerName + ": " + score;
        }
    }

    public void StartGame()
    {
        DataPersistent.Instance.SavePlayer();
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
        DataPersistent.Instance.SavePlayer();
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
    Application.Quit();
#endif
    }
}
