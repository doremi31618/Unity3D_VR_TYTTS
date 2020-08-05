using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
	public static class AudioConvert
	{
		public static string Convert(AudioClip clip, Enumerators.AudioEncoding encoding, bool increaseVolume = false, float volume = 1f)
		{
			byte[] audioArray;

			switch (encoding)
			{
				case Enumerators.AudioEncoding.LINEAR16:
					{
						if (increaseVolume)
						{
							clip.SetData(AudioClip2ByteConverter.ByteToFloat(
										 AudioClip2ByteConverter.AudioClipToByte(clip, increaseVolume, volume)), 0);
						}

						audioArray = AudioClip2PCMConverter.AudioClip2PCM(clip);
					}
					break;
				default:
					throw new System.NotSupportedException(encoding + " doesn't supported for converting!");
			}

			return System.Convert.ToBase64String(audioArray);
		}

		public static string Convert(float[] raw, Enumerators.AudioEncoding encoding, bool increaseVolume = false, float volume = 1f)
		{
			byte[] audioArray;

			switch (encoding)
			{
				case Enumerators.AudioEncoding.LINEAR16:
					{
						if (increaseVolume)
						{
							raw = AudioClip2ByteConverter.ByteToFloat(AudioClipRaw2ByteConverter.AudioClipRawToByte(raw, increaseVolume, volume));
						}

						audioArray = AudioClipRaw2PCMConverter.AudioClipRaw2PCM(raw);
					}
					break;
				default:
					throw new System.NotSupportedException(encoding + " doesn't supported for converting!");
			}

			return System.Convert.ToBase64String(audioArray);
		}

		public static AudioClip Convert(float[] samples, int channels = 2, int sampleRate = 16000)
		{
			AudioClip clip = AudioClip.Create($"AudioClip_{sampleRate}", samples.Length, channels, sampleRate, false);
			clip.SetData(samples, 0);
			return clip;
		}

		public static string ToBase64(this AudioClip clip, Enumerators.AudioEncoding encoding = Enumerators.AudioEncoding.LINEAR16, bool increaseVolume = false, float volume = 1f)
		{
			return Convert(clip, encoding, increaseVolume, volume);
		}

		public static string ToBase64(this float[] rawAudioClipData, Enumerators.AudioEncoding encoding = Enumerators.AudioEncoding.LINEAR16, bool increaseVolume = false, float volume = 1f)
		{
			return Convert(rawAudioClipData, encoding, increaseVolume, volume);
		}
	}
}