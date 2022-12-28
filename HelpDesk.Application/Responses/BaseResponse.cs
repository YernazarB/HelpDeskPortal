using HelpDesk.Domain.Enums;

namespace HelpDesk.Application.Responses
{
    public class BaseResponse
    {
        public BaseResponse(ResponseCode code = ResponseCode.Ok, string errorMessage = null)
        {
            Code = code;
            ErrorMessage = errorMessage;
        }

        public ResponseCode Code { get; set; }
        public bool Success => Code == ResponseCode.Ok;
        public string ErrorMessage { get; set; }
    }

    public class BaseResponse<T> : BaseResponse where T : class
    {
        public BaseResponse(T data = null, ResponseCode code = ResponseCode.Ok, string errorMessage = null) : base(code, errorMessage)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}
