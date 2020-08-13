using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition;
using FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples;
using UnityEngine.UI;


/*
腳本說明：
按下空白鍵打開UI介面
Is Use Recognition ： 只會影響是否觸發事件
detect target : 目標要辨識出的文字
result : 偵測到的文字
*/
public class SpeechRecognizer : MonoBehaviour
{
    public bool isUseRecognition = true;
    public bool isUseGUI = true;
    [Header("Recognition Setting")]
    public Enumerators.LanguageCode language = Enumerators.LanguageCode.cmn_Hant_TW;
    public string detectTarget = "我在這";

    string result;


    GCSpeechRecognition _speechRecognition;
    Dropdown _microphoneDevicesDropdown;
    Text resultText;
    // Start is called before the first frame update
    void Start()
    {
        InitSpeechRecognition();
        StartRecordButtonOnClickHandler();
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            isUseGUI=!isUseGUI;
            transform.GetComponentInChildren<Canvas>(true).gameObject.SetActive(isUseGUI);
            InitUI();
        }
    }
    void OnDestroy()
    {
        DestroySpeechRecognition();
    }
    void InitSpeechRecognition()
    {
        _speechRecognition = GCSpeechRecognition.Instance;
        _speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
        _speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
        _speechRecognition.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
        _speechRecognition.ListOperationsFailedEvent += ListOperationsFailedEventHandler;

        _speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
        _speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
        _speechRecognition.RecordFailedEvent += RecordFailedEventHandler;

        _speechRecognition.BeginTalkigEvent += BeginTalkigEventHandler;
        _speechRecognition.EndTalkigEvent += EndTalkigEventHandler;


        RefreshMicsButtonOnClickHandler();
        InitUI();

    }
    void InitUI()
    {
        if (!isUseGUI) return;
        GameObject canvas = transform.GetComponentInChildren<Canvas>(true).gameObject;
        _microphoneDevicesDropdown=canvas.transform.Find("_microphoneDevicesDropdown").GetComponent<Dropdown>();
        resultText = canvas.transform.Find("ResultText").GetComponent<Text>();

        _microphoneDevicesDropdown.onValueChanged.AddListener(MicrophoneDevicesDropdownOnValueChangedEventHandler);
        resultText.text = "Result : " + result;
    }
    void DestroySpeechRecognition()
    {
        _speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
        _speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;

        _speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
        _speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
        _speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

        _speechRecognition.BeginTalkigEvent -= BeginTalkigEventHandler;
        _speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;

    }



    bool checkAnswer(string Input)
    {
        result = Input;
        if (isUseGUI) resultText.text = "Result : " + result;

        char[] answerArray = detectTarget.ToCharArray();
        char[] inputArray = Input.ToCharArray();

        for (int i = 0; i < inputArray.Length; i++){
            for(int j=0; j<answerArray.Length;j++){
                Debug.Log(answerArray[j]);
               if( answerArray[j] == inputArray[i] ) {
                   resultText.text += " - correct answer";
                   return true;
               }
            }
        }

        return false;
    }

    void TriggerEvent()
    {
        if(!isUseRecognition)return;
        if (isUseGUI) resultText.text += " (Trigger Event)";
        Debug.Log("Trigger");
    }
    private void ListOperationsFailedEventHandler(string error)
    {
        print("List Operations Failed: " + error);
    }

    private void RecognizeFailedEventHandler(string error)
    {
        print("Recognize Failed: " + error);
    }
    private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
    {
        print("List Operations Success.\n");

        if (operationsResponse.operations != null)
        {
            print("Operations:\n");

            foreach (var item in operationsResponse.operations)
            {
                print("name: " + item.name + "; done: " + item.done + "\n");
            }
        }
    }

    public void StartRecordButtonOnClickHandler()
    {

        _speechRecognition.StartRecord(true);
    }
    public void StopRecordButtonOnClickHandler()
    {

        _speechRecognition.StopRecord();
    }
    private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
    {
        Debug.Log("Recognize Success.");
        InsertRecognitionResponseInfo(recognitionResponse);
    }
    private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
    {


        if (clip == null)
            return;

        RecognitionConfig config = RecognitionConfig.GetDefault();
        config.languageCode = language.Parse();
        config.speechContexts = new SpeechContext[0];
        config.audioChannelCount = clip.channels;
        // configure other parameters of the config if need

        GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
        {
            audio = new RecognitionAudioContent()
            {
                content = raw.ToBase64()
            },
            //audio = new RecognitionAudioUri() // for Google Cloud Storage object
            //{
            //	uri = "gs://bucketName/object_name"
            //},
            config = config
        };

        _speechRecognition.Recognize(recognitionRequest);
    }

    private void StartedRecordEventHandler()
    {
        // _speechRecognitionState.color = Color.green;
    }

    private void RecordFailedEventHandler()
    {
        // _speechRecognitionState.color = Color.red;
        Debug.Log("Start record Failed. Please check microphone device and try again.");

    }
    private void BeginTalkigEventHandler()
    {
        Debug.Log("<color=blue>Talk Began.</color>");
    }
    private void EndTalkigEventHandler(AudioClip clip, float[] raw)
    {
        Debug.Log("\n<color=blue>Talk Ended.</color>");

        FinishedRecordEventHandler(clip, raw);
    }
    private void DetectThresholdButtonOnClickHandler()
    {
        _speechRecognition.DetectThreshold();
    }
    private void RecognizeButtonOnClickHandler()
    {
        if (_speechRecognition.LastRecordedClip == null)
        {
            Debug.Log("<color=red>No Record found</color>");
            return;
        }

        FinishedRecordEventHandler(_speechRecognition.LastRecordedClip, _speechRecognition.LastRecordedRaw);
    }

    private void RefreshMicsButtonOnClickHandler()
    {

        _speechRecognition.RequestMicrophonePermission(null);
        if (isUseGUI)
        {
            if (_microphoneDevicesDropdown == null) _microphoneDevicesDropdown = GameObject.Find("_microphoneDevicesDropdown").GetComponent<Dropdown>();
            _microphoneDevicesDropdown.ClearOptions();

            for (int i = 0; i < _speechRecognition.GetMicrophoneDevices().Length; i++)
            {
                _microphoneDevicesDropdown.options.Add(new Dropdown.OptionData(_speechRecognition.GetMicrophoneDevices()[i]));
            }
        
            //smart fix of dropdowns
            _microphoneDevicesDropdown.value = 1;
            _microphoneDevicesDropdown.value = 0;
        }
        _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[0]);

    }
    private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
    {
        if (!_speechRecognition.HasConnectedMicrophoneDevices())
            return;
        _speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
    }
    private void InsertRecognitionResponseInfo(RecognitionResponse recognitionResponse)
    {
        if (recognitionResponse == null || recognitionResponse.results.Length == 0)
        {
            Debug.Log("\nWords not detected.");
            return;
        }

        //detect answer!!!
        Debug.Log("\n" + recognitionResponse.results[0].alternatives[0].transcript);
        if (checkAnswer(recognitionResponse.results[0].alternatives[0].transcript))
        {
            TriggerEvent();
        }

        var words = recognitionResponse.results[0].alternatives[0].words;
        result = "";
        if (words != null)
        {
            string times = string.Empty;

            foreach (var item in recognitionResponse.results[0].alternatives[0].words)
            {
                times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";
                result += "" + item.word;
            }

            // _resultText.text += "\n" + times;
        }

        string other = "\nDetected alternatives: ";

        foreach (var result in recognitionResponse.results)
        {
            foreach (var alternative in result.alternatives)
            {
                if (recognitionResponse.results[0].alternatives[0] != alternative)
                {
                    other += alternative.transcript + ", ";
                }
            }
        }

        // _resultText.text += other;
    }



    void OnGUI()
    {

        // for (int i = 0; i < _speechRecognition.GetMicrophoneDevices().Length; i++)
        // {
        //     Rect mircrophi(new Dropdown.OptionData(_speechRecognition.GetMicrophoneDevices()[i]));
        // }

        // //smart fix of dropdowns
        // _microphoneDevicesDropdown.value = 1;
        // _microphoneDevicesDropdown.value = 0;
    }
}
