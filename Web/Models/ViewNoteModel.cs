using Notechain;

namespace Web.Models
{
    public class ViewNoteModel
    {
        /// <summary>
        /// Block associated with note.
        /// </summary>
        public Block Block { get; set; }

        public ViewNoteModel(Block block)
        {
            Block = block;
        }
    }
}
