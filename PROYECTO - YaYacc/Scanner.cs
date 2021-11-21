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
            _regexp = regexp + (char)TokenType.T_EOF;
            _index = 0;
            _state = 0;
        }
        public Token GetToken()
        {
            Token result = new Token() { Value = "" };
            bool tokenFound = false;
            MatchCollection match;
            Regex regex;
            while (!tokenFound)
            {
                char peek = _regexp[_index];
                switch (_state)
                {
                    case 0:
                        //whitespace removal
                        switch (peek)
                        {
                            case (char)TokenType.T_ENDLINE:
                            //case (char)TokenType.T_NEWLINE:
                            case (char)TokenType.T_OR:
                            case (char)TokenType.T_SEPARATOR:
                            case (char)TokenType.T_EOF:
                                tokenFound = true;
                                result.Tag = (TokenType)peek;
                                result.Value = peek.ToString();
                                break;
                            default:
                                if(!char.IsWhiteSpace(peek))
                                { 

                                    int i = 0;
                                    tokenFound = true;
                                   
                                    if (peek == '\'')
                                    {
                                        _index++;
                                        peek = _regexp[_index];                                     
                                        do
                                        {
                                            i++;
                                            if (peek == '\\')
                                            {
                                                result.Value = result.Value + peek.ToString();
                                                _index++;
                                                peek = _regexp[_index];
                                            }
                                            result.Value = result.Value + peek.ToString();
                                            peek = _regexp[_index + i];

                                        } while (peek != '\'');

                                        if (i > 0)
                                        {
                                            _index += i - 1;
                                        }

                                        regex = new Regex(@"[\da-zA-Z \n\t\\'!\""#%&()\*\+,\-\./:;<=>?[\]^_{|}~]{1}");
                                        match = regex.Matches(result.Value);
                                        if (match.Count <= 6 && match.Count == result.Value.Length)
                                        {
                                            result.Tag = TokenType.T_TERMINAL;
                                        }
                                        else
                                        {
                                            result.Tag = TokenType.T_NONDEFINE;
                                        }

                                        _index++;
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
                                        regex = new Regex(@"[a-zA-Z][\w\d_']*");
                                        match = regex.Matches(result.Value);

                                        if (match.Count == 1 )
                                        {
                                            if (match[0].ToString() == result.Value)
                                            {
                                                result.Tag = TokenType.T_NONT;
                                            }
                                            else
                                            {
                                                result.Tag = TokenType.T_NONDEFINE;
                                            }

                                        }
                                        else
                                        {
                                            result.Tag = TokenType.T_NONDEFINE;
                                        }

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

        public Token identifyToken(string regexp)
        {
            _regexp = regexp + (char)TokenType.T_EOF;
            Token result = new Token() { Value = "" };
            MatchCollection match;
            Regex regex;


            char peek = _regexp[_index];

            if (peek == '\'') /*Valor terminal*/
            {
                _index++;
                for (int i = 0; i < _regexp.Length; i++)
                {

                    if (peek == '\\')
                    {
                        result.Value = result.Value + peek.ToString();
                        _index++;
                        peek = _regexp[_index];
                    }

                    result.Value = result.Value + peek.ToString();
                    peek = _regexp[i];
                }
                result.Value = result.Value.Trim('\'');
                regex = new Regex(@"[\da-zA-Z \n\t\\\'\!\""\#\%\&\(\)\*\+\,\-\.\/\:\;\<\=\>\?\[\]\^\{\|\}\~_]{1}");
                match = regex.Matches(result.Value);
                if (match.Count <= 6 && match.Count == result.Value.Length)
                {
                    result.Tag = TokenType.T_TERMINAL;
                }
                else
                {
                    result.Tag = TokenType.T_NONDEFINE;
                }
            }
            else/*Valor no Terminal*/
            {
                result.Tag = TokenType.T_NONT;
                for (int j = 1; j < _regexp.Length; j++)
                {
                    result.Value = result.Value + peek.ToString();
                    peek = _regexp[j];
                }
                regex = new Regex(@"[a-zA-Z][\w\d_']*");
                match = regex.Matches(result.Value);

                if (match.Count == 1)
                {
                    if (match[0].ToString() == result.Value)
                    {
                        result.Tag = TokenType.T_NONT;
                    }
                    else
                    {
                        result.Tag = TokenType.T_NONDEFINE;
                    }

                }
                else
                {
                    result.Tag = TokenType.T_NONDEFINE;
                }

            }
            _index = 0;
            return result;

        }


    }
}
