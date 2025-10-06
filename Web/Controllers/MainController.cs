using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Route("")]
    public class MainController : Controller
    {
        private readonly BlockChainService blockChainService;

        public MainController(BlockChainService blockChainService)
        {
            this.blockChainService = blockChainService;
        }

        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("add")]
        public IActionResult AddNote()
        {
            return View();
        }

        [Route("note/{height}")]
        public IActionResult ViewNote(int height)
        {
            if (blockChainService.BlockChain == null)
                return NotFound();
            
            var block = blockChainService.BlockChain[height];
            if (block == null)
                return NotFound();

            return View(new ViewNoteModel(block));
        }
    }
}
