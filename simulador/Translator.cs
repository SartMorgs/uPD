/*  @author: Morgana Sartor
 *  Data da última modificação: 20/08/2018
 *  Translator
 * 
 *  Classe destinada a traduzir o código Assembly para código de máquina.
 *  Instruções como LDI R1, 7 ou ADD R3, R1, R2 serão traduzidas para
 *  instruções do tipo "0000101000000111" e "0001011011000000".
 *  A tradução será feita a partir da variável Program.instructionLine que 
 *  recebe o código Assembly digitado/aberto pelo usuário no editor.
 *  O trabalho da classe é gerar um vetor de instruções em código de máquina,
 *  sendo as posições equivalentes ao endereçamento em memória ROM das instruções.
 *  
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace uPD
{
    class Translator
    {
        private static int countInstMain;
        private static int countInst, countFunctions, countInstProg = 0, lastMainAddress, lastFunctAddress;
        private static int[] lastIntAddress = new int[6];
        private static int[] countInstInt = new int[6] { 0, 0, 0, 0, 0, 0 };
        private static int[] countInstFunct;
        private static int[] first;
        private static int[] last;
        private static int[] addressFunctions;                                  // Vetor com os endereços das funções ordenados como em worldCall

        private static string[] instructionNoOrder;
        private static string[][] txt_bin;                                      // Vetor com os binários correspondentes a cada componente do Assembly, ex: {"00001", "00", "000001001"}
        private static string[] div_text;                                       // Vetor com o código Assembly, ex {
        private static string[] instruction;                                    // Vetor com as instruções em código de máquina
        private static string[] assemblyInstruction;                            // Vetor com as instruções em código assembly
        private static string[] wordCall;                                       // vetor de palavras das funções
        private static string[][] address;                                      // Vetor com os endereços 
        private static string[] inst;                                           // Vetor dos comandos assembly                                 

        public Translator()
        {
            address = null;
            wordCall = null;
            inst = null;
            div_text = null;
            txt_bin = null;
            assemblyInstruction = null;
            instruction = null;
            instructionNoOrder = null;
        }

        #region Gets and Sets
        // Retorna a quantidade de instruções da interrupção solicitada
        public int GetAmountInstInt(uint interruption)
        {
            return countInstInt[interruption];
        }

        // Retorna a quantidade de instruções da Main
        public int GetAmountInstMain()
        {
            return countInstMain;
        }

        // Retorna a quantidade de instruções de .PROG
        public int GetAmountInstProg()
        {
            return countInstProg;
        }

        // Retorna a quantidade de intruções
        public int GetAmountInst()
        {
            return countInst;
        }

        // Retorna a quantidade de funções
        public int GetAmountFunctions()
        {
            return countFunctions;
        }
        // Retorna o tamanho das funções
        public int[] GetAmountInstFunction()
        {
            return countInstFunct;
        }

        // Retorna o vetor de instruções binárias
        public string[] GetBinInstruction()
        {
            return instruction;
        }

        // Retorna o vetor de in struções Assembly
        public string[] GetAssemblyInstruction()
        {
            return assemblyInstruction;
        }

        // Retorna o vetor de funções existentes no programa
        public string[] GetFunctions()
        {
            return wordCall;
        }
        #endregion Gets and Sets

        public void SetFile(string fileAssembly)
        {
            string new_fileAssembly;
            // Divide as instruções, ex: {"LDI", "R2", "12"}
            string pattern = @"[\n|\t|\;| ]";
            new_fileAssembly = Regex.Replace(fileAssembly, pattern, "");
            string reserved_words = @"[^\bBEGIN\b|^\bEND\b|^\bLDI\b|^\bSTO\b|^R0|^R1|^R2|^R3|^0-9|^A-Za-z0-9]";
            div_text = Regex.Split(new_fileAssembly, reserved_words, RegexOptions.None);
            //div_text = System.Text.RegularExpressions.Regex.Split(fileAssembly, @"[(\, )|(/;\n )|(\b) (\s+)]", RegexOptions.None);
        }

        #region Find Begin and End's Functions
        // ***************************** FUNÇÃO QUE ARMAZENA OS PONTOS DETERMINÍSTICOS DE CADA BLOCO *********************************************
        private void SetAssemblyBlocks()
        {
            string currentCall = "MAIN";
            int currentFunction = 0, k = 0;
            countFunctions = 0;
            inst = new string[24] { "PROG", "BEGIN", "END", "NOP", "LDI", "ADD", "SUB", "OUT", "IN", "JI", "LD", "STO", "JZ", "JE", "AND", "OR", "XOR", "NOT", "CALL", "RETI", "SETR", "STOP", "RET", "CMP" };

            // Verifica a quantidade de funções existentes
            for (int i = 1; i < div_text.Length; i++)
            {
                int aux = 0;
                for (int j = 0; j < 24; j++)
                {
                    if(div_text[i] == "")
                    {
                        aux = 1;
                        break;
                    }
                    else if (div_text[i] == inst[j])
                    {
                        aux = 1;
                        break;
                    }
                    else if (div_text[i - 1] == "CALL")
                    {
                        aux = 1;
                        break;
                    }
                }
                if (div_text[i].Length > 4 && aux == 0)
                {
                    countFunctions++;
                }
                aux = 0;
            }

            countInstFunct = new int[countFunctions];
            first = new int[8 + countFunctions];
            last = new int[8 + countFunctions];


            if (countFunctions != 0)
            {
                wordCall = new string[countFunctions];
                countFunctions = 0;

                for (int i = 1; i < div_text.Length; i++)
                {
                    int aux = 0;
                    for (int j = 0; j < 24; j++)
                    {
                        if (div_text[i] == "")
                        {
                            aux = 1;
                            break;
                        }
                        else if (div_text[i] == inst[j])
                        {
                            aux = 1;
                            break;
                        }
                        else if (div_text[i - 1] == "CALL")
                        {
                            aux = 1;
                            break;
                        }
                    }
                    if (div_text[i].Length > 4 && aux == 0)
                    {
                        wordCall[countFunctions++] = div_text[i];
                        first[8 + k] = i + 1;
                        k++;
                        Console.WriteLine("ENTROU NO IF DAS FUNÇÕES");
                    }
                }
            }

            k = 0;


            // **********************          Define limites entre cada bloco do código (main, interrupções, funções)         *****************************
            for (int i = 0; i < div_text.Length; i++)
            {
                // Determina as posições do vetor de código assembly correspondentes ao início e ao fim da main
                if (div_text[i]== "BEGIN")
                {
                    first[currentFunction] = i + 1;
                }
                if (div_text[i] == "END")
                {
                    last[currentFunction] = i - 1;
                }

                // Determina as posições do vetor de código assembly correspondentes ao início e ao fim da Int0
                if (div_text[i].Contains("INT0"))
                {
                    currentCall = "INT0";
                    currentFunction = 1;
                    first[currentFunction] = i + 1;
                }
                if (div_text[i] == "RETI" && currentFunction == 1)
                {
                    last[currentFunction] = i;
                }

                // Determina as posições do vetor de código assembly correspondentes ao início e ao fim da Int1
                if (div_text[i].Contains("INT1"))
                {
                    currentCall = "INT1";
                    currentFunction = 2;
                    first[currentFunction] = i + 1;
                }
                if (div_text[i] == "RETI" && currentFunction == 2)
                {
                    last[currentFunction] = i;
                }

                // Determina as posições do vetor de código assembly correspondentes ao início e ao fim da Int2
                if (div_text[i].Contains("INT2"))
                {
                    currentCall = "INT2";
                    currentFunction = 3;
                    first[currentFunction] = i + 1;
                }

                if (div_text[i] == "RETI" && currentFunction == 3)
                    last[currentFunction] = i;

                // Determina as posições do vetor de código assembly correspondentes ao início e ao fim da Int3
                if (div_text[i].Contains("INT3"))
                {
                    currentCall = "INT3";
                    currentFunction = 4;
                    first[currentFunction] = i + 1;
                }
                if (div_text[i] == "RETI" && currentFunction == 4)
                    last[currentFunction] = i;

                // Determina as posições do vetor de código assembly correspondentes ao início e ao fim da Int4
                if (div_text[i].Contains("INT4"))
                {
                    currentCall = "INT4";
                    currentFunction = 5;
                    first[currentFunction] = i + 1;
                }
                if (div_text[i] == "RETI" && currentFunction == 5)
                    last[currentFunction] = i;

                // Determina as posições do vetor de código assembly correspondentes ao início e ao fim da Int5
                if (div_text[i].Contains("INT5"))
                {
                    currentCall = "INT5";
                    currentFunction = 6;
                    first[currentFunction] = i + 1;
                }
                if (div_text[i] == "RETI" && currentFunction == 6)
                    last[currentFunction] = i;

                if (countFunctions != 0)
                {
                    // Determina as posições do vetor de código assembly correspondentes ao início e ao fim das funções
                    if (k < countFunctions && div_text[i].Contains(wordCall[k]) && first[8 + k] >= i + 1)
                    {
                        currentCall = wordCall[k];
                    }
                    if (div_text[i] == "RET" && currentCall == wordCall[k])
                    {
                        last[8 + k] = i;
                        k++;
                    }

                }
            }
        }
        #endregion Find Begin and End's Functions


        #region Convert Assembly to Binary
        // ******************************** FUNÇÃO QUE CONVERTE CADA ELEMENTO DO ASSEMBLY EM BINÁRIO CORRESPONDENTE ******************************
        private void ConvertAssembly()
        {
            int j = 0, currentAddress = 0, numInstructions = 0;
            countInst = 0;
            countInstMain = 0;

            // Vetor que conterá os binários de cada assembly, ex para LDI, R0, 10: {"00001", "00", "00001010"}
            txt_bin = new string[9 + countFunctions][];
            address = new string[9 + countFunctions][];
            instructionNoOrder = new string[2048];

            for (int i1 = 0; i1 < 8 + countFunctions; i1++)
            {
                txt_bin[i1] = new string[255];
                address[i1] = new string[255];
                for (int i2 = 0; i2 < 255; i2++)
                {
                    txt_bin[i1][i2] = "000000000";
                }
            }

            
            // Rotina .PROG
            for (int i = 1; i < first[0]; i++)
            {
                if (div_text[i] == "SETR" && div_text[i + 1] == "INT")
                {
                    countInstProg++;
                    // Traduz os números do Imediato
                    int num = Convert.ToInt32(div_text[i + 2], 10), c = 9;
                    //System.Console.WriteLine("\n\n" + num + "\n\n");
                    char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
                    while (num != 0)
                    {
                        if ((num % 2) == 0)
                        {
                            bin[--c] = '0';
                        }
                        else
                        {
                            bin[--c] = '1';
                        }
                        num = num / 2;
                    }

                    string str = new string(bin);

                    txt_bin[0][j] = "10001";
                    txt_bin[0][j + 1] = "00";
                    txt_bin[0][j + 2] = str;
                    j += 3;
                    address[0][currentAddress++] = div_text[i];
                    instructionNoOrder[numInstructions++] = "SETR" + " " + "INT" + " " + div_text[i + 2];
                }
            }
            j = 0; currentAddress = 0;

            // Na MAIN, converte cada Assembly para binário 
            for (int i = first[0]; i <= last[0]; i++)
            {
                // Traduz o texto em Assembly para código binário de máquina
                switch (div_text[i])
                {
                    case "NOP":
                        txt_bin[1][j++] = "00000";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "NOP";
                        break;
                    case "LDI":
                        txt_bin[1][j++] = "00001";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "LDI" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "ADD":
                        txt_bin[1][j++] = "00010";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "ADD" + " " + div_text[i + 1] + " " + div_text[i + 2] + " " + div_text[i + 3];
                        break;
                    case "SUB":
                        txt_bin[1][j++] = "00011";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "SUB" + " " + div_text[i + 1] + " " + div_text[i + 2] + " " + div_text[i + 3];
                        break;
                    case "OUT":
                        txt_bin[1][j++] = "00100";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "OUT" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "IN":
                        txt_bin[1][j++] = "00101";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "IN" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "JI":
                        txt_bin[1][j++] = "00110";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "JI" + " " + div_text[i + 1];
                        break;
                    case "LD":
                        txt_bin[1][j++] = "00111";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "LD" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "STO":
                        txt_bin[1][j++] = "01000";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "STO" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "JZ":
                        txt_bin[1][j++] = "01001";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "JZ" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "JE":
                        txt_bin[1][j++] = "01010";
                        address[1][currentAddress++] = div_text[i] + div_text[i + 1];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "JE" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "AND":
                        txt_bin[1][j++] = "01011";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "AND" + " " + div_text[i + 1] + " " + div_text[i + 2] + " " + div_text[i + 3];
                        break;
                    case "OR":
                        txt_bin[1][j++] = "01100";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "OR" + " " + div_text[i + 1] + " " + div_text[i + 2] + " " + div_text[i + 3];
                        break;
                    case "XOR":
                        txt_bin[1][j++] = "01101";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "XOR" + " " + div_text[i + 1] + " " + div_text[i + 2] + " " + div_text[i + 3];
                        break;
                    case "NOT":
                        txt_bin[1][j++] = "01110";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "NOT" + " " + div_text[i + 1] + " " + div_text[i + 2];
                        break;
                    case "CALL":
                        txt_bin[1][j++] = "01111";
                        address[1][currentAddress++] = div_text[i + 1];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "CALL" + " " + div_text[i + 1];
                        break;
                    case "STOP":
                        txt_bin[1][j++] = "1001000000000000";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "STOP";
                        break;
                    case "CMP":
                        txt_bin[1][j++] = "10100";
                        address[1][currentAddress++] = div_text[i];
                        countInstMain++;
                        instructionNoOrder[numInstructions++] = "CMP" + " " + div_text[i + 1] + " " + div_text[i + 2] + " " + div_text[i + 3];
                        break;
                    case "R0":
                        txt_bin[1][j++] = "00";
                        break;
                    case "R1":
                        txt_bin[1][j++] = "01";
                        break;
                    case "R2":
                        txt_bin[1][j++] = "10";
                        break;
                    case "R3":
                        txt_bin[1][j++] = "11";
                        break;
                    case "":
                        break;
                    default:
                        if (div_text[i][0] == '0' || div_text[i][0] == '1' || div_text[i][0] == '2' || div_text[i][0] == '3' ||
                           div_text[i][0] == '4' || div_text[i][0] == '5' || div_text[i][0] == '6' || div_text[i][0] == '7' ||
                           div_text[i][0] == '8' || div_text[i][0] == '9')
                        {
                            // Traduz os números do Imediato
                            int num = Convert.ToInt32(div_text[i], 10), k = 9;
                            //System.Console.WriteLine("\n\n" + num + "\n\n");
                            char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
                            while (num != 0)
                            {
                                if ((num % 2) == 0)
                                {
                                    bin[--k] = '0';
                                }
                                else
                                {
                                    bin[--k] = '1';
                                }
                                num = num / 2;
                            }

                            string str = new string(bin);
                            txt_bin[1][j++] = str;
                        }

                        break;
                }
            }

            j = 0;
            currentAddress = 0;

            // Para cada função, converte cada Assembly para binário
            for (int i = 0; i < countFunctions; i++)
            {
                for (int k = first[8 + i]; k <= last[8 + i]; k++)
                {
                    switch (div_text[k])
                    {
                        case "NOP":
                            txt_bin[8 + i][j++] = "00000";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "NOP";
                            break;
                        case "LDI":
                            txt_bin[8 + i][j++] = "00001";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "LDI" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "ADD":
                            txt_bin[8 + i][j++] = "00010";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "ADD" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "SUB":
                            txt_bin[8 + i][j++] = "00011";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "SUB" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "OUT":
                            txt_bin[8 + i][j++] = "00100";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "OUT" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "IN":
                            txt_bin[8 + i][j++] = "00101";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "IN" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "JI":
                            txt_bin[8 + i][j++] = "00110";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "JI" + " " + div_text[k + 1];
                            break;
                        case "LD":
                            txt_bin[8 + i][j++] = "00111";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "LD" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "STO":
                            txt_bin[8 + i][j++] = "01000";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "STO" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "JZ":
                            txt_bin[8 + i][j++] = "01001";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "JZ" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "JE":
                            txt_bin[8 + i][j++] = "01010";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "JE" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "AND":
                            txt_bin[8 + i][j++] = "01011";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "AND" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "OR":
                            txt_bin[8 + i][j++] = "01100";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "OR" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "XOR":
                            txt_bin[8 + i][j++] = "01101";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "XOR" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "NOT":
                            txt_bin[8 + i][j++] = "01110";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "NOT" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "CALL":
                            txt_bin[8 + i][j++] = "01111";
                            address[8 + i][currentAddress++] = div_text[k + 1];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "CALL" + " " + div_text[k + 1];
                            break;
                        case "STOP":
                            txt_bin[8 + i][j++] = "1001000000000000";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "STOP";
                            break;
                        case "RET":
                            txt_bin[8 + i][j++] = "1001100000000000";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "RET";
                            break;
                        case "CMP":
                            txt_bin[8 + i][j++] = "10100";
                            address[8 + i][currentAddress++] = div_text[k];
                            countInstFunct[i]++;
                            instructionNoOrder[numInstructions++] = "CMP" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "R0":
                            txt_bin[8 + i][j++] = "00";
                            break;
                        case "R1":
                            txt_bin[8 + i][j++] = "01";
                            break;
                        case "R2":
                            txt_bin[8 + i][j++] = "10";
                            break;
                        case "R3":
                            txt_bin[8 + i][j++] = "11";
                            break;
                        case "":
                            break;
                        default:
                            if (div_text[k][0] == '0' || div_text[k][0] == '1' || div_text[k][0] == '2' || div_text[k][0] == '3' ||
                               div_text[k][0] == '4' || div_text[k][0] == '5' || div_text[k][0] == '6' || div_text[k][0] == '7' ||
                               div_text[k][0] == '8' || div_text[k][0] == '9')
                            {
                                // Traduz os números do Imediato
                                int num = Convert.ToInt32(div_text[k], 10), c = 9;
                                //System.Console.WriteLine("\n\n" + num + "\n\n");
                                char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
                                while (num != 0)
                                {
                                    if ((num % 2) == 0)
                                    {
                                        bin[--c] = '0';
                                    }
                                    else
                                    {
                                        bin[--c] = '1';
                                    }
                                    num = num / 2;
                                }

                                string str = new string(bin);
                                txt_bin[8 + i][j++] = str;
                            }
                            break;
                    }

                }
                j = 0;
                currentAddress = 0;
            }

            // Para cada interrupção, converte cada Assembly para binário
            for (int i = 1; i < 7; i++)
            {
                for (int k = first[i]; k <= last[i]; k++)
                {
                    switch (div_text[k])
                    {
                        case "NOP":
                            txt_bin[1 + i][j++] = "00000";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "NOP";
                            break;
                        case "LDI":
                            txt_bin[1 + i][j++] = "00001";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "LDI" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "ADD":
                            txt_bin[1 + i][j++] = "00010";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "ADD" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "SUB":
                            txt_bin[1 + i][j++] = "00011";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "SUB" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "OUT":
                            txt_bin[1 + i][j++] = "00100";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "OUT" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "IN":
                            txt_bin[1 + i][j++] = "00101";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "IN" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "JI":
                            txt_bin[1 + i][j++] = "00110";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "JI" + " " + div_text[k + 1];
                            break;
                        case "LD":
                            txt_bin[1 + i][j++] = "00111";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "LD" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "STO":
                            txt_bin[1 + i][j++] = "01000";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "STO" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "JZ":
                            txt_bin[1 + i][j++] = "01001";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "JZ" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "JE":
                            txt_bin[1 + i][j++] = "01010";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "JE" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "AND":
                            txt_bin[1 + i][j++] = "01011";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "AND" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "OR":
                            txt_bin[1 + i][j++] = "01100";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "OR" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "XOR":
                            txt_bin[1 + i][j++] = "01101";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "XOR" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "NOT":
                            txt_bin[1 + i][j++] = "01110";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "NOT" + " " + div_text[k + 1] + " " + div_text[k + 2];
                            break;
                        case "CALL":
                            txt_bin[1 + i][j++] = "01111";
                            address[1 + i][currentAddress++] = div_text[k + 1];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "CALL" + " " + div_text[k + 1];
                            break;
                        case "STOP":
                            txt_bin[1 + i][j++] = "1001000000000000";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "STOP";
                            break;
                        case "RETI":
                            txt_bin[1 + i][j++] = "1000000000000000";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "RETI";
                            break;
                        case "CMP":
                            txt_bin[1 + i][j++] = "10100";
                            address[1 + i][currentAddress++] = div_text[k];
                            countInstInt[i - 1]++;
                            instructionNoOrder[numInstructions++] = "CMP" + " " + div_text[k + 1] + " " + div_text[k + 2] + " " + div_text[k + 3];
                            break;
                        case "R0":
                            txt_bin[1 + i][j++] = "00";
                            break;
                        case "R1":
                            txt_bin[1 + i][j++] = "01";
                            break;
                        case "R2":
                            txt_bin[1 + i][j++] = "10";
                            break;
                        case "R3":
                            txt_bin[1 + i][j++] = "11";
                            break;
                        case "":
                            break;
                        default:
                            if (div_text[k][0] == '0' || div_text[k][0] == '1' || div_text[k][0] == '2' || div_text[k][0] == '3' ||
                               div_text[k][0] == '4' || div_text[k][0] == '5' || div_text[k][0] == '6' || div_text[k][0] == '7' ||
                               div_text[k][0] == '8' || div_text[k][0] == '9')
                            {
                                // Traduz os números do Imediato
                                int num = Convert.ToInt32(div_text[k], 10), c = 9;
                                System.Console.WriteLine("\n\n" + num + "\n\n");
                                char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
                                while (num != 0)
                                {
                                    if ((num % 2) == 0)
                                    {
                                        bin[--c] = '0';
                                    }
                                    else
                                    {
                                        bin[--c] = '1';
                                    }
                                    num = num / 2;
                                }

                                string str = new string(bin);
                                txt_bin[1 + i][j++] = str;
                            }
                            break;
                    }
                }
                j = 0;
                currentAddress = 0;
            }


            // Contagem total de instruções
            countInst = countInstMain + countInstInt[0] + countInstInt[1] + countInstInt[2] + countInstInt[3] + countInstInt[4] + countInstInt[5];
            for (int i = 0; i < countFunctions; i++)
            {
                countInst += countInstFunct[i];
            }

        }
        #endregion Convert Assembly to Binary


        //****************************** FUNÇÃO QUE GUARDA OS ENDEREÇOS DE ALOCAÇÃO NA ROM DE CADA FUNÇÃO ****************************************
        private void SetAddressFunctions()
        {
            int y = 0, programCounter, j = 0;
            addressFunctions = new int[countFunctions];

            programCounter = countInstProg + countInstMain;

            while (y < countFunctions)
            {
                j = 0;
                addressFunctions[y] = programCounter;
                for (int i = 0; i < countInstFunct[y]; i++)
                {
                    while (txt_bin[8 + y][j].Length != 5)
                    {
                        if (txt_bin[8 + y][j] == "1001100000000000")
                        {
                            break;
                        }
                        j++;
                    }

                    programCounter++;
                    j++;
                }
                y++;
            }
        }


        #region Join Binaries
        // ********************************* FUNÇÃO QUE UNE OS BINÁRIOS FORMANDO AS INSTRUÇÕES *******************************************
        public string[] InstructionGenerate()
        {
            int j = 0, y = 0, programCounter = 0, m = 0;
            lastMainAddress = 0; lastFunctAddress = 0;
            lastIntAddress[0] = 0; lastIntAddress[1] = 0; lastIntAddress[2] = 0; lastIntAddress[3] = 0; lastIntAddress[4] = 0; lastIntAddress[5] = 0;

            // Funções necessárias para gerar a instrução
            SetAssemblyBlocks();
            ConvertAssembly();
            SetAddressFunctions();

            instruction = new string[2048];
            assemblyInstruction = new string[2048];

            for(int i = 0; i < countInstProg; i++)
            {
                if(txt_bin[0][j] == "10001")
                {
                    instruction[programCounter++] = txt_bin[0][j] + txt_bin[0][j + 1] + txt_bin[0][j + 2];
                }
                j += 3;
            }

            y = 0; j = 0;

            // Para cada instrução da MAIN, junção e atribuição de endereço
            for (int i = 0; i < countInstMain; i++)
            {
                while (txt_bin[1][j].Length != 5 && txt_bin[1][j] != "1001000000000000")
                {
                    j++;
                }

                // Trata as instruções de estrura: "OUT R2, 10"
                if (txt_bin[1][j] == "00001" || txt_bin[1][j] == "00100" || txt_bin[1][j] == "00101" ||
                    txt_bin[1][j] == "00111" || txt_bin[1][j] == "01000" || txt_bin[1][j] == "01001" ||
                    txt_bin[1][j] == "01010")
                {
                    instruction[programCounter++] = txt_bin[1][j] + txt_bin[1][j + 1] + txt_bin[1][j + 2];
                }

                // Trata as instruções de estrutura: "JI, 56"
                else if (txt_bin[1][j] == "00110")      //instruções que utilizam o imediato e não utilizam R
                {
                    instruction[programCounter++] = txt_bin[1][j] + "00" + txt_bin[1][j + 1];
                }

                else if (txt_bin[1][j] == "01110")
                    instruction[programCounter++] = txt_bin[1][j] + txt_bin[1][j + 1] + "000000000";

                else if (txt_bin[1][j] == "00000")
                    instruction[programCounter++] = "0000000000000000";

                else if (txt_bin[1][j] == "1001000000000000")
                    instruction[programCounter++] = txt_bin[1][j];

                // Trata as chamadas de função
                else if (txt_bin[1][j] == "01111")
                {
                    int k = 0;
                    // Encontra qual função é a que foi chamada
                    while (address[1][i] != wordCall[k])
                    {
                        k++;
                    }
                    int num = addressFunctions[k], c = 9;
                    //System.Console.WriteLine("\n\n" + num + "\n\n");
                    char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
                    while (num != 0)
                    {
                        if ((num % 2) == 0)
                        {
                            bin[--c] = '0';
                        }
                        else
                        {
                            bin[--c] = '1';
                        }
                        num = num / 2;
                    }

                    string str = new string(bin);
                    instruction[programCounter++] = txt_bin[1][j] + "00" + str;
                }


                // Trata o restante das instruções
                else
                {
                    instruction[programCounter++] = txt_bin[1][j] + txt_bin[1][j + 1] + txt_bin[1][j + 2] + txt_bin[1][j + 3] + "00000";
                }
                j++;
            }
            lastMainAddress = programCounter;

            y = 0;
            j = 0;



            // As funções são após o programa da MAIN
            while (y < countFunctions)
            {
                j = 0;
                for (int i = 0; i < countInstFunct[y]; i++)
                {
                    while (txt_bin[8 + y][j].Length != 5 && txt_bin[8 + y][j] != "1001000000000000")
                    {
                        if (txt_bin[8 + y][j] == "1001100000000000")
                        {
                            instruction[programCounter++] = txt_bin[8 + y][j];
                            break;
                        }
                        j++;
                    }

                    // Trata as instruções de estrura: "OUT R2, 10"
                    if (txt_bin[8 + y][j] == "00001" || txt_bin[8 + y][j] == "00100" || txt_bin[8 + y][j] == "00101" ||
                        txt_bin[8 + y][j] == "00111" || txt_bin[8 + y][j] == "01000" || txt_bin[8 + y][j] == "01001" ||
                        txt_bin[8 + y][j] == "01010")
                        instruction[programCounter++] = txt_bin[8 + y][j] + txt_bin[8 + y][j + 1] + txt_bin[8 + y][j + 2];

                    // Trata as instruções de estrutura: "JI, 56"
                    else if (txt_bin[8 + y][j] == "00110")       //instruções que utilizam o imediato e não utilizam R
                        instruction[programCounter++] = txt_bin[8 + y][j] + "00" + txt_bin[8 + y][j + 1];

                    else if (txt_bin[8 + y][j] == "01110")
                        instruction[programCounter++] = txt_bin[8 + y][j] + txt_bin[8 + y][j + 1] + "000000000";

                    else if (txt_bin[8 + y][j] == "1001000000000000")
                        instruction[programCounter++] = txt_bin[8 + y][j];
                    else if (txt_bin[8 + y][j] == "1001100000000000")
                        break;

                    else if (txt_bin[8 + y][j] == "00000")
                        instruction[programCounter++] = "0000000000000000";

                    // Trata as chamadas de função
                    else if (txt_bin[8 + y][j] == "01111")
                    {
                        int k = 0;
                        // Encontra qual função é a que foi chamada
                        while (address[8 + y][i] != wordCall[k])
                        {
                            k++;
                        }
                        int num = addressFunctions[k], c = 9;
                        //System.Console.WriteLine("\n\n" + num + "\n\n");
                        char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
                        while (num != 0)
                        {
                            if ((num % 2) == 0)
                            {
                                bin[--c] = '0';
                            }
                            else
                            {
                                bin[--c] = '1';
                            }
                            num = num / 2;
                        }

                        string str = new string(bin);
                        instruction[programCounter++] = txt_bin[8 + y][j] + "00" + str;
                    }

                    // Trata o restante das instruções
                    else
                        instruction[programCounter++] = txt_bin[8 + y][j] + txt_bin[8 + y][j + 1] + txt_bin[8 + y][j + 2] + txt_bin[8 + y][j + 3] + "00000";
                    j++;
                }
                y++;
            }
            lastFunctAddress = programCounter;
            y = 0;

            while (y < 6)
            {
                j = 0;
                while (programCounter < (512 + (y * 255) + y))
                {
                    instruction[programCounter++] = "0000000000000000";
                }
                if (first[1 + y] != 0)
                {
                    for (int i = 0; i < countInstInt[y]; i++)

                    {
                        while (txt_bin[2 + y][j].Length != 5 && txt_bin[2 + y][j] != "1001000000000000")
                        {
                            if (txt_bin[2 + y][j] == "1000000000000000")
                            {
                                instruction[programCounter++] = txt_bin[2 + y][j];
                                break;
                            }
                            j++;
                        }

                        // Trata as instruções de estrura: "OUT R2, 10"
                        if (txt_bin[2 + y][j] == "00001" || txt_bin[2 + y][j] == "00100" || txt_bin[2 + y][j] == "01010" ||
                            txt_bin[2 + y][j] == "00111" || txt_bin[2 + y][j] == "01000" || txt_bin[2 + y][j] == "01001" ||
                            txt_bin[2 + y][j] == "00101")
                        {
                            instruction[programCounter++] = txt_bin[2 + y][j] + txt_bin[2 + y][j + 1] + txt_bin[2 + y][j + 2];
                        }

                        // Trata as instruções de estrutura: "JI, 56"
                        else if (txt_bin[2 + y][j] == "00110")       //instruções que utilizam o imediato e não utilizam R
                            instruction[programCounter++] = txt_bin[2 + y][j] + "00" + txt_bin[2 + y][j + 1];

                        else if (txt_bin[2 + y][j] == "01110")
                            instruction[programCounter++] = txt_bin[2 + y][j] + txt_bin[2 + y][j + 1] + "000000000";

                        else if (txt_bin[2 + y][j] == "1001000000000000")
                            instruction[programCounter++] = txt_bin[2 + y][j];

                        else if (txt_bin[2 + y][j] == "1000000000000000")
                            break;

                        else if (txt_bin[2 + y][j] == "00000")
                            instruction[programCounter++] = "0000000000000000";

                        // Trata as chamadas de função
                        else if (txt_bin[2 + y][j] == "01111")
                        {
                            int k = 0;
                            // Encontra qual função é a que foi chamada
                            while (address[2 + y][i] != wordCall[k])
                            {
                                k++;
                            }
                            int num = addressFunctions[k], c = 9;
                            //System.Console.WriteLine("\n\n" + num + "\n\n");
                            char[] bin = { '0', '0', '0', '0', '0', '0', '0', '0', '0' };
                            while (num != 0)
                            {
                                if ((num % 2) == 0)
                                {
                                    bin[--c] = '0';
                                }
                                else
                                {
                                    bin[--c] = '1';
                                }
                                num = num / 2;
                            }

                            string str = new string(bin);
                            instruction[programCounter++] = txt_bin[1][j] + "00" + str;
                        }

                        // Trata o restante das instruções
                        else
                            instruction[programCounter++] = txt_bin[2 + y][j] + txt_bin[2 + y][j + 1] + txt_bin[2 + y][j + 2] + txt_bin[2 + y][j + 3] + "00000";
                        j++;
                    }

                }
                lastIntAddress[y] = programCounter;
                y++;
            }

            while(programCounter < 2048)
            {
                instruction[programCounter++] = "0000000000000000";
            }

            // Guarda vetor de instruções em uma variável global
            return instruction;
        }
        #endregion Join Binaries


        public string[] assemblyInstructionGenerate()
        {
            int j = 0;
            for (int i = 0; i < 2048; i++)
            {
                if (i >= 0 && i <= lastFunctAddress - 1)
                {
                    assemblyInstruction[i] = instructionNoOrder[j++];
                }
                else if (i >= 512 && i <= lastIntAddress[0] - 1)
                {
                    assemblyInstruction[i] = instructionNoOrder[j++];
                }
                else if (i >= 768 && i <= lastIntAddress[1] - 1)
                {
                    assemblyInstruction[i] = instructionNoOrder[j++];
                }
                else if (i >= 1024 && i <= lastIntAddress[2] - 1)
                {
                    assemblyInstruction[i] = instructionNoOrder[j++];
                }
                else if (i >= 1280 && i <= lastIntAddress[3] - 1)
                {
                    assemblyInstruction[i] = instructionNoOrder[j++];
                }
                else if (i >= 1536 && i <= lastIntAddress[4] - 1)
                {
                    assemblyInstruction[i] = instructionNoOrder[j++];
                }
                else if (i >= 1791 && i <= lastIntAddress[5] - 1)
                {
                    assemblyInstruction[i] = instructionNoOrder[j++];
                }
                else
                    assemblyInstruction[i] = "NOP";
            }

            return assemblyInstruction;
        }
    }

}