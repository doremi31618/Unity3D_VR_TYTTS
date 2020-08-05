namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public interface IVoiceDetectionManager
    {
        bool HasDetectedVoice(byte[] data);

#if !NET_2_0 && !NET_2_0_SUBSET
		void DetectThreshold(int durationSec = 3);
#endif
	}
}