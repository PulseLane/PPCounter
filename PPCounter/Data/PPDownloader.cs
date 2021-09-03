using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using static PPCounter.Utilities.Structs;

namespace PPCounter.Data
{
    class PPDownloader : MonoBehaviour
    {
        private const string URI_PREFIX = "https://cdn.pulselane.dev/";
        private const string FILE_NAME = "raw_pp.json";
        public Action<Dictionary<string, RawPPData>> OnDataDownloaded;
        public Action OnError;

        public void StartDownloading()
        {
            StartCoroutine(GetRawPP());
        }

        IEnumerator GetRawPP()
        {
            string uri = URI_PREFIX + FILE_NAME;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
                // Request and wait for the desired page.
                Logger.log.Debug("Downloading pp data...");
                yield return webRequest.SendWebRequest();
                if (webRequest.isNetworkError)
                {
                    OnError?.Invoke();
                    Logger.log.Error($"Error downloading pp data: {webRequest.error}");
                    throw new WebException();
                }
                else
                {
                    try
                    {
                        var json = JsonConvert.DeserializeObject<Dictionary<string, RawPPData>>(webRequest.downloadHandler.text);
                        OnDataDownloaded?.Invoke(json);
                    }

                    catch (Exception e)
                    {
                        OnError?.Invoke();
                        Logger.log.Error($"Error processing json: {e.Message}");
                    }
                }
            }
        }
    }
}
