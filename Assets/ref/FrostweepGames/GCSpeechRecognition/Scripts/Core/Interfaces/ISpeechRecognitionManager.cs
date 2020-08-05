using System;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public interface ISpeechRecognitionManager
    {
        event Action<RecognitionResponse> RecognizeSuccessEvent;
		event Action<string> RecognizeFailedEvent;
		event Action<Operation> LongRunningRecognizeSuccessEvent;
		event Action<string> LongRunningRecognizeFailedEvent;
		event Action<Operation> GetOperationSuccessEvent;
		event Action<string> GetOperationFailedEvent;
		event Action<ListOperationsResponse> ListOperationsSuccessEvent;
		event Action<string> ListOperationsFailedEvent;

		Config CurrentConfig { get; }

        void SetConfig(Config config);
		bool CancelRequest(long id);
		int CancelAllRequests();
		long Recognize(GeneralRecognitionRequest request);
		long LongRunningRecognize(GeneralRecognitionRequest request);
		long GetOperation(string operation);
		long GetListOperations(string name = null, string filter = null, int pageSize = -1, string pageToken = null);	
	}
}