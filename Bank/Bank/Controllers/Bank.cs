using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Bank : ControllerBase
    {

        private readonly ILogger<Bank> _logger;

        public Bank(ILogger<Bank> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
