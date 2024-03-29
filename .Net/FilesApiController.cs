﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain.Files;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Sabio.Web.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesApiController : BaseApiController
    {
        private IFilesService _service = null;
        private IAuthenticationService<int> _authService = null;

        public FilesApiController(IFilesService service, ILogger<FilesApiController> logger,
            IAuthenticationService<int> authService): base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(FileAddRequest model)
        {
            ObjectResult result = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
                int id = _service.Add(model, userId);
                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }
            return result;
        }

    
         [HttpPost("upload")]
        public async Task<ActionResult<ItemsResponse<FileUpload>>> UploadFileAsync(IFormFile[] files)
        {
            ObjectResult result = null;
            try
            {
                if (files != null)
                {
                    int userId = _authService.GetCurrentUserId();
                    List<FileUpload> fileUrls = await _service.UploadFileAsync(files, userId);                    
             
                    ItemsResponse<FileUpload> response = new ItemsResponse<FileUpload> { Items = fileUrls };
                    
                result = StatusCode(201,response);
                }                              
             }
            catch (Exception ex)
            {
                base.Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
             }
            return result;
        }

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<File>>> GetAll(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<File> paged = _service.GetAll(pageIndex, pageSize);

                 if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                  else
                {
                    response = new ItemResponse<Paged<File>> { Item = paged };
                }

            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("createdby")] 
        public ActionResult<ItemResponse<Paged<File>>> GetByCreatedBy(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                int userId = _authService.GetCurrentUserId();
               Paged<File> paged = _service.GetByCreatedBy(pageIndex,pageSize,userId);

                if (paged == null)
                {

                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<File>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

        [HttpGet("filetype/{fileTypeId:int}")]
        public ActionResult<ItemResponse<Paged<File>>> GetByFileTypeId( int pageIndex, int pageSize, int fileTypeId)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                Paged<File> paged = _service.GetByFileTypeId(fileTypeId, pageIndex, pageSize);
                if (paged == null)
                {

                    code = 404;
                    response = new ErrorResponse("App Resource not found.");
                }
                else
                {
                    response = new ItemResponse<Paged<File>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }


        [HttpDelete("delete/{id:int}")]
        public ActionResult<ItemResponse<int>> UpdateIsDeleted(int id)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                _service.UpdateIsDeleted(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

        [HttpGet("deleted")] 
        public ActionResult<ItemResponse<Paged<File>>> GetByDeleted(int pageIndex, int pageSize)
        {
            int code = 200;
            BaseResponse response = null;
            try
            {
                
                Paged<File> paged = _service.GetByDeleted(pageIndex, pageSize);

                if (paged == null)
                {
                    code = 404;
                    response = new ErrorResponse("Records not found");
                }
                else
                {
                    response = new ItemResponse<Paged<File>> { Item = paged };
                }
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
                base.Logger.LogError(ex.ToString());
            }
            return StatusCode(code, response);
        }

    }
}
