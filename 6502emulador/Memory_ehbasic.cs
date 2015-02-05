
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502emulador
{
    public class Memory_ehBasic : cMemory
    {
    
        const ushort BASE_ROM = 0xC000;
        private byte[] rom;//= {0xA9,0x7f,0x69,0x01};
        public byte[] ram = new Byte[48 * 1024];
  
        public override byte Read(ushort dir)
        {
       
            if (dir >= 0 && dir < 0xC000)
            {
                return ram[dir] ;
            }
           
            if (dir >= 0xC000 )
            {
                if (dir == 0xF004)
                {
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                       
                      
                       
                        return Convert.ToByte(key.KeyChar);
                    }
                }
                return rom[dir-BASE_ROM];
            }
           
               return 0;
            
        }
        public void LoadRom(string fich)
        {
            if (rom == null)
            {
                byte[] fileBytes = File.ReadAllBytes(fich);
                this.rom = fileBytes;
            }
        }
        public override void Write(ushort dir, byte datos)
        {
            /* if (dir < 0x3fff)
                 ram[dir]=datos;*/
            int xll=0;
         
            ushort sel_memoria = (ushort)(dir & 0xF000);
           
            if (dir == 0xF001)
            {
                Console.Write(Convert.ToChar(datos));
                return;
            }
          
            if (dir >= 0 && dir < 0xC000)
            {
                ram[dir] = datos;
            }
            
           
         
         

        }
    }
}
