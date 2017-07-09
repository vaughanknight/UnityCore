using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NugetQuery
{
    public class Version
    {
        public string version { get; set; }
        public int downloads { get; set; }
    }

    public class Datum
    {
        public string registration { get; set; }
        public string id { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public string summary { get; set; }
        public string title { get; set; }
        public string licenseUrl { get; set; }
        public string projectUrl { get; set; }
        public List<object> tags { get; set; }
        public List<string> authors { get; set; }
        public int totalDownloads { get; set; }
        public List<Version> versions { get; set; }
        public string iconUrl { get; set; }
    }

    public class RootObject
    {
        public int totalHits { get; set; }
        public string lastReopen { get; set; }
        public string index { get; set; }
        public List<Datum> data { get; set; }
    }
}