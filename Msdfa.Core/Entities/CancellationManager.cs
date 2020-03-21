using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Msdfa.Core.Event;

namespace Msdfa.Core.Entities
{
    public class CancellationManager
    {
        public event EventHandler<ConfirmationRequestedEventArgs> CancelConfirmationRequested;
        protected virtual void OnConfirmationRequested(ConfirmationRequestedEventArgs e) => this.CancelConfirmationRequested?.Invoke(this, e);
        protected virtual void OnConfirmationRequested(string msg) => this.CancelConfirmationRequested?.Invoke(this, new ConfirmationRequestedEventArgs(msg));

        private readonly CancellationTokenSource _cancellationTokenSource;
        public CancellationToken Token => this._cancellationTokenSource.Token;

        public bool IsCancelRequested { get; set; }

        public CancellationManager()
        {
            this._cancellationTokenSource = new CancellationTokenSource();
        }

        public void CheckCancel()
        {
            if (!this.IsCancelRequested) return;

            var e = new ConfirmationRequestedEventArgs("Czy na pewno przerwać?");
            this.OnConfirmationRequested(e);

            this.IsCancelRequested = false;
            if (e.IsAccepted.HasValue && e.IsAccepted.Value == true)
            {
                this._cancellationTokenSource.Cancel();
                throw new Exception("Przerwano operację.");
            }
        }

        public void RequestCancelConfirmation()
        {
            var e = new ConfirmationRequestedEventArgs("Czy na pewno przerwać?");
            this.OnConfirmationRequested(e);

            this.IsCancelRequested = false;
            if (e.IsAccepted.HasValue && e.IsAccepted.Value == true)
            {
                this._cancellationTokenSource.Cancel();
                throw new Exception("Przerwano operację.");
            }
        }
    }
}
