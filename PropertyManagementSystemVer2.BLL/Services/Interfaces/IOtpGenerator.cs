using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystemVer2.BLL.Services.Interfaces
{
    public interface IOtpGenerator
    {
        string Generate(int length = 6);
    }
}
