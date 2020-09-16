/*  @author: Morgana Sartor, Thaynara Mitie
* Última modificação: 29/05/2019
* 
*  Classe destinada a executar operações lógicas entre dois registradores,
*  tais como ADD, SUB, NOT, AND, OR, XOR e CMP.
*  A operação a ser realizada virá do controle, com 3 bits.
*  Os valores de entrada da classe virão das saídas do banco de registradores
*  e os valor de saída da classe irá para o MUX, ou para a classe Controle,
*  caso seja operação de comparação (CMP).
*  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPD
{
    class ULA
    {
        private static int rs1;                 // Valor de entrada 1
        private static int rs2;                 // Valor de entrada 2
        private static int outUlaValue;         // Valor de saída 

        #region Gets and Sets
        // Seta o valor de rs1
        public void SetRs1(int value)
        {
            rs1 = value;
        }

        // Seta o valor de rs2
        public void SetRs2(int value)
        {
            rs2 = value;
        }

        // Retorna o valor de saída da ULA
        public int GetOutValue()
        {
            return outUlaValue;
        }
        #endregion Gets and Sets

        // Limpa os campos da ULA
        public void CleanBox()
        {
            rs1 = 0;
            rs2 = 0;
            outUlaValue = 0;
        }

        // Realiza a operação solicitada
        public void Operation(string selOp)
        {
            switch (selOp)
            {
                // Operação de adição
                case "add":
                    outUlaValue = rs1 + rs2;
                    break;
                // Operação de subtração
                case "sub":
                    outUlaValue = rs1 - rs2;
                    break;
                // Operação de AND
                case "and":
                    outUlaValue = rs1 & rs2;
                    break;
                // Operação de OR
                case "or":
                    outUlaValue = rs1 | rs2;
                    break;
                // Operação de XOR
                case "xor":
                    outUlaValue = rs1 ^ rs2;
                    break;
                // Operação de NOT
                case "not":
                    outUlaValue = ~ rs1;
                    break;
                // Operação de CMP
                case "cmp":
                    if (rs1 == rs2)
                        outUlaValue = 1;
                    else
                        outUlaValue = 0;
                    break;
                // Operação de Jump
                case "jmp":
                    if (rs1 == 0)
                        outUlaValue = 1;
                    else
                        outUlaValue = 0;
                    break;
            }
        }

    }
}
