using Microsoft.AspNetCore.Mvc;

namespace Pact.NET.Controllers;
[Route("[controller]/[action]")]
[ApiController]
public class MyController : ControllerBase
{
    [HttpGet]
    public int Abs(int i)
    {
        return Math.Abs(i);
    }
}
