using System.IO;
using UnityEngine;

public class LogToFile : MonoBehaviour
{
    private static LogToFile instance;

    private string logFilePath;

    private void Awake()
    {
        // シングルトンの実装
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいで削除されないようにする
            InitializeLogFile();          // 初期化
        }
        else
        {
            Destroy(gameObject); // 二重作成を防ぐ
        }
    }

    /// <summary>
    /// ログファイルの初期化
    /// </summary>
    private void InitializeLogFile()
    {
        //Debug.Log($"Log file path: {Application.persistentDataPath}/Logs/");

        string folderPath = Path.Combine(Application.persistentDataPath, "Logs");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath); // フォルダが存在しなければ作成
        }

        logFilePath = Path.Combine(folderPath, $"log_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt");

        // 初期メッセージを記録
        AppendToLogFile("=== Log Start ===");
    }

    /// <summary>
    /// ログを追記する
    /// </summary>
    /// <param name="message">記録するメッセージ</param>
    public void AppendToLogFile(string message)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"[{System.DateTime.Now:HH:mm:ss}] {message}");
        }
    }

    /// <summary>
    /// Unityコンソールログのキャプチャ
    /// </summary>
    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog; // Unityのログをキャプチャ
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog; // キャプチャを解除
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"{type}: {logString}";
        if (type == LogType.Exception || type == LogType.Error)
        {
            logEntry += $"\n{stackTrace}";
        }

        AppendToLogFile(logEntry);
    }
}
