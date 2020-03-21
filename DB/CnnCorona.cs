using Msdfa.Core.DB;
using System;
using System.Data;

namespace CrownDetector.DB
{
    public class CnnCorona : ConnectionBase_Pgsql
    {
        public override string Ip => "braderek.no-ip.org";
        public override string Sid => "coronadb";
        public override long Port => 10432;
        public override string UserName => "msdfa";
        public override string Pass => "Morcinka43";
    }
}
