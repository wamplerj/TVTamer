using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace TvCleaner
{
    public class Arguments
    {

        [Option('s', "source", Required = true, HelpText= "Source Folder: the location where tv episodes are downloaded to")]
        public string SourceFolder { get; set; }

        [Option('d', "destination", Required = true, HelpText = "Destination Folder: the location where tv episodes are perminently located")]
        public string DestinationFolder { get; set; }

        [Option("dry-run", Required = false, DefaultValue = false, HelpText = "Dry Run: Performs operations without moving, copying or deleting files")]
        public bool DryRun { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText();

            if (this.LastParserState.Errors.Any())
            {
                var errors = help.RenderParsingErrorsText(this, 2); // indent with two spaces

                if (!string.IsNullOrEmpty(errors))
                {
                    help.AddPreOptionsLine(string.Concat(Environment.NewLine, "ERROR(S):"));
                    help.AddPreOptionsLine(errors);
                }
            }

            // ...
            return help;
        }

    }
}
