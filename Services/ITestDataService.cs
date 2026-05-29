using System.Collections.Generic;
using MedicalCallServer.Models;

namespace MedicalCallServer.Services
{
    public interface ITestDataService
    {
        List<MedicalAssistanceCall> GetTestCalls();
        List<TCallStatus> GetTestStatuses();
    }
}