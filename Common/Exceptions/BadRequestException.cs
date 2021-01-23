using System;

namespace Common.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException()
            : base(ApiResultStatusCode.BadRequest, System.Net.HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message)
            : base(ApiResultStatusCode.BadRequest, message, System.Net.HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(object additionalData)
            : base(ApiResultStatusCode.BadRequest, null, System.Net.HttpStatusCode.BadRequest, additionalData)
        {
        }

        public BadRequestException(string message, object additionalData)
            : base(ApiResultStatusCode.BadRequest, message, System.Net.HttpStatusCode.BadRequest, additionalData)
        {
        }

        public BadRequestException(string message, Exception exception)
            : base(ApiResultStatusCode.BadRequest, message, exception, System.Net.HttpStatusCode.BadRequest)
        {
        }

        public BadRequestException(string message, Exception exception, object additionalData)
            : base(ApiResultStatusCode.BadRequest, message, System.Net.HttpStatusCode.BadRequest, exception, additionalData)
        {
        }
    }
}
