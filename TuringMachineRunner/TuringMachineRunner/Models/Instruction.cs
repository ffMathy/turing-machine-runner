using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TuringMachineRunner.Models
{
    class Instruction
    {
        public string GoToState { get; set; }
        public char WriteSymbol { get; set; }
        public Direction Direction { get; set; }
    }

    enum Direction {
        Left,
        Right,
        Halt
    }
}
