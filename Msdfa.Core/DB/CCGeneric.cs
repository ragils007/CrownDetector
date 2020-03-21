using Msdfa.Core.Unity;
using Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Msdfa.Core.DB
{
    /// <summary>
    /// Klasa jest wrapperem klasy Connection. W zależności od tego, czy została stworzona z istniejącym połączeniem
    ///     czy stworzyła własne - będzie odpowiednio zarządzać transakcjami oraz zamykaniem tego połączenia.
    /// </summary>
    [DebuggerDisplay("{this.Cnn.DbDescription}")]
    public abstract class CCGeneric<TConnection> : IDisposable
        where TConnection : IConnection
    {
        public abstract Func<TConnection> GetConnectionFunc { get; }

        //static ConnectionContext()
        //{
        //    UnityConfig.Container.RegisterType<IConnection, TConnection>();
        //}

        public TConnection Cnn { get; private set; }

        public readonly bool IsInOuterTransaction;
        public bool IsLocal { get; }

        // Ze względu na cache, loader jest inicjowany przy każdym użyciu jako nowy obiekt.
        //private Loader Loader => new Loader(this.Cnn);
        //public TDataType LoadOrGetItem<TDataType>(Expression<Func<object, TDataType>> item) where TDataType : class, IIdEntity => this.Loader.GetOrLoadItem(item);

        //public string DebugMessage { get; set; }

        public CCGeneric(IConnection cnn = null, [CallerMemberName] string callerName = "", [CallerFilePath] string filePath = "", [CallerLineNumber] int lineNumber = 0)
        {
            this.IsLocal = (cnn == null || cnn.Db.IsDisposed);
            this.IsInOuterTransaction = cnn?.Db.IsInTransaction ?? false;

            var methodName = callerName;
            var fileName = filePath.Split('\\').Last();
            var line = lineNumber;
            //var debugMessage = $"Metoda: '{name}' Plik: '{fileName}' Linia: {line}";
            var fileNameLine = $"{fileName} ({line})";
            var debugMessage = $"{fileNameLine.PadRight(50)} {methodName}";

            if (this.IsLocal)
            {
                //if (UnityConfig.Container.IsRegistered<IConnection>() == false) throw new Exception("Call CC.Configure<>() first to register database connection.");
                //this.Cnn = UnityConfig.Container.Resolve<IConnection>();
                this.Cnn = this.GetConnectionFunc();
            }
        }

        public void TransactionBegin_IfNotInOuterTransaction()
        {
            if (!this.IsInOuterTransaction) this.Cnn.Db.BeginTransaction();
        }

        public void TransactionCommit_IfNotInOuterTransaction()
        {
            if (!this.IsInOuterTransaction) this.Cnn.Db.CommitTransaction();
        }

        public void TransactionRollback_IfNotInOuterTransaction()
        {
            if (!this.IsInOuterTransaction) this.Cnn.Db.RollbackTransaction();
        }

        public bool IsInTransaction
        {
            get
            {
                return this.Cnn.Db.IsInTransaction;
            }
        }

        public void Dispose()
        {
            if (this.IsLocal) this.Cnn.Dispose();
        }

        public DateTime GetDBTime() => this.Cnn.Db.GetDBTime();

        //public static void Configure<TConnection>(IUnityContainer container) where TConnection : IConnection
        //{
        //    container.RegisterType<IConnection, TConnection>();
        //}
    }
}