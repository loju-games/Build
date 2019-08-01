using UnityEngine;

namespace Loju.Build
{

    [System.Serializable]
    public sealed class BuildInfo
    {

        public string version { get { return _version; } }
        public string buildNumber { get { return _buildNumber; } }
        public string platformName { get { return _platformName; } }

        [SerializeField] private string _version;
        [SerializeField] private string _buildNumber;
        [SerializeField] private string _platformName;

        public BuildInfo(string version, string buildNumber, string platformName)
        {
            _version = version;
            _buildNumber = buildNumber;
            _platformName = platformName;
        }

    }
}