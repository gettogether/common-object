using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Data.Interfaces
{
    public interface IPagingResult<T, C>
    {
        C Result
        {
            get;
            set;
        }

        int Total
        {
            get;
            set;
        }
    }
}
