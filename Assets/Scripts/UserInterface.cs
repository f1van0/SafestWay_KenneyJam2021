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

    public void Win()
    {
        WinPanel.SetActive(true);
    }

    public void Lose()
    {
        LosePanel.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
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
