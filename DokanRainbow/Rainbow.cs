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
    using FileAccess = DokanNet.FileAccess;

    internal class Rainbow : IDokanOperations
    {
        private readonly ItemServiceClient itemServiceClient;

        public Rainbow(string instanceUrl, string domain, string userName, string password)
        {
            this.itemServiceClient = new ItemServiceClient(instanceUrl, domain, userName, password);
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
            dynamic item = this.itemServiceClient.GetItem(itemPath.path);
            if (item != null && fileName.EndsWith(".yml"))
            {
                var memoryStream = new MemoryStream();
                var itemData = new ItemServiceItemData(item)
                    {
                        DatabaseName = "core"
                    };
                new YamlSerializationFormatter(null, null).WriteSerializedItem(itemData, memoryStream);

                memoryStream.Position = 0;
                int read = memoryStream.Read(buffer, Convert.ToInt32(offset), Convert.ToInt32(memoryStream.Length - offset));
                bytesRead = read;
                return NtStatus.Success;
            }

            bytesRead = 0;
            return NtStatus.FileInvalid;
        }

        public NtStatus WriteFile(string fileName, byte[] buffer, out int bytesWritten, long offset, DokanFileInfo info)
        {
            bytesWritten = 0;
            return NtStatus.Success;
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
                            CreationTime = item.__Created,
                            Attributes = FileAttributes.Normal,
                            LastWriteTime = item.__Updated,
                            Length = item.ToString().ToCharArray().Length
                        };
                }
                else
                {
                    fileInfo = new FileInformation()
                        {
                            FileName = item.ItemName?.Value,
                            CreationTime = item.__Created,
                            Attributes = FileAttributes.Directory,
                            LastWriteTime = item.__Updated,
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
                        CreationTime = child.__Created,
                        Attributes = FileAttributes.Directory,
                        LastWriteTime = child.__Updated,
                        Length = child.ToString().ToCharArray().Length
                    });
                    files.Add(new FileInformation()
                    {
                        FileName = $"{child.ItemName?.Value}.yml",
                        CreationTime = child.__Created,
                        Attributes = FileAttributes.Normal,
                        LastWriteTime = child.__Updated,
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
                            CreationTime = item.__Created,
                            Attributes = FileAttributes.Normal,
                            LastWriteTime = item.__Updated,
                            Length = item.ToString().ToCharArray().Length
                        });
                    }
                    else
                    {
                        files.Add(new FileInformation()
                        {
                            FileName = item.ItemName?.Value,
                            CreationTime = item.__Created,
                            Attributes = FileAttributes.Directory,
                            LastWriteTime = item.__Updated,
                            Length = item.ToString().ToCharArray().Length
                        });
                    }
                }
            }

            return NtStatus.Success;
        }

        public NtStatus SetFileAttributes(string fileName, FileAttributes attributes, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus SetFileTime(string fileName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime,
            DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus DeleteFile(string fileName, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus DeleteDirectory(string fileName, DokanFileInfo info)
        {
            return NtStatus.Success;
        }

        public NtStatus MoveFile(string oldName, string newName, bool replace, DokanFileInfo info)
        {
            return NtStatus.Success;
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
            volumeLabel = null;
            features = FileSystemFeatures.None;
            fileSystemName = null;
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
