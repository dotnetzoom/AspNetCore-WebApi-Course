using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Entities.Identity;
using Services.Contracts.Identity;
using Services.Identity;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    [Authorize(Roles = ConstantRoles.Admin)]
    public class SystemLogController : ControllerBase
    {
        private readonly IAppLogItemsService _appLogItemsService;

        public SystemLogController(
            IAppLogItemsService appLogItemsService)
        {
            _appLogItemsService = appLogItemsService ?? throw new ArgumentNullException(nameof(_appLogItemsService));
        }

        public async Task<ApiResult<PagedAppLogItemsViewModel>> Get(
                        string logLevel = "",
                        int pageNumber = 1,
                        int pageSize = -1,
                        string sort = "desc")
        {
            var itemsPerPage = 10;

            if (pageSize > 0)
                itemsPerPage = pageSize;

            var model = await _appLogItemsService.GetPagedAppLogItemsAsync(
                pageNumber, itemsPerPage, sort == "desc" ? SortOrder.Descending : SortOrder.Ascending, logLevel);

            model.LogLevel = logLevel;
            model.Paging.CurrentPage = pageNumber;
            model.Paging.ItemsPerPage = itemsPerPage;

            return model;
        }

        [HttpPost]
        public async Task<ApiResult> LogItemDelete(int id)
        {
            await _appLogItemsService.DeleteAsync(id);

            return Ok();
        }

        [HttpPost]
        public async Task<ApiResult> LogDeleteAll(string logLevel = "")
        {
            await _appLogItemsService.DeleteAllAsync(logLevel);

            return Ok();
        }

        [HttpPost]
        public async Task<ApiResult> LogDeleteOlderThan(string logLevel = "", int days = 5)
        {
            var cutoffUtc = DateTime.UtcNow.AddDays(-days);
            await _appLogItemsService.DeleteOlderThanAsync(cutoffUtc, logLevel);

            return Ok();
        }
    }
}