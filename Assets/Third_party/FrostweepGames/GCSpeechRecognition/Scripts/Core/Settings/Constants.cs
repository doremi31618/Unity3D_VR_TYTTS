namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class Constants
    {
		public const string ROOT_REQUEST_URL = "https://speech.googleapis.com";
		public const string API_VERSION = "/v1/";
		public const string API_KEY_PARAM = "?key=";

		public const string POST_RECOGNIZE_REQUEST_URL = "speech:recognize";
		public const string POST_LONG_RUNNING_RECOGNIZE_REQUEST_URL = "speech:longrunningrecognize";

		public const string GET_OPERATION_REQUEST_URL = "operations/{name}";
		public const string GET_LIST_OPERATIONS_REQUEST_URL = "operations";


		// not implemented yet
		public const string GET_LOCATION_OPERATION_REQUEST_URL = "{name=projects/*/locations/*/operations/*}";
		public const string GET_LOCATION_LIST_OPERATIONS_REQUEST_URL = "{name=projects/*/locations/*}/operations";   
    }
}