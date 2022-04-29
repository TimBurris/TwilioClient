using System;
using System.Collections.Generic;
using System.Text;

namespace TwilTool.Services.Sms
{


    public class ExecutionResult
    {
        public bool WasSuccessful { get; set; }
        public string Message { get; set; }

        public static ExecutionResult SuccessResult()
        {
            return new ExecutionResult() { WasSuccessful = true };
        }
        public static ExecutionResult<T> SuccessResult<T>(T result)
        {
            return new ExecutionResult<T>() { WasSuccessful = true, Result = result };
        }
        public static ExecutionResult FailedResult(string message)
        {
            return new ExecutionResult() { Message = message };
        }
    }

    public class ExecutionResult<T> : ExecutionResult
    {
        public T Result { get; set; }

        public static ExecutionResult<T> SuccessResult(T result)
        {
            return new ExecutionResult<T>() { Result = result, WasSuccessful = true };
        }

        new public static ExecutionResult<T> FailedResult(string message)
        {
            return new ExecutionResult<T>() { Message = message };
        }
    }
}
