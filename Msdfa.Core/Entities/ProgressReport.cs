using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Msdfa.Core.Event;

namespace Msdfa.Core.Entities
{
    public class ProgressReport : Progress<ProgressReport.ProgressDetails>
    {
        public event EventHandler<ProgressDetailsEventArgs> ReportEmited;
        protected virtual void OnReportEmited(ProgressDetails progress) => this.ReportEmited?.Invoke(this, new ProgressDetailsEventArgs(progress));

        private int _currentPercentage = -1;
        private string _caption { get; set; } = "Analizuję...";

        private string _currentMessage { get; set; }

        public class ProgressDetails
        {
            public string MessageText => this.Percentage == -1
                ? this.Message
                : $"{this.Percentage.ToString().PadLeft(3)}: {this.Message}";
            
            public int Percentage { get; set; }
            public string Caption { get; set; }
            public string Message { get; set; }

            public override string ToString() => this.MessageText;
        }

        public ProgressReport() : base() { }
        public ProgressReport(Action<ProgressDetails> handler) : base(handler) { }

        public void SetCaption(string caption)
        {
            this._caption = caption;
        }

        public void SetReportText(string message)
        {
            this._currentMessage = message;
        }

        public void Report(string message = null)
        {
            if (message != null) this._currentMessage = message;

            var rep = new ProgressDetails { Percentage = this._currentPercentage, Message = message, Caption = this._caption };
            this.OnReport(rep);
            this.OnReportEmited(rep);
        }

        public void ReportDetail(string detailMessage)
        {
            if (string.IsNullOrEmpty(this._currentMessage)) throw new Exception("Current message not set. Call Report() first.");

            var rep = new ProgressDetails { Percentage = this._currentPercentage, Message = $"{this._currentMessage} [{detailMessage}]", Caption = this._caption };

            this.OnReport(rep);
            this.OnReportEmited(rep);
        }

        public void Report(int percentage, string message)
        {
            this._currentPercentage = percentage;
            this.Report(message);
        }

        public void ReportD(decimal percentage, string message)
        {
            this._currentPercentage = (int)Math.Truncate(percentage * 100);
            this.Report(message);
        }

        public void SetPercentage(int value)
        {
            this._currentPercentage = value;
            this.Report();
        }

        public void SetPercentageD(decimal value)
        {
            this.SetPercentage((int)Math.Truncate(value * 100));
        }
    }

    public class ProgressDetailsEventArgs
    {
        public ProgressReport.ProgressDetails Progress { get; set; }

        public ProgressDetailsEventArgs(ProgressReport.ProgressDetails progress)
        {
            this.Progress = progress;
        }
    }
}
