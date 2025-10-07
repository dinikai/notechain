namespace Notechain
{
    public class Entry
    {
        /// <summary>
        /// Entry data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Entry additional string data.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Gets entry creation timestamp.
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.Now;

        public int Difficulty { get; }

        public Guid Id { get; } = Guid.NewGuid();

        public double HashesPerSecond { get; set; }

        public bool GenerationCancellationRequested => generationCancellationRequested;

        private bool generationCancellationRequested;

        public Entry(byte[] data, string comment, int difficulty)
        {
            Data = data;
            Comment = comment;
            Difficulty = difficulty;
        }

        public void CancelGeneration() => generationCancellationRequested = true;
    }
}
