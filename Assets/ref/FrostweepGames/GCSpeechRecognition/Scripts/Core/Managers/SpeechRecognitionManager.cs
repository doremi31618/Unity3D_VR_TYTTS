using UnityEngine;
using System;
using FrostweepGames.Plugins.Core;
using FrostweepGames.Plugins.Networking;
using Newtonsoft.Json;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
	public class SpeechRecognitionManager : IService, IDisposable, ISpeechRecognitionManager
	{
		public event Action<RecognitionResponse> RecognizeSuccessEvent;
		public event Action<string> RecognizeFailedEvent;
		public event Action<Operation> LongRunningRecognizeSuccessEvent;
		public event Action<string> LongRunningRecognizeFailedEvent;
		public event Action<Operation> GetOperationSuccessEvent;
		public event Action<string> GetOperationFailedEvent;
		public event Action<ListOperationsResponse> ListOperationsSuccessEvent;
		public event Action<string> ListOperationsFailedEvent;

		private GCSpeechRecognition _gcSpeechRecognition;

		private NetworkingService _networkingService;

		public Config CurrentConfig { get; private set; }

		public void Init()
		{
			_gcSpeechRecognition = GCSpeechRecognition.Instance;

			_networkingService = new NetworkingService();
			_networkingService.NetworkResponseEvent += NetworkResponseEventHandler;

			CurrentConfig = Resources.Load<Config>("GCSpeechRecognitonConfig");
		}

		public void Update()
		{
#if !NET_2_0 && !NET_2_0_SUBSET
			_networkingService?.Update();
#else
			if (_networkingService != null)
			{
				_networkingService.Update();
			}
#endif
		}

		public void Dispose()
		{
			_networkingService.NetworkResponseEvent -= NetworkResponseEventHandler;
			_networkingService.Dispose();
		}

		public void SetConfig(Config config)
		{
			CurrentConfig = config;
		}

		public bool CancelRequest(long id)
		{
			return _networkingService.CancelRequest(id);
		}

		public int CancelAllRequests()
		{
			return _networkingService.CancelAllRequests();
		}

		public long Recognize(GeneralRecognitionRequest request)
		{
			if (CurrentConfig == null)
				throw new NotImplementedException("Config isn't seted! Use SetConfig method!");

			if (request == null)
				throw new NullReferenceException("Recognition request is null");

			string postData = JsonConvert.SerializeObject(request);

			return _networkingService.SendRequest(
				GetAPiRouteEnd(Constants.POST_RECOGNIZE_REQUEST_URL),
				postData,
				NetworkEnumerators.RequestType.POST,
				new object[]
				{
					Enumerators.ApiType.RECOGNIZE
				});
		}

		public long LongRunningRecognize(GeneralRecognitionRequest request)
		{
			if (CurrentConfig == null)
				throw new NotImplementedException("Config isn't seted! Use SetConfig method!");

			if (request == null)
				throw new NullReferenceException("Recognition request is null");

			string postData = JsonConvert.SerializeObject(request);

			return _networkingService.SendRequest(
				GetAPiRouteEnd(Constants.POST_LONG_RUNNING_RECOGNIZE_REQUEST_URL),
				postData,
				NetworkEnumerators.RequestType.POST,
				new object[]
				{
					Enumerators.ApiType.LONG_RUNNING_RECOGNIZE
				});
		}

		public long GetOperation(string operation)
		{
			if (string.IsNullOrEmpty(operation))
				throw new NullReferenceException("operation id is null or empty");

			return _networkingService.SendRequest(
				GetAPiRouteEnd(Constants.GET_OPERATION_REQUEST_URL.Replace("{name}", operation)),
				string.Empty,
				NetworkEnumerators.RequestType.GET,
				new object[]
				{
					Enumerators.ApiType.OPERATION
				});
		}

		public long GetListOperations(string name = null, string filter = null, int pageSize = -1, string pageToken = null)
		{
			string uri = GetAPiRouteEnd(Constants.GET_LIST_OPERATIONS_REQUEST_URL);

			if (!string.IsNullOrEmpty(name))
			{
#if !NET_2_0 && !NET_2_0_SUBSET
				uri += $"&name={name}";
#else
				uri += "&name=" + name;
#endif
			}

			if (!string.IsNullOrEmpty(filter))
			{
#if !NET_2_0 && !NET_2_0_SUBSET
				uri += $"&filter={filter}";
#else
				uri += "&filter=" + filter;
#endif
			}

			if (pageSize != -1)
			{
#if !NET_2_0 && !NET_2_0_SUBSET
				uri += $"&pageSize={pageSize}";
#else
				uri += "&pageSize=" + pageSize;
#endif
			}

			if (!string.IsNullOrEmpty(pageToken))
			{
#if !NET_2_0 && !NET_2_0_SUBSET
				uri += $"&pageToken={pageToken}";
#else
				uri += "&pageToken=" + pageToken;
#endif
			}

			return _networkingService.SendRequest(
				uri,
				string.Empty,
				NetworkEnumerators.RequestType.GET,
				new object[]
				{
					Enumerators.ApiType.LIST_OPERATIONS
				});
		}

		private string GetAPiRouteEnd(string apiRoute)
		{
			return Constants.ROOT_REQUEST_URL + Constants.API_VERSION + apiRoute + Constants.API_KEY_PARAM + _gcSpeechRecognition.apiKey;
		}

		private void NetworkResponseEventHandler(NetworkResponse response)
		{
			if (response.HasError() && GCSpeechRecognition.Instance.isFullDebugLogIfError)
			{
				Debug.LogError(response.GetFullLog());
			}

			if (response.Parameters.Length > 0)
			{
				Enumerators.ApiType apiType = (Enumerators.ApiType)response.Parameters[0];

				switch (apiType)
				{
					case Enumerators.ApiType.RECOGNIZE:
						{
							if (response.HasError())
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								RecognizeFailedEvent?.Invoke(response.GetFullLog());
#else
								if (RecognizeFailedEvent != null)
								{
									RecognizeFailedEvent(response.GetFullLog());
								}
#endif
							}
							else
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								RecognizeSuccessEvent?.Invoke(JsonConvert.DeserializeObject<RecognitionResponse>(response.Response));
#else
								if (RecognizeSuccessEvent != null)
								{
									RecognizeSuccessEvent(JsonConvert.DeserializeObject<RecognitionResponse>(response.Response));
								}
#endif
							}
						}
						break;
					case Enumerators.ApiType.LONG_RUNNING_RECOGNIZE:
						{
							if (response.HasError())
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								LongRunningRecognizeFailedEvent?.Invoke(response.GetFullLog());
#else
								if (LongRunningRecognizeFailedEvent != null)
								{
									LongRunningRecognizeFailedEvent(response.GetFullLog());
								}
#endif
							}
							else
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								LongRunningRecognizeSuccessEvent?.Invoke(JsonConvert.DeserializeObject<Operation>(response.Response));
#else
								if (LongRunningRecognizeSuccessEvent != null)
								{
									LongRunningRecognizeSuccessEvent(JsonConvert.DeserializeObject<Operation>(response.Response));
								}
#endif
							}
						}
						break;
					case Enumerators.ApiType.LIST_OPERATIONS:
						{
							if (response.HasError())
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								ListOperationsFailedEvent?.Invoke(response.GetFullLog());
#else
								if (ListOperationsFailedEvent != null)
								{
									ListOperationsFailedEvent(response.GetFullLog());
								}
#endif
							}
							else
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								ListOperationsSuccessEvent?.Invoke(JsonConvert.DeserializeObject<ListOperationsResponse>(response.Response));
#else
								if (ListOperationsSuccessEvent != null)
								{
									ListOperationsSuccessEvent(JsonConvert.DeserializeObject<ListOperationsResponse>(response.Response));
								}
#endif
							}
						}
						break;
					case Enumerators.ApiType.OPERATION:
						{
							if (response.HasError())
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								GetOperationFailedEvent?.Invoke(response.GetFullLog());
#else
								if (GetOperationFailedEvent != null)
								{
									GetOperationFailedEvent(response.GetFullLog());
								}
#endif
							}
							else
							{
#if !NET_2_0 && !NET_2_0_SUBSET
								GetOperationSuccessEvent?.Invoke(JsonConvert.DeserializeObject<Operation>(response.Response));
#else
								if (GetOperationSuccessEvent != null)
								{
									GetOperationSuccessEvent(JsonConvert.DeserializeObject<Operation>(response.Response));
								}
#endif
							}
						}
						break;
					default: break;
				}
			}
		}
	}
}