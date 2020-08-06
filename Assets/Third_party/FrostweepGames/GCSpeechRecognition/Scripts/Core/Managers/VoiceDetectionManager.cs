using UnityEngine;
using FrostweepGames.Plugins.Core;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class VoiceDetectionManager : IService, IVoiceDetectionManager
    {
		private const double AUDIO_DETECT_RATIO = 32768.0;

		private ISpeechRecognitionManager _speechRecognitionManager;

		private IMediaManager _mediaManager;

        private double _threshold;

        public void Init()
        {
			_speechRecognitionManager = ServiceLocator.Get<ISpeechRecognitionManager>();
			_mediaManager = ServiceLocator.Get<IMediaManager>();

			_threshold = _speechRecognitionManager.CurrentConfig.voiceDetectionThreshold;
		}

		public void Dispose()
        {
        }

        public void Update()
        {
        }

        public bool HasDetectedVoice(byte[] data)
        {
            return ProcessData(data);
        }

		public void DetectThreshold(int durationSec = 3)
		{
			GCSpeechRecognition.Instance.StartCoroutine(_mediaManager.OneTimeRecord(durationSec, (samples) =>
			{
                float accum = 0f;
                for (int i = 0; i < samples.Length; i++)
                {
                    accum += Mathf.Abs(samples[i]);
                }

                _threshold = System.Math.Round(accum / (float)samples.Length, 6) * 5;
                _speechRecognitionManager.CurrentConfig.voiceDetectionThreshold = _threshold;
            }));
		}

		private bool ProcessData(byte[] data)
        {
            bool detected = false;
            double sumTwo = 0;
            double tempValue;

            for (int index = 0; index < data.Length; index += 2)
            {
                tempValue = (short)((data[index + 1] << 8) | data[index + 0]);

                tempValue /= AUDIO_DETECT_RATIO;

                sumTwo += tempValue * tempValue;

                if (tempValue > _threshold)
                    detected = true;
            }

            sumTwo /= (data.Length / 2);

            if (detected || sumTwo > _threshold)
                return true;
            else
                return false;
        }
    }
}