using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScripts : MonoBehaviour {

    public Button mRecordButton;
    public Button mPlayButton;
    public Button mCopyButton;

    public CharacterControls LizardBoy;

	// Use this for initialization
	void Start () {
        GameObject go = GameObject.Find("LizardBoy");
        LizardBoy = (CharacterControls)go.GetComponent(typeof(CharacterControls));

        //LizardBoy = GetComponent<CharacterControls>();

        mRecordButton = mRecordButton.GetComponent<Button>(); // GetComponent("RecordButton") as Button;
        mPlayButton = mPlayButton.GetComponent<Button>();
        mCopyButton = mCopyButton.GetComponent<Button>();

        //Calls the TaskOnClick/TaskWithParameters method when you click the Button
        mRecordButton.onClick.AddListener(RecordOnClick);
        mPlayButton.onClick.AddListener(PlayOnClick);
        mCopyButton.onClick.AddListener(CopyOnClick);
    }

    private bool isRecording = false;
    private bool isPlaying = false;

    void RecordOnClick()
    {
        Text RecordButtonText = (Text)mRecordButton.GetComponentInChildren(typeof(Text));

        if (!isRecording)
        {
            isRecording = true;
            LizardBoy.startRecording = true;
            LizardBoy.StartRecordAudio();

            RecordButtonText.text = "Pause";

            Debug.Log("Recording Started!");
        }
        else
        {
            isRecording = false;
            LizardBoy.startRecording = false;
            LizardBoy.StopRecordAudio();

            RecordButtonText.text = "Record";

            Debug.Log("Recording Stopped!");
        }
    }   

    void PlayOnClick()
    {
        Text PlayButtonText = (Text)mPlayButton.GetComponentInChildren(typeof(Text));

        if (!isRecording)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                LizardBoy.StartCoroutine("PlaybackRecording");

                PlayButtonText.text = "Stop";

                Debug.Log("Playing Scene!");
            }
            else
            {
                isPlaying = false;
                LizardBoy.StopCoroutine("PlaybackRecording");

                PlayButtonText.text = "Play";

                Debug.Log("Stoping Scene!");
            }
        } 
    }

    void CopyOnClick()
    {
        LizardBoy.CopyMe();
    }

}
