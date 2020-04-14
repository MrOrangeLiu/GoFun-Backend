using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DivingApplication.Entities;
using DivingApplication.Helpers;
using DivingApplication.Helpers.Extensions;
using DivingApplication.Helpers.ResourceParameters;
using DivingApplication.Models.Reports;
using DivingApplication.Repositories.Reports;
using DivingApplication.Repositories.Users;
using DivingApplication.Services.PropertyServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static DivingApplication.Entities.User;

namespace DivingApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : ControllerBase
    {

        private readonly IReportRepository _reportRepository;
        private readonly IMapper _mapper;
        private readonly IPropertyMappingService _propertyMapping;
        private readonly IPropertyValidationService _propertyValidation;
        private readonly IUserRepository _userRepository;



        public ReportsController(IReportRepository reportRepository, IMapper mapper, IPropertyMappingService propertyMapping, IPropertyValidationService propertyValidation, IUserRepository userRepository)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _propertyMapping = propertyMapping ?? throw new ArgumentNullException(nameof(propertyMapping));
            _propertyValidation = propertyValidation ?? throw new ArgumentNullException(nameof(propertyValidation));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }


        [Authorize(Policy = "VerifiedUsers")]
        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] ReportForCreatingDto report, [FromQuery] string fields)
        {

            if (!_propertyValidation.HasValidProperties<ReportOutputDto>(fields)) return BadRequest();

            var reportEntity = _mapper.Map<Report>(report);

            // Check the Login User Exist
            var loginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loginUser = await _userRepository.GetUser(loginUserId);

            if (loginUser == null) return NotFound();

            // Preparing the Properties
            reportEntity.AuthorId = loginUserId;
            reportEntity.CreatedAt = DateTime.Now;
            reportEntity.Solved = false;

            // Add Report to DB 
            await _reportRepository.AddReport(reportEntity);

            await _reportRepository.Save();

            var reportToReturn = _mapper.Map<ReportOutputDto>(reportEntity);

            return CreatedAtRoute("GetReport", new { reportId = reportToReturn.Id, fields }, reportToReturn.ShapeData(fields));
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{reportId}")]
        public async Task<IActionResult> AdminSolveReport(Guid reportId, [FromQuery] string fields, [FromBody] string note)
        {

            if (!_propertyValidation.HasValidProperties<ReportOutputDto>(fields)) return BadRequest();

            // Checking if the Login User is Admin
            var loginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loginUser = await _userRepository.GetUser(loginUserId);

            if (loginUser == null) return NotFound();
            if (loginUser.UserRole != Role.Admin) return Unauthorized();


            // Get the Current Report
            var reportFromRepo = await _reportRepository.GetReport(reportId);

            // Update here
            reportFromRepo.SolvedAt = DateTime.Now;
            reportFromRepo.SolvedById = loginUserId.ToString();
            reportFromRepo.Note = note;

            // Save to Db
            await _reportRepository.Save();


            var reportToReturn = _mapper.Map<ReportOutputDto>(reportFromRepo);

            return CreatedAtRoute("GetReport", new
            {
                reportId = reportToReturn.Id,
                fields
            }, reportToReturn.ShapeData(fields));
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("{reportId}", Name = "GetReport")]
        public async Task<IActionResult> GetReport(Guid reportId, [FromQuery] string fields)
        {

            if (!_propertyValidation.HasValidProperties<ReportOutputDto>(fields)) return BadRequest();

            // Get the Current Report
            var reportFromRepo = await _reportRepository.GetReport(reportId);

            // Tranform to the Return Type
            var reportToReturn = _mapper.Map<ReportOutputDto>(reportFromRepo);

            return Ok(reportToReturn.ShapeData(fields));
        }


        [Authorize(Roles = "Admin")]
        [HttpGet(Name = "GetReports")]
        public async Task<IActionResult> GetReports([FromQuery] ReportResourceParameters resourceParameters)
        {
            // Checking the OrderBy and Fields
            if (!_propertyMapping.ValidMappingExist<ReportOutputDto, Post>(resourceParameters.OrderBy)) return BadRequest();
            if (!_propertyValidation.HasValidProperties<ReportOutputDto>(resourceParameters.Fields)) return BadRequest();

            // Get the Reports according to the Parameters
            var reportsFromRepo = await _reportRepository.GetReports(resourceParameters);

            var previousPageLink = reportsFromRepo.HasPrevious ? CreateReportsUri(resourceParameters, UriType.PreviousPage, "GetReports") : null;
            var nextPageLink = reportsFromRepo.HasNext ? CreateReportsUri(resourceParameters, UriType.NextPage, "GetReports") : null;

            var metaData = new
            {
                totalCount = reportsFromRepo.TotalCount,
                pageSize = reportsFromRepo.PageSize,
                currentPage = reportsFromRepo.CurrentPage,
                totalPages = reportsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink,
            };

            Response.Headers.Add("Pagination", JsonSerializer.Serialize(metaData));

            // Tranform to the Return Type
            var reportsToReturn = _mapper.Map<IEnumerable<ReportOutputDto>>(reportsFromRepo);

            return Ok(reportsToReturn.ShapeData(resourceParameters.Fields));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{reportId}")]
        public async Task<IActionResult> DeleteReport(Guid reportId, [FromQuery] string fields)
        {

            // Checking again if the loginUser is Admin
            var loginUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loginUser = await _userRepository.GetUser(loginUserId);

            if (loginUser == null) return NotFound();
            if (loginUser.UserRole != Role.Admin) return Unauthorized();


            // Get the Current Report
            var reportFromRepo = await _reportRepository.GetReport(reportId);

            // Checking if the report Exists    
            if (reportFromRepo == null) return NotFound();

            // Delete the Report here
            _reportRepository.DeleteReport(reportFromRepo);
            await _reportRepository.Save();

            // Tranform to the Return Type
            var reportsToReturn = _mapper.Map<IEnumerable<ReportOutputDto>>(reportFromRepo);

            return Ok(reportsToReturn.ShapeData(fields));
        }


        private string CreateReportsUri(ReportResourceParameters postResourceParameters, UriType uriType, string routeName)
        {
            return Url.Link(routeName, postResourceParameters.CreateUrlParameters(uriType));
        }
    }
}