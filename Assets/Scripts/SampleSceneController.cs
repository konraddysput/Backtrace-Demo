using Backtrace.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using UnityEngine;

public class SampleSceneController : MonoBehaviour
{
    private BacktraceClient _client;
    void Start()
    {
        _client = BacktraceClient.Instance;
    }
    public void UnhandledException()
    {
        Debug.Log("throwing an unhandled exception");
        ThrowException();
    }

    public void BackgroundThreadException()
    {
        Debug.LogWarning("Throwing an unhandled exception from background thread");
        new Thread(() =>
        {
            SampleSceneController test = null;
            test.GetComponent("foo");

        }).Start();
    }

    public void HandledException()
    {
        Debug.Log("Sending handled exception to Backtrace");
        try
        {
            ThrowException();
        }
        catch (Exception e)
        {
            _client.Send(e, attributes: new Dictionary<string, string>() { { "time", Time.unscaledTime.ToString(CultureInfo.InvariantCulture) } });
        }
    }

    public void ANR()
    {
        const int anrTimeInMs = 11000;
        Thread.Sleep(anrTimeInMs);
    }

    public IEnumerator Oom()
    {
        Debug.LogWarning("Starting OOM. ");
        Debug.LogWarning("Application should ");
        var textures = new List<Texture2D>();
        while (true)
        {
            var texture = new Texture2D(512, 512, TextureFormat.ARGB32, true);
            texture.Apply();
            textures.Add(texture);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    public void Crash()
    {
        Debug.LogError("Forcing game crash.");
#if UNITY_EDITOR
        Debug.LogError("Preventing game crash in the Editor mode. Otherwise your editor will crash. Please run game in production mode on the physical device");
#else
        UnityEngine.Diagnostics.Utils.ForceCrash(UnityEngine.Diagnostics.ForcedCrashCategory.AccessViolation);
#endif
    }

    public void ExitOnException()
    {
        Debug.Log("Quitting application on the next game exception");
        Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
        {
            if (type == LogType.Exception)
            {
                Application.Quit(1);
            }
        };
        throw new Exception("Force application exit via Exception flow");
    }

    public void Exit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Use this function to extend stack trace information
    /// </summary>
    private void ThrowException()
    {
        ReadFile();
    }
    private void ReadFile()
    {
        System.IO.File.ReadAllBytes("path/to/not/existing/file");
    }
}
