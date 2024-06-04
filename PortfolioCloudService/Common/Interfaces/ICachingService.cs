using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface ICachingService
    {
        bool IsCacheValid(string filePath);
        string ReadCachedData(string filePath);
        void WriteCachedData(string filePath, string data);
    }
}
