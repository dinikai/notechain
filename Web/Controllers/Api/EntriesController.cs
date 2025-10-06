using Microsoft.AspNetCore.Mvc;
using Notechain;
using System.Text;

namespace Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EntriesController : ControllerBase
    {
        private readonly BlockChainService blockChainService;

        public EntriesController(BlockChainService blockChainService)
        {
            this.blockChainService = blockChainService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            if (blockChainService.BlockChain == null)
                return NotFound();

            return Ok(blockChainService.BlockChain.Reverse());
        }

        [HttpGet]
        public IActionResult GetById(Guid id)
        {
            if (blockChainService.BlockChain == null)
                return NotFound();

            return Ok(blockChainService.BlockChain[id]);
        }

        [HttpGet]
        public IActionResult GetByHeight(int height)
        {
            if (blockChainService.BlockChain == null)
                return NotFound();

            return Ok(blockChainService.BlockChain[height]);
        }

        [HttpGet]
        public IActionResult GetFiltered(string query)
        {
            if (blockChainService.BlockChain == null)
                return NotFound();

            query = query.Trim();

            var filtered = blockChainService.BlockChain
                .Where(b => b.Comment.Contains(query, StringComparison.OrdinalIgnoreCase))
                .Reverse();

            return Ok(filtered);
        }
    }
}
