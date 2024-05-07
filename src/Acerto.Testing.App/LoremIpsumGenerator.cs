namespace Acerto.Testing.App
{
    internal static class LoremIpsumGenerator
    {
        private const string LoremIpsum = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";

        private const int MaxLenght = 65535;

        public static string Generate(int lenght = MaxLenght)
        {
            var content = string.Join(Environment.NewLine, Enumerable.Repeat(LoremIpsum, lenght / LoremIpsum.Length));

            if (content.Length > MaxLenght)
            {
                content = content[..MaxLenght];
            }

            return content;
        }
    }
}
