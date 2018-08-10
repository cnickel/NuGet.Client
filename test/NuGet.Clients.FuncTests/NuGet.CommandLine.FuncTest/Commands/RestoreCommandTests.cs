// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NuGet.CommandLine.Test;
using NuGet.Frameworks;
using NuGet.ProjectModel;
using NuGet.Test.Utility;
using Xunit;

namespace NuGet.CommandLine.FuncTest.Commands
{
    public class RestoreCommandTests
    {
        [Fact]
        public async Task Restore_LegacyPackageReference_WithNuGetLockFile()
        {
            // Arrange
            using (var pathContext = new SimpleTestPathContext())
            {
                // Set up solution, project, and packages
                var solution = new SimpleTestSolutionContext(pathContext.SolutionRoot);

                var net45 = NuGetFramework.Parse("net45");

                var projectA = SimpleTestProjectContext.CreateLegacyPackageReference(
                    "a",
                    pathContext.SolutionRoot,
                    net45);

                projectA.Properties.Add("RestorePackagesWithLockFile", "true");

                var packageX = new SimpleTestPackageContext()
                {
                    Id = "x",
                    Version = "1.0.0"
                };

                projectA.AddPackageToAllFrameworks(packageX);
                solution.Projects.Add(projectA);
                solution.Create(pathContext.SolutionRoot);

                await SimpleTestPackageUtility.CreateFolderFeedV3Async(
                    pathContext.PackageSource,
                    packageX);

                var args = new string[]
                {
                    projectA.ProjectPath,
                    "-Source",
                    pathContext.PackageSource
                };

                // Act
                var result = RunRestore(pathContext, expectedExitCode: 1, additionalArgs: args);

                // Assert
                Assert.True(File.Exists(projectA.NuGetLockFileOutputPath));

                var lockFile = NuGetLockFileFormat.Read(projectA.NuGetLockFileOutputPath);
                Assert.Equal(1, lockFile.Targets.Count);

                var target = lockFile.Targets.First();
                Assert.Equal(".NETFramework,Version=v4.5", target.Name);
                Assert.Equal(1, target.Dependencies.Count);
                Assert.Equal("x", target.Dependencies[0].Id);
            }
        }

        public static CommandRunnerResult RunRestore(SimpleTestPathContext pathContext, int expectedExitCode = 0, params string[] additionalArgs)
        {
            var nugetExe = Util.GetNuGetExePath();

            // Store the dg file for debugging
            var envVars = new Dictionary<string, string>()
            {
                { "NUGET_HTTP_CACHE_PATH", pathContext.HttpCacheFolder }
            };

            var args = new string[]
            {
                "restore",
                "-Verbosity",
                "detailed"
            };

            args = args.Concat(additionalArgs).ToArray();

            // Act
            var r = CommandRunner.Run(
                nugetExe,
                pathContext.WorkingDirectory,
                string.Join(" ", args),
                waitForExit: true,
                environmentVariables: envVars);

            // Assert
            Assert.True(expectedExitCode == r.ExitCode, r.AllOutput);

            return r;
        }
    }
}
