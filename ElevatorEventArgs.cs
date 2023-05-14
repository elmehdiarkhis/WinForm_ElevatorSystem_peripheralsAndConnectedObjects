using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elevator
{
  public  class ElevatorEventArgs
    {
        public readonly string message;

        public ElevatorEventArgs(string message)
        { 
            this.message = message; 
        }
    }
}
