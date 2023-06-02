namespace Arcta.Lims.Machines.Protocols.Transport.Extensions
{
    internal static class StringExtensions
    {
        internal static string AsAsciiCharacterCodes(this string str) { return string.Join(" ", str.Select(ch => (int)ch)); }
        internal static string WithAsciiCodes(this string str)
        {
            var ch = str.ToCharArray();

            var logMessage = "";
            for (int i = 0; i < ch.Length; i++)
            {
                char c = ch[i];

                logMessage += c.AsciiCode();

                logMessage += i != ch.Length - 1 && c.IsAsciiControlOrSpaceChar() ? ", " : "";
            }
            return logMessage;
        }
    }
}
