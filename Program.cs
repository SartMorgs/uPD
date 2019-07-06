/*@autor: Morgana Sartor, Thaynara Mitie
 * Última modificação: 29/05/2019
 * 
 * 
 * Programa destinado a simular um processador monociclo composto por
 * 4 registradores internos de salvamento em seu banco de registradores, bem como possui uma
 * unidade lógica e aritmética (ULA)com 6 operação diferentes, uma unidade de controle a qual 
 * terá a função de decodififcar o opcode recebido (há 15 opcodes possíveis), e por fim, possuí
 * 3 memórias, da quais são RAM, ROM e LIFO.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace uPD
{
    static class Program
    {

        #region Variables
        // Programa escrito pelo usuário
        public static string fileName;                  // Programa atual
        public static string[] binProgram;              // Programa em binário
        public static string[] assemblyProgram;         // Programa Assembly
        public static int[] inValues = new int[512];    // Valores de entrada
        public static int[] inOrOut = new int[512];     // Set de endereços para IN ou OUT

        // Simulação
        public static string currentInstruction;        // Instrução atual
        public static int currentAddress;               // Endereço atual
        public static int inAddr;                       // Endereço de entrada
        public static int outAddr;                      // Endereço de saída
        public static int inValue;                      // Valor de entrada
        public static int outValue;                     // Valor de saída
        public static string error = "";                // Erros
        public static uint rs1;                         // Rs1
        public static uint rs2;                         // Rs2
        public static uint rdst;                        // Rdst
        public static int imed;                         // Imediato
        public static bool compare;                     // Flag CMP
        public static bool enablePC;                    // Flag de ativação do PC
        public static bool enableJmp;                   // Flag de ativação de Jump
        public static bool enableRet;                   // Flag de ativação de Ret
        public static string selectULA;                 // Select Ula
        public static string selectData;                // Seleção de origem de dados

        // Interrupções
        public static bool[] enableInt = new bool[6];       // Vetor de flags de interrupção
        public static int[] valueInt = new int[6];          // Valores das interrupções
        public static uint interruption;                    // Interrupção que está ativa
        public static bool enableInterruption;              // Flag que indica que uma interupção está ativa
        public static int[] addrInterruption = new int[6];  // Endereços de entrada das interrupções
        public static Queue<uint> interruptionQueue;        // Fila de interrupções

        // Tipo de arquivo VHDL
        public static string vhdl = null;                   // Tipo de arquivo de VHDL

        // Objetos dos componentes
        public static ControlUnit controller;
        public static InOut io;
        public static LIFO lifo;
        public static RAM ram;
        public static RegisterBase bco;
        public static ROM rom;
        public static Translator translator;
        public static ULA ula;

        #endregion Variables

        #region Instructions
        // Instrução LDI
        public static void LDI()
        {
            int imedBin;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            imedBin = Convert.ToInt32(currentInstruction.Substring(7, 9));
            imed = BinToInt(imedBin);

            bco.ChangeRegister(imed, rdst);
        }

        // Instrução ADD
        public static void ADD()
        {
            int outUla;
            int rs1Value, rs2Value;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            rs1 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 2))));
            rs2 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(9, 2))));

            rs1Value = bco.GetRegister(rs1);
            rs2Value = bco.GetRegister(rs2);
            bco.SetRs1(rs1Value);
            bco.SetRs2(rs2Value);
            ula.SetRs1(rs1Value);
            ula.SetRs2(rs2Value);
            ula.Operation("add");
            outUla = ula.GetOutValue();
            bco.ChangeRegister(outUla, rdst);
        }

        // Instrução SUB
        public static void SUB()
        {
            int outUla;
            int rs1Value, rs2Value;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            rs1 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 2))));
            rs2 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(9, 2))));

            rs1Value = bco.GetRegister(rs1);
            rs2Value = bco.GetRegister(rs2);
            bco.SetRs1(rs1Value);
            bco.SetRs2(rs2Value);
            ula.SetRs1(rs1Value);
            ula.SetRs2(rs2Value);
            ula.Operation("sub");
            outUla = ula.GetOutValue();
            bco.ChangeRegister(outUla, rdst);
        }

        // Instrução OUT
        public static void OUT()
        {
            int e, imedBin;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            imedBin = Convert.ToInt32(currentInstruction.Substring(7, 9));
            imed = BinToInt(imedBin);
            outAddr = imed;

            e = io.DataOut(Convert.ToInt32(rdst), imed);
            if (e == 1) error = "Registrador Inválido!";
            else if (e == 2) error = "Endereço de saída inválido";
            else if (e == 4) error = "Endereço já setado como entrada!";
            else if (e == 5) error = "Endereço já setado como interrupção!";
            else
            {
                error = "";
                outValue = bco.GetRegister(rdst);
                io.SetAddrType(outAddr, 2);
                io.SetOutValue(outValue);
            }
        }

        // Instrução IN
        public static void IN()
        {
            int e, imedBin;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            imedBin = Convert.ToInt32(currentInstruction.Substring(7, 9));
            imed = BinToInt(imedBin);

            e = io.DataIn(Convert.ToInt32(rdst), imed);
            if (e == 1) error = "Registrador Inválido!";
            else if (e == 2) error = "Endereço de entrada inválido";
            else if (e == 3) error = "Endereço já setado como saída!";
            else if (e == 5) error = "Endereço já setado como interrupção!";
            else if (inAddr != imed) error = "Endereço de entrada não correspondente";
            else
            {
                error = "";
                io.SetInValue(inValue);
                bco.ChangeRegister(inValue, rdst);
            }
        }

        // Instrução JI
        public static void JI()
        {
            // Program Counter
            return;
        }

        // Instrução LD
        public static void LD()
        {
            int value, imedBin;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            imedBin = Convert.ToInt32(currentInstruction.Substring(7, 9));
            imed = BinToInt(imedBin);

            value = ram.ViewMemoryValue(Convert.ToUInt32(imed));
            bco.ChangeRegister(value, rdst);
        }

        // Instrução STO
        public static void STO()
        {
            int value, imedBin;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            imedBin = Convert.ToInt32(currentInstruction.Substring(7, 9));
            imed = BinToInt(imedBin);

            value = bco.GetRegister(rdst);
            ram.ChangeMemoryValue(value, Convert.ToUInt32(imed));
        }

        // Instrução JZ
        public static void JZ()
        {
            ula.SetRs1(bco.GetRegister(Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))))));
            ula.Operation(selectULA);
            // Program Counter
            return;
        }

        // Instrução CMP
        public static void CMP()
        {
            int rs1Value, rs2Value;

            rs1 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 2))));
            rs2 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(9, 2))));

            rs1Value = bco.GetRegister(rs1);
            rs2Value = bco.GetRegister(rs2);

            bco.SetRs1(rs1Value);
            bco.SetRs2(rs2Value);

            ula.SetRs1(rs1Value);
            ula.SetRs2(rs2Value);

            ula.Operation("cmp");

            if (ula.GetOutValue() == 1) compare = true;
            else compare = false;
        }

        // Instrução JE
        public static void JE()
        {

            // Program Counter
        }

        // Instrução AND
        public static void AND()
        {
            int outUla;
            int rs1Value, rs2Value;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            rs1 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 2))));
            rs2 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(9, 2))));

            rs1Value = bco.GetRegister(rs1);
            rs2Value = bco.GetRegister(rs2);
            bco.SetRs1(rs1Value);
            bco.SetRs2(rs2Value);
            ula.SetRs1(rs1Value);
            ula.SetRs2(rs2Value);
            ula.Operation("and");
            outUla = ula.GetOutValue();
            bco.ChangeRegister(outUla, rdst);
        }

        // Instrução OR
        public static void OR()
        {
            int outUla;
            int rs1Value, rs2Value;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            rs1 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 2))));
            rs2 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(9, 2))));

            rs1Value = bco.GetRegister(rs1);
            rs2Value = bco.GetRegister(rs2);
            bco.SetRs1(rs1Value);
            bco.SetRs2(rs2Value);
            ula.SetRs1(rs1Value);
            ula.SetRs2(rs2Value);
            ula.Operation("or");
            outUla = ula.GetOutValue();
            bco.ChangeRegister(outUla, rdst);
        }

        // Instrução XOR
        public static void XOR()
        {
            int outUla;
            int rs1Value, rs2Value;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            rs1 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 2))));
            rs2 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(9, 2))));

            rs1Value = bco.GetRegister(rs1);
            rs2Value = bco.GetRegister(rs2);
            bco.SetRs1(rs1Value);
            bco.SetRs2(rs2Value);
            ula.SetRs1(rs1Value);
            ula.SetRs2(rs2Value);
            ula.Operation("xor");
            outUla = ula.GetOutValue();
            bco.ChangeRegister(outUla, rdst);
        }

        // Instrução NOT
        public static void NOT()
        {
            int outUla;
            int rs1Value, rs2Value;

            rdst = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(5, 2))));
            rs1 = Convert.ToUInt32(BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 2))));

            rs1Value = bco.GetRegister(rs1);
            bco.SetRs1(rs1Value);
            bco.SetRs2(0);
            ula.SetRs1(rs1Value);
            ula.Operation("not");
            outUla = ula.GetOutValue();
            bco.ChangeRegister(outUla, rdst);
        } 

        // Instrução CALL
        public static void CALL()
        {
            int r0, r1, r2, r3;

            // Salva o contexto
            r0 = bco.GetRegister(0);
            r1 = bco.GetRegister(1);
            r2 = bco.GetRegister(2);
            r3 = bco.GetRegister(3);
            lifo.Push(r0, r1, r2, r3, currentAddress+1);
            return;
        }

        // Instrução STOP
        public static void STOP()
        {
            // Trava o sistema
        }

        // Instrução RET
        public static void RET()
        {
            int r0, r1, r2, r3;

            // Pop LIFO
            lifo.Pop();

            // Restaura o contexto
            r0 = BinToInt(lifo.GetR0());
            r1 = BinToInt(lifo.GetR1());
            r2 = BinToInt(lifo.GetR2());
            r3 = BinToInt(lifo.GetR3());
            bco.ChangeRegister(r0, 0);
            bco.ChangeRegister(r1, 1);
            bco.ChangeRegister(r2, 2);
            bco.ChangeRegister(r3, 3);
        }

        // Instrução NOP
        public static void NOP()
        {
            // Não faz nada
        }

        // Instrução RETI
        public static void RETI()
        {
            int r0, r1, r2, r3;

            // Pop LIFO
            lifo.EnablePop();
            lifo.Pop();

            // Restaura contexto
            r0 = BinToInt(lifo.GetR0());
            r1 = BinToInt(lifo.GetR1());
            r2 = BinToInt(lifo.GetR2());
            r3 = BinToInt(lifo.GetR3());
            bco.ChangeRegister(r0, 0);
            bco.ChangeRegister(r1, 1);
            bco.ChangeRegister(r2, 2);
            bco.ChangeRegister(r3, 3);

            enableInterruption = false;
        }

        // Instrução SETR
        public static void SETR()
        {
            int imedBin;

            imedBin = Convert.ToInt32(currentInstruction.Substring(7, 9));
            imed = BinToInt(imedBin);

            // Habilita as interrupções
            enableInt[imed] = true;
        }
        #endregion Instructions

        #region Program Counter
        public static void ProgramCounter()
        {
            switch(currentInstruction.Substring(0, 5))
            {
                // JI
                case "00110":
                    if (enableInterruption)
                    {
                        int imed = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9)));
                        switch (interruption)
                        {
                            case 0:
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 512;
                                break;
                            case 1:
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 768;
                                break;
                            case 2:
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1024;
                                break;
                            case 3:
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1280;
                                break;
                            case 4:
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1536;
                                break;
                            case 5:
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1792;
                                break;
                        }
                    }
                    else
                    {
                        if(currentAddress > translator.GetAmountInstMain())
                        {
                            int contFun = translator.GetAmountInstMain();
                            for (uint i = 0; i < translator.GetAmountFunctions(); i++)
                            {
                                if (currentAddress <= (contFun + translator.GetAmountInstFunction()[i]))
                                {
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + contFun;   // Salta para o endereço
                                }
                                contFun += translator.GetAmountInstFunction()[i];
                            }
                        }
                        else
                            currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9)));   // Salta para o endereço
                    }
                    break;
                // JZ
                case "01001":
                    rdst = Convert.ToUInt32(currentInstruction.Substring(5, 2));
                    imed = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9)));

                    if (ula.GetOutValue() == 1)
                    {
                        if (enableInterruption)
                        {
                            switch (interruption)
                            {
                                case 0:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 512;
                                    break;
                                case 1:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 768;
                                    break;
                                case 2:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1024;
                                    break;
                                case 3:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1280;
                                    break;
                                case 4:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1536;
                                    break;
                                case 5:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1792;
                                    break;
                            }
                        }
                        else
                        {
                            if (currentAddress > translator.GetAmountInstMain())
                            {
                                int contFun = translator.GetAmountInstMain();
                                for (uint i = 0; i < translator.GetAmountFunctions(); i++)
                                {
                                    if (currentAddress <= (contFun + translator.GetAmountInstFunction()[i]))
                                    {
                                        currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + contFun;   // Salta para o endereço
                                    }
                                    contFun += translator.GetAmountInstFunction()[i];
                                }
                            }
                            else
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9)));   // Salta para o endereço
                        }
                    }
                    else
                    {
                        currentAddress += 1;
                        enableJmp = false;
                    }

                    break;
                // JE
                case "01010":
                    imed = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9)));
                    if (compare)
                    {
                        if (enableInterruption)
                        {
                            switch (interruption)
                            {
                                case 0:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 512;
                                    break;
                                case 1:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 768;
                                    break;
                                case 2:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1024;
                                    break;
                                case 3:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1280;
                                    break;
                                case 4:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1536;
                                    break;
                                case 5:
                                    currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + 1792;
                                    break;
                            }
                        }
                        else
                        {
                            if (currentAddress > translator.GetAmountInstMain())
                            {
                                int contFun = translator.GetAmountInstMain();
                                for (uint i = 0; i < translator.GetAmountFunctions(); i++)
                                {
                                    if (currentAddress <= (contFun + translator.GetAmountInstFunction()[i]))
                                    {
                                        currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9))) + contFun;   // Salta para o endereço
                                    }
                                    contFun += translator.GetAmountInstFunction()[i];
                                }
                            }
                            else
                                currentAddress = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9)));   // Salta para o endereço
                        }
                    }
                    else currentAddress += 1;
                    break;
                // CALL
                case "01111":
                    imed = BinToInt(Convert.ToInt32(currentInstruction.Substring(7, 9)));
                    if (enableInterruption)
                    {
                        error = "Não é possível realizar chamada de função em uma Interrupção!";
                    }
                    else
                        currentAddress = imed;                                          // Salta para o endereço da função
                    break;
                // STOP
                case "10010":
                    // O endereço não muda
                    break;
                // RET
                case "10011":
                    if (enableInterruption)
                        error = "Não há retorno de função em Interrupções!";
                    else
                        currentAddress = BinToInt(lifo.GetAddrPC());
                    break;
                // RETI
                case "10000":
                    currentAddress = BinToInt(lifo.GetAddrPC());
                    break;
                default:
                    currentAddress += 1;
                    break;
            }
        }
        #endregion Program Counter

        #region Interruptions
        // Fila de Interrupções
        public static void StackInterruption(uint intrr)
        {
            interruptionQueue.Enqueue(intrr);
        }

        // Chamada de interrupção
        public static void CallInterruption()
        {
            enableInterruption = true;
            interruption = interruptionQueue.Dequeue();

            // Salva contexto
            lifo.EnablePush();
            lifo.Push(bco.GetRegister(0), bco.GetRegister(1), bco.GetRegister(2), bco.GetRegister(3), currentAddress);

            // Endereço da interrupção
            switch (interruption)
            {
                case 0:
                    currentAddress = 512;
                    break;
                case 1:
                    currentAddress = 768;
                    break;
                case 2:
                    currentAddress = 1024;
                    break;
                case 3:
                    currentAddress = 1280;
                    break;
                case 4:
                    currentAddress = 1536;
                    break;
                case 5:
                    currentAddress = 1792;
                    break;
            }
            currentInstruction = binProgram[currentAddress];
        }
        #endregion Interruptions

        #region Functions
        // Converte de Binário para Inteiro
        public static int BinToInt(int bin)
        {
            int result = 0, aux = bin, i = 0;

            while (aux != 0)
            {
                result += (Convert.ToInt32(Math.Pow(Convert.ToDouble(2), Convert.ToDouble(i))) * (aux % 10));
                aux /= 10;
                i++;
            }

            return result;
        }

        // Traduz programa
        public static void Translate(string assembly, string filename)
        {
            translator = new Translator();
            translator.SetFile(assembly);
            binProgram = translator.InstructionGenerate();
            assemblyProgram = translator.assemblyInstructionGenerate();
            fileName = filename;
            if (binProgram == null) error = "Erro ao compilar código!";
        }

        // Inicialização da execução
        public static void StartSim()
        {
            controller = new ControlUnit();
            io = new InOut();
            lifo = new LIFO();
            ram = new RAM();
            bco = new RegisterBase();
            rom = new ROM();
            ula = new ULA();


            rom.SetProgram(binProgram);                                                 // Setando a ROM com o programa
            currentAddress = 0;                                                         // Setando o endereço inicial
            currentInstruction = rom.GetInstruction(Convert.ToUInt32(currentAddress));  // Setando a instrução inicial

            // Zerando vetor de IN
            for(uint i = 0; i < 512; i++)
            {
                inValues[i] = 0;
            }

            // Limpando
            io.CleanInBox();
            io.CleanOutBox();
            ram.CleanDataMemory();
            bco.CleanRegisterBCO();
            bco.CleanRs();
            ula.CleanBox();

            // Zerando variáveis
            rs1 = 0;
            rs2 = 0;
            rdst = 0;
            imed = 0;
            inValue = 0;
            outValue = 0;

            // Interrupções
            for (uint i = 0; i < 6; i++)
            {
                enableInt[i] = false;
                addrInterruption[i] = 513;
            }

            // InOut
            inValues = io.GetInVector();
            inOrOut = io.GetInOrOutVector();

            enableInterruption = false;
            interruption = 0;
            interruptionQueue = new Queue<uint>();
        }

        // Para de executar o programa
        public static void StopProgram()
        {
            currentInstruction = "XXXX";
            currentAddress = 0;
            binProgram = null;
            assemblyProgram = null;
            fileName = null;
            
            controller = null;
            io = null;
            lifo = null;
            ram = null;
            bco = null;
            rom = null;
            translator = null;
            ula = null;

            for(uint i = 0; i < 6; i++)
            {
                enableInt[i] = false;
            }

            error = "";
        }
        
        // Execução passo a passo
        public static void StepByStep()
        {
            // Configurações do controlador
            controller.SetCurrentInstruction(currentInstruction);
            controller.SetAddrInstruction(currentAddress);
            controller.SetControlFlags();

            #region Components Enable's Flag
            // PC
            enablePC = controller.GetEnablePC();

            // IO
            if (controller.GetWriteEnableIO() == true) io.EnableWrite();
            else io.DisableWrite();
            if (controller.GetEnableReadIO() == true) io.EnableRead();
            else io.DisableRead();

            // LIFO
            if (controller.GetEnablePush() == true) lifo.EnablePush();
            else lifo.DisablePush();
            if (controller.GetEnabelPop() == true) lifo.EnablePop();
            else lifo.DisablePop();

            // RAM
            if (controller.GetWriteEnableRAM() == true) ram.EnableWrite();
            else ram.DisableWrite();
            if (controller.GetWriteEnableRAM() == true) ram.EnableRAM();
            else ram.DisableRAM();

            // BCO
            if (controller.GetWriteEnableBCO() == true) bco.EnableWrite();
            else bco.DisableWrite();

            // ROM
            if (controller.GetEnableROM() == true) rom.EnableROM();
            else rom.DisableROM();


            enableJmp = controller.GetEnableJmp();
            enableRet = controller.GetEnableRet();

            // Selecionador de ULA
            switch (controller.GetSelectULA())
            {
                case 0:
                    selectULA = "add";
                    break;
                case 1:
                    selectULA = "sub";
                    break;
                case 2:
                    selectULA = "and";
                    break;
                case 3:
                    selectULA = "or";
                    break;
                case 4:
                    selectULA = "xor";
                    break;
                case 5:
                    selectULA = "not";
                    break;
                case 6:
                    selectULA = "cmp";
                    break;
                case 7:
                    selectULA = "jmp";
                    break;
            }

            // Selecionador de origem de dados
            switch (controller.GetSelectData())
            {
                case 0:
                    selectData = "00";
                    break;
                case 1:
                    selectData = "01";
                    break;
                case 2:
                    selectData = "10";
                    break;
                case 3:
                    selectData = "11";
                    break;
            }

            #endregion Components Enable's Flag

            // Se há interrupção na fila e não há nenhuma ativa
            if(interruptionQueue.Count != 0 && !enableInterruption)
            {
                CallInterruption();
                return;
            }
            else if (enableInterruption)
            {
                lifo.DisablePush();
            }
            else if (!enableInterruption)
            {
                lifo.DisablePop();
            }

            switch(currentInstruction.Substring(0, 5))
            {
                // LDI
                case "00001":
                    LDI();
                    break;
                // ADD
                case "00010":
                    ADD();
                    break;
                // SUB
                case "00011":
                    SUB();
                    break;
                case "00100":
                    OUT();
                    break;
                // IN
                case "00101":
                    IN();
                    break;
                // JI
                case "00110":
                    JI();
                    break;
                // LD
                case "00111":
                    LD();
                    break;
                // STO
                case "01000":
                    STO();
                    break;
                // JZ
                case "01001":
                    JZ();
                    break;
                // CMP
                case "10100":
                    CMP();
                    break;
                // JE
                case "01010":
                    JE();
                    break;
                // AND
                case "01011":
                    AND();
                    break;
                // OR
                case "01100":
                    OR();
                    break;
                // XOR
                case "01101":
                    XOR();
                    break;
                // NOT
                case "01110":
                    NOT();
                    break;
                // CALL
                case "01111":
                    CALL();
                    break;
                // STOP
                case "10010":
                    // O endereço não muda
                    break;
                // RET
                case "10011":
                    RET();
                    break;
                // NOP
                case "00000":
                    NOP();
                    break;
                // RETI
                case "10000":
                    RETI();
                    break;
                // SETR
                case "10001":
                    SETR();
                    break;
            }

            // Program Counter
            ProgramCounter();
            currentInstruction = rom.GetInstruction(Convert.ToUInt32(currentAddress));
        }
        #endregion

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form_uPD formP = new Form_uPD();
            formP.ShowDialog();
        }

    }
}

