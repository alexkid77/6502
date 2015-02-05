using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502emulador
{
    public abstract class  cMemory
    {
        public abstract byte Read(ushort dir);
        public abstract void Write(ushort dir,byte datos);
    }
}
