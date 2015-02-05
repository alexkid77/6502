using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502emulador
{

    public partial class c6502
    {
        delegate void direccionamiento();
        string debug_Str;
      public  cMemory Memory{get;set;}
        static byte FLAG_CARRY = 0x01;
        static byte FLAG_ZERO = 0x02;
        static byte FLAG_INTERRUPT = 0x04;
        static byte FLAG_DECIMAL = 0x08;
        static byte FLAG_BREAK = 0x10;
        static byte FLAG_CONSTANT = 0x20;
        static byte FLAG_OVERFLOW = 0x40;
        static byte FLAG_SIGN = 0x80;

        static ushort BASE_STACK = 0x100;

        public byte A;//reg acumulador
        public byte X;//reg indice x
        public byte Y;//reg indice Y

        public byte SP;///reg stack

        public ushort PC;//contador programa

        public byte reg_flags;//registro flags


        private ushort dir_resultado;//direccion de los diferentes direccionamientos
        private ushort dir_relativa;//direccion de salto relativa
    
        private byte opcode;
        void guarda_acumulador(ushort val)
        {
            this.A = (byte)(val & 0x00FF);//se guarda ultimo byte LSB
        }

        void guarda(ushort val)
        { //se puede guardar en ram o registro
            if (this.direccionamientos[this.opcode] == modos_dir.acc)
                this.guarda_acumulador(val);
            else
                this.Memory.Write(dir_resultado, (byte)(val & 0x00FF));
        }
        void setCarry()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_CARRY);
        }
        void clearCarry()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_CARRY);
        }

        void setZero()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_ZERO);
        }
        void clearZero()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_ZERO);
        }

        void setInterrupt()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_INTERRUPT);
        }
        void clearInterrupt()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_INTERRUPT);
        }


        void setDecimal()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_DECIMAL);
        }
        void clearDecimal()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_DECIMAL);
        }

        void setBreak()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_BREAK);
        }
        void clearBreak()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_BREAK);
        }

        void setConstant()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_CONSTANT);
        }
        void clearConstant()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_CONSTANT);
        }

        void setOverflow()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_OVERFLOW);
        }
        void clearOverflow()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_OVERFLOW);
        }

        void setSign()
        {
            this.reg_flags = (byte)(this.reg_flags | c6502.FLAG_SIGN);
        }
        void clearSign()
        {
            this.reg_flags = (byte)(this.reg_flags & ~c6502.FLAG_SIGN);
        }


        void zerocalc(ushort n)
        {
            if ((n & 0x00FF) >0)
                clearZero();
            else
                setZero();
         
               
        }


        void signcalc(ushort n)
        {
            if ((n & 0x0080) >=1) //si el ultimo bit es 1 se activa el negativo
                setSign();
            else
                clearSign();

        }
        void carrycalc(ushort n)
        {
            if ((n & 0xFF00) >=1)
                setCarry();
            else
                clearCarry();
        }

        void  overflowcalc(ushort n, byte m, ushort o) { /* n = result, m = accumulator, o = memory */
            if ((((n) ^ (ushort)(m)) & ((n) ^ (o)) & 0x0080)!=0) 
                setOverflow();
                else 
                clearOverflow();
        }

       

        /// pila

        void pushPC(ushort pc)
        {

            byte parte_alta=(byte)((pc >> 8) & 0xFF);
            byte parte_baja=(byte)(pc & 0xFF);

            ushort dir_alta=(ushort)(c6502.BASE_STACK + this.SP);
            ushort dir_baja=(ushort)(c6502.BASE_STACK +( (this.SP - 1) & 0xFF));
            this.Memory.Write(dir_alta,parte_alta );//guardar parte alta primero, primer byte
            this.Memory.Write(dir_baja,parte_baja );//guardar parte baja,byte menor peso
            this.SP -= 2;
          
        }

        void push(byte n)
        {
            this.Memory.Write((ushort)(c6502.BASE_STACK + this.SP--), n);
            //this.SP--;
        }

        byte pop()
        {
           byte res= this.Memory.Read((ushort)(c6502.BASE_STACK + (++this.SP)));
     
           return res;
        }

        ushort popPC()
        {
            ushort parte_baja = this.Memory.Read((ushort)(c6502.BASE_STACK + this.SP+1));
         
            ushort parte_alta = this.Memory.Read((ushort)(c6502.BASE_STACK + this.SP+2));
            this.SP+=2;
            return (ushort)( (parte_alta&0xFF) << 8 | (parte_baja&0xFF));//se une todo otra vez
        }

       public void Reset()
        {
            this.PC = (ushort)(((ushort)this.Memory.Read(0xFFFC)) | (((ushort)this.Memory.Read(0xFFFD)) << 8));//cargar direccion de boot almacenada en 0xFFFC(menor peso) y 0xFFFD(mayor peso) 
          
            this.A = 0;
            this.X = 0;
            this.Y = 0;
            this.SP= 0xFD;
            this.reg_flags |= FLAG_CONSTANT;
        }
        /// </summary>
        /// 
  
       public void Imp()
       { 
        //direccionamiento implicito
       }

       public void Acc()
       { 
        //dir acumulador
       }
       public void Imm()
       {
          
           dir_resultado = PC++;//inmediato
         //  PC++;
       }

        //Pagina cero
       public void zp()
       {
           dir_resultado = Memory.Read(PC++);
       }
        //pagina cero+X
       public void Zpx()
       {
           ushort temp = (ushort)(Memory.Read(PC++) + (ushort)this.X);
           temp =(ushort) (temp & 0xFF);
           dir_resultado = temp;
       }
       //pagina cero+Y
       public void Zpy()
       {
           ushort temp = (ushort)(Memory.Read(PC++) + (ushort)this.Y);
           temp = (ushort)(temp & 0xFF);
           dir_resultado = temp;
       }

       //Relativo suma o resta para saltos, byte en complemento a 2
       void Rel()
       {
           dir_relativa = Memory.Read(this.PC++);
           if ((dir_relativa & 0x80) == 0x80)
           {
               dir_relativa = (ushort)(dir_relativa | 0xFF00);
           }
       }

        //absolutto
        public void abso()
        {
            ushort baja = (ushort)Memory.Read(this.PC);
            ushort alta = (ushort)Memory.Read((ushort)(this.PC +1));
            alta = (ushort)(alta << 8);
            dir_resultado =  (ushort)(baja|alta);
            this.PC += 2;
        }

        //absoluto + X
        public void absx()
        {
            ushort baja = (ushort)Memory.Read(this.PC);
            ushort alta = (ushort)Memory.Read((ushort)(this.PC + 1));
            alta = (ushort)(alta << 8);
            dir_resultado = (ushort)(baja | alta);
            dir_resultado = (ushort)(dir_resultado + this.X);
            this.PC += 2;
        }

        //absoluto +Y
        public void absy()
        {
            ushort baja = (ushort)Memory.Read(this.PC);
            ushort alta = (ushort)Memory.Read((ushort)(this.PC + 1));
            alta = (ushort)(alta << 8);
            dir_resultado = (ushort)(baja | alta);
            dir_resultado = (ushort)(dir_resultado + this.Y);
            this.PC += 2;
        }
        //indirecto
        public void ind()
        {
            ushort baja = (ushort)Memory.Read(this.PC);
            ushort alta = (ushort)Memory.Read((ushort)(this.PC + 1));
            alta = (ushort)(alta << 8);
            ushort aux = (ushort)(baja | alta);
            ushort aux2 = (ushort)((aux & 0xFF00) | ((aux + 1) & 0x00FF));
            dir_resultado = (ushort)(Memory.Read(aux) | (Memory.Read(aux2)<<8));
            this.PC += 2;
        }
        //indirecto +X
        public void indx()
        {
            ushort aux;
            aux = (ushort)(Memory.Read(this.PC++) + this.X);
            aux = (ushort)(aux&0xFF);
            dir_resultado = (ushort)(Memory.Read((ushort)(aux & 0x00FF)) | (Memory.Read((ushort)((aux+1)&0x00FF)) <<8));
        }
        //indirecto + y
        public void indy()
        {
            ushort aux = Memory.Read(this.PC++);
            ushort aux2 =(ushort) ((aux & 0xFF00) | ((aux + 1) & 0x00FF));
        
            dir_resultado = (ushort)(Memory.Read(aux) | (Memory.Read(aux2) << 8));
            dir_resultado += (ushort)this.Y;
        }
        private ushort getvalue()
        {
            modos_dir dir = this.direccionamientos[opcode];
            if (dir == modos_dir.acc)
                return (ushort)this.A;
            else
                return (ushort)this.Memory.Read(dir_resultado);
          //  this.direccionamientos[this.opcode];
        }

        private modos_dir ejecuta_direccionamiento()
        {
            modos_dir dir = this.direccionamientos[this.opcode];
            switch (dir)
            {
                case modos_dir.abso:
                    this.abso();
                    break;
                case modos_dir.absx:
                    this.absx();
                    break;
                case modos_dir.absy:
                    this.absy();
                    break;
                case modos_dir.acc:
                    this.Acc();

                    break;
                case modos_dir.imm:
                    this.Imm();
                    break;
                case modos_dir.imp:
                    this.Imp();
                    break;
                case modos_dir.ind:
                    this.ind();
                    break;
                case modos_dir.indx:
                    this.indx();
                    break;
                case modos_dir.indy:
                    this.indy();
                    break;
                case modos_dir.rel:
                    this.Rel();
                    break;
                case modos_dir.zp:
                    this.zp();
                    break;
                case modos_dir.zpx:
                    this.Zpx();
                    break;
                case modos_dir.zpy:
                    this.Zpy();
                    break;
                default:
                    Console.Write("error");
                    break;

            }
            return dir;
        }

     
        ushort getvalue16()
        {
            ushort possig =(ushort) this.Memory.Read((ushort)(dir_resultado + 1));
            possig = (ushort)(possig << 8);
            return (ushort) (this.Memory.Read(dir_resultado) | possig );
        }
        private void inicia()
        {
            //direccionamiento ptr_imp = new direccionamiento(imp);
        }

      public  void ciclo()
        {
            this.opcode = this.Memory.Read(this.PC++);
            this.reg_flags |= FLAG_CONSTANT;
            this.ejecuta_direccionamiento();
            this.ejecuta_opcode();

        //  Console.WriteLine("A:0x{0} Y:0x{3} PC:0x{1} SP:0x{2}", this.A.ToString("x2"),this.PC.ToString("x2"),this.SP.ToString("x"),this.Y.ToString("x"));
          //  Console.WriteLine(operacion.ToString());

           // this.PC++;
        }
    
    }
}
