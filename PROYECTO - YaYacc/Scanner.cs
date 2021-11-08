using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PROYECTO___YaYacc
{
    public class Scanner
    {
        private string _regexp = "";
        private int _index = 0;
        private int _state = 0;
        public Scanner(string regexp)
        {
            _regexp = regexp + (char)TokenType.EOF;
            _index = 0;
            _state = 0;
        }
        public Token GetToken()
        {
            Token result = new Token() { Value = "" };
            bool tokenFound = false;
            while (!tokenFound)
            {
                char peek = _regexp[_index];
                switch (_state)
                {
                    case 0:
                        //whitespace removal
                        while (char.IsWhiteSpace(peek))
                        {
                            peek = _regexp[_index];
                            _index++;
                        }
                        switch (peek)
                        {
                            case (char)TokenType.T_ENDLINE:
                            case (char)TokenType.T_NEWLINE:
                            case (char)TokenType.T_OR:
                            case (char)TokenType.T_SEPARATOR:
                            case (char)TokenType.EOF:
                                tokenFound = true;
                                result.Tag = (TokenType)peek;
                                result.Value = peek.ToString();
                                break;
                            default:
                                int i = 0;
                                tokenFound = true;
                                if (peek == '\'')
                                {
                                    _index++;
                                    peek = _regexp[_index];
                                    result.Tag = TokenType.T_TERMINAL;
                                    do 
                                    {
                                        i++;
                                        result.Value = result.Value + peek.ToString();
                                        peek = _regexp[_index + i];
                                    } while (peek != '\'') ;
                                    if (i > 0)
                                    {
                                        _index += i - 1;
                                    }
                                    _index++;
                                    peek = _regexp[_index];
                                }
                                else
                                {
                                    result.Tag = TokenType.T_NONT;
                                    do
                                    {
                                        i++;
                                        result.Value = result.Value + peek.ToString();
                                        peek = _regexp[_index + i];
                                    } while (!char.IsWhiteSpace(peek));
                                    if (i > 0)
                                    {
                                        _index += i - 1;
                                    }

                                }
                                break;
                        }// SWITCH - peek

                        break; //Case 0

                    default:
                        break;
                } //SWITCH - state
                _index++;
            } //WHILE- tokenFound 
            return result;

        } //GetToken
    }
}
