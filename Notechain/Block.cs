using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Notechain
{
    /// <summary>
    /// Repsesents block in chain that contains data.
    /// </summary>
    internal class Block
    {
        /// <summary>
        /// Unique block ID.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Block position in chain (genesis block is 0).
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Block primary data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Block additional string data.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Block hash.
        /// </summary>
        public byte[] Hash { get; set; }

        /// <summary>
        /// Previous block in chain.
        /// <c>null</c> if current block is genesis block.
        /// </summary>
        public Block? Previous { get; set; }

        /// <summary>
        /// Local-time timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Nonce value that need to be changed to produce new hash.
        /// </summary>
        public long Nonce { get; set; }

        public Block(Guid id, uint height, byte[] data, string comment, byte[] hash, DateTime timestamp, long nonce)
        {
            Id = id;
            Height = height;
            Data = data;
            Comment = comment;
            Hash = hash;
            Timestamp = timestamp;
            Nonce = nonce;
        }

        /// <summary>
        /// Asynchronously generates a new block with the specified data and comment,
        /// performing a proof-of-work hash calculation to ensure the block's validity.
        /// </summary>
        /// <param name="data">The data to be stored in the block.</param>
        /// <param name="comment">A human-readable comment or description associated with the block.</param>
        /// <param name="previousBlock">The previous block in the chain, or <c>null</c> if this is the genesis block.</param>
        /// <returns>A new generated <see cref="Block"/>.</returns>
        public static async Task<Block> GenerateNew(byte[] data, string comment, Block? previousBlock)
        {
            uint height = 0;
            if (previousBlock != null)
                height = previousBlock.Height + 1;

            var block = new Block(
                id: Guid.NewGuid(),
                height: height,
                data: data,
                comment: comment,
                hash: [],
                timestamp: DateTime.Now,
                nonce: 0
            )
            {
                Previous = previousBlock
            };

            var sha256 = SHA256.Create();
            byte[] hash = [];
            long nonce = long.MinValue;
            do
            {
                if (nonce == long.MaxValue)
                    break;

                hash = await block.GetHashAsync(++nonce, sha256);
            } while (!IsHashValid(hash));

            block.Hash = hash;
            block.Nonce = nonce;

            return block;
        }

        /// <summary>
        /// Reads next block from stream.
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <param name="previousBlock">Previous block. <c>null</c> if no blocks was read.</param>
        /// <returns></returns>
        public static Block? FromStream(Stream stream, Block? previousBlock)
        {
            // ID
            byte[] buffer = new byte[16];
            int bytesRead = stream.Read(buffer);
            Guid id = new(buffer);

            // Return null, if there's end of stream
            if (bytesRead == 0)
                return null;

            // Height
            buffer = new byte[4];
            stream.Read(buffer);
            uint height = BitConverter.ToUInt32(buffer);

            // Hash
            byte[] hash = new byte[32];
            stream.Read(hash);

            // Previous hash
            byte[] previousHash = new byte[32];
            stream.Read(previousHash);

            // Timestamp
            buffer = new byte[8];
            stream.Read(buffer);
            DateTime timestamp = DateTime.FromBinary(BitConverter.ToInt64(buffer));

            // Nonce
            buffer = new byte[8];
            stream.Read(buffer);
            long nonce = BitConverter.ToInt64(buffer);

            // Comment length
            buffer = new byte[4];
            stream.Read(buffer);
            uint commentLength = BitConverter.ToUInt32(buffer);

            // Comment
            buffer = new byte[commentLength];
            stream.Read(buffer);
            string comment = Encoding.UTF8.GetString(buffer);

            // Data length
            buffer = new byte[8];
            stream.Read(buffer);
            ulong dataLength = BitConverter.ToUInt64(buffer);

            // Data
            byte[] data = new byte[dataLength];
            stream.Read(data);

            return new Block(
                id,
                height,
                data,
                comment,
                hash,
                timestamp,
                nonce
            )
            {
                Previous = previousBlock
            };
        }

        /// <summary>
        /// Returns <c>true</c> if <paramref name="hash"/> less than target, otherwise <c>false</c>.
        /// </summary>
        public static bool IsHashValid(byte[] hash)
        {
            var target = GetHashTarget(16);
            
            byte[] positiveHash = new byte[hash.Length + 1];
            Array.Copy(hash, positiveHash, hash.Length);

            var hashInt = new BigInteger(positiveHash);
            return hashInt < target;
        }

        /// <summary>
        /// Generates the target value for hash difficulty verification in the Proof of Work algorithm.
        /// The target is calculated as 2^(256 - <paramref name="bits"/>), where 256 is the hash length in bits.
        /// The higher the <paramref name="bits"/>, the lower the target and the higher the difficulty.
        /// </summary>
        public static BigInteger GetHashTarget(int bits)
        {
            BigInteger maxTarget = BigInteger.Pow(2, 256) - 1;
            return maxTarget >> bits;
        }

        /// <summary>
        /// Verifies the integrity of the block by recalculating its hash and comparing it to the stored hash.
        /// </summary>
        /// <returns><c>true</c> if the block's hash is valid and matches the computed hash; otherwise, <c>false</c>.</returns>
        public bool CheckForValidity()
        {
            byte[] feed = GetHashFeed();
            var hash = SHA256.HashData(feed);

            return hash.SequenceEqual(Hash);
        }

        /// <summary>
        /// Computes and returns block summary hash with specified <paramref name="nonce"/> value.
        /// </summary>
        private byte[] GetHash(long nonce)
        {
            byte[] feed = GetHashFeed(nonce);
            return SHA256.HashData(feed);
        }

        /// <summary>
        /// Computes and returns block summary hash.
        /// </summary>
        public byte[] GetHash() => GetHash(Nonce);

        /// <summary>
        /// Computes and returns block summary hash with specified <paramref name="nonce"/> value.
        /// </summary>
        public async Task<byte[]> GetHashAsync(long nonce, SHA256 sha256)
        {
            byte[] feed = GetHashFeed(nonce);
            using var ms = new MemoryStream(feed);
            return await sha256.ComputeHashAsync(ms);
        }

        /// <summary>
        /// Computes and returns block summary hash.
        /// </summary>
        public async Task<byte[]> GetHashAsync(SHA256 sha256) => await GetHashAsync(Nonce, sha256);

        /// <summary>
        /// Returns the concatenated block data for hashing with specified <paramref name="nonce"/> value.
        /// </summary>
        private byte[] GetHashFeed(long nonce)
        {
            var bytes = new List<byte>();

            // ID
            bytes.AddRange(Id.ToByteArray());

            // Height
            bytes.AddRange(BitConverter.GetBytes(Height));

            // Previous hash
            byte[] previousHash = Enumerable.Repeat<byte>(0, 32).ToArray();
            if (Previous != null)
                previousHash = Previous.Hash;
            bytes.AddRange(previousHash);

            // Timestamp
            bytes.AddRange(BitConverter.GetBytes(Timestamp.ToBinary()));
            
            // Nonce
            bytes.AddRange(BitConverter.GetBytes(nonce));

            // Comment
            bytes.AddRange(Encoding.UTF8.GetBytes(Comment));

            // Data
            bytes.AddRange(Data);

            return bytes.ToArray();
        }

        /// <summary>
        /// Returns the concatenated block data for hashing.
        /// </summary>
        public byte[] GetHashFeed() => GetHashFeed(Nonce);
    }
}
