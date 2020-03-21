using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Msdfa.Core.Entities
{
    public class ResultMessage
    {
        public ResultEnum Status { get; set; } = ResultEnum.Ok;
        public string Message { get; set; }

        public ResultMessage() { }
        public ResultMessage(ResultEnum status, string message = null)
        {
            this.Status = status;
            this.Message = message;
        }
    }

    public class ResultMessageDataList<TData> : ResultMessageList
        where TData: new()
    {
        public TData Data { get; set; }

        public ResultMessageDataList()
        {
            this.Data = new TData();
        }
    }

    public class ResultMessageList
    {
        public ResultEnum Status { get; set; } = ResultEnum.Ok;
        public List<string> MessageList { get; set; } = new List<string>();
        public List<string> ErrorList { get; set; } = new List<string>();
        public ProgressReport Progress { get; set; }

        public ResultMessageList() { }
        public ResultMessageList(ProgressReport pr) { this.Progress = pr; }
        public ResultMessageList(ResultEnum status, List<string> messageList = null)
        {
            this.Status = status;
            if (messageList != null) this.MessageList = messageList;
        }

        //public ResultMessageList CombineWith(ResultMessageList newList)
        //{
        //    var item = new ResultMessageList();
        //    item.Status = this.Status;
        //    item.MessageList.AddRange(this.MessageList);
        //    item.ErrorList.AddRange(this.ErrorList);

        //    if (newList.Status == ResultEnum.Error) item.Status = ResultEnum.Error;
        //    item.MessageList.AddRange(newList.MessageList);
        //    item.ErrorList.AddRange(newList.ErrorList);

        //    return item;
        //}

        public ResultMessageList CombineWith(ResultMessageList newList)
        {
            if (newList.Status == ResultEnum.Error) this.Status = ResultEnum.Error;
            newList.MessageList.ForEach(x => this.AddMessage(x));
            newList.ErrorList.ForEach(x => this.AddError(x));
            return this;
        }


        public void AddMessage(string msg, ProgressReport pr = null)
        {
            this.MessageList.Add(msg);
            if (pr != null) pr.Report(msg);
            else this.Progress?.Report(msg);
        }

        public void AddError(string msg, ProgressReport pr = null)
        {
            this.ErrorList.Add(msg);
            if (pr != null) pr.Report(msg);
            else this.Progress?.Report(msg);
        }

        public static ResultMessageList operator +(ResultMessageList a, ResultMessageList b)
        {
            if (a == null || b == null) throw new Exception("Invalid Operation (+) on null ResultMessageList property");
            return a.CombineWith(b);
        }
    }

    public class ResultMessageList<TStatus>
    {
        public TStatus Status { get; set; }
        public List<string> MessageList { get; set; } = new List<string>();

        public ResultMessageList() { }
        public ResultMessageList(TStatus status, List<string> messageList = null)
        {
            this.Status = status;
            if (messageList != null) this.MessageList = messageList;
        }

        public ResultMessageList<TStatus> CombineWith(ResultMessageList<TStatus> newList)
        {
            var item = new ResultMessageList<TStatus>();
            item.Status = this.Status;
            item.MessageList.AddRange(this.MessageList);
            item.MessageList.AddRange(newList.MessageList);
            return item;
        }
    }

    public class ResultList<TData>
    {
        public ResultEnum Status { get; set; } = ResultEnum.Ok;
        public List<string> Messages = new List<string>();
        public List<TData> Items = new List<TData>();
    }

    public class ResultItem<TData>
    {
        public ResultEnum Status { get; set; } = ResultEnum.Ok;
        public List<string> Messages = new List<string>();
        public TData Item { get; set; }
    }

    public class Result
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }

        public Result(bool isSuccess, string errorMessage)
        {
            this.IsSuccess = isSuccess;
            this.ErrorMessage = errorMessage;
        }

        public static Result<TEnum> Get<TEnum>(TEnum status, string message = null)
            where TEnum : struct, IConvertible
        {
            return new Result<TEnum>(status, message);
        }

        public static Result<TEnum, TData> Get<TEnum, TData>(TEnum status, TData data, string message = null)
            where TEnum : struct, IConvertible
            where TData : class
        {
            return new Result<TEnum, TData>(status, data, message);
        }
    }

    public class Result<TEnum>
    {
        public TEnum Status { get; set; }
        public string ErrorMessage { get; set; }

        public Result(TEnum status, string errorMessage = null)
        {
            this.Status = status;
            this.ErrorMessage = errorMessage;
        }
    }

    public class Result<TEnum, TData> : Result<TEnum>
    {
        public TData Data { get; set; }

        public Result(TEnum status, TData data, string errorMessage = null) : base(status, errorMessage)
        {
            this.Data = data;
        }
    }

    public enum ResultEnum
    {
        None,
        Ok,
        Warning,
        Error,
    }

}
