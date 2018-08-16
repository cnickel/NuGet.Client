// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using NuGet.Common;
using NuGet.Shared;

namespace NuGet.ProjectModel
{
    public class RestoreLockProperties : IEquatable<RestoreLockProperties>
    {
        /// <summary>
        /// Set when customer wants to opt into packages lock file
        /// </summary>
        public string RestorePackagesWithLockFile { get; }

        /// <summary>
        /// Packages.lock.json file path including file name if customer wants to override defualt file name.
        /// </summary>
        public string NuGetLockFilePath { get; }

        /// <summary>
        /// True, if updating lock file on restore is denied.
        /// </summary>
        public bool RestoreLockedMode { get; }

        /// <summary>
        /// if true, then restore will ignore existing lock file and regenerate restore graph.
        /// And if needed then overwrite the existing lock file.
        /// </summary>
        public bool ReevaluateNuGetLockFile { get; }

        public RestoreLockProperties()
        {
        }

        public RestoreLockProperties(
            string restorePackagesWithLockFile,
            string nuGetLockFilePath,
            bool restoreLockedMode,
            bool reevaluateNuGetLockFile)
        {
            RestorePackagesWithLockFile = restorePackagesWithLockFile;
            NuGetLockFilePath = nuGetLockFilePath;
            RestoreLockedMode = restoreLockedMode;
            ReevaluateNuGetLockFile = reevaluateNuGetLockFile;
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCodeCombiner();

            hashCode.AddObject(RestorePackagesWithLockFile);
            hashCode.AddObject(NuGetLockFilePath);
            hashCode.AddObject(RestoreLockedMode);
            hashCode.AddObject(ReevaluateNuGetLockFile);

            return hashCode.CombinedHash;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RestoreLockProperties);
        }

        public bool Equals(RestoreLockProperties other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return StringComparer.OrdinalIgnoreCase.Equals(RestorePackagesWithLockFile, other.RestorePackagesWithLockFile) &&
                PathUtility.GetStringComparerBasedOnOS().Equals(NuGetLockFilePath, other.NuGetLockFilePath) &&
                RestoreLockedMode == other.RestoreLockedMode &&
                ReevaluateNuGetLockFile == other.ReevaluateNuGetLockFile;
        }

        public RestoreLockProperties Clone()
        {
            return new RestoreLockProperties(RestorePackagesWithLockFile, NuGetLockFilePath, RestoreLockedMode, ReevaluateNuGetLockFile);
        }
    }
}
