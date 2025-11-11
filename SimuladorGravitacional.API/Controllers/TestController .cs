using Microsoft.AspNetCore.Mvc;
using System;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
	[HttpGet]
	public IActionResult Get()
	{
		return Ok(new
		{
			Message = "✅ API está funcionando!",
			Timestamp = DateTime.Now,
			Status = "Online",
			Endpoints = new[] {
				"GET /api/simulacao",
				"POST /api/simulacao",
				"GET /api/simulacao/{id}",
				"DELETE /api/simulacao/{id}",
				"GET /api/health"
			}
		});
	}
}