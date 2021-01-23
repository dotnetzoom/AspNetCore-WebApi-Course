using System;

namespace Common.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException()
            : base(ApiResultStatusCode.NotFound, System.Net.HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string message)
            : base(ApiResultStatusCode.NotFound, message, System.Net.HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(object additionalData)
            : base(ApiResultStatusCode.NotFound, null, System.Net.HttpStatusCode.NotFound, additionalData)
        {
        }

        public NotFoundException(string message, object additionalData)
            : base(ApiResultStatusCode.NotFound, message, System.Net.HttpStatusCode.NotFound, additionalData)
        {
        }

        public NotFoundException(string message, Exception exception)
            : base(ApiResultStatusCode.NotFound, message, exception, System.Net.HttpStatusCode.NotFound)
        {
        }

        public NotFoundException(string message, Exception exception, object additionalData)
            : base(ApiResultStatusCode.NotFound, message, System.Net.HttpStatusCode.NotFound, exception, additionalData)
        {
        }
    }
}
