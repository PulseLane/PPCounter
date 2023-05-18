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
        private const string ACCSABER_URL = "https://api.accsaber.com/";
        private const string PP_FILE_NAME = "raw_pp.json";
        private const string CURVE_FILE_NAME = "curves.json";
        private const string ACCSABER_RANKED_MAPS = "ranked-maps";
        public Action<Dictionary<string, RawPPData>> OnSSDataDownloaded;
        public Action<List<AccSaberRankedMap>> OnAccSaberDataDownloaded;
        public Action<Leaderboards> OnCurvesDownloaded;
        public Action OnError;

        public void StartDownloadingCurves()
        {
            GetCurves();
        }

        public void StartDownloadingSS()
        {
            GetRawSSPP();
        }

        public void StartDownloadingAccSaber()
        {
            GetAccSaberRankedMaps();
        }

        void GetRawSSPP()
        {
            string uri = URI_PREFIX + PP_FILE_NAME;
            StartCoroutine(MakeWebRequest(uri, OnSSDataDownloaded));
        }

        void GetAccSaberRankedMaps()
        {
            string uri = ACCSABER_URL + ACCSABER_RANKED_MAPS;
            StartCoroutine(MakeWebRequest(uri, OnAccSaberDataDownloaded));
        }

        void GetCurves()
        {
            string uri = URI_PREFIX + CURVE_FILE_NAME;
            StartCoroutine(MakeWebRequest(uri, OnCurvesDownloaded));
        }

        IEnumerator MakeWebRequest<T>(string uri, Action<T> OnDownloadComplete)
        {
            using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
            {
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
                        var json = JsonConvert.DeserializeObject<T>(webRequest.downloadHandler.text);

                        OnDownloadComplete?.Invoke(json);
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
