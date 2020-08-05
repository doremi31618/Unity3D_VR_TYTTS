using FrostweepGames.Plugins.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class GCSpeechRecognition : MonoBehaviour
    {
        private static GCSpeechRecognition _Instance;
        public static GCSpeechRecognition Instance
        {
            get
            {
				if (_Instance == null)
				{
					_Instance = new GameObject("[Singleton]GCSpeechRecognition").AddComponent<GCSpeechRecognition>();
					_Instance.configs = new List<Config>();
					_Instance.configs.Add(Resources.Load<Config>("GCSpeechRecognitonConfig"));
					_Instance.apiKey = string.Empty; // there could be a default api key
				}

                return _Instance;
            }
        }

		public event Action<RecognitionResponse> RecognizeSuccessEvent;
		public event Action<string> RecognizeFailedEvent;
		public event Action<Operation> LongRunningRecognizeSuccessEvent;
		public event Action<string> LongRunningRecognizeFailedEvent;
		public event Action<Operation> GetOperationSuccessEvent;
		public event Action<string> GetOperationFailedEvent;
		public event Action<ListOperationsResponse> ListOperationsSuccessEvent;
		public event Action<string> ListOperationsFailedEvent;

		public event Action StartedRecordEvent;
		public event Action<AudioClip, float[]> FinishedRecordEvent;
		public event Action RecordFailedEvent;

		public event Action BeginTalkigEvent;
		public event Action<AudioClip, float[]> EndTalkigEvent;

        private ISpeechRecognitionManager _speechRecognitionManager;

		private IMediaManager _mediaManager;

		private IVoiceDetectionManager _voiceDetectionManager;

		private bool _IsCurrentInstance { get { return Instance == this; } }

		[Header("----------Prefab Object Settings----------")]
		public bool isDontDestroyOnLoad = false;

		[Space]
		[Header("----------Recognition Configs----------")]
        public int currentConfigIndex;
        public List<Config> configs;

		[Space]
		[Header("----------Plugin Settings----------")]     
		public bool isFullDebugLogIfError = false;
		public string apiKey;

#if !NET_2_0 && !NET_2_0_SUBSET
		public AudioClip LastRecordedClip => _mediaManager.LastRecordedClip;
#else
		public AudioClip LastRecordedClip
		{
			get
			{
				return _mediaManager.LastRecordedClip;
			}
		}
#endif

#if !NET_2_0 && !NET_2_0_SUBSET
		public bool IsRecording => _mediaManager.IsRecording;
#else
		public bool LastRecordedClip
		{
			get
			{
				return _mediaManager.IsRecording;
			}
		}
#endif

#if !NET_2_0 && !NET_2_0_SUBSET
		public float[] LastRecordedRaw => _mediaManager.LastRecordedRaw;
#else
		public float[] LastRecordedRa
		{
			get
			{
				return _mediaManager.LastRecordedRaw;
			}
		}
#endif
		private void Awake()
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

			if (isDontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

            _Instance = this;

			if (configs == null || configs.Count == 0)
			{
				throw new MissingFieldException("NO CONFIG FOUND!");
			}

			ServiceLocator.Register<ISpeechRecognitionManager>(new SpeechRecognitionManager());
			ServiceLocator.Register<IVoiceDetectionManager>(new VoiceDetectionManager());
			ServiceLocator.Register<IMediaManager>(new MediaManager());
			ServiceLocator.InitServices();

			_mediaManager = ServiceLocator.Get<IMediaManager>();
            _speechRecognitionManager = ServiceLocator.Get<ISpeechRecognitionManager>();
			_voiceDetectionManager = ServiceLocator.Get<IVoiceDetectionManager>();

			_mediaManager.RecordStartedEvent += RecordStartedEventHandler;
            _mediaManager.RecordEndedEvent += RecordEndedEventHandler;
            _mediaManager.RecordFailedEvent += RecordFailedEventHandler;
            _mediaManager.TalkBeganEvent += TalkBeganEventHandler;
            _mediaManager.TalkEndedEvent += TalkEndedEventHandler;

            _speechRecognitionManager.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
            _speechRecognitionManager.RecognizeFailedEvent += RecognizeFailedEventHandler;
            _speechRecognitionManager.LongRunningRecognizeSuccessEvent += LongRunningRecognizeSuccessEventHandler;
            _speechRecognitionManager.LongRunningRecognizeFailedEvent += LongRunningRecognizeFailedEventHandler;
			_speechRecognitionManager.GetOperationSuccessEvent += GetOperationSuccessEventHandler;
			_speechRecognitionManager.GetOperationFailedEvent += GetOperationFailedEventHandler;
			_speechRecognitionManager.ListOperationsSuccessEvent += ListOperationsSuccessEventHandler;
			_speechRecognitionManager.ListOperationsFailedEvent += ListOperationsFailedEventHandler;

			_speechRecognitionManager.SetConfig(configs[Mathf.Clamp(currentConfigIndex, 0, configs.Count - 1)]);
		}

		private void Update()
		{
			if (!_IsCurrentInstance)
				return;

			ServiceLocator.Instance.Update();
		}

		private void OnDestroy()
		{
			if (!_IsCurrentInstance)
				return;

			_mediaManager.RecordStartedEvent -= RecordStartedEventHandler;
			_mediaManager.RecordEndedEvent -= RecordEndedEventHandler;
			_mediaManager.RecordFailedEvent -= RecordFailedEventHandler;
			_mediaManager.TalkBeganEvent -= TalkBeganEventHandler;
			_mediaManager.TalkEndedEvent -= TalkEndedEventHandler;

			_speechRecognitionManager.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
			_speechRecognitionManager.RecognizeFailedEvent -= RecognizeFailedEventHandler;
			_speechRecognitionManager.LongRunningRecognizeSuccessEvent -= LongRunningRecognizeSuccessEventHandler;
			_speechRecognitionManager.LongRunningRecognizeFailedEvent -= LongRunningRecognizeFailedEventHandler;
			_speechRecognitionManager.GetOperationSuccessEvent -= GetOperationSuccessEventHandler;
			_speechRecognitionManager.GetOperationFailedEvent -= GetOperationFailedEventHandler;
			_speechRecognitionManager.ListOperationsSuccessEvent -= ListOperationsSuccessEventHandler;
			_speechRecognitionManager.ListOperationsFailedEvent -= ListOperationsFailedEventHandler;

			ServiceLocator.Instance.Dispose();

			_Instance = null;	
		}

		public float GetLastFrame()
		{
			return _mediaManager.GetLastFrame();
		}

		public float GetMaxFrame()
		{
			return _mediaManager.GetMaxFrame();
		}

		public void StartRecord(bool withVoiceDetection)
        {
			Debug.Log("start record");
            _mediaManager.StartRecord(withVoiceDetection);
        }

        public void StopRecord()
        {
			Debug.Log("stop record");
            _mediaManager.StopRecord();
        }

		public void DetectThreshold()
		{
			_voiceDetectionManager.DetectThreshold();
		}

		public bool ReadyToRecord()
		{
			Debug.Log("ready to Record");
			return _mediaManager.ReadyToRecord();
		}

		public string[] GetMicrophoneDevices()
		{
			return _mediaManager.GetMicrophoneDevices();
		}

		public bool HasConnectedMicrophoneDevices()
		{
			return _mediaManager.HasConnectedMicrophoneDevices();
		}

		public void SetMicrophoneDevice(string device)
		{
			_mediaManager.SetMicrophoneDevice(device);
		}

		public void SaveLastRecordedAudioClip(string path)
		{
			_mediaManager.SaveLastRecordedAudioClip(path);
		}

		public long Recognize(GeneralRecognitionRequest recognitionRequest)
        {
            return _speechRecognitionManager.Recognize(recognitionRequest);
        }

		public long LongRunningRecognize(GeneralRecognitionRequest recognitionRequest)
		{
			return _speechRecognitionManager.LongRunningRecognize(recognitionRequest);
		}

		public long GetOperation(string operation)
		{
			return _speechRecognitionManager.GetOperation(operation);
		}

		public long GetListOperations(string name = null, string filter = null, int pageSize = -1, string pageToken = null)
		{
			return _speechRecognitionManager.GetListOperations(name, filter, pageSize, pageToken);
		}

		public bool CancelRequest(long id)
        {

            return _speechRecognitionManager.CancelRequest(id);
        }

		public int CancelAllRequests()
		{
			return _speechRecognitionManager.CancelAllRequests();
		}

		public bool HasMicrophonePermission()
		{
#if UNITY_2018_3_OR_NEWER && !NET_2_0 && !NET_2_0_SUBSET
			return _mediaManager.HasMicrophonePermission();
#else
			return true;
#endif
		}

		/// <summary>
		/// Currently works as synchronous function with callback when app unpauses
		/// could not work properly if has enabled checkbox regarding additional frame in pause
		/// </summary>
		/// <param name="callback"></param>
		public void RequestMicrophonePermission(Action<bool> callback)
		{
#if UNITY_2018_3_OR_NEWER && !NET_2_0 && !NET_2_0_SUBSET
			 _mediaManager.RequestMicrophonePermission(callback);
#endif
		}

		#region handlers
		private void RecognizeSuccessEventHandler(RecognitionResponse response)
        {
#if !NET_2_0 && !NET_2_0_SUBSET
			RecognizeSuccessEvent?.Invoke(response);
#else
			if (RecognizeSuccessEvent != null)
			{
				RecognizeSuccessEvent(response);
			}
#endif
		}

		private void LongRunningRecognizeSuccessEventHandler(Operation operation)
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			LongRunningRecognizeSuccessEvent?.Invoke(operation);
#else
			if (LongRunningRecognizeSuccessEvent != null)
			{
				LongRunningRecognizeSuccessEvent(operation);
			}
#endif
		}

		private void RecognizeFailedEventHandler(string error)
        {
#if !NET_2_0 && !NET_2_0_SUBSET
			RecognizeFailedEvent?.Invoke(error);
#else
			if (RecognizeFailedEvent != null)
			{
				RecognizeFailedEvent(error);
			}
#endif
		}

		private void LongRunningRecognizeFailedEventHandler(string error)
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			LongRunningRecognizeFailedEvent?.Invoke(error);
#else
			if (LongRunningRecognizeFailedEvent != null)
			{
				LongRunningRecognizeFailedEvent(error);
			}
#endif
		}

		private void RecordFailedEventHandler()
        {
#if !NET_2_0 && !NET_2_0_SUBSET
			RecordFailedEvent?.Invoke();
#else
			if (RecordFailedEvent != null)
			{
				RecordFailedEvent(error);
			}
#endif
        }

        private void TalkBeganEventHandler()
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			BeginTalkigEvent?.Invoke();
#else
			print("TalkBegan");
			if (BeginTalkigEvent != null)
			{
				BeginTalkigEvent();
			}
#endif
        }

        private void TalkEndedEventHandler(AudioClip clip, float[] raw)
        {
#if !NET_2_0 && !NET_2_0_SUBSET
			EndTalkigEvent?.Invoke(clip, raw);
#else
			if (EndTalkigEvent != null)
			{
				EndTalkigEvent(clip, raw);
			}
#endif
		}

		private void RecordStartedEventHandler()
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			StartedRecordEvent?.Invoke();
#else
			if (StartedRecordEvent != null)
			{
				StartedRecordEvent();
			}
#endif
        }

        private void RecordEndedEventHandler(AudioClip clip, float[] raw)
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			FinishedRecordEvent?.Invoke(clip, raw);
#else
			if (FinishedRecordEvent != null)
			{
				FinishedRecordEvent(clip, raw);
			}
#endif
        }

		private void GetOperationSuccessEventHandler(Operation operation)
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			GetOperationSuccessEvent?.Invoke(operation);
#else
			if (GetOperationSuccessEvent != null)
			{
				GetOperationSuccessEvent(operation);
			}
#endif
		}

		private void GetOperationFailedEventHandler(string error)
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			GetOperationFailedEvent?.Invoke(error);
#else
			if (GetOperationFailedEvent != null)
			{
				GetOperationFailedEvent(error);
			}
#endif
		}

		private void ListOperationsSuccessEventHandler(ListOperationsResponse operationsResponse)
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			ListOperationsSuccessEvent?.Invoke(operationsResponse);
#else
			if (ListOperationsSuccessEvent != null)
			{
				ListOperationsSuccessEvent(operationsResponse);
			}
#endif
		}

		private void ListOperationsFailedEventHandler(string error)
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			ListOperationsFailedEvent?.Invoke(error);
#else
			if (ListOperationsFailedEvent != null)
			{
				ListOperationsFailedEvent(error);
			}
#endif
		}

#endregion
	}
}