﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using MyApi.Models;

using Newtonsoft.Json;

using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    #region Swagger Annotations
    public class AddressDto
    {
        /// <summary>
        /// 3-letter ISO country code
        /// </summary>
        /// <example>Iran</example>
        [Required]
        public string Country { get; set; }

        /// <summary>
        /// Name of city
        /// </summary>
        /// <example>Seattle</example>
        [DefaultValue("Seattle")]
        public string City { get; set; }

        [JsonProperty("promo-code")]
        public string Code { get; set; }

        [JsonIgnore]
        public int Discount { get; set; }
    }

    ///// <summary>
    ///// Retrieves a specific product by unique id
    ///// </summary>
    ///// <param name="param1">Parameter 1 description</param>
    ///// <param name="param2">Parameter 2 description</param>
    ///// <param name="param3">Parameter 2 description</param>
    ///// <remarks>Awesomeness!</remarks>
    ///// <response code="200">Product created</response>
    ///// <response code="400">Product has missing/invalid values</response>
    ///// <response code="500">Oops! Can't create your product right now</response>
    //[HttpGet("Test")]
    //public ActionResult Test(/*
    //        IFormFile file, ...
    //        [FromQuery] Address address, ...
    //        [FromForm] parameter, ...
    //        [FromBody] parameter, ...
    //        [Required] parameter, ... */)
    //{
    //    throw new NotImplementedException();
    //}
    #endregion

    [ApiVersion("1")]
    [AllowAnonymous]
    public class TestController : BaseController
    {
        [HttpPost("[action]")]
        public ActionResult UploadFile1(IFormFile file1)
        {
            return Ok();
        }

        //It doesn't work anymore in recent versions because of replacing Swashbuckle.AspNetCore.Examples with Swashbuckle.AspNetCore.Filters
        //[AddSwaggerFileUploadButton]
        [HttpPost("[action]")]
        public ActionResult UploadFile2()
        {
            IFormFile file = Request.Form.Files[0];
            return Ok();
        }


        #region Action Annotations
        //Specific request content type
        //[Consumes("application/json")]
        //Specific response content type
        //[Produces("application/json")]

        //Specific response http status codes
        //[ProducesResponseType(200)]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[SwaggerResponse(200)]
        //[SwaggerResponse(StatusCodes.Status200OK)]

        //Specific response type & description
        //[ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
        //[SwaggerResponse(StatusCodes.Status200OK, type: typeof(IEnumerable<UserDto>))]
        //[SwaggerResponse(StatusCodes.Status200OK, "my custom descriptions", type: typeof(IEnumerable<UserDto>))]

        //[SwaggerOperation(OperationId = "CreateCart")]
        //[SwaggerOperation(OperationId = "DeleteCart", Summary = "Deletes a specific cart", Description = "Requires admin privileges")]
        //[SwaggerOperationFilter(typeof(MyCustomIOperationFilter))]
        //[SwaggerTag("Manipulate Carts to your heart's content", "http://www.tempuri.org")]
        #endregion

        [HttpPost("[action]")]
        [SwaggerRequestExample(typeof(UserDto), typeof(CreateUserRequestExample))]
        [SwaggerResponseExample(200, typeof(CreateUserResponseExample))]
        [SwaggerResponse(200)]
        [SwaggerResponse(StatusCodes.Status406NotAcceptable)]
        public ActionResult<UserDto> CreateUser(UserDto userDto)
        {
            return Ok(userDto);
        }

        ///// <summary>
        ///// Asign an address to user
        ///// </summary>
        ///// <param name="addressDto">Address of user</param>
        ///// <remarks>Awesomeness!</remarks>
        ///// <response code="200">Address added</response>
        ///// <response code="400">Address has missing/invalid values</response>
        ///// <response code="500">Oops! Can't create your Address right now</response>
        [HttpPost("[action]")]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        public ActionResult Address(AddressDto addressDto)
        {
            return Ok();
        }
    }
}

public class CreateUserRequestExample : IExamplesProvider<UserDto>
{
    public UserDto GetExamples()
    {
        return new UserDto
        {
            FullName = "محمدجواد ابراهیمی",
            Age = 25,
            UserName = "mjebrahimi",
            Email = "admin@site.com",
            Gender = Entities.GenderType.Male,
            Password = "1234567"
        };
    }
}

public class CreateUserResponseExample : IExamplesProvider<UserDto>
{
    public UserDto GetExamples()
    {
        return new UserDto
        {
            FullName = "محمدجواد ابراهیمی",
            Age = 25,
            UserName = "mjebrahimi",
            Email = "admin@site.com",
            Gender = Entities.GenderType.Male,
            Password = "1234567"
        };
    }
}
