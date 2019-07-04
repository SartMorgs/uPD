/*  @author: Thaynara Mitie
 *  
 *  INPUT / OUTPUT
 *  
 *  Classes desctinada a gerenciar até 6 interrupções externas que possam a vir ocorrer no
 *  processador, caso programadas. Cada interrupção terá uma entrada de dados de 16 bits, um endereço de 16 bits
 *  e uma saída de 16 bits.
 *  
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPD
{
    class InOut
    {
        // Valores de entrada e saída
        private static int[] inValues = new int[512];
        private static int[] outValues = new int[512];
        private static int[] inOrOut = new int[512];

        // Endereço de entrada e saída
        private static int outAddr;
        private static int inAddr;

        // Registrador solicitado
        private static int register;

        // Enable
        private static bool readEnable;
        private static bool writeEnable;

        public InOut()
        {
            readEnable = false;
            writeEnable = false;
            
            for(uint i = 0; i < 512; i++)
            {
                inValues[i] = 0;
                outValues[i] = 0;
                inOrOut[i] = 0;         // Indefinido
            }
        }

        #region Sets and Gets
        // Retorna o vetor de entradas
        public int[] GetInVector()
        {
            return inValues;
        }

        // Retorna o vetor de saídas
        public int[] GetOutVector()
        {
            return outValues;
        }

        // Retorna o vetor de configuração de endereços
        public int[] GetInOrOutVector()
        {
            return inOrOut;
        }

        // Seta o tipo de endereço
        public void SetAddrType(int addr, int t)
        {
            inOrOut[addr] = t;
        }
        
        // Seta o valor de entrada
        public void SetInValue(int value)
        {
            inValues[inAddr] = value;
        }

        // Seta o valor de saída
        public void SetOutValue(int value)
        {
            outValues[outAddr] = value;
        }

        // Retorna o valor de entrada
        public int GetInValue()
        {
            return inValues[inAddr];
        }
        
        // Retorna o valor de saída
        public int GetOutValue()
        {
            return outValues[outAddr];
        }

        // Retorna o endereço de saída
        public int GetOutAddr()
        {
            return outAddr;
        }

        // Retorna o endereço de entrada
        public int GetInAddr()
        {
            return inAddr;
        }

        // Retorna o registrador solicitado
        public int GetRegister()
        {
            return register;
        }

        // Retorna se a leitura está ativa
        public bool GetReadEnable()
        {
            return readEnable;
        }

        // Retorna se a escrita está ativa
        public bool GetWriteEnable()
        {
            return writeEnable;
        }
        #endregion Sets and Gets

        #region Enable and Disable
        // Habilita leitura/entrada
        public void EnableRead()
        {
            readEnable = true;
        }

        // Desabilita a leituraa/entrada
        public void DisableRead()
        {
            readEnable = false;
        }

        // Habilita escrita/saída
        public void EnableWrite()
        {
            writeEnable = true;
        }

        // Desabilita a escrita/saída
        public void DisableWrite()
        {
            writeEnable = false;
        }
        #endregion Enable and Disable

        #region Clean
        // Limpa os campos de saída
        public void CleanOutBox()
        {
            for (uint i = 0; i < 512; i++)
            {
                outValues[i] = 0;
            }
            outAddr = 0;
            register = 0;
        }
        
        // Limpa os campos de entrada
        public void CleanInBox()
        {
            for (uint i = 0; i < 512; i++)
            {
                inValues[i] = 0;
            }
            inAddr = 0;
            register = 0;
        }
        #endregion Clean

        #region Instruction IN and OUT
        // Instrução IN
        public int DataIn(int reg, int addr)
        {

            if (reg > 4) return 1;                  // Erro 1 = registrador inexistente
            else if (addr > 512) return 2;          // Erro 2 = Endereço de entrada inexistente
            else if (inOrOut[addr] == 2) return 3;  // Erro 3 = Endereço setado como saída
            else if (inOrOut[addr] == 3) return 5;  // Erro 5 = Endereço setado como interrupção
            // Retorno 0 = sem erros
            else
            {
                register = reg;
                inAddr = addr;

                return 0;
            }
        }

        // Instrução OUT
        public int DataOut(int reg, int addr)
        {
            if (reg > 4) return 1;
            else if (addr > 512) return 2;
            else if (inOrOut[addr] == 1) return 4;      // Erro 4 = Endereço setado como entrada
            else if (inOrOut[addr] == 3) return 5;      // Erro 5 = Endereço setado como interrupção
            else
            {
                register = reg;
                outAddr = addr;

                return 0;
            }
        }
        #endregion Instruction IN and OUT
    }
}
