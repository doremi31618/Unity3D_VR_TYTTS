namespace FrostweepGames.Plugins.Networking
{
    public class NetworkResponse
    {
        public long RequestId { get; private set; }
        public NetworkEnumerators.RequestType RequestType { get; private set; }
        public object[] Parameters { get; private set; }
        public string Response { get; private set; }
		public string Error { get; private set; }
		public long ResponseCode { get; private set; }

		public NetworkResponse(string resp, string err, long index, NetworkEnumerators.RequestType type, object[] param)
        {
            RequestType = type;
            RequestId = index;
            Response = resp;
            Error = err;
            Parameters = param;
        }

		public NetworkResponse(NetworkRequest request)
		{
			RequestType = request.RequestType;
			RequestId = request.RequestId;
			Response = request.Request.text;
			Error = request.Request.error;
			Parameters = request.Parameters;
			ResponseCode = request.Request.responseCode;
		}

		public string GetFullLog()
		{
			return Error + System.Environment.NewLine + Response;
		}

		public bool HasError()
		{
			return !string.IsNullOrEmpty(Error) || ResponseCode != 200;
		}
    }
}