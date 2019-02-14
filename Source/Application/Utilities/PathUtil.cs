using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using SystemInterface.IO;

namespace pdfforge.PDFCreator.Utilities
{
    public interface IPathUtil
    {
        // ReSharper disable once InconsistentNaming
        int MAX_PATH { get; }

        string ELLIPSIS { get; }

        string EllipsisForFilename(string filePath, int maxLength);

        string EllipsisForTooLongPath(string filePath);

        bool DirectoryIsEmpty(string path);

        bool CheckWritability(string directory);

        bool IsValidRootedPath(string path);

        PathUtilStatus IsValidRootedPathWithResponse(string path);

        bool IsValidFilename(string fileName);
    }

    public class PathUtil : IPathUtil
    {
        private readonly IDirectory _directory;
        private readonly IPath _path;

        public PathUtil(IPath path, IDirectory directory)
        {
            _path = path;
            _directory = directory;
        }

        public int MAX_PATH => 259;

        public string ELLIPSIS => "__";

        /// <summary>
        ///     Adds ellipsis to a path with a length longer than 255.
        /// </summary>
        /// <param name="filePath">full path to file</param>
        /// <param name="maxLength">maximum length of the string. This must be between 10 and MAX_PATH (260)</param>
        /// <returns>file path with ellipsis to ensure length under the max length </returns>
        public string EllipsisForFilename(string filePath, int maxLength)
        {
            if (filePath == null)
                throw new ArgumentNullException(nameof(filePath));

            if (filePath.EndsWith("\\"))
                throw new ArgumentException("The path has to be a file", nameof(filePath));

            if (maxLength < 10 || maxLength > MAX_PATH)
                throw new ArgumentException($"The desired length must be between 10 and {MAX_PATH}", nameof(maxLength));

            if (filePath.Length > maxLength)
            {
                int minUsefulFileLength = 4;

                var directory = PathSafe.GetDirectoryName(filePath) ?? "";
                var file = PathSafe.GetFileNameWithoutExtension(filePath);
                var extension = PathSafe.GetExtension(filePath);

                var remainingLengthForFile = maxLength - directory.Length - extension.Length - ELLIPSIS.Length - 1; // substract -1 to account for the slash between path and filename
                if (remainingLengthForFile < minUsefulFileLength)
                {
                    throw new ArgumentException("The path is too long", nameof(filePath)); //!
                }

                var partLength = remainingLengthForFile / 2;

                file = file.Substring(0, partLength) + ELLIPSIS + file.Substring(file.Length - partLength, partLength);
                filePath = PathSafe.Combine(directory, file + extension);
            }

            return filePath;
        }

        /// <summary>
        ///     Adds ellipsis to a path with a length longer than 255.
        /// </summary>
        /// <param name="filePath">full path to file</param>
        /// <returns>file path with ellipsis to ensure length under 255 </returns>
        public string EllipsisForTooLongPath(string filePath)
        {
            return EllipsisForFilename(filePath, MAX_PATH);
        }

        /// <summary>
        ///     Check if directory is writable.
        /// </summary>
        /// <param name="directory">Directory string or full file path</param>
        /// <returns>true if directory is writeable</returns>
        public bool CheckWritability(string directory)
        {
            directory = _path.GetFullPath(directory);

            var permissionSet = new PermissionSet(PermissionState.None);

            var fileIoPermission = new FileIOPermission(FileIOPermissionAccess.Write, directory);

            permissionSet.AddPermission(fileIoPermission);

            return permissionSet.IsSubsetOf(AppDomain.CurrentDomain.PermissionSet);
        }

        public bool DirectoryIsEmpty(string path)
        {
            return !_directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        ///     Checks if the given path is a (syntactically) valid rooted path, i.e. "C:\Temp\test.txt". This file is not required
        ///     to exist
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns>true, if the path is valid</returns>
        public bool IsValidRootedPath(string path)
        {
            return IsValidRootedPathWithResponse(path) == PathUtilStatus.Success;
        }

        public PathUtilStatus IsValidRootedPathWithResponse(string path)
        {
            if (string.IsNullOrEmpty(path))
                return PathUtilStatus.PathWasNullOrEmpty;

            if (path.Length < 3)
                return PathUtilStatus.InvalidRootedPath;

            if (((path.IndexOf(":", StringComparison.Ordinal) != 1) || (path.IndexOf("\\", StringComparison.Ordinal) != 2)) && !path.StartsWith(@"\\"))
                return PathUtilStatus.InvalidRootedPath;

            try
            {
                var fi = new FileInfo(path);
            }
            catch (ArgumentException)
            {
                return PathUtilStatus.ArgumentEx;
            }
            catch (NotSupportedException)
            {
                return PathUtilStatus.NotSupportedEx;
            }
            catch (PathTooLongException)
            {
                return PathUtilStatus.PathTooLongEx;
            }

            if (!path.StartsWith(@"\\"))
            {
                var driveLetter = char.ToUpperInvariant(path[0]);
                if (driveLetter >= 'A' && driveLetter <= 'Z')
                    return PathUtilStatus.Success;

                return PathUtilStatus.InvalidRootedPath;
            }

            return PathUtilStatus.Success;
        }

        //todo: Is this the same as valid rooted path?
        public bool IsValidFilename(string fileName)
        {
            Regex containsABadCharacter = new Regex("[" + Regex.Escape(new string(Path.GetInvalidFileNameChars())) + "]");

            if (containsABadCharacter.IsMatch(fileName))
                return false;

            return true;
        }
    }

    public enum PathUtilStatus
    {
        Success,
        PathWasNullOrEmpty,
        InvalidRootedPath,
        ArgumentEx,
        PathTooLongEx,
        NotSupportedEx,
    }
}
