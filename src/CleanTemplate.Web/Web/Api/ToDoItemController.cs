using Microsoft.AspNetCore.Mvc;
using CleanTemplate.Web.Web.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using CleanTemplate.Core.Interfaces;
using CleanTemplate.Core.Entity;

namespace CleanTemplate.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoItemsController : ControllerBase
    {
        private readonly IRepository _repository;

        /* Events written will include a property SourceContext with value 
         * "CleanTemplate.Web.Controllers.ToDoItemsController" that can 
         * later be used to filter out noisy events, or selectively write 
         * them to particular sinks.
         * 
         * Not all properties attached to an event need to be represented 
         * in the message template or output format; all properties are 
         * carried in a dictionary on the underlying LogEvent object. 
         */
        private ILogger myLog = Log.ForContext<ToDoItemsController>();

        public ToDoItemsController(IRepository repository)
        {
            _repository = repository;            
        }

        /// <summary>
        /// Returns a list of ToDoItems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ListToDoItems()
        {
            myLog.Information("This is a handler for {Path}", Request.Path);           

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
            myLog.Information("Getting item {ID}", id);
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
            var sampleDataLog = Log.ForContext("ToDoItemId", id);
            sampleDataLog.Information("Completing ToDoItem");  

            var toDoItem = _repository.GetById<ToDoItem>(id);
            toDoItem.MarkComplete();
            _repository.Update(toDoItem);

            sampleDataLog.Information("Finished");

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
                myLog.Error(ex, "Create new item failed");
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
            int recordsAdded = 0;//DatabasePopulator.PopulateDatabase(_repository);
            return Ok(recordsAdded);
        }
    }
}
