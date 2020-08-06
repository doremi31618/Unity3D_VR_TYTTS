﻿using System;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public static class Enumerators
    {
		public enum ApiType : int
		{
			UNDEFINED = 0,

			RECOGNIZE,
			LONG_RUNNING_RECOGNIZE,
			OPERATION,
			LIST_OPERATIONS,
			LOCATION_OPERATION,
			LOCATION_LIST_OPERATIONS
		}

		public enum AudioEncoding
        {
            ENCODING_UNSPECIFIED,

            LINEAR16,
            FLAC,
            MULAW,
            AMR,
            AMR_WB,
            OGG_OPUS,
            SPEEX_WITH_HEADER_BYTE,

			/// <summary>
			/// BETA FIELD
			/// </summary>
			MP3
		}

		public enum InteractionType
		{
			INTERACTION_TYPE_UNSPECIFIED,

			DISCUSSION,
			PRESENTATION,
			PHONE_CALL,
			VOICEMAIL,
			PROFESSIONALLY_PRODUCED,
			VOICE_SEARCH,
			VOICE_COMMAND,
			DICTATION
		}

		public enum MicrophoneDistance
		{
			MICROPHONE_DISTANCE_UNSPECIFIED,

			NEARFIELD,
			MIDFIELD,
			FARFIELD
		}

		public enum OriginalMediaType
		{
			ORIGINAL_MEDIA_TYPE_UNSPECIFIED,

			AUDIO,
			VIDEO
		}

		public enum RecordingDeviceType
		{
			RECORDING_DEVICE_TYPE_UNSPECIFIED,

			SMARTPHONE,
			PC,
			PHONE_LINE,
			VEHICLE,
			OTHER_OUTDOOR_DEVICE,
			OTHER_INDOOR_DEVICE
		}

		public enum LanguageCode
        {
            af_ZA,
            am_ET,
            hy_AM,
            az_AZ,
            id_ID,
            ms_MY,
            bn_BD,
            bn_IN,
            ca_ES,
            cs_CZ,
            da_DK,
            de_DE,
            en_AU,
            en_CA,
            en_GH,
            en_GB,
            en_IN,
            en_IE,
            en_KE,
            en_NZ,
            en_NG,
            en_PH,
            en_ZA,
            en_TZ,
            en_US,
            es_AR,
            es_BO,
            es_CL,
            es_CO,
            es_CR,
            es_EC,
            es_SV,
            es_ES,
            es_US,
            es_GT,
            es_HN,
            es_MX,
            es_NI,
            es_PA,
            es_PY,
            es_PE,
            es_PR,
            es_DO,
            es_UY,
            es_VE,
            eu_ES,
            fil_PH,
            fr_CA,
            fr_FR,
            gl_ES,
            ka_GE,
            gu_IN,
            hr_HR,
            zu_ZA,
            is_IS,
            it_IT,
            jv_ID,
            kn_IN,
            km_KH,
            lo_LA,
            lv_LV,
            lt_LT,
            hu_HU,
            ml_IN,
            mr_IN,
            nl_NL,
            ne_NP,
            nb_NO,
            pl_PL,
            pt_BR,
            pt_PT,
            ro_RO,
            si_LK,
            sk_SK,
            sl_SI,
            su_ID,
            sw_TZ,
            sw_KE,
            fi_FI,
            sv_SE,
            ta_IN,
            ta_SG,
            ta_LK,
            ta_MY,
            te_IN,
            vi_VN,
            tr_TR,
            ur_PK,
            ur_IN,
            el_GR,
            bg_BG,
            ru_RU,
            sr_RS,
            he_IL,
            ar_IL,
            ar_JO,
            ar_AE,
            ar_BH,
            ar_DZ,
            ar_SA,
            ar_IQ,
            ar_KW,
            ar_MA,
            ar_TN,
            ar_OM,
            ar_PS,
            ar_QA,
            ar_LB,
            ar_EG,
            fa_IR,
            hi_IN,
            th_TH,
            ko_KR,
            cmn_Hant_TW,
            yue_Hant_HK,
            ja_JP,
            cmn_Hans_HK,
            cmn_Hans_CN,
        }

		public static string Parse(this Enum value, char symbolFrom = '_', char symbolTo = '-')
		{
			return value.ToString().Replace(symbolFrom, symbolTo);
		}
    }
}