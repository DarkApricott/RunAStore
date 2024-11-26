using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public bool DoingTutorial;
    private int step;
    private bool replayTutOnStart;
    private int completedSteps;

    [SerializeField] private Animator animator;
    [SerializeField] private GameObject[] arrows;
    [SerializeField] private GameObject[] lines;
    [SerializeField] private GameObject[] helpingTexts;
    [SerializeField] private Toggle toggle;

    void Start()
    {
        // Deactivate all tutorial arrows initially
        foreach (var _arrow in arrows)
        {
            _arrow.SetActive(false);
        }

        // Check if the tutorial should replay on start
        if (PlayerPrefs.GetInt("ReplayTut") == 1)
        {
            DoingTutorial = true;
            arrows[0].SetActive(true); // Activate the first tutorial arrow
        }
        else
        {
            DoingTutorial = false;
            animator.SetTrigger("TutCompleted"); // Trigger tutorial completed animation
            StartCoroutine(TurnOffTut()); // Turn off tutorial after a delay
        }

        toggle.isOn = DoingTutorial; // Set the toggle state based on DoingTutorial
    }

    // Move to the next step of the tutorial
    public void NextStep(int _curStep)
    {
        if (DoingTutorial)
        {
            if (_curStep == step)
            {
                arrows[step].SetActive(false); // Deactivate current arrow
                step++;
                arrows[step].SetActive(true); // Activate next arrow
            }
        }
    }

    // Disable the current tutorial arrow
    public void DisableCurrentArrow()
    {
        if (DoingTutorial)
        {
            arrows[step].SetActive(false);
        }
    }

    // Mark a step as done and check if all steps are completed
    public void StepDone(int _whichStep)
    {
        if (DoingTutorial)
        {
            lines[_whichStep].SetActive(true); // Activate the line for the completed step
            completedSteps = 0;

            // Count the number of completed steps
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].activeInHierarchy == true)
                {
                    completedSteps++;
                }
            }

            // If all steps are completed, end the tutorial
            if (completedSteps == lines.Length)
            {
                DoingTutorial = false;
                animator.SetTrigger("TutCompleted"); // Trigger tutorial completed animation
                SetIfPlayTut(); // Update PlayerPrefs for replaying tutorial
            }
        }
    }

    // Show the helping text for a specific step
    public void HelpingTexts(int _whichText)
    {
        if (DoingTutorial)
        {
            TurnOffHelpingTexts(); // Turn off all helping texts
            helpingTexts[_whichText].SetActive(true); // Activate the specified helping text
        }
    }

    // Turn off all helping texts
    public void TurnOffHelpingTexts()
    {
        if (DoingTutorial)
        {
            foreach (var _helpingTexts in helpingTexts)
            {
                _helpingTexts.SetActive(false);
            }
        }
    }

    // Toggle the setting for replaying the tutorial on start
    public void SetIfPlayTut()
    {
        replayTutOnStart = !replayTutOnStart;
        PlayerPrefs.SetInt("ReplayTut", Convert.ToInt32(replayTutOnStart)); // Save the setting in PlayerPrefs
    }

    // Coroutine to turn off the tutorial animator after a delay
    private IEnumerator TurnOffTut()
    {
        yield return new WaitForSeconds(1);
        animator.gameObject.SetActive(false);
    }
}