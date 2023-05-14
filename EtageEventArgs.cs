using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator
{
    public class EtageEventArgs
    {

        public readonly string message;

        public EtageEventArgs(string message)
        {
            this.message = message;
        }


    }
}
