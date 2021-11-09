using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc
{
    public enum TokenType
    {
        T_NONT = 'N',
        T_TERMINAL = 'T',
        T_SEPARATOR = ':',
        //T_NEWLINE = '\n',
        T_OR = '|',
        T_ENDLINE = ';',
        T_NONDEFINE = (char)1,
        T_EOF = (char)0
    }
}
