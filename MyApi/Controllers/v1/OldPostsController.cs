﻿using AutoMapper;
using AutoMapper.QueryableExtensions;

using Data.Repositories;

using Entities;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MyApi.Models;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    [ApiVersion("1")]
    public class OldPostsController : BaseController
    {
        private readonly IRepository<Post> _repository;
        private readonly IMapper _mapper;

        public OldPostsController(IRepository<Post> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<PostSelectDto>>> Get(CancellationToken cancellationToken)
        {
            #region old code
            //var posts = await _repository.TableNoTracking
            //    .Include(p => p.Category).Include(p => p.Author).ToListAsync(cancellationToken);
            //var list = posts.Select(p =>
            //{
            //    var dto = Mapper.Map<PostDto>(p);
            //    return dto;
            //}).ToList();

            //var list = await _repository.TableNoTracking.Select(p => new PostDto
            //{
            //    Id = p.Id,
            //    Title = p.Title,
            //    Description = p.Description,
            //    CategoryId = p.CategoryId,
            //    AuthorId = p.AuthorId,
            //    AuthorFullName = p.Author.FullName,
            //    CategoryName = p.Category.Name
            //}).ToListAsync(cancellationToken);
            #endregion

            List<PostSelectDto> list = await _repository.TableNoTracking.ProjectTo<PostSelectDto>(_mapper.ConfigurationProvider)
                //.Where(postDto => postDto.Title.Contains("test") || postDto.CategoryName.Contains("test"))
                .ToListAsync(cancellationToken);

            return Ok(list);
        }

        [HttpGet("{id:guid}")]
        public async Task<ApiResult<PostSelectDto>> Get(Guid id, CancellationToken cancellationToken)
        {
            PostSelectDto dto = await _repository.TableNoTracking.ProjectTo<PostSelectDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

            //Post post = null; //Find from database by Id (include)
            //var resultDto = PostDto.FromEntity(post);

            if (dto == null)
            {
                return NotFound();
            }

            //dto.Category = "My custom value, not from mapping!";

            #region old code
            //var dto = new PostDto
            //{
            //    Id = model.Id,
            //    Title = model.Title,
            //    Description = model.Description,
            //    CategoryId = model.CategoryId,
            //    AuthorId = model.AuthorId,
            //    AuthorFullName = model.Author.FullName,
            //    CategoryName = model.Category.Name
            //};
            #endregion

            return dto;
        }

        [HttpPost]
        public async Task<ApiResult<PostSelectDto>> Create(PostDto dto, CancellationToken cancellationToken)
        {
            //var model = Mapper.Map<Post>(dto);
            Post model = dto.ToEntity(_mapper);

            #region old code
            //var model = new Post
            //{
            //    Title = dto.Title,
            //    Description = dto.Description,
            //    CategoryId = dto.CategoryId,
            //    AuthorId = dto.AuthorId
            //};
            #endregion

            await _repository.AddAsync(model, cancellationToken);

            #region old code
            //await _repository.LoadReferenceAsync(model, p => p.Category, cancellationToken);
            //await _repository.LoadReferenceAsync(model, p => p.Author, cancellationToken);
            //model = await _repository.TableNoTracking
            //    .Include(p => p.Category)
            //    .Include(p =>p.Author)
            //    .SingleOrDefaultAsync(p => p.Id == model.Id, cancellationToken);
            //var resultDto = new PostDto
            //{
            //    Id = model.Id,
            //    Title = model.Title,
            //    Description = model.Description,
            //    CategoryId = model.CategoryId,
            //    AuthorId = model.AuthorId,
            //    AuthorName = model.Author.FullName,
            //    CategoryName = model.Category.Name
            //};


            //var resultDto = await _repository.TableNoTracking.Select(p => new PostDto
            //{
            //    Id = p.Id,
            //    Title = p.Title,
            //    Description = p.Description,
            //    CategoryId = p.CategoryId,
            //    AuthorId = p.AuthorId,
            //    AuthorFullName = p.Author.FullName,
            //    CategoryName = p.Category.Name
            //}).SingleOrDefaultAsync(p => p.Id == model.Id, cancellationToken);
            #endregion

            PostSelectDto resultDto = await _repository.TableNoTracking.ProjectTo<PostSelectDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id == model.Id, cancellationToken);

            return resultDto;
        }

        [HttpPut]
        public async Task<ApiResult<PostSelectDto>> Update(Guid id, PostDto dto, CancellationToken cancellationToken)
        {
            //var postDto = new PostDto();
            //Create
            //var post = postDto.ToEntity(); // DTO => Entity
            //Update
            //var updatePost = postDto.ToEntity(post); // DTO => Entity (an exist)
            //GetById
            //var postDto = PostDto.FromEntity(model); // Entity => DTO


            Post model = await _repository.GetByIdAsync(cancellationToken, id);

            //Mapper.Map(dto, model);
            model = dto.ToEntity(_mapper, model);

            #region old code
            //model.Title = dto.Title;
            //model.Description = dto.Description;
            //model.CategoryId = dto.CategoryId;
            //model.AuthorId = dto.AuthorId;
            #endregion

            await _repository.UpdateAsync(model, cancellationToken);

            #region old code
            //var resultDto = await _repository.TableNoTracking.Select(p => new PostDto
            //{
            //    Id = p.Id,
            //    Title = p.Title,
            //    Description = p.Description,
            //    CategoryId = p.CategoryId,
            //    AuthorId = p.AuthorId,
            //    AuthorFullName = p.Author.FullName,
            //    CategoryName = p.Category.Name
            //}).SingleOrDefaultAsync(p => p.Id == model.Id, cancellationToken);
            #endregion

            PostSelectDto resultDto = await _repository.TableNoTracking.ProjectTo<PostSelectDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(p => p.Id == model.Id, cancellationToken);

            return resultDto;
        }

        [HttpDelete("{id:guid}")]
        public async Task<ApiResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            Post model = await _repository.GetByIdAsync(cancellationToken, id);
            await _repository.DeleteAsync(model, cancellationToken);

            return Ok();
        }
    }
}
