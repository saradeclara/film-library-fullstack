using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Enums;

namespace api.Helpers
{
    public class ResultWithData<T>
    {
        public Result Result { get; }
        public T? Data { get; }

        public ResultWithData(Result result, T? data)
        {
            Result = result;
            Data = data;
        }
    }
}