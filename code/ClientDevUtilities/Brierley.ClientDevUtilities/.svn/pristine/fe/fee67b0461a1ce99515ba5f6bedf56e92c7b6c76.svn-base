using System;
using System.Data;

namespace Brierley.FrameWork.Data.Sql
{
    public class LWDataReader : IDisposable
    {
        private IDataReader _reader;
        private bool _disposed = false;

        public LWDataReader(IDataReader reader)
        {
            _reader = reader;
        }

        public IDataReader Reader
        {
            get { return _reader; }
        }

        public bool Next()
        {
            if (_reader != null && !_reader.IsClosed)
            {
                return _reader.Read();
            }
            return false;
        }

        public object GetData(string fieldName)
        {
            return _reader[fieldName];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_reader != null && !_reader.IsClosed)
                    {
                        _reader.Close();
                    }
                }
                _disposed = true;
            }
        }
    }
}
