namespace SerialPortTest
{
    internal static class CharExtensions
    {
        internal static bool IsAsciiControlOrSpaceChar(this char c) { return c == 127 || (int)c < 32; }
        internal static string AsciiCode(this char c)
        {
            if (!c.IsAsciiControlOrSpaceChar())
            {
                return c.ToString();
            }

            var letters = (int)c switch
            {
                0 => "NULL",
                1 => "SOH",
                2 => "STX",
                3 => "ETX",
                4 => "EOT",
                5 => "ENQ",
                6 => "ACK",
                7 => "BEL",
                8 => "BS",
                9 => "HT",
                10 => "LF",
                11 => "VT",
                12 => "FF",
                13 => "CR",
                14 => "SO",
                15 => "SI",
                16 => "DLE",
                17 => "DC1",
                18 => "DC2",
                19 => "DC3",
                20 => "DC4",
                21 => "NAK",
                22 => "SYN",
                23 => "ETB",
                24 => "CAN",
                25 => "EM",
                26 => "SUB",
                27 => "ESC",
                28 => "FS",
                29 => "GS",
                30 => "RS",
                31 => "US",
                127 => "DEL",
                _ => ((int)c).ToString()
            };
            return $"<{letters}>";
        }
    }
}
