using Newtonsoft.Json;
using System;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
	[Serializable]
    public class RecognitionAudio
    {
    }

	[Serializable]
	public class RecognitionAudioContent : RecognitionAudio
	{
		public string content = string.Empty;
	}

	[Serializable]
	public class RecognitionAudioUri : RecognitionAudio
	{
		public string uri = string.Empty;
	}

	[Serializable]
    public class RecognitionConfig
    {
        public Enumerators.AudioEncoding encoding; 
        public double sampleRateHertz;
		public double audioChannelCount;
		public bool enableSeparateRecognitionPerChannel;
		public string languageCode; 
        public double maxAlternatives; 
        public bool profanityFilter;
		public SpeechContext[] speechContexts = new SpeechContext[0];
        public bool enableWordTimeOffsets;
		public bool enableAutomaticPunctuation;
		public RecognitionMetadata metadata;
		public string model;
		public bool useEnhanced;
		
		[JsonIgnore]
		/// <summary>
		/// BETA FIELD. Ignored in json serialization
		/// </summary>
		public bool enableSpeakerDiarization;

		[JsonIgnore]
		/// <summary>
		/// BETA FIELD. Ignored in json serialization
		/// </summary>
		public double diarizationSpeakerCount;

		[JsonIgnore]
		/// <summary>
		/// BETA FIELD. Ignored in json serialization
		/// </summary>
		public SpeakerDiarizationConfig diarizationConfig;

		[JsonIgnore]
		/// <summary>
		/// BETA FIELD. Ignored in json serialization
		/// </summary>
		public string[] alternativeLanguageCodes;

		public static RecognitionConfig GetDefault()
		{
			return new RecognitionConfig()
			{
				encoding = Enumerators.AudioEncoding.LINEAR16,
				sampleRateHertz = 16000,
				audioChannelCount = 2,
				enableSeparateRecognitionPerChannel = false,
				maxAlternatives = 10,
				profanityFilter = false,
				enableWordTimeOffsets = true,
				enableAutomaticPunctuation = true,
				metadata = new RecognitionMetadata()
				{
					audioTopic = string.Empty,
					originalMediaType = Enumerators.OriginalMediaType.ORIGINAL_MEDIA_TYPE_UNSPECIFIED,
					industryNaicsCodeOfAudio = 0,
					interactionType = Enumerators.InteractionType.INTERACTION_TYPE_UNSPECIFIED,
					microphoneDistance = Enumerators.MicrophoneDistance.MICROPHONE_DISTANCE_UNSPECIFIED,
					originalMimeType = string.Empty,
					recordingDeviceName = string.Empty,
					recordingDeviceType = Enumerators.RecordingDeviceType.RECORDING_DEVICE_TYPE_UNSPECIFIED
				},
				 model = "default",
				 useEnhanced = true,
				 languageCode = Enumerators.LanguageCode.en_GB.Parse(),
			};
		}
	}


	/// <summary>
	/// BETA CLASS
	/// </summary>
	[Serializable]
	public class SpeakerDiarizationConfig
	{
		/// <summary>
		/// BETA FIELD
		/// </summary>
		public bool enableSpeakerDiarization;

		/// <summary>
		/// BETA FIELD
		/// </summary>
		public double minSpeakerCount;

		/// <summary>
		/// BETA FIELD
		/// </summary>
		public double maxSpeakerCount;
	}

	[Serializable]
	public class RecognitionMetadata
	{
		public Enumerators.InteractionType interactionType;
		public double industryNaicsCodeOfAudio;
		public Enumerators.MicrophoneDistance microphoneDistance;
		public Enumerators.OriginalMediaType originalMediaType;
		public Enumerators.RecordingDeviceType recordingDeviceType;
		public string recordingDeviceName;
		public string originalMimeType;
		//public string obfuscatedId; // deprecated
		public string audioTopic;
	}
	   
	[Serializable]
    public class SpeechContext
    {
		public string[] phrases = new string[0];
    }

	[Serializable]
	public class WordInfo
	{
		public string startTime;
		public string endTime;
		public string word;
	}

	[Serializable]
	public class Operation
	{
		public string name;
		public LongRunningRecognizeMetadata metadata;
		public bool done;
		public Status error;
		public RecognitionResponse response;
	}

	[Serializable]
	public class SpeechRecognitionAlternative
	{
		public string transcript;
		public double confidence;
		public WordInfo[] words;
	}

	[Serializable]
	public class Status
	{
		public double code;
		public string message;
		public object[] details;
	}

	[Serializable]
	public class LongRunningRecognizeMetadata
	{
		public double progressPercent;
		public string startTime;  // Timestamp format
		public string lastUpdateTime;  // Timestamp format
	}

	[Serializable]
	public class SpeechRecognitionResult
	{
		public SpeechRecognitionAlternative[] alternatives = new SpeechRecognitionAlternative[0];
		public double channelTag;
	}

	[Serializable]
	public class RecognitionResponse
	{
		public SpeechRecognitionResult[] results = new SpeechRecognitionResult[0];
	}

	[Serializable]
	public class ListOperationsResponse
	{
		public Operation[] operations;
		public string nextPageToken;
	}


	[Serializable]
    public class GeneralRecognitionRequest
    {
        public RecognitionConfig config = new RecognitionConfig();
        public RecognitionAudio audio = new RecognitionAudio();
    }
}