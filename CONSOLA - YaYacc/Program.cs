﻿using PROYECTO___YaYacc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace CONSOLA___YaYacc
{
    class Program
    {
        static void Main(string[] args)
        {
            //Entrega 3
            //Con la entidad gramatica validada hacer un LALR que valide el ingreso de palabras en consola
            Console.WriteLine("Ingrese una expresión");
            string regexp = "S' : S ; S# : '0@' S '1\t' | '0\n'";
            Scanner scanner = new Scanner(regexp);

            Token nextToken;

            do
            {
                nextToken = scanner.GetToken();
                Console.WriteLine("Token: {0} , Valor {1}", nextToken.Tag, nextToken.Value);
            } while (nextToken.Tag != TokenType.T_EOF);


            Console.ReadLine();
        }
    }
}
