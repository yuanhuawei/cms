using System;
using System.IO;

namespace MaiDarServer.Plugin.Features
{
    public interface IFileSystem : IPlugin
    {
        Action<object, FileSystemEventArgs> OnFileSystemChanged { get; }
    }
}
