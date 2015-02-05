using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _6502emulador
{
  public partial  class c6502
    {
        enum modos_dir { imp, indx, zp, imm, acc, abso, rel, indy, zpx, absy, absx, zpy, ind };
        enum opc
        {
            adc, and, asl, bcc, bcs, beq, bit, bmi, bne, bpl, brk, bvc, bvs, clc,
            cld, cli, clv, cmp, cpx, cpy, dec, dex, dey, eor, inc, inx, iny, jmp,
            jsr, lda, ldx, ldy, lsr, nop, ora, pha, php, pla, plp, rol, ror, rti,
            rts, sbc, sec, sed, sei, sta, stx, sty, tax, tay, tsx, txa, txs, tya,
            /******OPCODES ILEGALES******/
            slo, rla, sre, rra, sax, lax, dcp, isb, anc, alr, arr, xaa, axs,
            ahx, shy, shx, las
        };

        modos_dir[] direccionamientos = {
       modos_dir.imp, modos_dir.indx,  modos_dir.imp, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.acc,  modos_dir.imm, modos_dir.abso, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* 0 */
/* 1 */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absx, modos_dir.absx, /* 1 */
/* 2 */    modos_dir.abso,modos_dir.indx,  modos_dir.imp, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.acc,  modos_dir.imm, modos_dir.abso, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* 2 */
/* 3 */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absx, modos_dir.absx, /* 3 */
/* 4 */     modos_dir.imp, modos_dir.indx,  modos_dir.imp, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.acc,  modos_dir.imm, modos_dir.abso, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* 4 */
/* 5 */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absx, modos_dir.absx, /* 5 */
/* 6 */     modos_dir.imp, modos_dir.indx,  modos_dir.imp, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.acc,  modos_dir.imm,  modos_dir.ind, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* 6 */
/* 7 */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absx, modos_dir.absx, /* 7 */
/* 8 */     modos_dir.imm, modos_dir.indx,  modos_dir.imm, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.imp,  modos_dir.imm, modos_dir.abso, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* 8 */
/* 9 */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpy,  modos_dir.zpy,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absy, modos_dir.absy, /* 9 */
/* A */     modos_dir.imm, modos_dir.indx,  modos_dir.imm, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.imp,  modos_dir.imm, modos_dir.abso, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* A */
/* B */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpy,  modos_dir.zpy,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absy, modos_dir.absy, /* B */
/* C */     modos_dir.imm, modos_dir.indx,  modos_dir.imm, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.imp,  modos_dir.imm, modos_dir.abso, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* C */
/* D */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absx, modos_dir.absx, /* D */
/* E */     modos_dir.imm, modos_dir.indx,  modos_dir.imm, modos_dir.indx,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,   modos_dir.zp,  modos_dir.imp,  modos_dir.imm,  modos_dir.imp,  modos_dir.imm, modos_dir.abso, modos_dir.abso, modos_dir.abso, modos_dir.abso, /* E */
/* F */     modos_dir.rel, modos_dir.indy,  modos_dir.imp, modos_dir.indy,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.zpx,  modos_dir.imp, modos_dir.absy,  modos_dir.imp, modos_dir.absy, modos_dir.absx, modos_dir.absx, modos_dir.absx, modos_dir.absx  /* F */
        };

        opc[] op = {
        /*        |  0       |  1        |  2    |  3  |     4  |         5  |   6  |        7  |       8  |     9  |       A  |     B  |       C  |     D  |       E  |     F  |      */
        /* 0 */    opc.brk,  opc.ora,  opc.nop,  opc.slo,  opc.nop,  opc.ora,  opc.asl,  opc.slo,  opc.php,  opc.ora,  opc.asl,  opc.nop,  opc.nop,  opc.ora,  opc.asl,  opc.slo, /* 0 */
        /* 1 */    opc.bpl,  opc.ora,  opc.nop,  opc.slo,  opc.nop,  opc.ora,  opc.asl,  opc.slo,  opc.clc,  opc.ora,  opc.nop,  opc.slo,  opc.nop,  opc.ora,  opc.asl,  opc.slo, /* 1 */
        /* 2 */    opc.jsr,  opc.and,  opc.nop,  opc.rla,  opc.bit,  opc.and,  opc.rol,  opc.rla,  opc.plp,  opc.and,  opc.rol,  opc.nop,  opc.bit,  opc.and,  opc.rol,  opc.rla, /* 2 */
        /* 3 */    opc.bmi,  opc.and,  opc.nop,  opc.rla,  opc.nop,  opc.and,  opc.rol,  opc.rla,  opc.sec,  opc.and,  opc.nop,  opc.rla,  opc.nop,  opc.and,  opc.rol,  opc.rla, /* 3 */
        /* 4 */    opc.rti,  opc.eor,  opc.nop,  opc.sre,  opc.nop,  opc.eor,  opc.lsr,  opc.sre,  opc.pha,  opc.eor,  opc.lsr,  opc.nop,  opc.jmp,  opc.eor,  opc.lsr,  opc.sre, /* 4 */
        /* 5 */    opc.bvc,  opc.eor,  opc.nop,  opc.sre,  opc.nop,  opc.eor,  opc.lsr,  opc.sre,  opc.cli,  opc.eor,  opc.nop,  opc.sre,  opc.nop,  opc.eor,  opc.lsr,  opc.sre, /* 5 */
        /* 6 */    opc.rts,  opc.adc,  opc.nop,  opc.rra,  opc.nop,  opc.adc,  opc.ror,  opc.rra,  opc.pla,  opc.adc,  opc.ror,  opc.nop,  opc.jmp,  opc.adc,  opc.ror,  opc.rra, /* 6 */
        /* 7 */    opc.bvs,  opc.adc,  opc.nop,  opc.rra,  opc.nop,  opc.adc,  opc.ror,  opc.rra,  opc.sei,  opc.adc,  opc.nop,  opc.rra,  opc.nop,  opc.adc,  opc.ror,  opc.rra, /* 7 */
        /* 8 */    opc.nop,  opc.sta,  opc.nop,  opc.sax,  opc.sty,  opc.sta,  opc.stx,  opc.sax,  opc.dey,  opc.nop,  opc.txa,  opc.nop,  opc.sty,  opc.sta,  opc.stx,  opc.sax, /* 8 */
        /* 9 */    opc.bcc,  opc.sta,  opc.nop,  opc.nop,  opc.sty,  opc.sta,  opc.stx,  opc.sax,  opc.tya,  opc.sta,  opc.txs,  opc.nop,  opc.nop,  opc.sta,  opc.nop,  opc.nop, /* 9 */
        /* A */    opc.ldy,  opc.lda,  opc.ldx,  opc.lax,  opc.ldy,  opc.lda,  opc.ldx,  opc.lax,  opc.tay,  opc.lda,  opc.tax,  opc.nop,  opc.ldy,  opc.lda,  opc.ldx,  opc.lax, /* A */
        /* B */    opc.bcs,  opc.lda,  opc.nop,  opc.lax,  opc.ldy,  opc.lda,  opc.ldx,  opc.lax,  opc.clv,  opc.lda,  opc.tsx,  opc.lax,  opc.ldy,  opc.lda,  opc.ldx,  opc.lax, /* B */
        /* C */    opc.cpy,  opc.cmp,  opc.nop,  opc.dcp,  opc.cpy,  opc.cmp,  opc.dec,  opc.dcp,  opc.iny,  opc.cmp,  opc.dex,  opc.nop,  opc.cpy,  opc.cmp,  opc.dec,  opc.dcp, /* C */
        /* D */    opc.bne,  opc.cmp,  opc.nop,  opc.dcp,  opc.nop,  opc.cmp,  opc.dec,  opc.dcp,  opc.cld,  opc.cmp,  opc.nop,  opc.dcp,  opc.nop,  opc.cmp,  opc.dec,  opc.dcp, /* D */
        /* E */    opc.cpx,  opc.sbc,  opc.nop,  opc.isb,  opc.cpx,  opc.sbc,  opc.inc,  opc.isb,  opc.inx,  opc.sbc,  opc.nop,  opc.sbc,  opc.cpx,  opc.sbc,  opc.inc,  opc.isb, /* E */
        /* F */    opc.beq,  opc.sbc,  opc.nop,  opc.isb,  opc.nop,  opc.sbc,  opc.inc,  opc.isb,  opc.sed,  opc.sbc,  opc.nop,  opc.isb,  opc.nop,  opc.sbc,  opc.inc,  opc.isb  /* F */
        };

      
    }
}
