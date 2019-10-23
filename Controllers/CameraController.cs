using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatchItREST.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CatchItREST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CameraController : ControllerBase
    {
        private readonly IConfiguration config;
        public CameraController(IConfiguration cfg)
        {
            config = cfg;
        }
        // GET: api/Camera
        [HttpGet]
        public async Task<IActionResult> Get()
        {            
            return Ok(state.IsMotionDetected);
        }
    }
}
