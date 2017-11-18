// Copyright (c) Jeff Rebacz
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TfsCodeReviewCloser
{
    class Program
    {
        private class ProgramOptions
        {
            [CommandLine.Option('s', "server", DefaultValue = "http://tfs:8080/tfs", HelpText = "URL of Team Foundation Server")]
            public string TfsUrl { get; set; }

            [CommandLine.Option('u', "user", Required = true, HelpText = "TFS user name (for finding your code review and setting \"Closed By\")")]
            public string TfsUserName { get; set; }

            public static ProgramOptions Singleton
            {
                get { return LazyInitialized.Value; }
            }
            private static readonly Lazy<ProgramOptions> LazyInitialized = new Lazy<ProgramOptions>(() => new ProgramOptions());
        }

        static void Main(string[] args)
        {
            try
            {
                if (!CommandLine.Parser.Default.ParseArguments(args, ProgramOptions.Singleton))
                {
                    Console.WriteLine(CommandLine.Text.HelpText.AutoBuild(ProgramOptions.Singleton).ToString());
                    return;
                }

                CodeReviewCloser reviewCloser = new CodeReviewCloser(ProgramOptions.Singleton.TfsUrl, ProgramOptions.Singleton.TfsUserName);
                reviewCloser.CloseCompleted();
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }
    }
}
