namespace LuaLib.Emit
{
    public enum OpCodes
    {
        #region Lua 5.1

        MOVE, // A B        R(A) := R(B)

        LOADK,  // A Bx     R(A) := Kst(Bx)
        LOADBOOL, // A B C  R(A) := (Bool)B; if (C) pc++
        LOADNIL, // A B     R(A) := ... := R(B) := nil

        GETUPVAL, // A B    R(A) := UpValue[B]
        GETGLOBAL, // A Bx  R(A) := Glob[Kst(Bx)]
        GETTABLE, // A B C  R(A) := R(B)[RK(C)]

        SETGLOBAL, // A Bx  Glob[Kst(Bx)] := R(A)
        SETUPVAL, // A B    UpValue[B] := R(A)
        SETTABLE, // A B C  R(A)[RK(B)] := RK(C)

        NEWTABLE, // A B C  R(A) := {} (size = B, C)

        SELF, // A B C      R(A + 1) := R(B); R(A) := R(B)[RK(C)]

        ADD, // A B C       R(A) := RK(B) + RK(C)
        SUB, // A B C       R(A) := RK(B) - RK(C)
        MUL, // A B C       R(A) := RK(B) * RK(C)
        DIV, // A B C       R(A) := RK(B) / RK(C)
        MOD, // A B C       R(A) := RK(B) % RK(C)
        POW, // A B C       R(A) := RK(B) ^ RK(C)
        UNM, // A B         R(A) := -R(B)
        NOT, // A B         R(A) := not R(B)
        LEN, // A B         R(A) := length of R(B)

        CONCAT, // A B C    R(A) := R(B).. ... ..R(C)

        JMP, // sBx         pc += sBx

        EQ, // A B C        if ((RK(B) == RK(C)) != A) pc++
        LT, // A B C        if ((RK(B) < RK(C)) != A) pc++
        LE, // A B C        if ((RK(B) <= RK(C)) != A) pc++

        TEST, // A C        if !(R(A) <=> C) pc++
        TESTSET, // A B C   if (R(B) <=> C) R(A) := R(B) else pc++

        CALL, // A B C      R(A), ... , R(A + C - 2) := R(A)(R(A + 1)), ..., R(A + B - 1)
        TAILCALL, // A B C  return R(A)(R(A + 1), ..., R(A + B - 1))
        RETURN, // A B      return R(A), ..., R(A + B - 2)

        FORLOOP, // A sBx   R(A) += R(A + 2); if R(A) <?= R(A + 1) { pc += sBx; R(A + 3) = R(A) }
        FORPREP, // A sBx   R(A) -= R(A + 2); pc += sBx
        TFORLOOP, // A C    R(A + 3), ..., R(A + 2 + C) := R(A)(R(A + 1), R(A + 2)); if R(A + 3) != nil { R(A + 2) = R(A + 3) } else pc++

        SETLIST, // A B C   R(A)[(C - 1) * FPF + i] := R(A + i), 1 <= i <= B

        CLOSE, // A         close all vars in the stack up to (>=) R(A)
        CLOSURE, // A Bx    R(A) := closure(KPROTO[Bx], R(A), ..., R(A + n))

        VARARG, // A B       R(A), R(A + 1), ..., R(A + B - 1) = vararg

        #endregion

        #region Lua 5.2

        LOADKX, // Ax

        GETTABUP, // A B C
        SETTABUP, // A B C

        TFORCALL, // A C

        EXTRAARG, // Ax

        #endregion

        #region Lua 5.3

        IDIV, // A B C

        BAND, // A B C
        BOR, // A B C
        BXOR, // A B C
        SHL, // A B C
        SHR, // A B C
        BNOT, // A B

        #endregion

        #region Lua 5.4

        LOADI, // A sBx
        LOADF, // A sBx
        LOADFALSE, // A
        LFALSESKIP, // A
        LOADTRUE, // A

        GETI, // A B C
        GETFIELD, // A B C

        SETI, // A B C
        SETFIELD, // A B C

        ADDI, // A B C
        ADDK, // A B C
        SUBK, // A B C
        MULK, // A B C
        MODK, // A B C
        POWK, // A B C
        DIVK, // A B C
        IDIVK, // A B C

        BANDK, // A B C
        BORK, // A B C
        BXORK, // A B C

        SHRI, // A B sC
        SHLI, // A B sC

        MMBIN, // A B C
        MMBINI, // A sB C k
        MMBINK, // A B C k

        TBC, // A

        EQK, // A B k
        EQI, // A sB k
        LTI, // A sB k
        LEI, // A sB k
        GTI, // A sB k
        GEI, // A sB k

        RETURN0, // Nothing
        RETURN1, // A

        TFORPREP, // A Bx

        VARARGPREP, // A

        #endregion

        #region LuaU (more info about opcodes at https://github.com/Roblox/luau/blob/master/Compiler/include/Luau/Bytecode.h)

        U_NOP, // nop
        U_BREAK, // debugger break

        U_LOADNIL, // A
        U_LOADB, // A B C
        U_LOADN, // A D
        U_LOADK, // A D

        U_MOVE, // A B

        U_GETGLOBAL, // A C
        U_SETGLOBAL, // A C

        U_GETUPVAL, // A B
        U_SETUPVAL, // A B

        U_CLOSEUPVALS, // A

        U_GETIMPORT, // A D
       
        U_GETTABLE, // A B C
        U_SETTABLE, // A B C

        U_GETTABLEKS, // A B C
        U_SETTABLEKS, // A B C

        U_GETTABLEN, // A B C
        U_SETTABLEN, // A B C

        U_NEWCLOSURE, // A D
        U_NAMECALL, // A B C
        U_CALL, // A B C
        U_RETURN, // A B

        U_JUMP, // D
        U_JUMPBACK, // D
        U_JUMPIF, // A D
        U_JUMPIFNOT, // A D
        U_JUMPIFEQ, // A D
        U_JUMPIFLE, // A D
        U_JUMPIFLT, // A D
        U_JUMPIFNOTEQ, // A D
        U_JUMPIFNOTLE, // A D
        U_JUMPIFNOTLT, // A D

        U_ADD, // A B C
        U_SUB, // A B C
        U_MUL, // A B C
        U_DIV, // A B C
        U_MOD, // A B C
        U_POW, // A B C

        U_ADDK, // A B C
        U_SUBK, // A B C 
        U_MULK, // A B C
        U_DIVK, // A B C
        U_MODK, // A B C
        U_POWK, // A B C

        U_AND, // A B C
        U_OR, // A B C

        U_ANDK, // A B C
        U_ORK, // A B C

        U_CONCAT, // A B C

        U_NOT, // A B
        U_MINUS, // A B
        U_LENGTH, // A B

        U_NEWTABLE, // A B
        U_DUPTABLE, // A D
        U_SETLIST, // A B C
       
        U_FORNPREP, // A D
        U_FORNLOOP, // A D
        U_FORGLOOP, // A D
        U_FORGPREP_INEXT, // none
        U_FORGLOOP_INEXT, // none
        U_FORGPREP_NEXT, // none
        U_FORGLOOP_NEXT, // none

        U_GETVARARGS, // A B

        U_DUPCLOSURE, // A D

        U_PREPVARARGS, // A

        U_LOADKX, // A
        U_JUMPX, // E

        U_FASTCALL, // A C
        U_COVERAGE, // E
        U_CAPTURE, // A B

        U_JUMPIFEQK, // A D
        U_JUMPIFNOTEQK, // A D

        U_FASTCALL1, // A B C
        U_FASTCALL2, // A B C
        U_FASTCALL2K, // A B C

        U__COUNT, // none

        #endregion

        // Custom opcodes

        INVALID,
        NOP // used in decompiler
    }
}
