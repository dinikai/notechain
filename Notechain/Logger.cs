using System.Text;

namespace Notechain
{
    internal static class Logger
    {
        public static void LogBlock(Block block)
        {
            string header = $"----- Block #{block.Height} ({block.Id}) -----";
            Console.WriteLine(header);

            Console.WriteLine($"ID: {block.Id}");
            Console.WriteLine($"Height: {block.Height}");
            Console.WriteLine($"Hash: {BitConverter.ToString(block.Hash).ToLower().Replace("-", "")}");

            if (block.Previous != null)
                Console.WriteLine($"Hash of prev. block: {BitConverter.ToString(block.Previous.Hash).ToLower().Replace("-", "")}");

            Console.WriteLine($"Nonce: {block.Nonce}");
            Console.WriteLine($"Timestamp: {block.Timestamp}");

            Console.WriteLine();
            Console.WriteLine("----- Data (UTF-8) -----");
            Console.WriteLine(Encoding.UTF8.GetString(block.Data));

            Console.WriteLine();
            Console.WriteLine("----- Comment -----");
            Console.WriteLine(block.Comment);

            Console.WriteLine(new string('-', header.Length));
        }
    }
}
