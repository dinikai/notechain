using Notechain;

namespace Web
{
    public class BlockChainService
    {
        public BlockChain? BlockChain { get; }
        public Stream Stream { get; }

        /// <summary>
        /// Gets chain file path.
        /// </summary>
        public string Path => System.IO.Path.Combine(environment.ContentRootPath, "data");

        /// <summary>
        /// Gets queue of entries to be added in blocks.
        /// </summary>
        public Entry[] EntriesQueue => entriesQueue.Where(e => !e.GenerationCancellationRequested).ToArray();

        /// <summary>
        /// Gets current processing entry.
        /// </summary>
        public Entry? ProcessingEntry => processingEntry;

        /// <summary>
        /// Queue of entries to be added in blocks.
        /// </summary>
        private readonly Queue<Entry> entriesQueue = [];
        private bool isQueueProcessing = false;
        private Entry? processingEntry;

        private readonly IWebHostEnvironment environment;

        public BlockChainService(IWebHostEnvironment environment)
        {
            this.environment = environment;

            Stream = new FileStream(Path, FileMode.OpenOrCreate);

            BlockChain = BlockChain.FromStream(Stream);
            BlockChain ??= new BlockChain("Main", []);
        }

        /// <summary>
        /// Adds entry to queue and starts block generation, if it isn't started yet.
        /// </summary>
        public void AddEntryToQueue(Entry entry)
        {
            entriesQueue.Enqueue(entry);

            if (!isQueueProcessing)
                Task.Run(ProcessQueue);
        }

        /// <summary>
        /// Prevents block generation associated with entry with ID <paramref name="id"/>.
        /// Cancels the current generation.
        /// </summary>
        public void RemoveEntryFromQueue(Guid id)
        {
            IEnumerable<Entry> query = EntriesQueue;
            if (ProcessingEntry != null)
                query = EntriesQueue.Prepend(ProcessingEntry);

            query.FirstOrDefault(e => e.Id == id)?.CancelGeneration();
        }

        private void ProcessQueue()
        {
            if (BlockChain == null)
                return;

            isQueueProcessing = true;
            
            while (entriesQueue.TryDequeue(out var entry))
            {
                processingEntry = entry;

                var newBlock = Block.GenerateNew(entry, BlockChain.LastOrDefault());
                if (newBlock != null)
                {
                    BlockChain.AddBlock(newBlock);
                    BlockChain.WriteBlocksToStream(Stream);
                }
            }

            processingEntry = null;
            isQueueProcessing = false;
        }
    }
}
