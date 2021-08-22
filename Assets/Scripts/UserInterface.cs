using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public static UserInterface instance;

    public Button StartJourneyButton;
    Field field;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public Text powerText;

    private bool isAudioOn;
    public Button SoundOnOffButton;
    public Sprite AudioOn;
    public Sprite AudioOff;

    public GameObject tutorPanel;
    public GameObject tutor1;
    public GameObject tutor2;
    public GameObject tutor3;
    public GameObject tutor4;
    public GameObject tutor5;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            Debug.Log("Field is already exists!");
        }
        else
        {
            instance = this.GetComponent<UserInterface>();
        }
    }

    public void TurnAudioOnOff()
    {
        if (isAudioOn)
        {
            SoundOnOffButton.GetComponent<Image>().sprite = AudioOff;
            SoundManager.instance.audioSource.volume = 0;
        }
        else
        {
            SoundOnOffButton.GetComponent<Image>().sprite = AudioOn;
            SoundManager.instance.audioSource.volume = 1;

        }
        isAudioOn = !isAudioOn;

    }

    public void OpenTutorial1()
    {
        tutor1.SetActive(true);
        tutorPanel.SetActive(true);
    }

    public void OpenTutorial2()
    {
        tutor2.SetActive(true);
        tutorPanel.SetActive(true);
    }

    public void OpenTutorial3()
    {
        tutor3.SetActive(true);
        tutorPanel.SetActive(true);
    }

    public void OpenTutorial4()
    {
        tutor4.SetActive(true);
        tutorPanel.SetActive(true);
    }

    public void OpenTutorial5()
    {
        tutor5.SetActive(true);
        tutorPanel.SetActive(true);
    }

    public void ChangePowerText(int power)
    {
        powerText.text = power.ToString();
    }

    public void Win()
    {
        WinPanel.SetActive(true);
    }

    public void Lose()
    {
        LosePanel.SetActive(true);
    }

    public void ToNextLevel()
    {
        WinPanel.SetActive(false);
        field.ToNextLevel();
    }

    public void Retry()
    {
        LosePanel.SetActive(false);
        field.RestartLevel();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start()
    {
        tutorPanel.SetActive(false);
        tutor1.SetActive(false);
        tutor2.SetActive(false);
        tutor3.SetActive(false);
        tutor4.SetActive(false);
        tutor5.SetActive(false);

        isAudioOn = true;
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        StartJourneyButton.interactable = false;
        field = FindObjectOfType<Field>();
    }

    public void SetInteractiveToStartJourneyButton(bool _isInteractive)
    {
        StartJourneyButton.interactable = _isInteractive;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
