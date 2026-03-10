using System;
using System.IO;
using Avalonia.Threading;

namespace ArisenEditorFramework.Utilities;

public partial class ArisenFileSystemWatcher : IDisposable
{
    public static Action<object, FileSystemEventArgs>? Changed;
    public static Action<object, RenamedEventArgs>? Renamed;
    public static Action<object, FileSystemEventArgs>? Created;
    public static Action<object, FileSystemEventArgs>? Deleted;
    public static Action<object, ErrorEventArgs>? Errored;

    private FileSystemWatcher? m_Watcher;

    public ArisenFileSystemWatcher(string path)
    {
        if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

        m_Watcher = new FileSystemWatcher()
        {
            Path = path,
            NotifyFilter = NotifyFilters.Attributes
                         | NotifyFilters.Security
                         | NotifyFilters.Size
                         | NotifyFilters.CreationTime
                         | NotifyFilters.DirectoryName
                         | NotifyFilters.FileName
                         | NotifyFilters.LastAccess
                         | NotifyFilters.LastWrite
        };

        m_Watcher.IncludeSubdirectories = true;
        m_Watcher.Changed += (s, e) => Dispatcher.UIThread.Post(() => Changed?.Invoke(s, e));
        m_Watcher.Renamed += (s, e) => Dispatcher.UIThread.Post(() => Renamed?.Invoke(s, e));
        m_Watcher.Deleted += (s, e) => Dispatcher.UIThread.Post(() => Deleted?.Invoke(s, e));
        m_Watcher.Created += (s, e) => Dispatcher.UIThread.Post(() => Created?.Invoke(s, e));
        m_Watcher.Error += (s, e) => Dispatcher.UIThread.Post(() => Errored?.Invoke(s, e));
        
        m_Watcher.EnableRaisingEvents = true;
    }

    public void Dispose()
    {
        if (m_Watcher != null)
        {
            m_Watcher.EnableRaisingEvents = false;
            m_Watcher.Dispose();
            m_Watcher = null;
        }
    }
}
