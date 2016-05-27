// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.IO;

namespace Benchmarks.Utility.Helpers
{
    public class CommandLineRunner
    {
        private static readonly CommandLineRunner _default = new CommandLineRunner("cmd");

        private readonly string _command;

        public static CommandLineRunner GetDefaultInstance() => _default;

        public CommandLineRunner() : this("cmd") { }

        public CommandLineRunner(string command)
        {
            _command = command;

            Timeout = TimeSpan.FromMinutes(1);
        }

        public bool UseShellExecute { get; set; } = false;

        public bool RedirectOutput { get; set; }

        public string LastOutput { get; set; }

        public int LastExitCode { get; set; }

        public TimeSpan Timeout { get; set; }

        public int Execute(string arguments)
        {
            return Execute(arguments, null);
        }

        public int Execute(string arguments, string workingDirectory)
        {
            var startinfo = new ProcessStartInfo(_command, ProcessArguments(arguments))
            {
                UseShellExecute = UseShellExecute,
                RedirectStandardOutput = RedirectOutput,
                WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory()
            };

            var proc = Process.Start(startinfo);
            var exit = proc.WaitForExit((int)Timeout.TotalMilliseconds);

            LastExitCode = proc.ExitCode;

            if (!UseShellExecute && RedirectOutput)
            {
                LastOutput = proc.StandardOutput.ReadToEnd();
            }

            return exit ? LastExitCode : -1;
        }

        private string ProcessArguments(string arguments)
        {
            if (_command == "cmd")
            {
                return $"/C \"{arguments}\"";
            }
            else
            {
                return arguments;
            }
        }
    }
}
