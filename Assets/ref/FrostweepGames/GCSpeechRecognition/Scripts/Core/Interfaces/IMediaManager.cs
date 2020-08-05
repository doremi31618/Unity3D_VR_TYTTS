using System;
using System.Collections;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public interface IMediaManager
    {
		event Action MicrophoneDeviceSelectedEvent;

		event Action RecordStartedEvent;
		event Action RecordFailedEvent;
		event Action<AudioClip, float[]> RecordEndedEvent;

		event Action TalkBeganEvent;
		event Action<AudioClip, float[]> TalkEndedEvent;

		bool IsRecording { get; }
		string MicrophoneDevice { get; }
		AudioClip LastRecordedClip { get; }
		float[] LastRecordedRaw { get; }
		bool DetectVoice { get; }

		float GetLastFrame();
		float GetMaxFrame();
		void StartRecord(bool withVoiceDetection = false);
		void StopRecord();
		bool ReadyToRecord();
		bool HasConnectedMicrophoneDevices();
		void SetMicrophoneDevice(string device);
		string[] GetMicrophoneDevices();
		void SaveLastRecordedAudioClip(string path);
		IEnumerator OneTimeRecord(int durationSec, Action<float[]> callback, int sampleRate = 16000);

#if UNITY_2018_3_OR_NEWER && !NET_2_0 && !NET_2_0_SUBSET
		bool HasMicrophonePermission();
		void RequestMicrophonePermission(Action<bool> callback);
#endif
	}
}