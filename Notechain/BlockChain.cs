using System.Collections;
using System.Text;

namespace Notechain
{
    /// <summary>
    /// Represents the block chain.
    /// </summary>
    public class BlockChain : IEnumerable<Block>
    {
        /// <summary>
        /// Fixed file header length.
        /// </summary>
        public static readonly int HeaderLength = 256;

        /// <summary>
        /// Gets number of all blocks in the chain.
        /// </summary>
        public int Count => blocks.Count;

        /// <summary>
        /// Gets or sets chain title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets verification state of this chain.
        /// </summary>
        public bool IsVerified => blocks.All(b => b.IsValid);

        /// <summary>
        /// Gets block from the chain by ID.
        /// </summary>
        public Block? this[Guid id]
        {
            get => blocks.FirstOrDefault(b => b.Id == id);
        }

        /// <summary>
        /// Gets block from the chain by height.
        /// </summary>
        public Block? this[int height]
        {
            get => blocks.FirstOrDefault(b => b.Height == height);
        }

        /// <summary>
        /// Stores all blocks in the chain.
        /// </summary>
        private readonly List<Block> blocks = [];

        /// <summary>
        /// Stores blocks ready to be written.
        /// </summary>
        private readonly Queue<Block> queuedBlocks = [];

        public BlockChain(string title, List<Block> blocks)
        {
            Title = title;
            this.blocks = blocks;
        }

        public IEnumerator<Block> GetEnumerator() => blocks.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => blocks.GetEnumerator();

        /// <summary>
        /// Reads chain header and all blocks from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns></returns>
        public static BlockChain? FromStream(Stream stream)
        {
            int bytesRead = 0;

            // Read the header
            byte[] buffer = new byte[4];
            bytesRead += stream.Read(buffer);
            int blockNumber = BitConverter.ToInt32(buffer);

            if (bytesRead == 0)
                return null;

            buffer = new byte[4];
            bytesRead += stream.Read(buffer);
            uint titleLength = BitConverter.ToUInt32(buffer);

            buffer = new byte[titleLength];
            bytesRead += stream.Read(buffer);
            string title = Encoding.UTF8.GetString(buffer);

            // Read remaining zeros
            stream.Read(new byte[HeaderLength], 0, HeaderLength - bytesRead);

            // Read all blocks
            var blocks = new List<Block>();
            Block? block = null;

            for (int i = 0; i < blockNumber; i++)
            {
                block = Block.FromStream(stream, block);

                if (block == null)
                    break;

                blocks.Add(block);
            }

            return new BlockChain(title, blocks);
        }

        /// <summary>
        /// Adds <paramref name="block"/> to the chain and puts it on the write queue.
        /// </summary>
        public void AddBlock(Block block)
        {
            blocks.Add(block);
            queuedBlocks.Enqueue(block);
        }

        /// <summary>
        /// Writes queued blocks to <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write.</param>
        public void WriteBlocksToStream(Stream stream)
        {
            WriteHeader(stream, false);

            stream.Seek(0, SeekOrigin.End);

            while (queuedBlocks.TryDequeue(out var block))
            {
                block.WriteBlockToStream(stream);
            }

            stream.Flush();
        }

        /// <summary>
        /// Writes new file header to <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Stream to write.</param>
        /// <param name="returnPosition">Determines whether to restore the stream’s position to its original value after writing.</param>
        private void WriteHeader(Stream stream, bool returnPosition)
        {
            long position = stream.Position;

            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(BitConverter.GetBytes(Count));
            stream.Write(BitConverter.GetBytes(Title.Length));
            stream.Write(Encoding.UTF8.GetBytes(Title));

            // Write remaining zeros to match the length
            int remainingZeros = HeaderLength - (int)stream.Position;
            stream.Write(Enumerable.Repeat<byte>(0, remainingZeros).ToArray());

            if (returnPosition)
                stream.Seek(position, SeekOrigin.Begin);
        }
    }
}
