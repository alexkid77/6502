using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502emulador
{
    public partial class c6502
    {


        void adc()//suma con acarreo
        {
            ushort valor = this.getvalue();
            ushort resultado = (ushort)((ushort)this.A + valor + (ushort)(this.reg_flags & FLAG_CARRY));//suma al acumulador el valor mas acarreo si hay
            carrycalc(resultado);
            zerocalc(resultado);
            overflowcalc(resultado, this.A, valor);
            signcalc(resultado);


            this.guarda_acumulador(resultado);
        }

        //and logico
        void and()
        {
            ushort valor = this.getvalue();
            ushort resultado = (ushort)(this.A & valor);
            zerocalc(resultado);
            signcalc(resultado);
            this.guarda_acumulador(resultado);

        }
        // bit a la izquierda
        void asl()
        {
            ushort valor = this.getvalue();
            ushort resultado = (ushort)(valor << 1);
            carrycalc(resultado);
            zerocalc(resultado);
            signcalc(resultado);
            this.guarda(resultado);

        }

        //salto sin carry
        void bcc()
        {
            if ((this.reg_flags & FLAG_CARRY) == 0)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }

        //salto con carry
        void bcs()
        {
            if ((this.reg_flags & FLAG_CARRY) == FLAG_CARRY)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }
        //salto cuando es cero
        void beq()
        {
            if ((this.reg_flags & FLAG_ZERO) == FLAG_ZERO)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }

        void bit()
        {
            ushort valor = this.getvalue();
            ushort resultado = (ushort)(this.A & valor);
            zerocalc(resultado);
            this.reg_flags = (byte)(reg_flags & 0x3F | (byte)(valor & 0xC0));
        }

        //salta si es negativo
        void bmi()
        {
            if ((this.reg_flags & FLAG_SIGN) == FLAG_SIGN)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }

        //salta si no es 0
        void bne()
        {
            if ((this.reg_flags & FLAG_ZERO) == 0)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }


        //salta si es positivo
        void bpl()
        {
            if ((this.reg_flags & FLAG_SIGN) == 0)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }

        //interrupcion
        void brk()
        {
            this.PC++;
            this.pushPC(this.PC);
            push((byte)(this.reg_flags | FLAG_BREAK));
            setInterrupt();
            this.PC = (ushort)((ushort)this.Memory.Read(0xFFFE) | (ushort)(this.Memory.Read(0xFFFF) << 8));
        }
        //salto sin overflow
        void bvc()
        {
            if ((this.reg_flags & FLAG_OVERFLOW) == 0)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }

        //salta si hay overflow
        void bvs()
        {
            if ((this.reg_flags & FLAG_OVERFLOW) == FLAG_OVERFLOW)
            {
                ushort pcaux = this.PC;
                this.PC += dir_relativa;
            }
        }
        //limpia flag carry
        void clc()
        {
            this.clearCarry();
        }
        //limpia flag decimal
        void cld()
        {
            this.clearDecimal();
        }

        //limpia flag interrupcion
        void cli()
        {
            this.clearInterrupt();
        }

        //limpia overflow
        void clv()
        {
            clearOverflow();
        }


        //comparar (hace una resta)
        void cmp()
        {
            //restar y no guardas,actualizar flags
            ushort valor = getvalue();
            ushort resultado = (ushort)((ushort)this.A - valor);
            if (this.A >= (byte)(valor & 0x00FF))
                this.setCarry();
            else
                clearCarry();


            if (this.A == (byte)(valor & 0x00FF))
                this.setZero();
            else
                this.clearZero();
            this.signcalc(resultado);
        }

        //compara A con X
        void cpx()
        {
            ushort valor = getvalue();
            ushort resultado = (ushort)((ushort)this.X - (ushort)valor);

            if (this.X >= (byte)(valor & 0x00FF))
                this.setCarry();
            else
                clearCarry();


            if (this.X == (byte)(valor & 0x00FF))
                this.setZero();
            else
                this.clearZero();
            this.signcalc(resultado);
        }
        //compara A con X
        void cpy()
        {
            ushort valor = getvalue();
            ushort resultado = (ushort)((ushort)this.Y - (ushort)valor);

            if (this.Y >= (byte)(valor & 0x00FF))
                this.setCarry();
            else
                clearCarry();

            if (this.Y == (byte)(valor & 0x00FF))
                this.setZero();
            else
                this.clearZero();
            this.signcalc(resultado);
        }
        //decrementa A
        void dec()
        {
            ushort valor = getvalue();
            ushort resultado = (ushort)(valor - 1);
            this.zerocalc(resultado);
            this.signcalc(resultado);
            this.guarda(resultado);
        }

        //decrementa X
        void dex()
        {
            this.X--;
            this.zerocalc(this.X);
            this.signcalc(this.X);
        }
        //decrementa Y
        void dey()
        {
            this.Y--;
            this.zerocalc(this.Y);
            this.signcalc(this.Y);
        }

        //hacer xor 
        void eor()
        {
            //xor;
            ushort valor = this.getvalue();
            ushort res = (ushort)((ushort)(this.A) ^ valor);
            this.zerocalc(res);
            this.signcalc(res);
            this.guarda_acumulador(res);
        }
        //incrementa
        void inc()
        {
            ushort valor = this.getvalue();
            ushort res = (ushort)(valor + 1);
            this.zerocalc(res);
            this.signcalc(res);
            this.guarda(res);

        }
        //incrementa x
        void inx()
        {
            this.X++;
            this.zerocalc(this.X);
            this.signcalc(this.X);
        }

        //incrementa y
        void iny()
        {
            this.Y++;
            this.zerocalc(this.Y);
            this.signcalc(this.Y);
        }

        //salta
        void jmp()
        {
            this.PC = dir_resultado;
        }
        //salto a una subrutina(guarda el pc para volver luego)
        void jsr()
        {
            this.pushPC((ushort)(this.PC - 1));
            this.PC = dir_resultado;
        }

        //load
        void lda()
        {
            ushort valor = this.getvalue();
            this.A = (byte)(valor & 0x00FF);
            this.zerocalc(this.A);
            this.signcalc(this.A);
        }
        //load en X
        void ldx()
        {
            ushort valor = this.getvalue();
            byte x = (byte)(valor & 0x00FF);
            this.X = x;
            this.zerocalc(x);
            this.signcalc(x);
        }
        //load en Y
        void ldy()
        {
            ushort valor = this.getvalue();
            this.Y = (byte)(valor & 0x00FF);
            this.zerocalc(this.Y);
            this.signcalc(this.Y);

        }

        //shift a la derecha 1 bit
        void lsr()
        {
            ushort valor = this.getvalue();
            ushort res =(ushort)( valor >> 1);
            if ((valor & 1) == 1)//activa carry si el bit desplazado es 1
            {
                this.setCarry();
            }
            else
            {
                this.clearCarry();
            }

            this.zerocalc(res);
            this.signcalc(res);
            this.guarda(res);
        }
        //nop
        void nop()
        { 
        
        }

        //or logico
        void ora()
        {
            ushort valor = this.getvalue();
            ushort res = (ushort)((ushort)(this.A) | valor);
            this.zerocalc(res);
            this.signcalc(res);
            this.guarda_acumulador(res);
        
        }

        //apila A
        void pha()
        {
            this.push(this.A);
        }

        //apila los flags
        void php()
        {
            this.push((byte)(this.reg_flags | FLAG_BREAK));
        }
        //desapila 
        void pla()
        {
            this.A = pop();
            this.zerocalc(this.A);
            this.signcalc(this.A);
        }

        //deapila flags
        void plp()
        {
            this.reg_flags = (byte)(this.pop() | FLAG_CONSTANT);
        }

        //rotacion a la izquierda
        void rol()
        {
            ushort valor = getvalue();
            ushort res = (ushort)((ushort)(valor << 1) | ((ushort)(this.reg_flags & FLAG_CARRY)));
            this.carrycalc(res);
            this.zerocalc(res);
            this.signcalc(res);

            this.guarda(res);
        }
        //rotacion a la derecha
        void ror()
        {
            ushort valor = getvalue();
            ushort res = (ushort)((ushort)(valor >> 1) | ((ushort)(this.reg_flags & FLAG_CARRY) << 7));
            if ((valor & 1) == 1)
                this.setCarry();
            else
                this.clearCarry();
            this.zerocalc(res);
            this.signcalc(res);

            this.guarda(res);
        }
        //retorna interrupcion
        void rti()
        {
            this.reg_flags = pop();
            ushort temp=popPC();
            this.PC = temp;
        }
        void rts()//volver subrutina
        {
            ushort pc_temp = popPC();
            this.PC =(ushort)( pc_temp+1);
        }

        void sbc()//resta
        {
            ushort valor = (ushort)(getvalue() ^ 0x00FF);
            ushort res = (ushort)(this.A + valor + (ushort)(this.reg_flags & FLAG_CARRY));
            this.carrycalc(res);
            this.zerocalc(res);
            this.overflowcalc(res, this.A, valor);
            this.signcalc(res);
            this.guarda_acumulador(res);

        }
        //activa registros
        void sec()
        {
            this.setCarry();
        }
        void sed()
        {
            this.setDecimal();
        }

        void sei()
        {
            setInterrupt();
        }

        //guarda A
        void sta()
        {
            this.guarda(this.A);
        }
        //guarda stx
        void stx()
        {
            this.guarda(this.X);
        }

        //guarda sty
        void sty()
        {
            this.guarda(this.Y);
        }
        //mueve X a A
        void tax()
        {
            this.X=this.A;
            this.zerocalc(this.X);
            this.signcalc(this.X);

        }
        //mueve A a Y
        void tay()
        {
            this.Y = this.A;
            this.zerocalc(this.Y);
            this.signcalc(this.Y);
        }
        //mueve x al stack
        void tsx()
        {
            this.X = this.SP;
            zerocalc(this.X);
            signcalc(this.X);
        }
        //mueve A a X
        void txa()
        {
            this.A = this.X;
            zerocalc(this.A);
            signcalc(this.A);
        }
        //mueve STACK a X
        void txs()
        {
            this.SP = this.X;
        }
        //mueve Y a A
        void tya()
        {
            this.A = this.Y;
            zerocalc(this.A);
            signcalc(this.A);
        }

      
        private opc ejecuta_opcode()
        {

            opc operacion = this.op[this.opcode];
         //  Console.WriteLine(this.PC.ToString("x")+" "+operacion.ToString());
            switch (operacion)
            {
                case opc.tya:
                    this.tya();
                    break;
                case opc.txs:
                    this.txs();
                    break;
                case opc.txa:
                    this.txa();
                    break;
                case opc.tsx:
                    this.tsx();
                    break;
                 
                case opc.tay:
                    this.tay();
                    break;
                case opc.tax:
                    this.tax();
                    break;
                case opc.sty:
                    this.sty();
                    break;
                case opc.stx:
                    this.stx();
                    break;
                case opc.sta:
                    this.sta();
                    break;

                case opc.sei:
                    this.sei();
                    break;
                case opc.sed:
                    this.sed();
                    break;
                case opc.sec:
                    this.sec();
                    break;
                    
                case opc.sbc:
                    this.sbc();
                    break;
                case opc.rti:
                    this.rti();
                    break;
                case opc.rts:
                    this.rts();
                    break;
                case opc.ror:
                    this.ror();
                    break;
                case opc.rol:
                    this.rol();
                    break;
                case opc.plp:
                    this.plp();
                    break;

                case opc.pha:
                    this.pha();
                    break;
                case opc.php:
                    this.php();
                    break;
                case opc.pla:
                    this.pla();
                    break;
                case opc.ora:
                    this.ora();
                    break;
                case opc.nop:
                    this.nop();
                    break;
                case opc.lsr:
                    this.lsr();
                    break;
             
                case opc.ldy:
                    this.ldy();
                    break;
                case opc.lda:
                    this.lda();
                    break;
                case opc.ldx:
                    this.ldx();
                    break;

                case opc.jmp:
                    this.jmp();
                    break;
                case opc.jsr:
                    this.jsr();
                    break;
                case opc.inc:
                    this.inc();
                    break;
                case opc.inx:
                    this.inx();
                    break;
                case opc.iny:
                    this.iny();
                    break;
                case opc.eor:
                    this.eor();
                    break;
                case opc.dey:
                    this.dey();
                    break;
                case opc.dex:
                    this.dex();
                    break;
                case opc.dec:
                    this.dec();
                    break;
                case opc.cpx:
                    this.cpx();
                    break;
                case opc.cpy:
                    this.cpy();
                    break;
                case opc.cmp:
                    this.cmp();
                    break;
                case opc.clv:
                    this.clv();
                    break;
                case opc.clc:
                    this.clc();
                    break;
                case opc.cld:
                    this.cld();
                    break;
                case opc.cli:
                    this.cli();
                    break;


                case opc.adc:
                    this.adc();
                    break;
                case opc.and:
                    this.and();
                    break;
                case opc.asl:
                    this.asl();
                    break;
                case opc.bcc:
                    this.bcc();
                    break;
                case opc.bcs:
                    this.bcs();
                    break;
                case opc.beq:
                    this.beq();
                    break;
                case opc.bit:
                    this.bit();

                    break;
                case opc.bmi:
                    this.bmi();
                    break;
                case opc.bne:
                    this.bne();
                    break;
                case opc.bpl:
                    this.bpl();
                    break;
                case opc.bvs:
                    this.bvs();
                    break;
                case opc.bvc:
                    this.bvc();
                    break;
                case opc.brk:
                    this.brk();
                    break;
          
                default:
                    Console.WriteLine("Error opcode");
                    break;
            }
            return operacion;
        }
    }
}
