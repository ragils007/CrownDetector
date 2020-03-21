using System;
using System.Collections.Generic;
using System.Text;

namespace Msdfa.Core.DB
{
    public abstract class ConnectionBase_Pgsql : ConnectionBase
    {
        public override DatabaseType DatabaseType => DatabaseType.Postgres;
        public virtual int ConnectionIdleLifetime { get; set; } = 15;
        
        public override string ConnectionString => $@"Server={this.Ip};Port={this.Port};Database={this.Sid};User Id={this.UserName};Password = {this.Pass};Pooling=true;MinPoolSize={this.MinPoolSize};MaxPoolSize={this.MaxPoolSize};ConnectionIdleLifetime={this.ConnectionIdleLifetime};";
        
        public ConnectionBase_Pgsql() : base()
        {
            if (this.DefaultSchema != null) this.Query($"SET search_path TO {this.DefaultSchema}").Execute();
        }
    }
}
