using System;
using System.Collections.Generic;
using System.Text;

namespace Msdfa.Core.DB
{
    public abstract class ConnectionBase_Oracle : ConnectionBase
    {
        public override DatabaseType DatabaseType => DatabaseType.Oracle;
        public virtual long IncrPoolSize { get; set; } = 1;
        public virtual long DecrPoolSize { get; set; } = 1;

        public override string ConnectionString => $@"Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST ={this.Ip})(PORT={this.Port}))(CONNECT_DATA=(SERVICE_NAME={this.Sid})));User Id = {this.UserName}; Password={this.Pass};Max Pool Size={this.MaxPoolSize};Min Pool Size={this.MinPoolSize};Incr Pool Size={this.IncrPoolSize};Decr Pool Size={this.DecrPoolSize};Connection Timeout = 60";

        public ConnectionBase_Oracle() : base()
        {
            if (this.DefaultSchema != null) this.Query($"ALTER SESSION SET CURRENT_SCHEMA = :DEFAULT_SCHEMA")
                    .Bind("DEFAULT_SCHEMA", this.DefaultSchema)
                    .Execute();
        }
    }
}
