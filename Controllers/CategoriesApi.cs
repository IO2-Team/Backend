/*
 * System rezerwacji miejsc na eventy
 *
 * Niniejsza dokumentacja stanowi opis REST API implemtowanego przez serwer centralny. Endpointy 
 *
 * The version of the OpenAPI document: 1.0.0
 * Contact: XXX@pw.edu.pl
 * Generated by: https://openapi-generator.tech
 */

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Org.OpenAPITools.Models;

namespace Org.OpenAPITools.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public class CategoriesApiController : ControllerBase
    { 
        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="sessionToken">session Token</param>
        /// <param name="categoryName">name of category</param>
        /// <response code="201">created</response>
        /// <response code="400">category already exist</response>
        [HttpPost]
        [Route("/categories")]
        public virtual IActionResult AddCategories([FromHeader][Required()]string sessionToken, [FromQuery (Name = "categoryName")][Required()]string categoryName)
        {

            //TODO: Uncomment the next line to return response 201 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(201, default(Category));
            //TODO: Uncomment the next line to return response 400 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(400);
            string exampleJson = null;
            exampleJson = "{\r\n  \"name\" : \"Sport\",\r\n  \"id\" : 1\r\n}";
            
            var example = exampleJson != null
            ? Newtonsoft.Json.JsonConvert.DeserializeObject<Category>(exampleJson)
            : default(Category);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }

        /// <summary>
        /// Return list of all categories
        /// </summary>
        /// <response code="200">successful operation</response>
        [HttpGet]
        [Route("/categories")]
        public virtual IActionResult GetCategories()
        {

            //TODO: Uncomment the next line to return response 200 or use other options such as return this.NotFound(), return this.BadRequest(..), ...
            // return StatusCode(200, default(List<Category>));
            string exampleJson = null;
            exampleJson = "[ {\r\n  \"name\" : \"Sport\",\r\n  \"id\" : 1\r\n}, {\r\n  \"name\" : \"Sport\",\r\n  \"id\" : 1\r\n} ]";
            
            var example = exampleJson != null
            ? Newtonsoft.Json.JsonConvert.DeserializeObject<List<Category>>(exampleJson)
            : default(List<Category>);
            //TODO: Change the data returned
            return new ObjectResult(example);
        }
    }
}
