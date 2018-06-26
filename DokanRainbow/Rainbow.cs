namespace DokanRainbow
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.AccessControl;
    using System.Text;
    using DokanNet;
    using DokanRainbow.Sitecore;
    using global::Rainbow.Storage.Sc;
    using global::Rainbow.Storage.Yaml;
    using global::Sitecore;
    using Convert = System.Convert;
    using FileAccess = DokanNet.FileAccess;

    internal class Rainbow : IDokanOperations
    {
        private readonly ItemServiceClient itemServiceClient;
        private readonly RainbowFormatterService rainbowFormatterService;

        public Rainbow(string instanceUrl, string domain, string userName, string password, string databaseName)
        {
            this.itemServiceClient = new ItemServiceClient(instanceUrl, domain, userName, password, databaseName);
            this.rainbowFormatterService = new RainbowFormatterService();
        }

        public NtStatus CreateFile(string fileName, FileAccess access, FileShare share, FileMode mode, FileOptions options,
            FileAttributes attributes, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public void Cleanup(string fileName, DokanFileInfo info)
        {
        }

        public void CloseFile(string fileName, DokanFileInfo info)
        {
        }

        public NtStatus ReadFile(string fileName, byte[] buffer, out int bytesRead, long offset, DokanFileInfo info)
        {
            var itemPath = PathUtil.GetItemPath(fileName);
            var items = this.itemServiceClient.GetItemInAllLanguages(itemPath.path);
            if (items != null && fileName.EndsWith(".yml"))
            {
                var memoryStream = rainbowFormatterService.GetRainbowContents(items, this.itemServiceClient.DatabaseName);

                int count = Convert.ToInt32(Math.Min(memoryStream.Length, buffer.Length) - offset);
                int read = count > 0 ? memoryStream.Read(buffer, Convert.ToInt32(offset), count) : 0;
                bytesRead = read;
                return NtStatus.Success;
            }

            bytesRead = 0;
            return NtStatus.FileInvalid;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, DokanFileInfo info)
        {
            bytesWritten = 0;
            return NtStatus.AccessDenied;
        }

        public NtStatus FlushFileBuffers(string fileName, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus GetFileInformation(string fileName, out FileInformation fileInfo, DokanFileInfo info)
        {
            fileInfo = new FileInformation();
            var itemPath = PathUtil.GetItemPath(fileName);
            dynamic item = this.itemServiceClient.GetItem(itemPath.path);
            if (item != null)
            {
                if (fileName.EndsWith(".yml"))
                {
                    fileInfo = new FileInformation()
                        {
                            FileName = $"{item.ItemName?.Value}.yml",
                            CreationTime = DateUtil.IsoDateToDateTime(item.__Created?.ToString()),
                            Attributes = FileAttributes.Normal,
                            LastWriteTime = DateUtil.IsoDateToDateTime(item.__Updated?.ToString()),
                            Length = item.ToString().ToCharArray().Length
                        };
                }
                else
                {
                    fileInfo = new FileInformation()
                        {
                            FileName = item.ItemName?.Value,
                            CreationTime = DateUtil.IsoDateToDateTime(item.__Created?.ToString()),
                            Attributes = FileAttributes.Directory,
                            LastWriteTime = DateUtil.IsoDateToDateTime(item.__Updated?.ToString()),
                            Length = item.ToString().ToCharArray().Length
                        };
                }
            }

            return NtStatus.Success;
        }

        public NtStatus FindFiles(string fileName, out IList<FileInformation> files, DokanFileInfo info)
        {
            return this.FindFilesWithPattern(fileName, "*", out files, info);
        }

        private List<string> logs = new List<string>();

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, DokanFileInfo info)
        {
            files = new List<FileInformation>();

            var itemPath = PathUtil.GetItemPath(fileName, searchPattern);
            if (itemPath.childrenOf)
            {
                var children = this.itemServiceClient.GetChildren(itemPath.path);
                foreach (dynamic child in children)
                {
                    files.Add(new FileInformation()
                    {
                        FileName = child.ItemName?.Value,
                        CreationTime = DateUtil.IsoDateToDateTime(child.__Created?.ToString()),
                        Attributes = FileAttributes.Directory,
                        LastWriteTime = DateUtil.IsoDateToDateTime(child.__Updated?.ToString()),
                        Length = child.ToString().ToCharArray().Length
                    });
                    files.Add(new FileInformation()
                    {
                        FileName = $"{child.ItemName?.Value}.yml",
                        CreationTime = DateUtil.IsoDateToDateTime(child.__Created?.ToString()),
                        Attributes = FileAttributes.Normal,
                        LastWriteTime = DateUtil.IsoDateToDateTime(child.__Updated?.ToString()),
                        Length = child.ToString().ToCharArray().Length
                    });
                }
            }
            else
            {
                dynamic item = this.itemServiceClient.GetItem(itemPath.path);
                if (item != null)
                {
                    if (searchPattern.EndsWith(".yml"))
                    {
                        files.Add(new FileInformation()
                        {
                            FileName = $"{item.ItemName?.Value}.yml",
                            CreationTime = DateUtil.IsoDateToDateTime(item.__Created?.ToString()),
                            Attributes = FileAttributes.Normal,
                            LastWriteTime = DateUtil.IsoDateToDateTime(item.__Updated?.ToString()),
                            Length = item.ToString().ToCharArray().Length
                        });
                    }
                    else
                    {
                        files.Add(new FileInformation()
                        {
                            FileName = item.ItemName?.Value,
                            CreationTime = DateUtil.IsoDateToDateTime(item.__Created?.ToString()),
                            Attributes = FileAttributes.Directory,
                            LastWriteTime = DateUtil.IsoDateToDateTime(item.__Updated?.ToString()),
                            Length = item.ToString().ToCharArray().Length
                        });
                    }
                }
            }

            return NtStatus.Success;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, DokanFileInfo info)
        {
            return NtStatus.AccessDenied;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime,
            DokanFileInfo info)
        {
            return NtStatus.AccessDenied;
        }

        public NtStatus DeleteFile(string fileName, DokanFileInfo info)
        {
            return NtStatus.AccessDenied;
        }

        public NtStatus DeleteDirectory(string fileName, DokanFileInfo info)
        {
            return NtStatus.AccessDenied;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, DokanFileInfo info)
        {
            return NtStatus.AccessDenied;
        }

        public NtStatus SetEndOfFile(string fileName, long length, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus SetAllocationSize(string fileName, long length, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus LockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus GetDiskFreeSpace(out long freeBytesAvailable, out long totalNumberOfBytes, out long totalNumberOfFreeBytes,
            DokanFileInfo info)
        {
            freeBytesAvailable = 0;
            totalNumberOfBytes = 0;
            totalNumberOfFreeBytes = 0;
            return NtStatus.Success;
        }

        public NtStatus GetVolumeInformation(out string volumeLabel, out FileSystemFeatures features, out string fileSystemName,
            out uint maximumComponentLength, DokanFileInfo info)
        {
            volumeLabel = $"{this.itemServiceClient.DatabaseName} on {this.itemServiceClient.HostName}";
            fileSystemName = "Sitecore";
            features = FileSystemFeatures.ReadOnlyVolume;
            maximumComponentLength = 0;
            return NtStatus.Success;
        }

        public NtStatus GetFileSecurity(string fileName, out FileSystemSecurity security, AccessControlSections sections,
            DokanFileInfo info)
        {
            security = null;
            return NtStatus.Success;
        }

        public NtStatus SetFileSecurity(string fileName, FileSystemSecurity security, AccessControlSections sections,
            DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus Mounted(DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus Unmounted(DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus FindStreams(string fileName, out IList<FileInformation> streams, DokanFileInfo info)
        {
            streams = null;
            return NtStatus.Success;
        }
    }
}
