using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TutorialGuideUI : MonoBehaviour
{
    [SerializeField] NearFarInteractor leftNearFarInteractor;
    [SerializeField] NearFarInteractor rightNearFarInteractor;

    [SerializeField] GameObject leftNearFarInteractorVisual;
    [SerializeField] GameObject rightNearFarInteractorVisual;

    public List<GameObject> physicalButtons = new List<GameObject>();

    [SerializeField] Button ExperimentModeButton;
    [SerializeField] Button TestModeButton;

    [SerializeField] TMP_InputField ParticipantNoField;

    private TouchScreenKeyboard keyboard;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartUIMode();
        ParticipantNoField.onSelect.AddListener(OnParticipantFieldSelected);
        ParticipantNoField.onValueChanged.AddListener(OnTextChanged);

        //buttons

        ExperimentModeButton.onClick.AddListener(StartExperiment);
        TestModeButton.onClick.AddListener(StartTestMode);

    }

    private void OnParticipantFieldSelected(string text)
    {
        Debug.Log("FIELD SELECTED");

        ParticipantNoField.ActivateInputField();

        keyboard = TouchScreenKeyboard.Open(
            ParticipantNoField.text,
            TouchScreenKeyboardType.NumberPad
        );
    }


    void StartExperiment() {
        if (int.TryParse(ParticipantNoField.text, out int participantNumber))
        {
            GameManager.Instance.participantNumber = participantNumber;
            GameManager.Instance.isParticipant = true;
            CloseUIMode();

        }
        else
        {
            Debug.LogError("Invalid participant number");
        }

        
    }

    void StartTestMode() {
        GameManager.Instance.isParticipant = false;
        CloseUIMode();
    }

    void OnTextChanged(string newText) {
        if (newText.Length > 0) ExperimentModeButton.interactable = true;
    }

    void StartUIMode() {
        leftNearFarInteractor.enableFarCasting = true;
        rightNearFarInteractor.enableFarCasting = true;
        leftNearFarInteractor.interactionLayers = InteractionLayerMask.GetMask("TutorialUI");
        rightNearFarInteractor.interactionLayers = InteractionLayerMask.GetMask("TutorialUI");
        foreach (GameObject physicalButton in physicalButtons)
        {
            physicalButton.SetActive(false);
        }

    }

    void CloseUIMode() {
        leftNearFarInteractor.enableFarCasting = false;
        rightNearFarInteractor.enableFarCasting = false;

        leftNearFarInteractor.interactionLayers = InteractionLayerMask.GetMask("Default");
        rightNearFarInteractor.interactionLayers = InteractionLayerMask.GetMask("Default");

        foreach (GameObject physicalButton in physicalButtons) {
            physicalButton.SetActive(true);
        }

        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
