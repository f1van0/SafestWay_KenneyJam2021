using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public Button StartJourneyButton;
    Field field;

    // Start is called before the first frame update
    void Start()
    {
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
