using System;
using System.Collections.Generic;

namespace ComputationalLinguistics.Tools
{
    public static class ExceptionBuilder
    {
        public static IEnumerable<string> GetExceptionMessages(Exception ex)
        {
            var msg = new List<string>();
            msg.Add(ex.Message);

            var e = ex.InnerException;
            while (e != null)
            {
                msg.Add(e.Message);
                e = e.InnerException;
            }

            return msg;
        }
    }
}