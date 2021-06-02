using Backtrace.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BacktraceManager : MonoBehaviour
{
    private const string BacktraceGameObjectName = "Backtrace";
    private const string BacktraceDatabasePath = "${Application.persistentDataPath}/backtrace";
    private const string BacktraceSubmissionUrl = "";

    private readonly Dictionary<string, string> _attributes = new Dictionary<string, string>()
    {
        {"author", "Backtrace" },
        { "device.name", "devicename"},
        { "project-name", "Backtrace demo project" },
    };

    void Awake()
    {
        var client = GameObject.Find(BacktraceGameObjectName)?.GetComponent<BacktraceClient>();
        if (client == null)
        {
            if (string.IsNullOrEmpty(BacktraceSubmissionUrl))
            {
                throw new ArgumentException("Please set BacktraceSubmissionUrl value in the BacktraceManager class.");
            }
            client = BacktraceClient.Initialize(
                 url: BacktraceSubmissionUrl,
                 databasePath: BacktraceDatabasePath,
                 attributes: _attributes,
                 gameObjectName: BacktraceGameObjectName);
            Debug.Log("Backtrace-manager: Initialized Backtrace integration via API");
        }
        else
        {
            client.SetAttributes(_attributes);
            Debug.Log("Backtrace-manager: Initialized Backtrace integration via GameObject");
        }
    }
}
