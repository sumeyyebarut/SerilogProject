using Microsoft.AspNetCore.Mvc;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SerilogProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> logger;

        public HomeController(ILogger<HomeController> logger)
        {
            this.logger = logger;
        }


        [HttpGet]
        public int GetResult(int number1,int number2)
        {
            int result = 0;
            
            try
            {
                var user = new { Name = "Sumeyye", Surname = "Barut" };

                Log.Information("{@user} trying to divide {number1} by {number2}",user,number1,number2);
                
                result = number1 / number2;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Something went wrong while dividing {number1} by {number2} ",number1,number2 );
            }
            return result;
        }

    }
}
