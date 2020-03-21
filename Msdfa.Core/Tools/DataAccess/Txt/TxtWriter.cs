using System.IO;

namespace Msdfa.Tools.DataAccess.Txt
{
    public class TxtWriter<T> : BaseDataWriter<T> where T : new()
    {
        protected TxtWriter()
            :base("\t")
        {}

        protected TxtWriter(StreamWriter streamWriter) 
            : base(streamWriter, "\t")
        {}

        public static TxtWriter<T> Create(string filePath)
        {
            AssertPathExists(filePath);
            var streamWriter = new StreamWriter(filePath);
            var txtWriter = new TxtWriter<T>(streamWriter);

            return txtWriter;
        }

        public static TxtWriter<T> Create()
        {
            return new TxtWriter<T>();
        }
    }
}
