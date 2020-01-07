using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class MainMenuButtonScript : MonoBehaviour, IFocusable, IInputClickHandler, ISpeechHandler
{

    private MeshRenderer meshRenderer;
    private Color initialColor;
    private GameObject singlePlayerTooltip;
    private GameObject multiPlayerTooltip;
    private GameObject sampleProjTooltip;

    [SerializeField]
    private bool isSinglePlayer;

    [SerializeField]
    private bool isMultiPlayer;

    [SerializeField]
    private bool isSampleProj;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        initialColor = meshRenderer.material.color;

        if (isSinglePlayer)
        {
            singlePlayerTooltip = GameObject.Find("SinglePlayerToolTip");
            singlePlayerTooltip.SetActive(false);
        }

        if (isMultiPlayer)
        {
            multiPlayerTooltip = GameObject.Find("MultiPlayerToolTip");
            multiPlayerTooltip.SetActive(false);
        }

        if (isSampleProj)
        {
            sampleProjTooltip = GameObject.Find("SampleToolTip");
            sampleProjTooltip.SetActive(false);
        }

    }

    public void OnFocusEnter()
    {

        meshRenderer.material.EnableKeyword("_EMISSION");
        meshRenderer.material.SetColor("_EmissionColor", Color.green);
        meshRenderer.material.color = Color.green;

        if (isSinglePlayer)
        {
            singlePlayerTooltip.SetActive(true);
        }

        if (isMultiPlayer)
        {
            multiPlayerTooltip.SetActive(true);
        }

        if (isSampleProj)
        {
            sampleProjTooltip.SetActive(true);
        }
    }

    public void OnFocusExit()
    {
        meshRenderer.material.DisableKeyword("_EMISSION");
        meshRenderer.material.SetColor("_EmissionColor", Color.white);
        meshRenderer.material.color = initialColor;

        if (isSinglePlayer)
        {
            singlePlayerTooltip.SetActive(false);
        }

        if (isMultiPlayer)
        {
            multiPlayerTooltip.SetActive(false);
        }

        if (isSampleProj)
        {
            sampleProjTooltip.SetActive(false);
        }
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (isSinglePlayer)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SinglePlayerScene");
        }

        if (isSampleProj)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BasicGaze");
        }

        if (isMultiPlayer)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MultiPlayerScene");
        }

    }

    public void OnSpeechKeywordRecognized(SpeechEventData eventData)
    {
        if (eventData.RecognizedText == "single player")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SinglePlayerScene");
        }

        if (eventData.RecognizedText == "sample")
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("BasicGaze");
        }
    }
}
