using Mono.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace POTify
{
    internal class Program
    {
        private const string Version = "1.0.2";

        private static bool ShowHelp = false;
        private static string Input = string.Empty;
        private static string Output = string.Empty;
        private static bool CreateOutput = false;
        private static bool RoundToNearest = false;

        public static int RoundToNextPowerOfTwo(int value)
        {
            int next = CeilToNextPowerOfTwo(value);
            int prev = next >> 1;
            return next - value <= value - prev ? next : prev;
        }


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
                { "o|output=", $"Output folder", o => Output = o },
                { "c|create", "Create output folder if does not exist", c => CreateOutput = c != null },
                { "n|nearest", "Round to nearest power of two", n => RoundToNearest = n != null },
                { "h|help", "Show help", h => ShowHelp = h != null },
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
                    if (CreateOutput)
                    {
                        Directory.CreateDirectory(output);
                    }
                    else
                    {
                        throw new OptionException("Output is not a valid folder.", "output");
                    }
                }

                var acceptedFiletypes = new string[] { ".png", ".jpg" };
                var processFiles = Directory.GetFileSystemEntries(input, "*", new EnumerationOptions { IgnoreInaccessible = true, RecurseSubdirectories = true, MatchCasing = MatchCasing.CaseInsensitive })
                    .Where(file => acceptedFiletypes.Contains(Path.GetExtension(file), StringComparer.CurrentCultureIgnoreCase))
                    .ToList();

                foreach ( var file in processFiles)
                {
                    var outputFolder = Path.GetDirectoryName(file.Replace(input, ""));
                    var filename = Path.GetFileName(file);
                    Console.WriteLine($"Processing file '{filename}'");
                    var outputPath = $"{output}{outputFolder}";
                    var outputFile = Path.Combine(outputPath, filename);
                    using var image = Image.Load<Rgba32>(file);
                    var newWidth = RoundToNearest ? RoundToNextPowerOfTwo(image.Width) : CeilToNextPowerOfTwo(image.Width);
                    var newHeight = RoundToNearest ? RoundToNextPowerOfTwo(image.Height) : CeilToNextPowerOfTwo(image.Height);
                    image.Mutate(x => { x.Resize(newWidth, newHeight); });
                    Directory.CreateDirectory(outputPath);  
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