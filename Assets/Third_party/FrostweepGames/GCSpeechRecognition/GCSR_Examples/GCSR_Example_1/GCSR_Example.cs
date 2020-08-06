using System;
using UnityEngine;
using UnityEngine.UI;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
	public class GCSR_Example : MonoBehaviour
	{
		private GCSpeechRecognition _speechRecognition;
		
		private Button _startRecordButton,
					   _stopRecordButton,
					   _getOperationButton,
					   _getListOperationsButton,
					   _detectThresholdButton,
					   _cancelAllRequestsButton,
					   _recognizeButton,
					   _refreshMicrophonesButton;

		private Image _speechRecognitionState;

		private Text _resultText;

		private Toggle _voiceDetectionToggle,
					   _recognizeDirectlyToggle,
					   _longRunningRecognizeToggle;

		private Dropdown _languageDropdown,
						 _microphoneDevicesDropdown;

		private InputField _contextPhrasesInputField,
						   _operationIdInputField;

		private Image _voiceLevelImage;
		public string resultWords="";

		private void Start()
		{
			_speechRecognition = GCSpeechRecognition.Instance;
			_speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
			_speechRecognition.LongRunningRecognizeSuccessEvent += LongRunningRecognizeSuccessEventHandler;
			_speechRecognition.LongRunningRecognizeFailedEvent += LongRunningRecognizeFailedEventHandler;
			_speechRecognition.GetOperationSuccessEvent += GetOperationSuccessEventHandler;
			_speechRecognition.GetOperationFailedEvent += GetOperationFailedEventHandler;
			_speechRecognition.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
			_speechRecognition.ListOperationsFailedEvent += ListOperationsFailedEventHandler;

			_speechRecognition.FinishedRecordEvent += FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent += RecordFailedEventHandler;

			_speechRecognition.BeginTalkigEvent += BeginTalkigEventHandler;
			_speechRecognition.EndTalkigEvent += EndTalkigEventHandler;

			_startRecordButton = transform.Find("Canvas/Button_StartRecord").GetComponent<Button>();
			_stopRecordButton = transform.Find("Canvas/Button_StopRecord").GetComponent<Button>();
			_getOperationButton = transform.Find("Canvas/Button_GetOperation").GetComponent<Button>();
			_getListOperationsButton = transform.Find("Canvas/Button_GetListOperations").GetComponent<Button>();
			_detectThresholdButton = transform.Find("Canvas/Button_DetectThreshold").GetComponent<Button>();
			_cancelAllRequestsButton = transform.Find("Canvas/Button_CancelAllRequests").GetComponent<Button>();
			_recognizeButton = transform.Find("Canvas/Button_Recognize").GetComponent<Button>();
			_refreshMicrophonesButton = transform.Find("Canvas/Button_RefreshMics").GetComponent<Button>();

			_speechRecognitionState = transform.Find("Canvas/Image_RecordState").GetComponent<Image>();

			_resultText = transform.Find("Canvas/Panel_ContentResult/Text_Result").GetComponent<Text>();

			_voiceDetectionToggle = transform.Find("Canvas/Toggle_DetectVoice").GetComponent<Toggle>();
			_recognizeDirectlyToggle = transform.Find("Canvas/Toggle_RecognizeDirectly").GetComponent<Toggle>();
			_longRunningRecognizeToggle = transform.Find("Canvas/Toggle_LongRunningRecognize").GetComponent<Toggle>();

			_languageDropdown = transform.Find("Canvas/Dropdown_Language").GetComponent<Dropdown>();
			_microphoneDevicesDropdown = transform.Find("Canvas/Dropdown_MicrophoneDevices").GetComponent<Dropdown>();		

			_contextPhrasesInputField = transform.Find("Canvas/InputField_SpeechContext").GetComponent<InputField>();
			_operationIdInputField = transform.Find("Canvas/InputField_Operation").GetComponent<InputField>();

			_voiceLevelImage = transform.Find("Canvas/Panel_VoiceLevel/Image_Level").GetComponent<Image>();

			_startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
			_stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);
			_getOperationButton.onClick.AddListener(GetOperationButtonOnClickHandler);
			_getListOperationsButton.onClick.AddListener(GetListOperationsButtonOnClickHandler);
			_detectThresholdButton.onClick.AddListener(DetectThresholdButtonOnClickHandler);
			_cancelAllRequestsButton.onClick.AddListener(CancelAllRequetsButtonOnClickHandler);
			_recognizeButton.onClick.AddListener(RecognizeButtonOnClickHandler);
			_refreshMicrophonesButton.onClick.AddListener(RefreshMicsButtonOnClickHandler);

			_microphoneDevicesDropdown.onValueChanged.AddListener(MicrophoneDevicesDropdownOnValueChangedEventHandler);

			_startRecordButton.interactable = true;
			_stopRecordButton.interactable = false;
			_speechRecognitionState.color = Color.yellow;

			_languageDropdown.ClearOptions();

			for (int i = 0; i < Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++)
			{
				_languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
			}

			_languageDropdown.value = _languageDropdown.options.IndexOf(_languageDropdown.options.Find(x => x.text == Enumerators.LanguageCode.en_GB.Parse()));

			RefreshMicsButtonOnClickHandler();
		}

		private void OnDestroy()
		{
			_speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
			_speechRecognition.LongRunningRecognizeSuccessEvent -= LongRunningRecognizeSuccessEventHandler;
			_speechRecognition.LongRunningRecognizeFailedEvent -= LongRunningRecognizeFailedEventHandler;
			_speechRecognition.GetOperationSuccessEvent -= GetOperationSuccessEventHandler;
			_speechRecognition.GetOperationFailedEvent -= GetOperationFailedEventHandler;
			_speechRecognition.ListOperationsSuccessEvent -= ListOperationsSuccessEventHandler;
			_speechRecognition.ListOperationsFailedEvent -= ListOperationsFailedEventHandler;

			_speechRecognition.FinishedRecordEvent -= FinishedRecordEventHandler;
			_speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

			_speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
		}

		private void Update()
		{
			// update the volume bar
			if(_speechRecognition.IsRecording)
			{
				// Debug.Log("GetMaxFrame"+_speechRecognition.GetMaxFrame());
				if (_speechRecognition.GetMaxFrame() > 0)
				{
					float max = (float)_speechRecognition.configs[_speechRecognition.currentConfigIndex].voiceDetectionThreshold;
					float current = _speechRecognition.GetLastFrame() / max;

					if(current >= 1f)
					{
						_voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 1f), 30 * Time.deltaTime);
					}
					else
					{
						_voiceLevelImage.fillAmount = Mathf.Lerp(_voiceLevelImage.fillAmount, Mathf.Clamp(current / 2f, 0, 0.5f), 30 * Time.deltaTime);
					}

					_voiceLevelImage.color = current >= 1f ? Color.green : Color.red;
				}
			}
			else
			{
				_voiceLevelImage.fillAmount = 0f;
			}
		}

		private void RefreshMicsButtonOnClickHandler()
		{
			_speechRecognition.RequestMicrophonePermission(null);

			_microphoneDevicesDropdown.ClearOptions();

			for (int i = 0; i < _speechRecognition.GetMicrophoneDevices().Length; i++)
			{
				_microphoneDevicesDropdown.options.Add(new Dropdown.OptionData(_speechRecognition.GetMicrophoneDevices()[i]));
			}

			//smart fix of dropdowns
			_microphoneDevicesDropdown.value = 1;
			_microphoneDevicesDropdown.value = 0;
		}

		private void MicrophoneDevicesDropdownOnValueChangedEventHandler(int value)
		{
			if (!_speechRecognition.HasConnectedMicrophoneDevices())
				return;
			_speechRecognition.SetMicrophoneDevice(_speechRecognition.GetMicrophoneDevices()[value]);
		}
		

		public void StartRecordButtonOnClickHandler()
		{
			_startRecordButton.interactable = false;
			_stopRecordButton.interactable = true;
			_detectThresholdButton.interactable = false;
			_resultText.text = string.Empty;

			_speechRecognition.StartRecord(_voiceDetectionToggle.isOn);
		}

		public void StopRecordButtonOnClickHandler()
		{
			_stopRecordButton.interactable = false;
			_startRecordButton.interactable = true;
			_detectThresholdButton.interactable = true;

			_speechRecognition.StopRecord();
		}

		private void GetOperationButtonOnClickHandler()
		{
			if(string.IsNullOrEmpty(_operationIdInputField.text))
			{
				_resultText.text = "<color=red>Operatinon name is empty</color>";
				return;
			}

			_speechRecognition.GetOperation(_operationIdInputField.text);
		}

		private void GetListOperationsButtonOnClickHandler()
		{
			// some parameters could be seted
			_speechRecognition.GetListOperations();
		}

		private void DetectThresholdButtonOnClickHandler()
		{
			_speechRecognition.DetectThreshold();
		}

		private void CancelAllRequetsButtonOnClickHandler()
		{
			_speechRecognition.CancelAllRequests();
		}

		private void RecognizeButtonOnClickHandler()
		{
			if (_speechRecognition.LastRecordedClip == null)
			{
				_resultText.text = "<color=red>No Record found</color>";
				return;
			}

			FinishedRecordEventHandler(_speechRecognition.LastRecordedClip, _speechRecognition.LastRecordedRaw);
		}

		private void StartedRecordEventHandler()
		{
			_speechRecognitionState.color = Color.green;
		}

		private void RecordFailedEventHandler()
		{
			_speechRecognitionState.color = Color.red;
			_resultText.text = "<color=red>Start record Failed. Please check microphone device and try again.</color>";

			_stopRecordButton.interactable = false;
			_startRecordButton.interactable = true;
		}

		private void BeginTalkigEventHandler()
		{
			_resultText.text = "<color=blue>Talk Began.</color>";
		}

		private void EndTalkigEventHandler(AudioClip clip, float[] raw)
		{
			_resultText.text += "\n<color=blue>Talk Ended.</color>";

			FinishedRecordEventHandler(clip, raw);
		}

		private void FinishedRecordEventHandler(AudioClip clip, float[] raw)
		{
			if (!_voiceDetectionToggle.isOn && _startRecordButton.interactable)
			{
				_speechRecognitionState.color = Color.yellow;
			}

			if (clip == null || !_recognizeDirectlyToggle.isOn)
				return;

			RecognitionConfig config = RecognitionConfig.GetDefault();
			config.languageCode = ((Enumerators.LanguageCode)_languageDropdown.value).Parse();
			config.speechContexts = new SpeechContext[]
			{
				new SpeechContext()
				{
					phrases = _contextPhrasesInputField.text.Replace(" ", string.Empty).Split(',')
				}
			};
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

			if (_longRunningRecognizeToggle.isOn)
			{
				_speechRecognition.LongRunningRecognize(recognitionRequest);
			}
			else
			{
				_speechRecognition.Recognize(recognitionRequest);
			}
		}

		private void GetOperationFailedEventHandler(string error)
		{
			_resultText.text = "Get Operation Failed: " + error;
		}

		private void ListOperationsFailedEventHandler(string error)
		{
			_resultText.text = "List Operations Failed: " + error;
		}

		private void RecognizeFailedEventHandler(string error)
        {
            _resultText.text = "Recognize Failed: " + error;
        }

		private void LongRunningRecognizeFailedEventHandler(string error)
		{
			_resultText.text = "Long Running Recognize Failed: " + error;
		}

		private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
		{
			_resultText.text = "List Operations Success.\n";

			if (operationsResponse.operations != null)
			{
				_resultText.text += "Operations:\n";

				foreach (var item in operationsResponse.operations)
				{
					_resultText.text += "name: " + item.name + "; done: " + item.done + "\n";
				}
			}
		}

		private void GetOperationSuccessEventHandler(Operation operation)
		{
			_resultText.text = "Get Operation Success.\n";
			_resultText.text += "name: " + operation.name + "; done: " + operation.done;

			if(operation.done && (operation.error == null || string.IsNullOrEmpty(operation.error.message)))
			{
				InsertRecognitionResponseInfo(operation.response);
			}		
		}

		private void RecognizeSuccessEventHandler(RecognitionResponse recognitionResponse)
        {
			_resultText.text = "Recognize Success.";
			InsertRecognitionResponseInfo(recognitionResponse);
        }

        private void LongRunningRecognizeSuccessEventHandler(Operation operation)
        {
			if (operation.error != null || !string.IsNullOrEmpty(operation.error.message))
				return;

			_resultText.text = "Long Running Recognize Success.\n Operation name: " + operation.name;

			if (operation != null && operation.response != null && operation.response.results.Length > 0)
            {
                _resultText.text = "Long Running Recognize Success.";
				_resultText.text += "\n" + operation.response.results[0].alternatives[0].transcript;

				string other = "\nDetected alternatives:\n";

                foreach (var result in operation.response.results)
                {
                    foreach (var alternative in result.alternatives)
                    {
						if (operation.response.results[0].alternatives[0] != alternative)
						{
							other += alternative.transcript + ", ";
						}
                    }
                }

                _resultText.text += other;
            }
            else
            {
                _resultText.text = "Long Running Recognize Success. Words not detected.";
            }
        }

		private void InsertRecognitionResponseInfo(RecognitionResponse recognitionResponse)
		{
			if (recognitionResponse == null || recognitionResponse.results.Length == 0)
			{
				_resultText.text = "\nWords not detected.";
				return;
			}

			_resultText.text += "\n" + recognitionResponse.results[0].alternatives[0].transcript;

			var words = recognitionResponse.results[0].alternatives[0].words;
			resultWords="";
			if (words != null)
			{
				string times = string.Empty;

				foreach (var item in recognitionResponse.results[0].alternatives[0].words)
				{
					times += "<color=green>" + item.word + "</color> -  start: " + item.startTime + "; end: " + item.endTime + "\n";
					resultWords+=""+ item.word;
				}

				_resultText.text += "\n" + times;
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

			_resultText.text += other;
		}
    }
}