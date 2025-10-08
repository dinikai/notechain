using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Notechain;
using System.Text;

namespace Web.Controllers.Api
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly BlockChainService blockChainService;

        public QueueController(BlockChainService blockChainService)
        {
            this.blockChainService = blockChainService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            if (blockChainService.ProcessingEntry != null)
                return Ok(blockChainService.EntriesQueue.Prepend(blockChainService.ProcessingEntry));

            return Ok(blockChainService.EntriesQueue);
        }

        [HttpGet]
        public IActionResult GetProcessing()
        {
            return Ok(blockChainService.ProcessingEntry);
        }

        [HttpPost]
        public IActionResult Add(string data, string comment)
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            blockChainService.AddEntryToQueue(new Entry(byteData, comment, 26));

            return Ok();
        }

        [HttpPost]
        public IActionResult Remove(Guid id)
        {
            blockChainService.RemoveEntryFromQueue(id);
            return Ok();
        }

        [HttpPost]
        public IActionResult Clear()
        {
            blockChainService.ClearQueue();
            return Ok();
        }
    }
}
