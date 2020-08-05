using System;
using System.Collections.Generic;

namespace FrostweepGames.Plugins.Networking
{
    public class NetworkingService : IDisposable
    {
        public event Action<NetworkResponse> NetworkResponseEvent;

        private List<NetworkRequest> _networkRequests;

        private List<NetworkResponse> _networkResponses;

        private long _requestsSent = 0;

        public NetworkingService()
        {
            _networkRequests = new List<NetworkRequest>();
            _networkResponses = new List<NetworkResponse>();
        }

        public void Update()
        {
            for(int i = 0; i < _networkRequests.Count; i++)
            {
                if (_networkRequests[i].Request.isDone)
                {
                    NetworkResponse response = new NetworkResponse(_networkRequests[i]);
                    _networkResponses.Add(response);

#if NET_2_0 || NET_2_0_SUBSET
					if (NetworkResponseEvent != null)
                        NetworkResponseEvent(response);
#else
					NetworkResponseEvent?.Invoke(response);
#endif
                    _networkRequests.RemoveAt(i--);
                }
            }
        }

        public void Dispose()
        {
            _networkRequests.Clear();
            _networkResponses.Clear();
			_requestsSent = 0;
			NetworkResponseEvent = null;
		}

        public long SendRequest(string uri, string data, NetworkEnumerators.RequestType requestType, object[] param = null, bool checkCertificates = false)
        {
            long netIndex = _requestsSent++;

            NetworkRequest netRequest = new NetworkRequest(uri, data, netIndex, requestType, param, checkCertificates);

            _networkRequests.Add(netRequest);

            netRequest.Send();

            return netIndex;
        }

        public bool CancelRequest(long id)
        {
			NetworkRequest request = _networkRequests.Find(x => x.RequestId == id);

            if(request != null)
            {
                request.Request.Cancel();
                _networkRequests.Remove(request);
				return true;
            }

			return false;
        }

		public int CancelAllRequests()
		{
			int canceledCount = 0;

			try
			{
				for (int i = 0; i < _networkRequests.Count; i++)
				{
					_networkRequests[i].Request.Cancel();
					_networkRequests.RemoveAt(i--);
					canceledCount++;
				}
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogException(e);
			}

			return canceledCount;
		}
	}
}