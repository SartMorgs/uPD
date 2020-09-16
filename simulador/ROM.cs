using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//A principio a ROM será uma herança do tradutor para utilizar os endereços necessários para os saltos.

namespace uPD
{
    class ROM
    {
        private static bool enable;                                             // Flag de ativação da ROM
        private static string currentAddress;                                   // Endereço da instrução que está sendo executada
        private static string currentInstruction;                               // Instrução que está seno executada
        private static int sizeMainFunction;                                    // Tamanho da função MAIN no programa
        private static int[] sizeFunction;                                      // Tamanho das funções no programa
        private static int amountInstruction;                                   // Quantidade de instruções no código
        private static string[] binProgram;                                     // Programa traduzido
        private static int pushAddrLIFO;                                        // Endereço que deverá ser empilhado na LIFO
        private static int popAddrLIFO;                                         // Endereço que deverá ser desempilhado na LIFO

        // Construtor
        public ROM()
        {
            enable = false;
            sizeMainFunction = 0;
            amountInstruction = 0;
            pushAddrLIFO = 0;
            popAddrLIFO = 0;
            currentInstruction = "XXXX";
            currentAddress = "XXXX";
        }

        // Instancia quantas funções há no programa
        public void SetAmountFunctions(int amountFunctions){
            sizeFunction = new int[amountFunctions];
        }

        #region Gets and Sets
        // Retorna o endereço da instrução atual
        public string GetCurrentAddress()
        {
            return currentAddress;
        }

        // Retorna a instrução atual
        public string GetCurrentInstruction()
        {
            return currentInstruction;
        }

        // Retorna uma instrução específica da ROM
        public string GetInstruction(uint addr)
        {
            return binProgram[addr];
        }

        // Seta o tamanho da função MAIN
        public void SetSizeMainFunction(int value)
        {
            sizeMainFunction = value;
        }

        // Retorna o tamanho da função MAIN
        public int GetSizeMainFunction()
        {
            return sizeMainFunction;
        }

        // Seta o tamanho da função especificada
        public void SetSizeFunction(int value, uint function)
        {
            sizeFunction[function] = value;
        }
        
        // Retorna o tamanho da função especificada
        public int GetSizeFunction(uint function)
        {
            return sizeFunction[function];
        }

        // Seta a quantidade de instruções no código
        public void GetAmountInstruction(int value)
        {
            amountInstruction = value;
        }

        // Retorna a quantidade de instrução no código
        public int GetAmountInstruction()
        {
            return amountInstruction;
        }

        // Seta o programa na ROM
        public void SetProgram(string[] prog)
        {
            binProgram = prog;
        }

        // Seta o endereço que deve ser empilhado na LIFO
        public void SetPushAddrLIFO(int addr)
        {
            pushAddrLIFO = addr;
        }

        // Retorna o endereço que deve ser empilhado na LIFO
        public int GetPushAddrLIFO()
        {
            return pushAddrLIFO;
        }

        // Seta o endereço que deve ser desempilhado na LIFO
        public void SetPopAddrLIFO(int addr)
        {
            popAddrLIFO = addr;
        }

        // Retorna o endereço que deve ser desempilhado na LIFO
        public int GetPopAddrLIFO()
        {
            return popAddrLIFO;
        }
        #endregion Gets and Sets

        #region Enable and Disable
        // Ativa a memória ROM
        public void EnableROM()
        {
            enable = true;
        }

        // Desativa a memória ROM
        public void DisableROM()
        {
            enable = false;
        }
        #endregion Enable and Disable

        // Limpa o programa na ROM
        public void CleanProgramMemory()
        {
            for(uint i = 0; i < 2047; i++)
            {
                binProgram[i] = "0000000000000000";
            }
        }

        //#region Program Counter
        //// Inicializando o programa
        //public void StartProgramCounter()
        //{
        //    currentInstruction = binProgram[0];
        //    currentAddress = "000000000";
        //}

        //public void addAddress(int valueRegister1, int valueRegister2, int valueRegisterDestiny, int lastAddrLIFO)
        //{
        //    string address;
        //    int currentAddr, nextAddr;

        //    currentAddr = Convert.ToInt32(currentAddress);

        //     // Instrução JI
        //    if(binProgram[currentAddr][0] == '0' && binProgram[currentAddr][1] == '0' && binProgram[currentAddr][2] == '1' && binProgram[currentAddr][3] == '1' && binProgram[currentAddr][4] == '0')
        //    {
        //        // O próximo endereço será o endereço de salto
        //        address =   binProgram[currentAddr][7].ToString() + binProgram[currentAddr][8].ToString() + binProgram[currentAddr][9].ToString() + binProgram[currentAddr][10].ToString() + binProgram[currentAddr][11].ToString() + 
        //                    binProgram[currentAddr][12].ToString() + binProgram[currentAddr][13].ToString() + binProgram[currentAddr][14].ToString() + binProgram[currentAddr][15].ToString();
        //    }
        //    // Instrução JZ
        //    else if (binProgram[currentAddr][0] == '0' && binProgram[currentAddr][1] == '1' && binProgram[currentAddr][2] == '0' && binProgram[currentAddr][3] == '0' && binProgram[currentAddr][4] == '1')
        //    {
        //        if(valueRegisterDestiny == 0)
        //        {
        //            // O próximo endereço será o endereço de salto
        //            address = binProgram[currentAddr][7].ToString() + binProgram[currentAddr][8].ToString() + binProgram[currentAddr][9].ToString() + binProgram[currentAddr][10].ToString() + binProgram[currentAddr][11].ToString() +
        //                        binProgram[currentAddr][12].ToString() + binProgram[currentAddr][13].ToString() + binProgram[currentAddr][14].ToString() + binProgram[currentAddr][15].ToString();
        //        }
        //        else
        //        {
        //            nextAddr = currentAddr + 1;
        //            // Converte um inteiro decimal para uma string binária
        //            int k = 9, num  = nextAddr;
        //            //System.Console.WriteLine("\n\n" + num + "\n\n");
        //            char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
        //            while (num != 0)
        //            {
        //                if ((num % 2) == 0)
        //                {
        //                    bin[--k] = '0';
        //                }
        //                else
        //                {
        //                    bin[--k] = '1';
        //                }
        //                num = num / 2;
        //            }

        //            string str = new string(bin);
        //            address = str;
        //        }
        //    }
        //    // Instrução JE
        //    else if (binProgram[currentAddr][0] == '0' && binProgram[currentAddr][1] == '1' && binProgram[currentAddr][2] == '0' && binProgram[currentAddr][3] == '1' && binProgram[currentAddr][4] == '0')
        //    {
        //        if(valueRegister1 == valueRegister2)
        //        {
        //            // O próximo endereço será o endereço de salto
        //            address = binProgram[currentAddr][7].ToString() + binProgram[currentAddr][8].ToString() + binProgram[currentAddr][9].ToString() + binProgram[currentAddr][10].ToString() + binProgram[currentAddr][11].ToString() +
        //                        binProgram[currentAddr][12].ToString() + binProgram[currentAddr][13].ToString() + binProgram[currentAddr][14].ToString() + binProgram[currentAddr][15].ToString();
        //        }
        //        else
        //        {
        //            nextAddr = currentAddr + 1;
        //            // Converte um inteiro decimal para uma string binária
        //            int k = 9, num = nextAddr;
        //            //System.Console.WriteLine("\n\n" + num + "\n\n");
        //            char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
        //            while (num != 0)
        //            {
        //                if ((num % 2) == 0)
        //                {
        //                    bin[--k] = '0';
        //                }
        //                else
        //                {
        //                    bin[--k] = '1';
        //                }
        //                num = num / 2;
        //            }

        //            string str = new string(bin);
        //            address = str;
        //        }
        //    }
        //    // Instrução CALL
        //    else if (binProgram[currentAddr][0] == '0' && binProgram[currentAddr][1] == '1' && binProgram[currentAddr][2] == '1' && binProgram[currentAddr][3] == '1' && binProgram[currentAddr][4] == '1')
        //    {
        //        // O próximo endereço será o endereço de salto
        //        address = binProgram[currentAddr][7].ToString() + binProgram[currentAddr][8].ToString() + binProgram[currentAddr][9].ToString() + binProgram[currentAddr][10].ToString() + binProgram[currentAddr][11].ToString() +
        //                    binProgram[currentAddr][12].ToString() + binProgram[currentAddr][13].ToString() + binProgram[currentAddr][14].ToString() + binProgram[currentAddr][15].ToString();
        //        // Adiciona próximo endereço a LIFO
        //        pushAddrLIFO = currentAddr + 1;
        //    }
        //    // Instrução RET
        //    else if (binProgram[currentAddr][0] == '1' && binProgram[currentAddr][1] == '0' && binProgram[currentAddr][2] == '0' && binProgram[currentAddr][3] == '1' && binProgram[currentAddr][4] == '1')
        //    {
        //        // Pega o endereço que será desempilhado da LIFO
        //        // Converte um inteiro decimal para uma string binária
        //        int k = 9, num = popAddrLIFO;
        //        //System.Console.WriteLine("\n\n" + num + "\n\n");
        //        char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
        //        while (num != 0)
        //        {
        //            if ((num % 2) == 0)
        //            {
        //                bin[--k] = '0';
        //            }
        //            else
        //            {
        //                bin[--k] = '1';
        //            }
        //            num = num / 2;
        //        }

        //        string str = new string(bin);
        //        address = str;
        //    }
        //    // Instrução RETI
        //    else if (binProgram[currentAddr][0] == '1' && binProgram[currentAddr][1] == '0' && binProgram[currentAddr][2] == '0' && binProgram[currentAddr][3] == '0' && binProgram[currentAddr][4] == '0')
        //    {
        //        // Pega o endereço que será desempilhado da LIFO
        //        // Converte um inteiro decimal para uma string binária
        //        int k = 9, num = popAddrLIFO;
        //        //System.Console.WriteLine("\n\n" + num + "\n\n");
        //        char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
        //        while (num != 0)
        //        {
        //            if ((num % 2) == 0)
        //            {
        //                bin[--k] = '0';
        //            }
        //            else
        //            {
        //                bin[--k] = '1';
        //            }
        //            num = num / 2;
        //        }

        //        string str = new string(bin);
        //        address = str;
        //    }
        //    // Demais instruções
        //    else
        //    {
        //        nextAddr = currentAddr + 1;

        //        // Converte um inteiro decimal para uma string binária
        //        int k = 9, num = nextAddr;
        //        //System.Console.WriteLine("\n\n" + num + "\n\n");
        //        char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
        //        while (num != 0)
        //        {
        //            if ((num % 2) == 0)
        //            {
        //                bin[--k] = '0';
        //            }
        //            else
        //            {
        //                bin[--k] = '1';
        //            }
        //            num = num / 2;
        //        }

        //        string str = new string(bin);
        //        address = str;
        //    }

        //    // Próximo endereço e instrução
        //    currentAddress = address;
        //    currentInstruction = binProgram[Convert.ToInt32(currentAddress)];
        //}
        //#endregion Program Counter

    }
}
