using Microsoft.AspNetCore.Mvc;
using MedicalCallServer.Models;
using MedicalCallServer.Services;

namespace MedicalCallServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalAssistanceCallsController : ControllerBase
    {
        private readonly ITestDataService _testDataService;
        public MedicalAssistanceCallsController(ITestDataService testDataService) =>
            _testDataService = testDataService;

        [HttpGet]
        public IActionResult GetCalls() => Ok(_testDataService.GetTestCalls());

        [HttpGet("{id}")]
        public IActionResult GetCall(int id) =>
            _testDataService.GetTestCalls().FirstOrDefault(c => c.Id == id) is { } call
                ? Ok(call) : NotFound();

        [HttpPost]
        public IActionResult CreateCall([FromBody] MedicalAssistanceCall call)
        {
            var allCalls = _testDataService.GetTestCalls();

            call.Id = allCalls.Count > 0 ? allCalls.Max(c => c.Id) + 1 : 1;
            if (call.StatusId == 0) call.StatusId = (int)TCallStatus.csNew;

            allCalls.Add(call);            
            return CreatedAtAction(nameof(GetCall), new { id = call.Id }, call);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateCall(int id, [FromBody] MedicalAssistanceCall updatedCall)
        {
            var allCalls = _testDataService.GetTestCalls();
            var index = allCalls.FindIndex(c => c.Id == id);
            if (index == -1) return NotFound();
            
            updatedCall.Id = id;
            allCalls[index] = updatedCall;

            return Ok(updatedCall);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCall(int id)
        {
            var allCalls = _testDataService.GetTestCalls();
            var callToDelete = allCalls.FirstOrDefault(c => c.Id == id);
            if (callToDelete == null) return NotFound();

            allCalls.Remove(callToDelete);
            return NoContent();
        }

        [HttpGet("search")]
        public IActionResult SearchCalls(
            [FromQuery] string? status = null,
            [FromQuery] int? statusId = null,
            [FromQuery] CallType? callType = null,
            [FromQuery] DateTime? callDate = null)
        {
            var filtered = _testDataService.GetTestCalls().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(status))
                filtered = filtered.Where(c => c.Status.Contains(status, StringComparison.OrdinalIgnoreCase));
            if (statusId.HasValue)
                filtered = filtered.Where(c => c.StatusId == statusId.Value);
            if (callType.HasValue)
                filtered = filtered.Where(c => c.TypeOfCall == callType.Value);
            if (callDate.HasValue)
                filtered = filtered.Where(c => c.CallDate?.Date == callDate.Value.Date);

            return Ok(filtered.ToList());
        }

        [HttpGet("statuses")]
        public IActionResult GetStatuses()
        {
            var statuses = _testDataService.GetTestStatuses()
                .Select(s => new { id = (int)s, name = CallStatusHelper.GetName(s) })
                .ToList();
            return Ok(statuses);
        }
    }
}
