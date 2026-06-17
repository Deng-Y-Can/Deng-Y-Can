using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class JobsController : ControllerBase
    {
        private readonly JobService _jobService;

        public JobsController(JobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllJobs()
        {
            var jobs = await _jobService.GetAllJobsAsync();
            return ApiResponse.Success(jobs);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Schedule(string jobName, int intervalSeconds, string description = null)
        {
            try
            {
                await _jobService.ScheduleSimpleJobAsync(jobName, intervalSeconds, description);
                return ApiResponse.Success(message: $"Job '{jobName}' scheduled");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Pause(string jobName, string group = "default")
        {
            try
            {
                await _jobService.PauseJobAsync(jobName, group);
                return ApiResponse.Success(message: $"Job '{jobName}' paused");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Resume(string jobName, string group = "default")
        {
            try
            {
                await _jobService.ResumeJobAsync(jobName, group);
                return ApiResponse.Success(message: $"Job '{jobName}' resumed");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> Delete(string jobName, string group = "default")
        {
            try
            {
                await _jobService.DeleteJobAsync(jobName, group);
                return ApiResponse.Success(message: $"Job '{jobName}' deleted");
            }
            catch (Exception ex)
            {
                return ApiResponse.Fail(ex.Message);
            }
        }
    }
}
