using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502emulador
{
    class Program
    {
        static void Main(string[] args)
        {
            c6502 pro = new c6502();
            Memory_manager memoria = new Memory_manager();
          memoria.LoadRom("osi_bas.bin");
       //   Memory_ehBasic memoria = new Memory_ehBasic();
         //   memoria.LoadRom("ehbasic.bin");
            pro.Memory = memoria;
            pro.Reset();
         
            while (true)
            {
                pro.ciclo();
            }


        }
    }
}
