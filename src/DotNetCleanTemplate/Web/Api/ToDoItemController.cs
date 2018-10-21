using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DotNetCleanTemplate.Core;
using DotNetCleanTemplate.Core.Entity;
using DotNetCleanTemplate.Core.Interfaces;
using DotNetCleanTemplate.Events;
using DotNetCleanTemplate.Web.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetCleanTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger _logger;

        public ToDoItemsController(IRepository repository, ILogger<ToDoItemsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of ToDoItems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ListToDoItems()
        {
            _logger.LogInformation(LoggingEventsConstants.GetItem, "Getting a list of items");

            var items = _repository.List<ToDoItem>()
                            .Select(ToDoItemDTO.FromToDoItem);
            return Ok(items);
        }

        /// <summary>
        /// Returns an individual ToDoItem by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public IActionResult GetToDoItemById(int id)
        {
            _logger.LogInformation(LoggingEventsConstants.GetItem, "Getting item {ID}", id);
            var item = ToDoItemDTO.FromToDoItem(_repository.GetById<ToDoItem>(id));
            return Ok(item);
        }

        /// <summary>
        /// Creates a new ToDoItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateToDoItem([FromBody] ToDoItemDTO item)
        {
            var todoItem = new ToDoItem()
            {
                Title = item.Title,
                Description = item.Description
            };
            _repository.Add(todoItem);
            return Ok(ToDoItemDTO.FromToDoItem(todoItem));
        }

        /// <summary>
        /// Updates an existing ToDo item by replacing it
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPut]
        public IActionResult UpdateToDoItem([FromBody] ToDoItemDTO item)
        {
            var todoItem = new ToDoItem()
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description
            };
            _repository.Update(todoItem);
            return Ok(ToDoItemDTO.FromToDoItem(todoItem));
        }

        /// <summary>
        /// Updates a ToDo item by marking it complete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/complete")]
        public IActionResult Complete(int id)
        {
            var toDoItem = _repository.GetById<ToDoItem>(id);
            toDoItem.MarkComplete();
            _repository.Update(toDoItem);

            return Ok(ToDoItemDTO.FromToDoItem(toDoItem));
        }

        /// <summary>
        /// Always returns an error for testing purposes
        /// </summary>
        /// <returns></returns>
        [HttpGet("error")]
        public IActionResult ShowAnError()
        {
            try
            {
                throw new Exception("Something really, really, bad happened!");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(LoggingEventsConstants.InsertItem, ex, "Create new item failed");
            }

            return NotFound();
        }

        /// <summary>
        /// Generate sample data in the repository
        /// </summary>
        /// <returns></returns>
        [HttpGet("samples")]
        public IActionResult CreateSampleData()
        {
            int recordsAdded = DatabasePopulator.PopulateDatabase(_repository);
            return Ok(recordsAdded);
        }
    }
}
