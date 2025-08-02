using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace ReverseGeoAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ReverseGeoController : ControllerBase
{
    private readonly ILogger<ReverseGeoController> _logger;
    private string baseUrl = "https://maps.googleapis.com/maps/api/geocode/json?latlng=";

    private string urlParameters = "&key=" + Environment.GetEnvironmentVariable("GOOGLE_API_KEY");


    public ReverseGeoController(ILogger<ReverseGeoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("GetGeoLocation")]
    public ActionResult GetGeoLocation(float latitude, float longitude)
    {

        HttpClient client = new HttpClient();

        client.BaseAddress = new Uri(baseUrl + latitude + "," + longitude + urlParameters);

        string urlParametersModified = "?latlng=" + latitude + "," + longitude + urlParameters;


        HttpResponseMessage response = client.GetAsync(urlParametersModified).Result;

        if (response.IsSuccessStatusCode)
        {
            var data = response.Content.ReadAsStringAsync().Result;

            var stateCode = JsonObject.Parse(data)["plus_code"]["compound_code"].ToString().Split(",")[1].Trim();

            return Ok(stateCode);
        }
        else
        {
            if ((int)response.StatusCode == 404)
            {
                return NotFound();
            }
            else if ((int)response.StatusCode == 401)
            {
                return Unauthorized();
            }
            else if ((int)response.StatusCode == 403)
            {
                return Forbid();
            }
            else if ((int)response.StatusCode == 500)
            {
                return StatusCode(500);
            }
        }
        return Ok();

    }
}