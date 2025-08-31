using Microsoft.AspNetCore.Mvc;
using Temporalio.Client;
using System;
using System.Threading.Tasks;

namespace TemporalWebApi.Controllers
{
    [ApiController]
    [Route("orchestrations")]
    public class OrchestrationsController : ControllerBase
    {
        private readonly TemporalClient _client;
        private readonly string _taskQueue = "webapi-queue";

        public OrchestrationsController(TemporalClient client)
        {
            _client = client;
        }

        // POST /orchestrations/start?input=foo&instanceId=abc-123
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromQuery] string input, [FromQuery] string? instanceId = null)
        {
            var id = string.IsNullOrWhiteSpace(instanceId) ? $"abc-{Guid.NewGuid()}" : instanceId;

            var handle = await _client.StartWorkflowAsync(
                (AbcWorkflow wf) => wf.RunAsync(input),
                new(id: id, taskQueue: _taskQueue));

            var baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

            return Ok(new
            {
                id,
                runtimeStatusUrl = $"{baseUrl}/orchestrations/{id}",
                statusQueryGetUri = $"{baseUrl}/orchestrations/{id}",
                sendEventPostUri = $"{baseUrl}/orchestrations/{id}/raiseEvent/Approve",
                terminatePostUri = $"{baseUrl}/orchestrations/{id}/terminate",
                resultGetUri = $"{baseUrl}/orchestrations/{id}/result"
            });
        }

        // GET /orchestrations/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStatus(string id)
        {
            var handle = _client.GetWorkflowHandle<AbcWorkflow>(id);
            var desc = await handle.DescribeAsync();

            object? query = null;
            try { query = await handle.QueryAsync(wf => wf.GetStatus()); } catch { }

            return Ok(new
            {
                id,
                runtimeStatus = desc.Status.ToString(),
                startTimeUtc = desc.StartTime,
                closeTimeUtc = desc.CloseTime,
                historyLength = desc.HistoryLength,
                statusPayload = query
            });
        }

        // POST /orchestrations/{id}/raiseEvent/{signalName}
        [HttpPost("{id}/raiseEvent/{signalName}")]
        public async Task<IActionResult> RaiseEvent(string id, string signalName)
        {
            var handle = _client.GetWorkflowHandle(id); // �^�Ȃ��n���h��
            await handle.SignalAsync(signalName, Array.Empty<object?>()); // �� ����ŕ�����V�O�i���łɊm��
            return Ok(new { id, signaled = signalName });
        }

        //// POST /orchestrations/{id}/raiseEvent/{signalName}
        //// �{�f�B�� JSON 1������z��i�����Ȃ� object?[] �ɓW�J�j
        //[HttpPost("{id}/raiseEvent/{signalName}")]
        //public async Task<IActionResult> RaiseEvent(string id, string signalName, [FromBody] object? payload)
        //{
        //    var handle = _client.GetWorkflowHandle(id);
        //    var args = payload is null ? Array.Empty<object?>() : new object?[] { payload };
        //    await handle.SignalAsync(signalName, args); // �����I�� args ��n���̂��R�c
        //    return Ok(new { id, signaled = signalName });
        //}

        // POST /orchestrations/{id}/terminate
        [HttpPost("{id}/terminate")]
        public async Task<IActionResult> Terminate(string id, [FromQuery] string? reason = null)
        {
            var handle = _client.GetWorkflowHandle(id);
            await handle.TerminateAsync(reason ?? "terminated by API");
            return Ok(new { id, terminated = true });
        }

        // GET /orchestrations/{id}/result
        [HttpGet("{id}/result")]
        public async Task<IActionResult> GetResult(string id)
        {
            var handle = _client.GetWorkflowHandle<AbcWorkflow>(id);
            var result = await handle.GetResultAsync<string>();
            return Ok(new { id, result });
        }
    }
}
