using System;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public static class AudioClipRaw2ByteConverter
    {
        public static byte[] AudioClipRawToByte(float[] samples, bool increaseVolume = false, float volumeMultiplier = 1f)
        {
            if (increaseVolume)
            {
                samples = AudioTools.IncreaseVolume(samples, volumeMultiplier);
            }

            return FloatToByte(samples);
        }

        public static byte[] FloatToByte(float[] samples)
        {
            Int16[] intData = new Int16[samples.Length];

            Byte[] bytesData = new Byte[samples.Length * 2];

            int rescaleFactor = 32767;

            for (int i = 0; i < samples.Length; i++)
            {
                intData[i] = (short)(samples[i] * rescaleFactor);
                Byte[] byteArr = new Byte[2];
                byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(bytesData, i * 2);
            }

            return bytesData;
        }

        public static float[] ByteToFloat(byte[] bytesData)
        {
            int rescaleFactor = 32767;

            float[] samples = new float[bytesData.Length / 2];

            for (int i = 0; i < samples.Length; i++)
            {
                samples[i] = (float)BitConverter.ToInt16(bytesData, i * 2) / rescaleFactor;
            }

            return samples;
        }
    }
}