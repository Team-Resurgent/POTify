using Mono.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace POTify
{
    internal class Program
    {
        private const string Version = "1.0.1";

        private static bool ShowHelp = false;
        private static string Input = string.Empty;
        private static string Output = string.Empty;

        private static int CeilToNextPowerOfTwo(int value)
        {
            int powOfTwo = 1;
            while (powOfTwo < value)
            {
                powOfTwo <<= 1;
            }
            return powOfTwo;
        }

        static void Main(string[] args)
        {
            var options = new OptionSet {
                { "i|input=", $"Input folder", i => Input = i },
                { "o|output=", $"Input folder", o => Output = o },
                { "h|help", "show help", h => ShowHelp = h != null },
            };

            try
            {
                options.Parse(args);
                if (ShowHelp && args.Length == 1)
                {
                    ConsoleUtil.ShowHelpHeader(Version, options);
                    return;
                }

                var input = Path.GetFullPath(Input);
                if (!Directory.Exists(input))
                {
                    throw new OptionException("Input is not a valid folder.", "input");
                }

                var output = Path.GetFullPath(Output);
                if (!Directory.Exists(output))
                {
                    throw new OptionException("Output is not a valid folder.", "output");
                }

                var acceptedFiletypes = new string[] { ".png", ".jpg" };
                var processFiles = Directory.GetFileSystemEntries(input, "*", new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = false, MatchCasing = MatchCasing.CaseInsensitive })
                    .Where(file => acceptedFiletypes.Contains(Path.GetExtension(file), StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                foreach ( var file in processFiles)
                {
                    var filename = Path.GetFileName(file);
                    Console.WriteLine($"Processing file '{filename}'");
                    var outputFile = Path.Combine(output, filename);
                    using var image = Image.Load<Rgba32>(file);
                    var newWidth = CeilToNextPowerOfTwo(image.Width);
                    var newHeight = CeilToNextPowerOfTwo(image.Height);
                    image.Mutate(x => { x.Resize(newWidth, newHeight); });
                    image.Save(outputFile);
                }
            }
            catch (OptionException e)
            {
                ConsoleUtil.ShowOptionException(e);
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Done.");
        }
    }
}