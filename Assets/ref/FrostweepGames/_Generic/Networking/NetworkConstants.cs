namespace FrostweepGames.Plugins.Networking
{
    public class NetworkConstants
    {
        public const string PACKAGE_NAME = "com.companyname.appname";
        public const string KEY_SIGNATURE = "key-fingerprint";

        // WARNING! WWW class doesnt support DELETE method. Has LIMITATIONS in API. PLEASE USE WEB_REQUEST in RELEASE
        public const NetworkEnumerators.NetworkMethod NETWORK_METHOD = NetworkEnumerators.NetworkMethod.WEB_REQUEST;
    }
}