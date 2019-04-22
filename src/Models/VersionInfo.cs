using System;
using System.Collections.Generic;

namespace MobApps.Models
{
    public class VersionInfo
    {
        public string LatestVersion { get; set; }

        public Uri Url { get; set; }

        public IReadOnlyCollection<string> ReleaseNotes { get; set; }
    }
}
