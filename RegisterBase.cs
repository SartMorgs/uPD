/* @author: Morgana Sartor
 * 
 * BANCO DE REGISTRADORES
 * 
 * Classe destinada a implementar um banco de registradores com 4 
 * registradores de salvamento, cada um com 16 bits.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace uPD
{
    class RegisterBase
    {
        private static int[] registerValue = new int[4];                        // Vetor com os valores dos registradores do banco
        private static int rs1;                                                 // Valor de Rs1
        private static int rs2;                                                 // Valor de Rs2
        private static bool enable;                                             // Flag de ativação do Banco de Registradores
        private static bool writeEnbale;                                        // Flag de ativação de escrita nos registradores

        // Construtor
        public RegisterBase()
        {
            enable = false;
            writeEnbale = false;
            rs1 = 0; rs2 = 0;

            for(uint i = 0; i < 4; i++)
            {
                registerValue[i] = 0;
            }
        }

        #region Gets and Sets
        // Seta o valor de Rs1
        public void SetRs1(int value)
        {
            rs1 = value;
        }

        // Retorna o valor de Rs1
        public int GetRs1()
        {
            return rs1;
        }

        // Seta o valor de Rs2
        public void SetRs2(int value)
        {
            rs2 = value;
        }

        // Retorna o valor de Rs2
        public int GetRs2()
        {
            return rs2;
        }

        // Retorna o valor de um registrador especificado
        public int GetRegister(uint register)
        {
            return registerValue[register];
        }
        #endregion Gets and Sets

        #region Enble and Disable
        // Ativa o Banco de Registradores
        public void EnableBCO()
        {
            enable = true;
        }

        // Desavtiva o Banco de Registradores
        public void DisableBCO()
        {
            enable = false;
        }

        // Ativa a escrita no Banco de Registradores
        public void EnableWrite()
        {
            writeEnbale = true;
        }

        // Desativa a escrita no Banco de Registradores
        public void DisableWrite()
        {
            writeEnbale = false;
        }
        #endregion Enable and Disable

        #region Clean
        // Limpa o valor dos registradores rs1 e rs2
        public void CleanRs()
        {
            rs1 = 0;
            rs2 = 0;
        }

        // Limpa o valor dos registradores do banco
        public void CleanRegisterBCO()
        {
            for(uint i = 0; i < 4; i++)
            {
                registerValue[i] = 0;
            }
        }
        #endregion Clean

        #region Change Register's Value
        // Alterar o valor de um registrador solicitado
        public void ChangeRegister(int value, uint register)
        {
            registerValue[register] = value;
        }
        #endregion Change Register's Value

    }
}
