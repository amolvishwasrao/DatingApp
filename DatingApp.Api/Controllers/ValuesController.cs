using DatingApp.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController:ControllerBase
    {
        public readonly DataContext _context;

        
        public ValuesController(DataContext context)
        {
            _context = context;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetValues()
        {
            var values= await _context.Values.ToListAsync();
            return Ok(values);
        } 

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async  Task<IActionResult> GetValue(int id)
        {
            var values=await _context.Values.Where(p=>p.Id==id).FirstOrDefaultAsync();
            return Ok(values);
        } 


        [HttpPost]
        public void Post([FromBody] string values)
        {

        }

        [HttpPut("{id}")]
        public void Put(int id,[FromBody] string values)
        {

        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {}
    }
}