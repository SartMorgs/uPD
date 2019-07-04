/* @author: Morgana Sartor, Thaynara Mitie
 * última modificação: 30/05/2019
* 
*  Unidade de Controle
* 
*  Classe destinada a executar os comandos de controle do processador.
*  A classe irá quebrar os valores de acordo com o opcode e setará os 
*  enables necessários para que as operações sejam realizadas
*  de acordo com a instrução.
*  
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uPD
{
    class ControlUnit
    {
        private static string currentInstruction;               // Instrução atual
        private static int currentAddr;                         // Endereço da instrução atual
        // SELECT
        private static int selectULA;                           // Selecionador de instrução da ULA
        private static int selectData;                          // Selecionador de origem do dado do imediato
        // WRITE ENABLE
        private static bool writeEnableBCO;                     // Write enable do Banco de Registradores
        private static bool writeEnableRAM;                     // Write enable da RAM
        private static bool writeEnableIO;                      // Write enable de IO
        // ENABLE
        private static bool enableROM;                          // Ativador da ROM
        private static bool enableRAM;                          // Ativador da RAM
        private static bool enablePC;                           // Ativador do Program Counter
        private static bool enableReadIO;                       // Ativadore de leitura IO
        private static bool enablePush;                         // Ativador push da LIFO
        private static bool enablePop;                          // Ativador pop da LIFO
        private static bool enableJmp;                          // Ativador de jumper
        private static bool enableRet;                          // Ativadot de 

        #region Gets e Sets
        // Seta a instrução atual
        public void SetCurrentInstruction(string instruction)
        {
            currentInstruction = instruction;
        }

        // Seta o endereço da instrução atual
        public void SetAddrInstruction(int addr)
        {
            currentAddr = addr;
        }

        // Retorna o selecionador da ULA
        public int GetSelectULA()
        {
            return selectULA;
        }

        // Retorna o selecionador da origem do dado do imediato
        public int GetSelectData()
        {
            return selectData;
        }

        // Retorna o write enable do BCO
        public bool GetWriteEnableBCO()
        {
            return writeEnableBCO;
        }

        // Retorna o write enable da RAM
        public bool GetWriteEnableRAM()
        {
            return writeEnableRAM;
        }

        // Retorna o write enable do IO
        public bool GetWriteEnableIO()
        {
            return writeEnableIO;
        }

        // Retorna o enable da ROM
        public bool GetEnableROM()
        {
            return enableROM;
        }

        // Retorna o enable da RAM
        public bool GetEnableRAM()
        {
            return enableRAM;
        }

        // Retorna o enable do PC
        public bool GetEnablePC()
        {
            return enablePC;
        }

        // Retorna o enable da leitura do IO
        public bool GetEnableReadIO()
        {
            return enableReadIO;
        }

        // Retorna o enable do Push na LIFO
        public bool GetEnablePush()
        {
            return enablePush;
        }

        // Retorna o enable do Pop na LIFO
        public bool GetEnabelPop()
        {
            return enablePop;
        }

        // Retorna o enable do Jump
        public bool GetEnableJmp()
        {
            return enableJmp;
        }

        // Retorna o enable do
        public bool GetEnableRet()
        {
            return enableRet;
        }
        #endregion Gets e Sets

        #region Controller
        public void SetControlFlags()
        {
            string opcode = currentInstruction.Substring(0, 5);

            switch(opcode)
            {
                // LDI
                case "00001":
                    // Selecionadores
                    selectULA = 10;  selectData = 0;

                    // Write Enable
                    writeEnableBCO = true;  writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false;   enablePC = true;    enableJmp = false;  enableROM = false;  enableRAM = false;  enablePush = false; enablePop = false;  enableRet = false;
                    break;
                // ADD
                case "00010":
                    // Selecionadores
                    selectULA = 0; selectData = 3;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // SUB
                case "00011":
                    // Selecionadores
                    selectULA = 1; selectData = 3;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // OUT
                case "00100":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = false; writeEnableIO = true;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // IN
                case "00101":
                    // Selecionadores
                    selectULA = 10; selectData = 1;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = true; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // JI
                case "00110":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = true; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // LD
                case "00111":
                    // Selecionadores
                    selectULA = 10; selectData = 2;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = true; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // STO
                case "01000":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = true; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = true; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // JZ
                case "01001":
                    // Selecionadores
                    selectULA = 7; selectData = 3;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = true; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // CMP
                case "10100":
                    // Selecionadores
                    selectULA = 6; selectData = 3;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // JE
                case "01010":
                    // Selecionadores
                    selectULA = 6; selectData = 3;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    if(Program.compare) enableJmp = true;
                    else enableJmp = false;
                    break;
                // AND
                case "01011":
                    // Selecionadores
                    selectULA = 2; selectData = 3;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // OR
                case "01100":
                    // Selecionadores
                    selectULA = 3; selectData = 3;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // XOR
                case "01101":
                    // Selecionadores
                    selectULA = 4; selectData = 3;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // NOT
                case "01110":
                    // Selecionadores
                    selectULA = 5; selectData = 3;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // CALL
                case "01111":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = true; enablePop = false; enableRet = false;
                    break;
                // STOP
                case "10010":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = false; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                // RET
                case "10011":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = true; enableRet = false;
                    break;
                // NOP
                case "00000":
                    // Selecionadores
                    selectULA = 10;  selectData = 4;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false;   enablePC = false;   enableJmp = false;  enableROM = false; enableRAM = false; enablePush = false; enablePop = false;  enableRet = false;
                    break;
                // RETI
                case "10000":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = true; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = true; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = true; enableRet = false;
                    break;
                // SETR
                case "10001":
                    // Selecionadores
                    selectULA = 10; selectData = 4;

                    // Write Enable
                    writeEnableBCO = false; writeEnableRAM = false; writeEnableIO = false;

                    // Enable
                    enableReadIO = false; enablePC = false; enableJmp = false; enableROM = false; enableRAM = false; enablePush = false; enablePop = false; enableRet = false;
                    break;
                default:
                    break;
            }
        }
        #endregion Controller

    }
}

