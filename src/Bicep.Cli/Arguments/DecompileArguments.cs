// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO;
using Bicep.Core.FileSystem;
using LanguageConstants = Bicep.Core.LanguageConstants;

namespace Bicep.Cli.Arguments
{
    public class DecompileArguments : ArgumentsBase
    {
        public DecompileArguments(string[] args) : base(Constants.Command.Decompile)
        {
            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLowerInvariant())
                {
                    case "--stdout":
                        OutputToStdOut = true;
                        break;
                    case "--force":
                        AllowOverwrite = true;
                        break;
                    case "--outdir":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outdir parameter expects an argument");
                        }
                        if (OutputDir is not null)
                        {
                            throw new CommandLineException($"The --outdir parameter cannot be specified twice");
                        }
                        OutputDir = args[i + 1];
                        i++;
                        break;
                    case "--outfile":
                        if (args.Length == i + 1)
                        {
                            throw new CommandLineException($"The --outfile parameter expects an argument");
                        }
                        if (OutputFile is not null)
                        {
                            throw new CommandLineException($"The --outfile parameter cannot be specified twice");
                        }
                        OutputFile = args[i + 1];
                        i++;
                        break;
                    default:
                        if (args[i].StartsWith("--"))
                        {
                            throw new CommandLineException($"Unrecognized parameter \"{args[i]}\"");
                        }
                        if (InputFile is not null)
                        {
                            throw new CommandLineException($"The input file path cannot be specified multiple times");
                        }
                        InputFile = args[i];
                        break;
                }
            }

            if (InputFile is null)
            {
                throw new CommandLineException($"The input file path was not specified");
            }

            if (OutputToStdOut && OutputDir is not null)
            {
                throw new CommandLineException($"The --outdir and --stdout parameters cannot both be used");
            }
            if (OutputToStdOut && OutputFile is not null)
            {
                throw new CommandLineException($"The --outfile and --stdout parameters cannot both be used");
            }

            if (OutputDir is not null && OutputFile is not null)
            {
                throw new CommandLineException($"The --outdir and --outfile parameters cannot both be used");
            }

            if (OutputDir is not null)
            {
                var outputDir = PathHelper.ResolvePath(OutputDir);

                if (!Directory.Exists(outputDir))
                {
                    throw new CommandLineException(string.Format(CliResources.DirectoryDoesNotExistFormat, outputDir));
                }
            }

            if (!OutputToStdOut && !AllowOverwrite)
            {
                string outputFilePath = Path.ChangeExtension(PathHelper.ResolvePath(InputFile), LanguageConstants.LanguageFileExtension);
                if (File.Exists(outputFilePath))
                {
                    throw new CommandLineException($"The output path \"{outputFilePath}\" already exists. Use --force to overwrite the existing file.");
                }

            }
        }

        public bool OutputToStdOut { get; }

        public string InputFile { get; }

        public string? OutputDir { get; }

        public string? OutputFile { get; }

        public bool AllowOverwrite { get; }
    }
}
