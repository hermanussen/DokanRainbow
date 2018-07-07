namespace DokanRainbow
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.AccessControl;
    using DokanNet;
    using DokanRainbow.Sitecore;
    using global::Sitecore;
    using Convert = System.Convert;
    using FileAccess = DokanNet.FileAccess;

    /// <summary>
    /// Implements the interface that is needed to implement operations in Dokany.
    /// </summary>
    internal class Rainbow : IDokanOperations
    {
        /// <summary>
        /// Used for getting items from Sitecore.
        /// </summary>
        private readonly ItemServiceClient itemServiceClient;

        /// <summary>
        /// Used for formatting Sitecore data into the Rainbow file format.
        /// </summary>
        private readonly RainbowFormatterService rainbowFormatterService;

        public Rainbow(string instanceUrl, string domain, string userName, string password, string databaseName, int cacheTimeSeconds)
        {
            this.itemServiceClient = new ItemServiceClient(instanceUrl, domain, userName, password, databaseName, cacheTimeSeconds);
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

        public NtStatus FindFilesWithPattern(string fileName, string searchPattern, out IList<FileInformation> files, DokanFileInfo info)
        {
            files = new List<FileInformation>();

            var itemPath = PathUtil.GetItemPath(fileName, searchPattern);
            if (itemPath.childrenOf)
            {
                // Return all children as files/folders
                var children = this.itemServiceClient.GetChildren(itemPath.path);
                foreach (dynamic child in children)
                {
                    // If there are no children, than it's pointless to offer a folder to go deeper
                    if (bool.TrueString.Equals(child.HasChildren?.Value, StringComparison.InvariantCultureIgnoreCase))
                    {
                        // Add the directory, so child items can be accessed
                        files.Add(new FileInformation()
                        {
                            FileName = child.ItemName?.Value,
                            CreationTime = DateUtil.IsoDateToDateTime(child.__Created?.ToString()),
                            Attributes = FileAttributes.Directory,
                            LastWriteTime = DateUtil.IsoDateToDateTime(child.__Updated?.ToString()),
                            Length = child.ToString().ToCharArray().Length
                        });
                    }

                    // Add the Rainbow YAML file
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
