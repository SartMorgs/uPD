/* @autor: Morgana Sartor, Thaynara Mitie
 * Data da última modificação: 29/05/2019
 * RAM
 * 
 * Memória de programa, com 2047(2^11 -1) posições de 16 bits cada,
 * das quais são destinadas de [0:511] área de programa, de [512:767] área da interrupção 0,
 * de [768:1023] área da interrupção 1, de [1024:1279] área da interrupção 2, de [1280:1535] área da interrupção 3,
 * de [1536:1791] área da interrupção 4 e de [1792:2047] área da interrupção 5.
 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPD
{
    class RAM
    {
        private static int[] memory = new int[512];                        // Vetor da Memória RAM
        private static bool enable;                                         // Enable da RAM
        private static bool writeEnable;                                    // Write Enable RAM

        // Construtor
        public RAM()
        {
            enable = false;
            writeEnable = false;


            for(uint i = 0; i < 512; i++)
            {
                memory[i] = 0;
            }
        }
        

        #region Gets e Sets
        // Retorna o vetor de memória
        public int[] GetMemory()
        {
            return memory;
        }
        #endregion Gets e Sets

        #region Enable and Disable
        // Ativa memória
        public void EnableRAM()
        {
            enable = true;
        }

        // Desativa memória
        public void DisableRAM()
        {
            enable = false;
        }

        // Ativa a escrita na memória
        public void EnableWrite()
        {
            writeEnable = true;
        }

        // Desativa a escrita na memória
        public void DisableWrite()
        {
            writeEnable = false;
        }
        #endregion Enable and Disable

        // Limpa os dados da memória
        public void CleanDataMemory()
        {
            for(uint i = 0; i < 512; i++)
            {
                memory[i] = 0;
            }
        }

        #region Change Memory's value
        public void ChangeMemoryValue(int value, uint position)
        {
            memory[position] = value;
        }

        public int ViewMemoryValue(uint position)
        {
            return memory[position];
        }
        #endregion Change Memory's value
    }
}
