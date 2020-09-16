/* @author: Morgana Sartor, Thaynara Mitie
 * Última modificação: 29/05/2019
 * 
 *  MEMÓRIA LAST IN FIRST OUT - LIFO
 *  
 *  Classe destinada a implementar memória LIFO quando houver ações 
 *  de RET e CALL vindas do opcode. A classe possui 10 posições de 
 *  memória(tamanho pode ser modificado), cada uma delas contendo informações
 *  de registradores (banco e PC) em uma string
 *  
 */

using System;
using System.Collections.Generic;
using System.Collections; // Biblioteca para o Uso de pilha e fila
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace uPD
{
    class LIFO
    {
        private static bool push;                                       // Flag de empilhar
        private static bool pop;                                        // Flag de desempilhar
        private static Stack<string> saveSystem = new Stack<string>();  // Vetor da memória LIFO
        // Valores dos registradores
        private static int valueR0;
        private static int valueR1;
        private static int valueR2;
        private static int valueR3;

        private static int valueAddrPC;                                 // valor do endereço do PC
        
        // Construtor
        public LIFO()
        {
            push = false;
            pop = false;
        }

        #region Gets and Sets
        // Retorna o valor do R0
        public int GetR0()
        {
            return valueR0;
        }

        // Retorna o valor do R1
        public int GetR1()
        {
            return valueR1;
        }

        // Retorna o valor do R2
        public int GetR2()
        {
            return valueR2;
        }

        // Retorna o valor do R3
        public int GetR3()
        {
            return valueR3;
        }

        // Retorna o valor do PC
        public int GetAddrPC()
        {
            return valueAddrPC;
        }

        // Retorna se o PUSH está ativo
        public bool GetPushEnable()
        {
            return push;
        }

        // Retorna se o POP está ativo
        public bool GetPopEnable()
        {
            return pop;
        }
        #endregion Gets and Sets

        #region Enable and Disable
        // Ativar o push
        public void EnablePush()
        {
            push = true;
        }

        // Desativar o push
        public void DisablePush()
        {
            push = false;
        }

        // Ativar o pop
        public void EnablePop()
        {
            pop = true;
        }

        // Desativar o pop
        public void DisablePop()
        {
            pop = false;
        }
        #endregion Enable and Disable

        #region Push and Pop LIFO Memory
        public void Push(int R0, int R1, int R2, int R3, int PC)
        {
            string str_R0;                  // String do valor de R0
            string str_R1;                  // String do valor de R1
            string str_R2;                  // String do valor de R2
            string str_R3;                  // String do valor de R3
            string str_PC;                  // String do valor de R4
            string valuePush;               // String do que devve ser empilhado

            valueR0 = R0;
            valueR1 = R1;
            valueR2 = R2;
            valueR3 = R3;

            string str;
            int k, num;

            #region R0's region of conversion
            // Converte um inteiro decimal para uma string binária
            k = 9; num = R0;
            //System.Console.WriteLine("\n\n" + num + "\n\n");
            char[] bin1 = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
            while (num != 0)
            {
                if ((num % 2) == 0)
                {
                    bin1[--k] = '0';
                }
                else
                {
                    bin1[--k] = '1';
                }
                num = num / 2;
            }

            str = new string(bin1);
            str_R0 = str;
            #endregion R0's region of conversion

            #region R1's region of conversion
            // Converte um inteiro decimal para uma string binária
            k = 9; num = R1;
            //System.Console.WriteLine("\n\n" + num + "\n\n");
            char[] bin2 = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
            while (num != 0)
            {
                if ((num % 2) == 0)
                {
                    bin2[--k] = '0';
                }
                else
                {
                    bin2[--k] = '1';
                }
                num = num / 2;
            }

            str = new string(bin2);
            str_R1 = str;
            #endregion R1's region of conversion

            #region R2's region of conversion
            // Converte um inteiro decimal para uma string binária
            k = 9; num = R2;
            //System.Console.WriteLine("\n\n" + num + "\n\n");
            char[] bin3 = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
            while (num != 0)
            {
                if ((num % 2) == 0)
                {
                    bin3[--k] = '0';
                }
                else
                {
                    bin3[--k] = '1';
                }
                num = num / 2;
            }

            str = new string(bin3);
            str_R2 = str;
            #endregion R2's region of conversion

            #region R3's region of conversion
            // Converte um inteiro decimal para uma string binária
            k = 9; num = R3;
            //System.Console.WriteLine("\n\n" + num + "\n\n");
            char[] bin4 = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
            while (num != 0)
            {
                if ((num % 2) == 0)
                {
                    bin4[--k] = '0';
                }
                else
                {
                    bin4[--k] = '1';
                }
                num = num / 2;
            }

            str = new string(bin4);
            str_R3 = str;
            #endregion R3's region of conversion

            #region PC's region of conversion
            // Converte um inteiro decimal para uma string binária
            k = 11; num = PC;
            //System.Console.WriteLine("\n\n" + num + "\n\n");
            char[] bin5 = { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0'};
            while (num != 0)
            {
                if ((num % 2) == 0)
                {
                    bin5[--k] = '0';
                }
                else
                {
                    bin5[--k] = '1';
                }
                num = num / 2;
            }

            str = new string(bin5);
            str_PC = str;
            #endregion PC's region of conversion

            valuePush = str_R3 + str_R2 + str_R1 + str_R0 + str_PC;
            saveSystem.Push(valuePush);                                 // Empilha na LIFO
        }

        public void Pop()
        {
            string valuePop;

            valuePop = saveSystem.Pop();

            // Separando os valores dos registradores
            valueR0 = Convert.ToInt32(valuePop.Substring(27, 9));
            valueR1 = Convert.ToInt32(valuePop.Substring(18, 9));
            valueR2 = Convert.ToInt32(valuePop.Substring(9, 9));
            valueR3 = Convert.ToInt32(valuePop.Substring(0, 9));
            valueAddrPC = Convert.ToInt32(valuePop.Substring(36, 11));

        }
        #endregion Push and Pop LIFO Memory

    }
}
