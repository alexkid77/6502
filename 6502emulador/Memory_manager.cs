using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502emulador
{
    public class Memory_manager : cMemory
    {
        byte acia_Estado;
        const ushort BASE_ROM = 0xC000;
        private byte[] rom;//= {0xA9,0x7f,0x69,0x01};
        public byte[] ram = new Byte[32 * 1024];
        byte buffer_teclado;
        public override byte Read(ushort dir)
        {
           
            ushort sel_memoria = (ushort)(dir & 0xF000);
               switch (sel_memoria)
               {
                   case 0x0000:
                   case 0x1000:
                   case 0x2000:
                   case 0x3000:
                   case 0x4000:
                   case 0x5000:
                   case 0x6000:
                   case 0x7000:
                       return ram[dir];
                   case 0xA000:
                       //ACIA
                       if (dir == 0xA000)
                       {
                           if (Console.KeyAvailable)
                           {
                              ConsoleKeyInfo key = Console.ReadKey(true);
                             buffer_teclado= Convert.ToByte( key.KeyChar);
                              return 3;
                           }
                           return 2;
                       }
                       if (dir == 0xA001)
                       {
                           return buffer_teclado;
                       }

                       
                       break;

                   case 0xC000:///C
                   case 0xD000:///D
                   case 0xE000://E
                   case 0xF000://F
                       ushort cal =(ushort)( dir-BASE_ROM);
                       return rom[cal];
                   
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
            int jj = 0;
            ushort sel_memoria = (ushort)(dir & 0xF000);
            switch (sel_memoria)
            {
               
                case 0x0000:
                case 0x1000:
                case 0x2000:
                case 0x3000:
                case 0x4000:
                case 0x5000:
                case 0x6000:
                case 0x7000:
                    if (dir > 0x1000)
                        jj++;
                    ram[dir] = datos;
                    break;
                case 0xA000:
                    //ACIA
                    if (dir == 0xA000)
                    {
                        acia_Estado = datos;
                    }
                    if (dir == 0xA001)
                    {
                        Console.Write(Convert.ToChar(datos));
                    }

                    break;
                case BASE_ROM:
                    ushort cal = (ushort)(dir - BASE_ROM);
                    rom[cal] = datos;
                    break;

            }

        }
    }
}
