using Mono.Options;

namespace POTify
{
    public static class ConsoleUtil
    {
        public static void ShowHelpHeader(string version, OptionSet options)
        {
            Console.WriteLine("POTify {0}", version);
            Console.WriteLine("POTify by EqUiNoX, texture power of 2 coverter.");
            Console.WriteLine();
            Console.WriteLine("Usage: POTify [options]+");
            Console.WriteLine();
            options.WriteOptionDescriptions(Console.Out);
        }

        public static void ShowOptionException(OptionException optionException)
        {
            Console.Write("POTify by EqUiNoX: ");
            Console.WriteLine(optionException.Message);
            Console.WriteLine("Try `POTify --help' for more information.");
        }
    }
}
