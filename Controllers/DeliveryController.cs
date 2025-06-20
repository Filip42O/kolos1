using KolokwiumTemplate.DTO_s;
using KolokwiumTemplate.Service;
using Microsoft.AspNetCore.Mvc;

namespace KolokwiumTemplate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IDbService _db;
        public ProjectsController(IDbService db) { _db = db; }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var project = await _db.GetProjectByIdAsync(id);
            if (project == null) return NotFound();
            return Ok(project);
        }
    }
    
    
    
    
    
    [ApiController]
    [Route("api/[controller]")]
    public class ArtifactsController : ControllerBase
    {
        private readonly IDbService _db;
        public ArtifactsController(IDbService db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ArtifactWithProjectCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _db.CreateArtifactAndProjectAsync(dto);
                return CreatedAtAction(
                    actionName: "GetById",
                    controllerName: "Projects",
                    routeValues: new { id = created.ProjectId },
                    value: created
                );
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(new { error = knf.Message });
            }
            catch (InvalidOperationException ioe)
            {
                return Conflict(new { error = ioe.Message });
            }
        }
    }
    
    
    
    
    
}