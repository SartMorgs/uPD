using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Text.RegularExpressions;

namespace uPD
{
    public partial class Form_uPD : Form
    {
        #region Variables
        SaveFileDialog SF = new SaveFileDialog();
        OpenFileDialog OF = new OpenFileDialog();
        StreamWriter SW;
        StreamReader SR;

        string fileName = null;
        string path = null;
        string instructionLine = null;
        static string[] oldRom = Regex.Split(Componentes_VHDL.rom, @"\n", RegexOptions.None);
        string[] newRom = new string[oldRom.Length + 2047];

        int previousAddr = 0;
        string previousinstruction = null;

        bool compilation = false;
        bool endProgram = true;
        bool playProgram = true;
        bool valueHexa = false;

        int inAddr;
        int inValue;
        int outValue;
        int outAddr;

        string r0_hex = "0000";
        string r1_hex = "0000";
        string r2_hex = "0000";
        string r3_hex = "0000";

        int r0 = 0;
        int r1 = 0;
        int r2 = 0;
        int r3 = 0;
        #endregion Variables

        #region Methods
        // Muda valores para Hexa
        private void ChangeHexa()
        {
            if (valueHexa)
            {
                // RAM
                int k = 0;
                // Split = 256
                for (int i = 0; i < 64; i++)
                {
                    for (int j = 2; j < 10; j++)
                    {
                        dataGrid_RAM[j, i].Value = Hexadecimal(Program.ram.GetMemory()[k++]);
                    }
                }

                // Atribui os valores dos registradores temporários
                r0_hex = Hexadecimal(Program.bco.GetRegister(0));
                r1_hex = Hexadecimal(Program.bco.GetRegister(1));
                r2_hex = Hexadecimal(Program.bco.GetRegister(2));
                r3_hex = Hexadecimal(Program.bco.GetRegister(3));

                // Registradores
                lb_REG0.Text = Hexadecimal(Program.bco.GetRegister(0));
                lb_REG1.Text = Hexadecimal(Program.bco.GetRegister(1));
                lb_REG2.Text = Hexadecimal(Program.bco.GetRegister(2));
                lb_REG3.Text = Hexadecimal(Program.bco.GetRegister(3));
                txt_PC.Text = Hexadecimal(Program.currentAddress);

                // In
                txt_valueIn.Text = Hexadecimal(inValue);

                // Out
                outValue = Convert.ToInt32(txt_valueOut.Text);
                txt_valueOut.Text = Hexadecimal(outValue);
            }
            else
            {
                // RAM
                ChangeRAMTable();

                // Atribui valores aos registradores temporários
                r0 = Program.bco.GetRegister(0);
                r1 = Program.bco.GetRegister(1);
                r2 = Program.bco.GetRegister(2);
                r3 = Program.bco.GetRegister(3);

                // Registradores
                lb_REG0.Text = Program.bco.GetRegister(0).ToString();
                lb_REG1.Text = Program.bco.GetRegister(1).ToString();
                lb_REG2.Text = Program.bco.GetRegister(2).ToString();
                lb_REG3.Text = Program.bco.GetRegister(3).ToString();
                txt_PC.Text = Program.currentAddress.ToString();

                // In
                txt_valueIn.Text = inValue.ToString();
                tb_AddrInt0.Text = Program.addrInterruption[0].ToString();
                tb_AddrInt1.Text = Program.addrInterruption[1].ToString();
                tb_AddrInt2.Text = Program.addrInterruption[2].ToString();
                tb_AddrInt3.Text = Program.addrInterruption[3].ToString();
                tb_AddrInt4.Text = Program.addrInterruption[4].ToString();
                tb_AddrInt5.Text = Program.addrInterruption[5].ToString();

                // Out
                txt_valueOut.Text = outValue.ToString();
            }
        }

        // Mostra valores em Hexa
        private void ShowHexaValue()
        {
            if (valueHexa)
            {
                // RAM
                for (int i = 0; i < 64; i++)
                {
                    for (int j = 2; j < 10; j++)
                    {
                        dataGrid_RAM[j, i].Value = Hexadecimal(Program.ram.GetMemory()[i + (j - 2)]);
                    }
                }

                // Limpa todos os destaques;
                lb_REG0.BackColor = Color.White;
                lb_REG1.BackColor = Color.White;
                lb_REG2.BackColor = Color.White;
                lb_REG3.BackColor = Color.White;

                // Registradores
                lb_REG0.Text = Hexadecimal(Program.bco.GetRegister(0));
                lb_REG1.Text = Hexadecimal(Program.bco.GetRegister(1));
                lb_REG2.Text = Hexadecimal(Program.bco.GetRegister(2));
                lb_REG3.Text = Hexadecimal(Program.bco.GetRegister(3));
                txt_PC.Text = Hexadecimal(Program.currentAddress);

                // Destaca linha de registrador se ele mudar de valor
                if (Hexadecimal(Program.bco.GetRegister(0)) != r0_hex) lb_REG0.BackColor = Color.GreenYellow;
                if (Hexadecimal(Program.bco.GetRegister(1)) != r1_hex) lb_REG1.BackColor = Color.GreenYellow;
                if (Hexadecimal(Program.bco.GetRegister(2)) != r2_hex) lb_REG2.BackColor = Color.GreenYellow;
                if (Hexadecimal(Program.bco.GetRegister(3)) != r3_hex) lb_REG3.BackColor = Color.GreenYellow;

                // Atribui registradores temporários
                r0_hex = Hexadecimal(Program.bco.GetRegister(0));
                r1_hex = Hexadecimal(Program.bco.GetRegister(1));
                r2_hex = Hexadecimal(Program.bco.GetRegister(2));
                r3_hex = Hexadecimal(Program.bco.GetRegister(3));

                // Out
                if (Program.io.GetWriteEnable())
                {
                    outValue = Program.outAddr;
                    outAddr = Program.outValue;
                }
                txt_AddrOut.Text = Hexadecimal(outAddr);
                txt_valueOut.Text = Hexadecimal(outValue);
            }
        }

        // Converte para Hexadecimal
        private string Hexadecimal(int value)
        {

            string str;
            int k, num;

            // Converte um inteiro decimal para uma string binária
            k = 4; num = value;
            //System.Console.WriteLine("\n\n" + num + "\n\n");
            char[] bin1 = { '0', '0', '0', '0'};
            while (num != 0)
            {
                if ((num % 16) == 0)
                {
                    bin1[--k] = '0';
                }
                else
                {
                    switch (num % 16)
                    {
                        case 1:
                            bin1[--k] = '1';
                            break;
                        case 2:
                            bin1[--k] = '2';
                            break;
                        case 3:
                            bin1[--k] = '3';
                            break;
                        case 4:
                            bin1[--k] = '4';
                            break;
                        case 5:
                            bin1[--k] = '5';
                            break;
                        case 6:
                            bin1[--k] = '6';
                            break;
                        case 7:
                            bin1[--k] = '7';
                            break;
                        case 8:
                            bin1[--k] = '8';
                            break;
                        case 9:
                            bin1[--k] = '9';
                            break;
                        case 10:
                            bin1[--k] = 'A';
                            break;
                        case 11:
                            bin1[--k] = 'B';
                            break;
                        case 12:
                            bin1[--k] = 'C';
                            break;
                        case 13:
                            bin1[--k] = 'D';
                            break;
                        case 14:
                            bin1[--k] = 'E';
                            break;
                        case 15:
                            bin1[--k] = 'F';
                            break;
                        default:
                            break;
                    }
                }
                num = num / 16;
            }

            str = new string(bin1);

            return str;
        }

        // Mudança na tabela da RAM
        private void ChangeRAMTable()
        {
            int k = 0;
            // Split = 256
            for (int i = 0; i < 64; i++)
            {
                for(int j = 2; j < 10; j++)
                {
                    dataGrid_RAM[j, i].Value = Program.ram.GetMemory()[k++];
                }
            }
        }

        // Fim de execução
        private void EndExecution()
        {
            compilation = false;
            endProgram = true;
            previousAddr = 0;

            // Desmarcando instrução
            dataGrid_Adress.Rows.Clear();

            // Zerando Registradores
            lb_REG0.Text = 0.ToString();
            lb_REG1.Text = 0.ToString();
            lb_REG2.Text = 0.ToString();
            lb_REG3.Text = 0.ToString();
            txt_PC.Text = 0.ToString();

            // Zerando RAM
            dataGrid_RAM.Rows.Clear();

            // In Out
            tb_AddrInt0.Text = 0.ToString();
            tb_AddrInt1.Text = 0.ToString();
            tb_AddrInt2.Text = 0.ToString();
            tb_AddrInt3.Text = 0.ToString();
            tb_AddrInt4.Text = 0.ToString();
            tb_AddrInt5.Text = 0.ToString();

            // Interface
            tab_Editar.BackColor = Color.White;
            tab_Executar_Texto.SelectedTab = tab_Editar;

            // Desativando edição no texto
            rchtxt_Editor.Enabled = true;

            // Ativando e desativando botões de execução
            bt_execucao.Enabled = false;
            bt_passo_passo_prosseguir.Enabled = false;
            bt_stop.Enabled = false;
            bt_pause.Enabled = false;
            bt_voltar_comeco.Enabled = false;
            bt_compilar.Enabled = true;
            bt_abrir_arquivo.Enabled = true;
            bt_New_File.Enabled = true;
            btn_LoadVHDLFile.Enabled = false;

            Program.StopProgram();

            Array.Clear(newRom, 0, newRom.Length);
        }

        // Execução
        private async void Execute()
        {
            if (!compilation || endProgram)
            {
                return;
            }
            else
            {

                while (playProgram && !endProgram)
                {
                    if (Program.currentAddress == 0)
                    {
                        dataGrid_Adress.Rows[Program.currentAddress].DefaultCellStyle.BackColor = Color.GreenYellow;
                    }
                    else
                    {
                        // Highlight linha
                        dataGrid_Adress.Rows[Program.currentAddress].DefaultCellStyle.BackColor = Color.GreenYellow;
                        dataGrid_Adress.Rows[previousAddr].DefaultCellStyle.BackColor = Color.White;
                        previousAddr = Program.currentAddress;
                        previousinstruction = Program.currentInstruction;
                    }

                    #region Interruptions
                    // Ativa/Desativa Interrupções
                    bt_INT0.Enabled = Program.enableInt[0];
                    bt_INT1.Enabled = Program.enableInt[1];
                    bt_INT2.Enabled = Program.enableInt[2];
                    bt_INT3.Enabled = Program.enableInt[3];
                    bt_INT4.Enabled = Program.enableInt[4];
                    bt_INT5.Enabled = Program.enableInt[5];


                    txt_fila_inter.Text = " Fila de interrupções: ";
                    foreach (var intrr in Program.interruptionQueue.ToArray())
                    {
                        txt_fila_inter.Text += " | " + intrr.ToString();
                    }
                    #endregion Interruptions

                    // Foca linha selecionada
                    dataGrid_Adress.FirstDisplayedScrollingRowIndex = Program.currentAddress;

                    Program.StepByStep();                                       // Execução passo a passo

                    // Componentes/Blocos do Processador
                    ChangeComponentFlags();

                    #region Erro
                    if (Program.error != "")
                    {
                        var result = MessageBox.Show(Program.error + "\nA execução será encerrada.", "Erro!", MessageBoxButtons.OK);
                        if (result == DialogResult.OK)
                        {
                            EndExecution();
                            Program.error = "";
                            return;
                        }
                    }
                    #endregion Erro

                    if (valueHexa)
                    {
                        ShowHexaValue();
                    }
                    else
                    {
                        // Limpa todos os destaques;
                        lb_REG0.BackColor = Color.White;
                        lb_REG1.BackColor = Color.White;
                        lb_REG2.BackColor = Color.White;
                        lb_REG3.BackColor = Color.White;

                        // Registradores
                        lb_REG0.Text = Program.bco.GetRegister(0).ToString();
                        lb_REG1.Text = Program.bco.GetRegister(1).ToString();
                        lb_REG2.Text = Program.bco.GetRegister(2).ToString();
                        lb_REG3.Text = Program.bco.GetRegister(3).ToString();
                        txt_PC.Text = Program.currentAddress.ToString();

                        // Destaca linha de registrador se ele mudar de valor
                        if (Program.bco.GetRegister(0) != r0) lb_REG0.BackColor = Color.GreenYellow;
                        if (Program.bco.GetRegister(1) != r1) lb_REG1.BackColor = Color.GreenYellow;
                        if (Program.bco.GetRegister(2) != r2) lb_REG2.BackColor = Color.GreenYellow;
                        if (Program.bco.GetRegister(3) != r3) lb_REG3.BackColor = Color.GreenYellow;

                        // Atribui registradores temporários
                        r0 = Program.bco.GetRegister(0);
                        r1 = Program.bco.GetRegister(1);
                        r2 = Program.bco.GetRegister(2);
                        r3 = Program.bco.GetRegister(3);

                        //Out
                        txt_AddrOut.Text = Program.outAddr.ToString();
                        txt_valueOut.Text = Program.outValue.ToString();

                        // Muda tabela RAM
                        ChangeRAMTable();
                    }

                    // Verifica se o programa chegou ao fim
                    if ((Program.currentAddress == Program.translator.GetAmountInstMain() + Program.translator.GetAmountInstProg()) && previousinstruction.Substring(0, 5) != "01111")
                    {
                        #region Last Execute
                        Program.StepByStep();                                       // Execução passo a passo

                        // Componentes/Blocos do Processador
                        ChangeComponentFlags();

                        #region Erro
                        if (Program.error != "")
                        {
                            var result2 = MessageBox.Show(Program.error + "\nA execução será encerrada.", "Erro!", MessageBoxButtons.OK);
                            if (result2 == DialogResult.OK)
                            {
                                EndExecution();
                                Program.error = "";
                                return;
                            }
                        }
                        #endregion Erro

                        if (valueHexa)
                        {
                            ShowHexaValue();
                        }
                        else
                        {
                            // Limpa todos os destaques;
                            lb_REG0.BackColor = Color.White;
                            lb_REG1.BackColor = Color.White;
                            lb_REG2.BackColor = Color.White;
                            lb_REG3.BackColor = Color.White;

                            // Registradores
                            lb_REG0.Text = Program.bco.GetRegister(0).ToString();
                            lb_REG1.Text = Program.bco.GetRegister(1).ToString();
                            lb_REG2.Text = Program.bco.GetRegister(2).ToString();
                            lb_REG3.Text = Program.bco.GetRegister(3).ToString();
                            txt_PC.Text = Program.currentAddress.ToString();

                            // Destaca linha de registrador se ele mudar de valor
                            if (Program.bco.GetRegister(0) != r0) lb_REG0.BackColor = Color.GreenYellow;
                            if (Program.bco.GetRegister(1) != r1) lb_REG1.BackColor = Color.GreenYellow;
                            if (Program.bco.GetRegister(2) != r2) lb_REG2.BackColor = Color.GreenYellow;
                            if (Program.bco.GetRegister(3) != r3) lb_REG3.BackColor = Color.GreenYellow;

                            // Atribui registradores temporários
                            r0 = Program.bco.GetRegister(0);
                            r1 = Program.bco.GetRegister(1);
                            r2 = Program.bco.GetRegister(2);
                            r3 = Program.bco.GetRegister(3);

                            //Out
                            txt_AddrOut.Text = Program.outAddr.ToString();
                            txt_valueOut.Text = Program.outValue.ToString();

                            // Muda tabela RAM
                            ChangeRAMTable();
                        }
                        #endregion Last Execute

                        var result = MessageBox.Show("O programa chegou ao fim. Deseja reiniciar a execução?", "Fim da Execução", MessageBoxButtons.YesNo);
                        if (result == DialogResult.Yes)
                        {
                            for (int i = 0; i < dataGrid_Adress.RowCount; i++)
                            {
                                dataGrid_Adress.Rows[i].DefaultCellStyle.BackColor = Color.White;
                            }

                            Program.StartSim(); Program.StartSim();

                            // Foca linha selecionada
                            dataGrid_Adress.FirstDisplayedScrollingRowIndex = Program.currentAddress;

                            // Registradores
                            lb_REG0.Text = Program.bco.GetRegister(0).ToString();
                            lb_REG1.Text = Program.bco.GetRegister(1).ToString();
                            lb_REG2.Text = Program.bco.GetRegister(2).ToString();
                            lb_REG3.Text = Program.bco.GetRegister(3).ToString();
                            txt_PC.Text = Program.currentAddress.ToString();

                            // Muda tabela RAM
                            ChangeRAMTable();
                        }
                        else
                        {
                            EndExecution();
                        }
                    }

                    await Task.Delay(1000);
                }
            }
        }

        // Blocos
        private void ChangeComponentFlags()
        {
            // Pop
            if (Program.lifo.GetPopEnable())
            {
                lb_POP.BackColor = Color.LightGreen;
                lb_OUT_PC.Text = "PC = " + Program.lifo.GetAddrPC();
                lb_OUT_R0.Text = "R0 = " + Program.lifo.GetR0();
                lb_OUT_R1.Text = "R1 = " + Program.lifo.GetR1();
                lb_OUT_R2.Text = "R2 = " + Program.lifo.GetR2();
                lb_OUT_R3.Text = "R3 = " + Program.lifo.GetR3();
                lb_OUT_PC.BackColor = Color.LightGreen;
                lb_OUT_R0.BackColor = Color.LightGreen;
                lb_OUT_R1.BackColor = Color.LightGreen;
                lb_OUT_R2.BackColor = Color.LightGreen;
                lb_OUT_R3.BackColor = Color.LightGreen;
                lb_DIN_LIFO.BackColor = Color.LightGreen;
            }
            else
            {
                lb_POP.BackColor = Color.White;
                lb_OUT_PC.Text = "PC";
                lb_OUT_R0.Text = "R0";
                lb_OUT_R1.Text = "R1";
                lb_OUT_R2.Text = "R2";
                lb_OUT_R3.Text = "R3";
                lb_OUT_PC.BackColor = Color.White;
                lb_OUT_R0.BackColor = Color.White;
                lb_OUT_R1.BackColor = Color.White;
                lb_OUT_R2.BackColor = Color.White;
                lb_OUT_R3.BackColor = Color.White;
                lb_DIN_LIFO.BackColor = Color.White;
            }

            // Push
            if (Program.lifo.GetPushEnable())
            {
                lb_PUSH.BackColor = Color.LightGreen;
                lb_IN_PC.Text = "PC = " + Program.lifo.GetAddrPC();
                lb_IN_R0.Text = "R0 = " + Program.lifo.GetR0();
                lb_IN_R1.Text = "R1 = " + Program.lifo.GetR1();
                lb_IN_R2.Text = "R2 = " + Program.lifo.GetR2();
                lb_IN_R3.Text = "R3 = " + Program.lifo.GetR3();
                lb_IN_PC.BackColor = Color.LightGreen;
                lb_IN_R0.BackColor = Color.LightGreen;
                lb_IN_R1.BackColor = Color.LightGreen;
                lb_IN_R2.BackColor = Color.LightGreen;
                lb_IN_R3.BackColor = Color.LightGreen;
                lb_DOUT_LIFO.BackColor = Color.LightGreen;
            }
            else
            {
                lb_PUSH.BackColor = Color.White;
                lb_IN_PC.Text = "PC";
                lb_IN_R0.Text = "R0";
                lb_IN_R1.Text = "R1";
                lb_IN_R2.Text = "R2";
                lb_IN_R3.Text = "R3";
                lb_IN_PC.BackColor = Color.White;
                lb_IN_R0.BackColor = Color.White;
                lb_IN_R1.BackColor = Color.White;
                lb_IN_R2.BackColor = Color.White;
                lb_IN_R3.BackColor = Color.White;
                lb_DOUT_LIFO.BackColor = Color.White;
            }

            // Sel Imed
            if (Program.controller.GetSelectData() != 4)
            {
                lb_SEL_IMED.Text = "SEL_IMED = " + Program.selectData;
                lb_SEL_IMED_MUX.Text = "SEL_IMED = " + Program.selectData;
                lb_SEL_IMED.BackColor = Color.LightGreen;
                lb_SEL_IMED_MUX.BackColor = Color.LightGreen;
                if(Program.controller.GetSelectData() == 0)
                {
                    lb_SEL_IMED_00.BackColor = Color.LightGreen;
                    lb_SEL_IMED_01.BackColor = Color.White;
                    lb_SEL_IMED_10.BackColor = Color.White;
                    lb_SEL_IMED_11.BackColor = Color.White;
                }
                else if (Program.controller.GetSelectData() == 1)
                {
                    lb_SEL_IMED_00.BackColor = Color.White;
                    lb_SEL_IMED_01.BackColor = Color.LightGreen;
                    lb_SEL_IMED_10.BackColor = Color.White;
                    lb_SEL_IMED_11.BackColor = Color.White;
                }
                else if (Program.controller.GetSelectData() == 2)
                {
                    lb_SEL_IMED_00.BackColor = Color.White;
                    lb_SEL_IMED_01.BackColor = Color.White;
                    lb_SEL_IMED_10.BackColor = Color.LightGreen;
                    lb_SEL_IMED_11.BackColor = Color.White;
                }
                else if (Program.controller.GetSelectData() == 3)
                {
                    lb_SEL_IMED_00.BackColor = Color.White;
                    lb_SEL_IMED_01.BackColor = Color.White;
                    lb_SEL_IMED_10.BackColor = Color.White;
                    lb_SEL_IMED_11.BackColor = Color.LightGreen;
                }
                label1.BackColor = Color.LightGreen;
            }
            else
            {
                lb_SEL_IMED.Text = "SEL_IMED";
                lb_SEL_IMED_MUX.Text = "SEL_IMED";
                lb_SEL_IMED.BackColor = Color.White;
                lb_SEL_IMED_MUX.BackColor = Color.White;
                lb_SEL_IMED_00.BackColor = Color.White;
                lb_SEL_IMED_01.BackColor = Color.White;
                lb_SEL_IMED_10.BackColor = Color.White;
                lb_SEL_IMED_11.BackColor = Color.White;
                label1.BackColor = Color.White;
            }

            // Sel Jump
            if (Program.controller.GetEnableJmp()) lb_SEL_JM.BackColor = Color.LightGreen;
            else lb_SEL_JM.BackColor = Color.LightGreen;

            // Write Enable BCO
            if (Program.controller.GetWriteEnableBCO())
            {
                lb_WR_BCO.BackColor = Color.LightGreen;
                lb_WR_BCO_BCO.BackColor = Color.LightGreen;
            }
            else
            {
                lb_WR_BCO.BackColor = Color.White;
                lb_WR_BCO_BCO.BackColor = Color.White;
            }

            // Write Enable RAM
            if (Program.controller.GetWriteEnableRAM())
            {
                lb_WR_RAM.BackColor = Color.LightGreen;
                lb_WR_RAM_BCO.BackColor = Color.LightGreen;
                lb_WR_RAM_RAM.BackColor = Color.LightGreen;
            }
            else
            {
                lb_WR_RAM.BackColor = Color.White;
                lb_WR_RAM_BCO.BackColor = Color.White;
                lb_WR_RAM_RAM.BackColor = Color.White;
            }

            // Write Enable IO
            if (Program.controller.GetWriteEnableIO())
            {
                lb_WR_IO.BackColor = Color.LightGreen;
                lb_WR_IO_IO.BackColor = Color.LightGreen;
            }
            else
            {
                lb_WR_IO.BackColor = Color.White;
                lb_WR_IO_IO.BackColor = Color.White;
            }

            // Read IO
            if (Program.controller.GetEnableReadIO())
            {
                lb_RD_IO.BackColor = Color.LightGreen;
                lb_RD_IO_IO.BackColor = Color.LightGreen;
            }
            else
            {
                lb_RD_IO.BackColor = Color.White;
                lb_RD_IO_IO.BackColor = Color.White;
            }

            // Sel ULA
            if(Program.controller.GetSelectULA() != 10)
            {
                lb_SEL_ULA.Text = "SEL_ULA = " + Program.selectULA;
                lb_SEL_ULA_ULA.Text = "SEL_ULA = " + Program.selectULA;
                lb_SEL_ULA.BackColor = Color.LightGreen;
                lb_SEL_ULA_ULA.BackColor = Color.LightGreen;
                lb_w_Rs1.Text = "w_RS1 = " + Program.bco.GetRs1();
                lb_w_Rs2.Text = "w_RS2 = " + Program.bco.GetRs2();
                lb_w_Rs1.BackColor = Color.LightGreen;
                lb_w_Rs2.BackColor = Color.LightGreen;
            }
            else
            {
                lb_SEL_ULA.Text = "SEL_ULA";
                lb_SEL_ULA_ULA.Text = "SEL_ULA";
                lb_SEL_ULA.BackColor = Color.White;
                lb_SEL_ULA_ULA.BackColor = Color.White;
                lb_w_Rs1.Text = "w_RS1";
                lb_w_Rs2.Text = "w_RS2";
                lb_w_Rs1.BackColor = Color.White;
                lb_w_Rs2.BackColor = Color.White;
            }

            // Enable ROM
            if (Program.controller.GetEnableROM())
            {
                lb_EN_ROM.BackColor = Color.LightGreen;
                lb_EN_ROM_ROM.BackColor = Color.LightGreen;
            }
            else
            {
                lb_EN_ROM.BackColor = Color.White;
                lb_EN_ROM_ROM.BackColor = Color.White;
            }
        }

        // Verifica a palavra escrita no editor
        private void CheckKeyword(string word, Color color, int startIndex)
        {
            if (this.rchtxt_Editor.Text.Contains(word))
            {
                int index = -1;
                int selectStart = this.rchtxt_Editor.SelectionStart;

                while((index = this.rchtxt_Editor.Text.IndexOf(word, (index + 1))) != -1)
                {
                    this.rchtxt_Editor.Select((index + startIndex), word.Length);
                    this.rchtxt_Editor.SelectionColor = color;
                    this.rchtxt_Editor.Select(selectStart, 0);
                    this.rchtxt_Editor.SelectionColor = Color.Black;
                }
            }
        }

        // Verifica o tamanho (largura)
        private int GetWidth()
        {
            int w = 25;
            int line = rchtxt_Editor.Lines.Length;

            if(line <= 99)
            {
                w = 20 + (int)rchtxt_Editor.Font.Size;
            }
            else if(line <= 999)
            {
                w = 30 + (int)rchtxt_Editor.Font.Size;
            }
            else
            {
                w = 50 + (int)rchtxt_Editor.Font.Size;
            }

            return w;
        }

        // Adiciona o número de linhas
        private void AddLineNumbers()
        {
            // Cria e seta um ponto em 0,0
            Point pt = new Point(0, 0);
            int First_Index = rchtxt_Editor.GetCharIndexFromPosition(pt);
            int First_Line = rchtxt_Editor.GetLineFromCharIndex(First_Index);

            // Seta as coordenadas X e Y do ponto para a largura e a altura 
            pt.X = ClientRectangle.Width;
            pt.Y = ClientRectangle.Height;

            // Pega o último índice e a última linha do editor
            int Last_Index = rchtxt_Editor.GetCharIndexFromPosition(pt);
            int Last_Line = rchtxt_Editor.GetLineFromCharIndex(Last_Index);

            // Seta o alinhamento central do LineNumber
            LineNumber.SelectionAlignment = HorizontalAlignment.Center;

            // Seta o LineNumber para null e a largura
            LineNumber.Text = "";
            LineNumber.Width = GetWidth();

            // Adiciona cada número de linha
            for(int i = First_Line; i <= Last_Line + 2; i++)
            {
                LineNumber.Text += i + 1 + "\n";
            }
        }
        #endregion Methods

        public Form_uPD()
        {
            InitializeComponent();
            msg_inicial();
        }

        #region Execute
        // Passo a passo
        private void pp_Click(object sender, EventArgs e)
        {
            if (!compilation) return;
            if (endProgram) return;

            if(Program.currentAddress == 0)
            {
                dataGrid_Adress.Rows[Program.currentAddress].DefaultCellStyle.BackColor = Color.GreenYellow;
            }
            else
            {
                // Highlight linha
                dataGrid_Adress.Rows[Program.currentAddress].DefaultCellStyle.BackColor = Color.GreenYellow;
                dataGrid_Adress.Rows[previousAddr].DefaultCellStyle.BackColor = Color.White;
                previousAddr = Program.currentAddress;
                previousinstruction = Program.currentInstruction;
            }

            #region Interruptions
            // Ativa/Desativa Interrupções
            bt_INT0.Enabled = Program.enableInt[0];
            bt_INT1.Enabled = Program.enableInt[1];
            bt_INT2.Enabled = Program.enableInt[2];
            bt_INT3.Enabled = Program.enableInt[3];
            bt_INT4.Enabled = Program.enableInt[4];
            bt_INT5.Enabled = Program.enableInt[5];


            txt_fila_inter.Text = " Fila de interrupções: ";
            foreach (var intrr in Program.interruptionQueue.ToArray())
            {
                txt_fila_inter.Text += " | " + intrr.ToString();
            }
            #endregion Interruptions

            // Foca linha selecionada
            dataGrid_Adress.FirstDisplayedScrollingRowIndex = Program.currentAddress;

            // Atribuindo valor de endereço de entrada (Interface)
            Program.inAddr = Convert.ToInt32(txt_AddrIn.Text);
            Program.inValue = inValue;

            Program.StepByStep();                                       // Execução passo a passo

            // Componentes/Blocos do Processador
            ChangeComponentFlags();

            #region Erro
            if (Program.error != "")
            {
                var result = MessageBox.Show(Program.error + "\nA execução será encerrada.", "Erro!", MessageBoxButtons.OK);
                if (result == DialogResult.OK)
                {
                    EndExecution();
                    Program.error = "";
                    return;
                }
            }
            #endregion Erro

            if (valueHexa)
            {
                ShowHexaValue();
            }
            else
            {
                // Limpa todos os destaques;
                lb_REG0.BackColor = Color.White;
                lb_REG1.BackColor = Color.White;
                lb_REG2.BackColor = Color.White;
                lb_REG3.BackColor = Color.White;

                // Registradores
                lb_REG0.Text = Program.bco.GetRegister(0).ToString();
                lb_REG1.Text = Program.bco.GetRegister(1).ToString();
                lb_REG2.Text = Program.bco.GetRegister(2).ToString();
                lb_REG3.Text = Program.bco.GetRegister(3).ToString();
                txt_PC.Text = Program.currentAddress.ToString();

                // Destaca linha de registrador se ele mudar de valor
                if (Program.bco.GetRegister(0) != r0) lb_REG0.BackColor = Color.GreenYellow;
                if (Program.bco.GetRegister(1) != r1) lb_REG1.BackColor = Color.GreenYellow;
                if (Program.bco.GetRegister(2) != r2) lb_REG2.BackColor = Color.GreenYellow;
                if (Program.bco.GetRegister(3) != r3) lb_REG3.BackColor = Color.GreenYellow;

                // Atribui registradores temporários
                r0 = Program.bco.GetRegister(0);
                r1 = Program.bco.GetRegister(1);
                r2 = Program.bco.GetRegister(2);
                r3 = Program.bco.GetRegister(3);

                // Out
                txt_AddrOut.Text = Program.outAddr.ToString();
                txt_valueOut.Text = Program.outValue.ToString();

                // Muda tabela RAM
                ChangeRAMTable();
            }

            // Verifica se o programa chegou ao fim
            if (Program.currentAddress == (Program.translator.GetAmountInstMain() + Program.translator.GetAmountInstProg()) && previousinstruction.Substring(0, 5) != "01111" && previousinstruction.Substring(0, 5) != "00110" && previousinstruction.Substring(0, 5) != "01001" && previousinstruction.Substring(0, 5) != "01010")
            {
                #region Last Execute
                Program.StepByStep();                                       // Execução passo a passo

                // Componentes/Blocos do Processador
                ChangeComponentFlags();

                #region Erro
                if (Program.error != "")
                {
                    var result2 = MessageBox.Show(Program.error + "\nA execução será encerrada.", "Erro!", MessageBoxButtons.OK);
                    if (result2 == DialogResult.OK)
                    {
                        EndExecution();
                        Program.error = "";
                        return;
                    }
                }
                #endregion Erro

                if (valueHexa)
                {
                    ShowHexaValue();
                }
                else
                {
                    // Limpa todos os destaques;
                    lb_REG0.BackColor = Color.White;
                    lb_REG1.BackColor = Color.White;
                    lb_REG2.BackColor = Color.White;
                    lb_REG3.BackColor = Color.White;

                    // Registradores
                    lb_REG0.Text = Program.bco.GetRegister(0).ToString();
                    lb_REG1.Text = Program.bco.GetRegister(1).ToString();
                    lb_REG2.Text = Program.bco.GetRegister(2).ToString();
                    lb_REG3.Text = Program.bco.GetRegister(3).ToString();
                    txt_PC.Text = Program.currentAddress.ToString();

                    // Destaca linha de registrador se ele mudar de valor
                    if (Program.bco.GetRegister(0) != r0) lb_REG0.BackColor = Color.GreenYellow;
                    if (Program.bco.GetRegister(1) != r1) lb_REG1.BackColor = Color.GreenYellow;
                    if (Program.bco.GetRegister(2) != r2) lb_REG2.BackColor = Color.GreenYellow;
                    if (Program.bco.GetRegister(3) != r3) lb_REG3.BackColor = Color.GreenYellow;

                    // Atribui registradores temporários
                    r0 = Program.bco.GetRegister(0);
                    r1 = Program.bco.GetRegister(1);
                    r2 = Program.bco.GetRegister(2);
                    r3 = Program.bco.GetRegister(3);

                    //Out
                    txt_AddrOut.Text = Program.outAddr.ToString();
                    txt_valueOut.Text = Program.outValue.ToString();

                    // Muda tabela RAM
                    ChangeRAMTable();
                }
                #endregion Last Execute

                var result = MessageBox.Show("O programa chegou ao fim. Deseja reiniciar a execução?", "Fim da Execução", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    for (int i = 0; i < dataGrid_Adress.RowCount; i++)
                    {
                        dataGrid_Adress.Rows[i].DefaultCellStyle.BackColor = Color.White;
                    }

                    Program.StartSim(); Program.StartSim();

                    // Foca linha selecionada
                    dataGrid_Adress.FirstDisplayedScrollingRowIndex = Program.currentAddress;

                    // Registradores
                    lb_REG0.Text = Program.bco.GetRegister(0).ToString();
                    lb_REG1.Text = Program.bco.GetRegister(1).ToString();
                    lb_REG2.Text = Program.bco.GetRegister(2).ToString();
                    lb_REG3.Text = Program.bco.GetRegister(3).ToString();
                    txt_PC.Text = Program.currentAddress.ToString();

                    // Muda tabela RAM
                    ChangeRAMTable();
                }
                else
                {
                    EndExecution();
                }
            }
        }

        private void bt_voltar_comeco_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < dataGrid_Adress.RowCount; i++)
            {
                dataGrid_Adress.Rows[i].DefaultCellStyle.BackColor = Color.White;
            }

            Program.StartSim();

            // Foca linha selecionada
            dataGrid_Adress.FirstDisplayedScrollingRowIndex = Program.currentAddress;

            // Atribui valores dos registradores temporários
            r0 = r1 = r2 = r3 = 0;
            r0_hex = r1_hex = r2_hex = r3_hex = "0000";

            // Desativa botões de interrupção
            bt_INT0.Enabled = false;
            bt_INT1.Enabled = false;
            bt_INT2.Enabled = false;
            bt_INT3.Enabled = false;
            bt_INT4.Enabled = false;
            bt_INT5.Enabled = false;

            // Registradores
            lb_REG0.Text = Program.bco.GetRegister(0).ToString();
            lb_REG1.Text = Program.bco.GetRegister(1).ToString();
            lb_REG2.Text = Program.bco.GetRegister(2).ToString();
            lb_REG3.Text = Program.bco.GetRegister(3).ToString();
            txt_PC.Text = Program.currentAddress.ToString();

            // Muda tabela RAM
            ChangeRAMTable();
        }

        private void bt_execucao_Click(object sender, EventArgs e)
        {
            if (!playProgram) playProgram = true;
            Execute();
        }
        #endregion Execute

        private void checkBox_valores_hexa_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_hexValue.Checked)
            {
                valueHexa = true;
                ChangeHexa();
            }
            else
            {
                valueHexa = false;
                ChangeHexa();
            }
        }

        #region Compile
        private void bt_compilar_Click(object sender, EventArgs e)
        {

            if (txt_editar.Text == "")
            {
                txt_Mensagens.Text += Environment.NewLine + "Não há código para ser compilado!\n";
                return;
            }

            // Se há um código não salvo no editor
            if (txt_editar.Text != "" && txt_editar.Text != instructionLine)
            {
                SF.Title = "Novo Arquivo";
                SF.Filter = "Arquivo Assembly (*.asm)|*.asm| Arquivo texto (*.txt)|*.txt";
                SF.ShowDialog();

                if (string.IsNullOrEmpty(SF.FileName) == false)
                {
                    try
                    {
                        using (SW = new StreamWriter(SF.FileName, false, Encoding.UTF8))
                        {
                            //Escrevendo no arquivo
                            instructionLine = txt_editar.Text;
                            SW.Write(txt_editar.Text);
                            SW.Flush();
                            string newtab = "tab_" + fileName;

                            // Após salvar tenta compilar
                            try
                            {
                                // Salvando no Programa
                                Program.Translate(txt_editar.Text, Path.GetFileName(SF.FileName));
                                if(Program.error != "")
                                {
                                    MessageBox.Show(Program.error, "Erro", MessageBoxButtons.OK);
                                    return;
                                }
                                // Inicializa o Programa
                                Program.StartSim();

                                #region Start ROM
                        int y = -1, cont = 0, continst = 0;
                        string[] functions = Program.translator.GetFunctions();
                        int[] instFunction = Program.translator.GetAmountInstFunction();

                        dataGrid_Adress.ColumnCount = 4;
                        dataGrid_Adress.ColumnHeadersVisible = true;
                        DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
                        dataGrid_Adress.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

                        dataGrid_Adress.Columns[0].Name = "Área";
                        dataGrid_Adress.Columns[1].Name = "Endereço";
                        dataGrid_Adress.Columns[2].Name = "Binário";
                        dataGrid_Adress.Columns[3].Name = "Assembly";

                        #region Start ROM Vector
                        for (int x = 0; x < 512; x++)
                        {
                            y++;
                            if (x < Program.translator.GetAmountInstProg() + Program.translator.GetAmountInstMain())
                                dataGrid_Adress.Rows.Add("Área de Programa", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            else if (cont < Program.translator.GetAmountFunctions())
                            {
                                if (x == Program.translator.GetAmountInstMain()) y = 0;
                                dataGrid_Adress.Rows.Add("Function " + functions[cont], Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                                if (continst < instFunction[cont] - 1) continst++;
                                else
                                {
                                    continst = 0;
                                    cont++;
                                    y = -1;
                                }
                            }
                            else dataGrid_Adress.Rows.Add("Área de Programa", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                        }
                        y = 0;
                        for (int x = 512; x < 768; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT0", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 768; x < 1024; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT1", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1024; x < 1280; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT2", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1280; x < 1536; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT3", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1536; x < 1791; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT4", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1791; x < 2047; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT5", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        #endregion Start ROM Vector

                        #endregion Start ROM

                                #region Start RAM
                        dataGrid_RAM.ColumnCount = 10;
                        dataGrid_RAM.ColumnHeadersVisible = true;

                        columnHeaderStyle = new DataGridViewCellStyle();
                        columnHeaderStyle.BackColor = Color.Beige;
                        columnHeaderStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        dataGrid_RAM.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

                        dataGrid_RAM.Columns[0].Name = "Área";
                        dataGrid_RAM.Columns[1].Name = "Endereço";
                        dataGrid_RAM.Columns[2].Name = "+ 00";
                        dataGrid_RAM.Columns[3].Name = "+ 01";
                        dataGrid_RAM.Columns[4].Name = "+ 02";
                        dataGrid_RAM.Columns[5].Name = "+ 03";
                        dataGrid_RAM.Columns[6].Name = "+ 04";
                        dataGrid_RAM.Columns[7].Name = "+ 05";
                        dataGrid_RAM.Columns[8].Name = "+ 06";
                        dataGrid_RAM.Columns[9].Name = "+ 07";

                        y = 0;
                        int[] memoria = Program.ram.GetMemory();

                        #region Start RAM Vector
                        for (int x = 0; x < 512; x += 8)
                        {
                            dataGrid_RAM.Rows.Add("Área de Programa", Convert.ToString(y), memoria[x], memoria[x + 1], memoria[x + 2], memoria[x + 3], memoria[x + 4], memoria[x + 5], memoria[x + 6], memoria[x + 7]);
                            y += 8;
                        }

                        #endregion Start RAM Vector
                        #endregion Start RAM

                                compilation = true;
                                endProgram = false;

                                // Inicia registradores com 0
                                r0 = r1 = r2 = r3 = 0;
                                r0_hex = r1_hex = r2_hex = r3_hex = "0000";

                                // Desativando edição no texto
                                rchtxt_Editor.Enabled = false;

                                // Ativando botões de execução
                                bt_execucao.Enabled = true;
                                bt_passo_passo_prosseguir.Enabled = true;
                                bt_stop.Enabled = true;
                                bt_pause.Enabled = true;
                                bt_voltar_comeco.Enabled = true;
                                bt_compilar.Enabled = false;
                                bt_abrir_arquivo.Enabled = false;
                                bt_New_File.Enabled = false;
                                btn_LoadVHDLFile.Enabled = true;

                                tab_Editar.BackColor = Color.GreenYellow;
                                tab_Executar_Texto.SelectedTab = tabExecutar;
                            }
                            catch
                            {
                                txt_Mensagens.Text += Environment.NewLine + "Erro ao compilar código!\n";

                            }

                        }
                    }
                    catch { MessageBox.Show("Erro ao criar Arquivo"); }
                }
            }
            else
            {
                try
                {

                    instructionLine = txt_editar.Text;
                    //Escrevendo no arquivo
                    fileName = Path.GetFileName(SF.FileName);
                    string newtab = "tab_" + fileName;

                    // Após salvar tenta compilar
                    try
                    {
                        // Salvando no Programa
                        Program.Translate(txt_editar.Text, fileName);
                        if (Program.error != "")
                        {
                            MessageBox.Show(Program.error, "Erro", MessageBoxButtons.OK);
                            return;
                        }
                        // Inicializa o Programa
                        Program.StartSim();

                        #region Start ROM
                        int y = -1, cont = 0, continst = 0;
                        string[] functions = Program.translator.GetFunctions();
                        int[] instFunction = Program.translator.GetAmountInstFunction();

                        dataGrid_Adress.ColumnCount = 4;
                        dataGrid_Adress.ColumnHeadersVisible = true;
                        DataGridViewCellStyle columnHeaderStyle = new DataGridViewCellStyle();
                        dataGrid_Adress.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

                        dataGrid_Adress.Columns[0].Name = "Área";
                        dataGrid_Adress.Columns[1].Name = "Endereço";
                        dataGrid_Adress.Columns[2].Name = "Binário";
                        dataGrid_Adress.Columns[3].Name = "Assembly";

                        #region Start ROM Vector
                        for (int x = 0; x < 512; x++)
                        {
                            y++;
                            if (x < Program.translator.GetAmountInstProg() + Program.translator.GetAmountInstMain())
                                dataGrid_Adress.Rows.Add("Área de Programa", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            else if (cont < Program.translator.GetAmountFunctions())
                            {
                                if (x == Program.translator.GetAmountInstMain()) y = 0;
                                dataGrid_Adress.Rows.Add("Function " + functions[cont], Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                                if (continst < instFunction[cont] - 1) continst++;
                                else
                                {
                                    continst = 0;
                                    cont++;
                                    y = -1;
                                }
                            }
                            else dataGrid_Adress.Rows.Add("Área de Programa", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                        }
                        y = 0;
                        for (int x = 512; x < 768; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT0", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 768; x < 1024; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT1", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1024; x < 1280; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT2", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1280; x < 1536; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT3", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1536; x < 1791; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT4", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        for (int x = 1791; x < 2047; x++)
                        {
                            dataGrid_Adress.Rows.Add("INT5", Convert.ToString(y), Program.binProgram[x], Program.assemblyProgram[x]);
                            y++;
                        }
                        y = 0;
                        #endregion Start ROM Vector

                        #endregion Start ROM

                        #region Start RAM
                        dataGrid_RAM.ColumnCount = 10;
                        dataGrid_RAM.ColumnHeadersVisible = true;

                        columnHeaderStyle = new DataGridViewCellStyle();
                        columnHeaderStyle.BackColor = Color.Beige;
                        columnHeaderStyle.Font = new Font("Microsoft Sans Serif", 8, FontStyle.Bold);
                        dataGrid_RAM.ColumnHeadersDefaultCellStyle = columnHeaderStyle;

                        dataGrid_RAM.Columns[0].Name = "Área";
                        dataGrid_RAM.Columns[1].Name = "Endereço";
                        dataGrid_RAM.Columns[2].Name = "+ 00";
                        dataGrid_RAM.Columns[3].Name = "+ 01";
                        dataGrid_RAM.Columns[4].Name = "+ 02";
                        dataGrid_RAM.Columns[5].Name = "+ 03";
                        dataGrid_RAM.Columns[6].Name = "+ 04";
                        dataGrid_RAM.Columns[7].Name = "+ 05";
                        dataGrid_RAM.Columns[8].Name = "+ 06";
                        dataGrid_RAM.Columns[9].Name = "+ 07";

                        y = 0;
                        int[] memoria = Program.ram.GetMemory();

                        #region Start RAM Vector
                        for (int x = 0; x < 512; x += 8)
                        {
                            dataGrid_RAM.Rows.Add("Área de Programa", Convert.ToString(y), memoria[x], memoria[x + 1], memoria[x + 2], memoria[x + 3], memoria[x + 4], memoria[x + 5], memoria[x + 6], memoria[x + 7]);
                            y += 8;
                        }

                        #endregion Start RAM Vector
                        #endregion Start RAM

                        compilation = true;
                        endProgram = false;

                        // Inicia registradores com 0
                        r0 = r1 = r2 = r3 = 0;
                        r0_hex = r1_hex = r2_hex = r3_hex = "0000";

                        // Desativando edição no texto
                        rchtxt_Editor.Enabled = false;

                        // Ativando botões de execução
                        bt_execucao.Enabled = true;
                        bt_passo_passo_prosseguir.Enabled = true;
                        bt_stop.Enabled = true;
                        bt_pause.Enabled = true;
                        bt_voltar_comeco.Enabled = true;
                        bt_compilar.Enabled = false;
                        bt_abrir_arquivo.Enabled = false;
                        bt_New_File.Enabled = false;
                        btn_LoadVHDLFile.Enabled = true;

                        tab_Editar.BackColor = Color.GreenYellow;
                        tab_Executar_Texto.SelectedTab = tabExecutar;
                    }
                    catch
                    {
                        txt_Mensagens.Text += Environment.NewLine + "Erro ao compilar código!\n";

                    }
                }
                catch { MessageBox.Show("Erro ao criar Arquivo"); }

                
            }

            #region Color Text
            this.CheckKeyword("ADD", Color.Blue, 0);
            this.CheckKeyword("SUB", Color.Blue, 0);
            this.CheckKeyword("STO", Color.Blue, 0);
            this.CheckKeyword("LD", Color.Blue, 0);
            this.CheckKeyword("IN", Color.Blue, 0);
            this.CheckKeyword("OR", Color.Blue, 0);
            this.CheckKeyword("XOR", Color.Blue, 0);
            this.CheckKeyword("NOT", Color.Blue, 0);
            this.CheckKeyword("CMP", Color.Blue, 0);
            this.CheckKeyword("JI", Color.Blue, 0);
            this.CheckKeyword("JE", Color.Blue, 0);
            this.CheckKeyword("JZ", Color.Blue, 0);
            this.CheckKeyword("LDI", Color.Blue, 0);
            this.CheckKeyword("NOP", Color.Blue, 0);
            this.CheckKeyword("OUT", Color.Blue, 0);
            this.CheckKeyword("AND", Color.Blue, 0);
            this.CheckKeyword("SETR", Color.Blue, 0);

            this.CheckKeyword("CALL", Color.BlueViolet, 0);
            this.CheckKeyword("RET", Color.BlueViolet, 0);

            this.CheckKeyword("BEGIN", Color.Green, 0);
            this.CheckKeyword("END", Color.Green, 0);

            this.CheckKeyword(".INT0", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT1", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT2", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT3", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT4", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT5", Color.MediumVioletRed, 0);
            this.CheckKeyword("RETI", Color.MediumVioletRed, 0);
            this.CheckKeyword("INT", Color.PaleVioletRed, 0);
            this.CheckKeyword("SETR", Color.Blue, 0);
            #endregion Color Text


        }
        #endregion Compile

        #region New File
        private void bt_New_File_Click(object sender, EventArgs e)
        {
            if (txt_editar.Text != "" && txt_editar.Text != instructionLine)
            {
                SF.Title = "Novo Arquivo";
                SF.Filter = "Arquivo Assembly (*.asm)|*.asm| Arquivo texto (*.txt)|*.txt";
                SF.ShowDialog();

                if (string.IsNullOrEmpty(SF.FileName) == false)
                {
                    try
                    {
                        using (SW = new StreamWriter(SF.FileName, false, Encoding.UTF8))
                        {
                            SW.Write(txt_editar.Text);
                            SW.Flush();
                            instructionLine = null;
                            fileName = null;
                            path = null;
                            Program.fileName = null;
                            Program.binProgram = null;
                            Program.assemblyProgram = null;
                            tab_Editar.Text = "Editar";
                        }
                    }

                    catch { MessageBox.Show("Erro ao criar Arquivo"); }
                }
            }
            else
            {
                instructionLine = null;
                fileName = null;
                path = null;
                Program.fileName = null;
                Program.binProgram = null;
                Program.assemblyProgram = null;
                tab_Editar.Text = "Editar";
            }
            txt_editar.Clear();
            rchtxt_Editor.Clear();

            OF.Dispose();
            SF.Dispose();
        }
        #endregion New File

        #region MenuStrip
        #region Save File
        private void bt_salvar_Click(object sender, EventArgs e)
        {

        }
        #endregion Save File

        #region Open File
        private void bt_abrir_arquivo_Click(object sender, EventArgs e)
        {
            OF.Filter = "Arquivo Assembly (*.asm)|*.asm| Arquivo texto (*.txt)|*.txt";
            OF.ShowDialog();

            if (string.IsNullOrEmpty(OF.FileName) == false)
            {
                try
                {
                    using (SR = new StreamReader(OF.FileName, Encoding.GetEncoding(CultureInfo.GetCultureInfo("pt-BR").TextInfo.ANSICodePage)))
                    {

                        txt_editar.Text = SR.ReadToEnd().Trim();
                        rchtxt_Editor.Text = txt_editar.Text;
                        instructionLine = txt_editar.Text;
                        path = OF.FileName;
                        fileName = Path.GetFileName(OF.FileName);
                        Program.fileName = fileName;

                    }
                    tab_Editar.Text = fileName;

                    #region Color Text
                    this.CheckKeyword("ADD", Color.Blue, 0);
                    this.CheckKeyword("SUB", Color.Blue, 0);
                    this.CheckKeyword("STO", Color.Blue, 0);
                    this.CheckKeyword("LD", Color.Blue, 0);
                    this.CheckKeyword("IN", Color.Blue, 0);
                    this.CheckKeyword("OR", Color.Blue, 0);
                    this.CheckKeyword("XOR", Color.Blue, 0);
                    this.CheckKeyword("NOT", Color.Blue, 0);
                    this.CheckKeyword("CMP", Color.Blue, 0);
                    this.CheckKeyword("JI", Color.Blue, 0);
                    this.CheckKeyword("JE", Color.Blue, 0);
                    this.CheckKeyword("JZ", Color.Blue, 0);
                    this.CheckKeyword("LDI", Color.Blue, 0);
                    this.CheckKeyword("NOP", Color.Blue, 0);
                    this.CheckKeyword("OUT", Color.Blue, 0);
                    this.CheckKeyword("AND", Color.Blue, 0);
                    this.CheckKeyword("SETR", Color.Blue, 0);

                    this.CheckKeyword("CALL", Color.BlueViolet, 0);
                    this.CheckKeyword("RET", Color.BlueViolet, 0);

                    this.CheckKeyword("BEGIN", Color.Green, 0);
                    this.CheckKeyword("END", Color.Green, 0);

                    this.CheckKeyword(".INT0", Color.MediumVioletRed, 0);
                    this.CheckKeyword(".INT1", Color.MediumVioletRed, 0);
                    this.CheckKeyword(".INT2", Color.MediumVioletRed, 0);
                    this.CheckKeyword(".INT3", Color.MediumVioletRed, 0);
                    this.CheckKeyword(".INT4", Color.MediumVioletRed, 0);
                    this.CheckKeyword(".INT5", Color.MediumVioletRed, 0);
                    this.CheckKeyword("RETI", Color.MediumVioletRed, 0);
                    this.CheckKeyword("INT", Color.PaleVioletRed, 0);
                    this.CheckKeyword("SETR", Color.Blue, 0);
                    #endregion Color Text
                }
                catch (Exception)
                {
                    MessageBox.Show("Nenhum arquivo foi copiado!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
        #endregion Open File

        #region Exit
        private void Menu_Strip_Sair_Click(object sender, EventArgs e)
        {
           this.Close();
        }
        #endregion Exit

        #region Save as
        private void Menu_Strip_Salvar_como_Click(object sender, EventArgs e)
        {
            // Se o arquivo já estiver aberto
            if(path != null)
            {
                try
                {
                    using (SW = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        StringBuilder sb = new StringBuilder();

                        foreach (string line in rchtxt_Editor.Lines)
                            sb.AppendLine(line);

                        Clipboard.SetText(sb.ToString());
                        txt_editar.Text = Clipboard.GetText();
                        Clipboard.Clear();

                        instructionLine = txt_editar.Text;
                        Program.fileName = fileName;
                        SW.Write(txt_editar.Text);
                        SW.Flush();
                    }
                }
                catch (Exception)
                {
                    //Aqui a ordem será (conteudo mensagem, titulo mensagem, ação na mensagem, ícone).
                    MessageBox.Show("O arquivo não pode ser salvo!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw;
                }
            }
            else
            {
                string palavra_Inicial = txt_editar.Text;
                SF.Filter = "Arquivo Assembly (*.asm)|*.asm| Arquivo texto (*.txt)|*.txt";
                SF.ShowDialog();

                if (string.IsNullOrEmpty(SF.FileName) == false)
                {
                    try
                    {
                        using (SW = new StreamWriter(SF.FileName, false, Encoding.UTF8))
                        {
                            path = SF.FileName;
                            fileName = Path.GetFileName(SF.FileName);

                            StringBuilder sb = new StringBuilder();

                            foreach (string line in rchtxt_Editor.Lines)
                                sb.AppendLine(line);

                            Clipboard.SetText(sb.ToString());
                            txt_editar.Text = Clipboard.GetText();

                            instructionLine = txt_editar.Text;
                            Program.fileName = fileName;
                            SW.Write(txt_editar.Text);
                            SW.Flush();

                        }
                    }

                    catch (Exception)
                    {
                        //Aqui a ordem será (conteudo mensagem, titulo mensagem, ação na mensagem, ícone).
                        MessageBox.Show("O arquivo não pode ser salvo!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        throw;
                    }
                }
            }

            #region Color Text
            this.CheckKeyword("ADD", Color.Blue, 0);
            this.CheckKeyword("SUB", Color.Blue, 0);
            this.CheckKeyword("STO", Color.Blue, 0);
            this.CheckKeyword("LD", Color.Blue, 0);
            this.CheckKeyword("IN", Color.Blue, 0);
            this.CheckKeyword("OR", Color.Blue, 0);
            this.CheckKeyword("XOR", Color.Blue, 0);
            this.CheckKeyword("NOT", Color.Blue, 0);
            this.CheckKeyword("CMP", Color.Blue, 0);
            this.CheckKeyword("JI", Color.Blue, 0);
            this.CheckKeyword("JE", Color.Blue, 0);
            this.CheckKeyword("JZ", Color.Blue, 0);
            this.CheckKeyword("LDI", Color.Blue, 0);
            this.CheckKeyword("NOP", Color.Blue, 0);
            this.CheckKeyword("OUT", Color.Blue, 0);
            this.CheckKeyword("AND", Color.Blue, 0);
            this.CheckKeyword("SETR", Color.Blue, 0);

            this.CheckKeyword("CALL", Color.BlueViolet, 0);
            this.CheckKeyword("RET", Color.BlueViolet, 0);

            this.CheckKeyword("BEGIN", Color.Green, 0);
            this.CheckKeyword("END", Color.Green, 0);

            this.CheckKeyword(".INT0", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT1", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT2", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT3", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT4", Color.MediumVioletRed, 0);
            this.CheckKeyword(".INT5", Color.MediumVioletRed, 0);
            this.CheckKeyword("RETI", Color.MediumVioletRed, 0);
            this.CheckKeyword("INT", Color.PaleVioletRed, 0);
            this.CheckKeyword("SETR", Color.Blue, 0);
            #endregion Color Text
        }
        #endregion Save as

        #endregion

        private void bt_Limpar_Mensagens_Click(object sender, EventArgs e)
        {
            txt_Mensagens.Clear();
            txt_Msg_Exec.Clear();
        }

        public void msg_inicial()
        {
            txt_editar.ForeColor = Color.Gray;
            txt_editar.Text = "Insira seu código Assembly aqui." + Environment.NewLine + Environment.NewLine +
                             "Para exemplos, vá no Menu Arquivo -> Exemplos" + Environment.NewLine;

            rchtxt_Editor.ForeColor = Color.Gray;
            rchtxt_Editor.Text = txt_editar.Text;
                            
        }

        #region MenuStrip_exemplos
        private void operaçõesAritméticasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_editar.Name = "operacoes_ULA";
            txt_editar.Text = "BEGIN" + Environment.NewLine +
                              "LDI, R1, 20;" + Environment.NewLine +
            "LDI, R0, 20;" + Environment.NewLine +
            "CMP, R3, R1, R0;" + Environment.NewLine +
            "ADD, R2, R1, R0;" + Environment.NewLine +
            "AND, R3, R1, R0;" + Environment.NewLine +
            "LDI, R2, 10;" + Environment.NewLine +
            "LDI, R3, 11;" + Environment.NewLine +
            "XOR, R0, R2, R3;" + Environment.NewLine +
            "OR, R3, R2, R1;" + Environment.NewLine +
            "SUB, R3, R0, R2;" + Environment.NewLine +
            "NOT, R1, R0;" + Environment.NewLine +
            "END";
            rchtxt_Editor.Text = txt_editar.Text;
        }

        private void carregamentoEArmazenamentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_editar.Name = "manipulacao_RAM";
            txt_editar.Text = "BEGIN" + Environment.NewLine +
            "LDI, R1, 20;" + Environment.NewLine +
            "STO, R1, 01;" + Environment.NewLine +
            "LDI, R0, 20;" + Environment.NewLine +
            "ADD, R2, R1, R0;" + Environment.NewLine +
            "STO, R2, 02;" + Environment.NewLine +
            "LD, R3, 01;" + Environment.NewLine +
            "STO, R3, 03;" + Environment.NewLine +
            "LDI, R2, 10;" + Environment.NewLine +
            "END";
            rchtxt_Editor.Text = txt_editar.Text;
        }


        private void chamadasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_editar.Name = "call_ret";
            txt_editar.Text =
            "BEGIN" + Environment.NewLine +
            "LDI, R1, 15;" + Environment.NewLine +
            "STO, R1, 0;" + Environment.NewLine +
            "CALL, CHAMADA;" + Environment.NewLine +
            "LDI, R2, 68;" + Environment.NewLine +
            "STO, R3, 255;" + Environment.NewLine +
            "CALL, FACTI;" + Environment.NewLine +
            "END" + Environment.NewLine +
            Environment.NewLine +
            "CHAMADA" + Environment.NewLine +
            "LDI, R3, 120;" + Environment.NewLine +
            "LDI, R0, 11;" + Environment.NewLine +
            "ADD, R3, R1, R2;" + Environment.NewLine +
            "RET;" + Environment.NewLine +
            Environment.NewLine +
            "FACTI" + Environment.NewLine +
            "LDI, R0, 1;" + Environment.NewLine +
            "LDI, R1, 2;" + Environment.NewLine +
            "AND, R2, R0, R1;" + Environment.NewLine +
            "RET;";
            rchtxt_Editor.Text = txt_editar.Text;
        }

        private void jIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_editar.Name = "salto_JI";
            txt_editar.Text = "BEGIN" + Environment.NewLine +
            "LDI, R0, 10;" + Environment.NewLine +
            "LDI, R1, 20;" + Environment.NewLine +
            "LDI, R2, 30;" + Environment.NewLine +
            "JI, 7;" + Environment.NewLine +
            "STO, R3, 02;" + Environment.NewLine +
            "LDI, R0, 10;" + Environment.NewLine +
            "LDI, R3, 40;" + Environment.NewLine +
            "LDI, R3, 50;" + Environment.NewLine +
            "STO, R3, 02;" + Environment.NewLine +
            "JI, 1;" + Environment.NewLine +
            "END";
            rchtxt_Editor.Text = txt_editar.Text;
        }

        private void saltoCondicionalJEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_editar.Name = "salto_JE";
            txt_editar.Text = "BEGIN" + Environment.NewLine +
            "LDI, R0, 33;" + Environment.NewLine +
            "LDI, R1, 1;" + Environment.NewLine +
            "JE, R0, 1;" + Environment.NewLine +
            "CMP, R3, R1, R0;" + Environment.NewLine +
            "JE, R1, 6;" + Environment.NewLine +
            "ADD, R2, R1, R0;" + Environment.NewLine +
            "LDI, R2, 250;" + Environment.NewLine +
            "LDI, R3, 1;" + Environment.NewLine +
            "CMP, R3, R3, R2;" + Environment.NewLine +
            "JE, R3, 1;" + Environment.NewLine +
            "END";
            rchtxt_Editor.Text = txt_editar.Text;
        }


        private void saltoCondicionalJZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_editar.Name = "salto_JZ";
            txt_editar.Text = "BEGIN" + Environment.NewLine +
            "LDI, R2, 10;" + Environment.NewLine +
            "LDI, R3, 20;" + Environment.NewLine +
            "AND, R0, R2, R3;" + Environment.NewLine +
            "JZ, R3, 8;" + Environment.NewLine +
            "STO, R0, 0;" + Environment.NewLine +
            "STO, R1, 255;" + Environment.NewLine +
            "STO, R2, 511;" + Environment.NewLine +
            "STO, R3, 02;" + Environment.NewLine +
            "SUB, R1, R3, R2;" + Environment.NewLine +
            "SUB, R1, R1, R2;" + Environment.NewLine +
            "JZ, R1, 01;" + Environment.NewLine +
            "END";
            rchtxt_Editor.Text = txt_editar.Text;
        }

        private void interrupçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txt_editar.Name = "interrupcoes";
            txt_editar.Text = ".PROG" + Environment.NewLine +
            "SETR, INT, 0" + Environment.NewLine +
            "SETR, INT, 1" + Environment.NewLine +
            "SETR, INT, 2" + Environment.NewLine +
            "SETR, INT, 3" + Environment.NewLine +
            "SETR, INT, 4" + Environment.NewLine +
            Environment.NewLine +
            "BEGIN" + Environment.NewLine +
            "LDI, R1, 20;" + Environment.NewLine +
            "STO, R1, 01;" + Environment.NewLine +
            "LDI, R0, 70;" + Environment.NewLine +
            "ADD, R2, R1, R0;" + Environment.NewLine +
            "AND, R3, R1, R0;" + Environment.NewLine +
            "STO, R2, 02;" + Environment.NewLine +
            "STO, R3, 03;" + Environment.NewLine +
            "LDI, R2, 10;" + Environment.NewLine +
            "LDI, R3, 11;" + Environment.NewLine +
            "XOR, R0, R2, R3;" + Environment.NewLine +
            "LD, R1, 02;" + Environment.NewLine +
            "OR, R3, R2, R1;" + Environment.NewLine +
            "END" + Environment.NewLine +
             Environment.NewLine +
            ".INT0" + Environment.NewLine +
            "IN, R0, 0;" + Environment.NewLine +
            "LDI, R1, 7;" + Environment.NewLine +
            "XOR, R3, R0, R1;" + Environment.NewLine +
            "STO, R0, 0;" + Environment.NewLine +
            "STO, R1, 01;" + Environment.NewLine +
            "STO, R3, 02;" + Environment.NewLine +
            "RETI" + Environment.NewLine +
            Environment.NewLine +
            ".INT1" + Environment.NewLine +
            "IN, R0, 1;" + Environment.NewLine +
            "LDI, R3, 89;" + Environment.NewLine +
            "ADD, R2, R1, R0;" + Environment.NewLine +
            "STO, R2, 255;" + Environment.NewLine +
            "RETI" + Environment.NewLine +
            Environment.NewLine +
            ".INT2" + Environment.NewLine +
            "IN, R3, 5;" + Environment.NewLine +
            "LDI, R1, 2;" + Environment.NewLine +
            "SUB, R2, R3, R1;" + Environment.NewLine +
            "STO, R1, 255;" + Environment.NewLine +
            "STO, R2, 510;" + Environment.NewLine +
            "STO, R3, 256;" + Environment.NewLine +
            "OUT, R2, 2;" + Environment.NewLine +
            "RETI" + Environment.NewLine +
            Environment.NewLine +
            ".INT3" + Environment.NewLine +
            "LDI, R1, 23;" + Environment.NewLine +
            "OUT, R1, 5;" + Environment.NewLine +
            "RETI" + Environment.NewLine +
            Environment.NewLine +
            ".INT4" + Environment.NewLine +
            "IN, R0, 5;" + Environment.NewLine +
            "IN, R1, 4;" + Environment.NewLine +
            "IN, R2, 1;" + Environment.NewLine +
            "IN, R3, 3;" + Environment.NewLine +
            "LDI, R2, 2;" + Environment.NewLine +
            "ADD, R3, R1, R2;" + Environment.NewLine +
            "STO, R3, 255;" + Environment.NewLine +
            "OUT, R3, 0;" + Environment.NewLine +
            "RETI";
            rchtxt_Editor.Text = txt_editar.Text;
        }


        #endregion

        #region color editor 
        private void txt_editar_TextChanged(object sender, EventArgs e)
        {

        }

        #endregion color editor

        private void bt_stop_Click(object sender, EventArgs e)
        {
            EndExecution();
        }

        private void bt_pause_Click(object sender, EventArgs e)
        {
            playProgram = false;
        }

        private void cb_hexAddr_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_hexAddr.Checked)
            {
                // ROM
                for (int i = 1; i < 2047; i++)
                {
                    dataGrid_Adress[1, i].Value = Hexadecimal(Convert.ToInt32(dataGrid_Adress[1, i].Value));
                }

                // RAM
                for (int i = 0; i < 512; i += 8)
                {
                    dataGrid_RAM[1, i / 8].Value = Hexadecimal(i);
                }

                // IN
                txt_AddrIn.Text = Hexadecimal(inAddr);
                tb_AddrInt0.Text = Hexadecimal(Program.addrInterruption[0]);
                tb_AddrInt1.Text = Hexadecimal(Program.addrInterruption[1]);
                tb_AddrInt2.Text = Hexadecimal(Program.addrInterruption[2]);
                tb_AddrInt3.Text = Hexadecimal(Program.addrInterruption[3]);
                tb_AddrInt4.Text = Hexadecimal(Program.addrInterruption[4]);
                tb_AddrInt5.Text = Hexadecimal(Program.addrInterruption[5]);

                // OUT
                txt_AddrOut.Text = Hexadecimal(outAddr);
            }
            else
            {
                // IN
                txt_AddrIn.Text = inAddr.ToString();

                // OUT
                txt_AddrOut.Text = outAddr.ToString();

                #region ROM
                int cont = 0, continst = 0, y = -1;
                int[] instFunction = Program.translator.GetAmountInstFunction();

                // ROM
                for (int x = 0; x < 512; x++)
                {
                    y++;
                    if (x < Program.translator.GetAmountInstMain())
                    {
                        dataGrid_Adress[1, x].Value = y;
                    }
                    else if (cont < Program.translator.GetAmountFunctions())
                    {
                        if (x == Program.translator.GetAmountInstMain())
                        {
                            y = 0;
                        }
                        dataGrid_Adress[1, x].Value = y;
                        if (continst < instFunction[cont] - 1) continst++;
                        else
                        {
                            continst = 0;
                            cont++;
                            y = -1;
                        }
                    }
                    else dataGrid_Adress[1, x].Value = y;
                }
                y = 0;
                for (int x = 512; x < 768; x++)
                {
                    dataGrid_Adress[1, x].Value = y;
                    y++;
                }
                y = 0;
                for (int x = 768; x < 1024; x++)
                {
                    dataGrid_Adress[1, x].Value = y;
                    y++;
                }
                y = 0;
                for (int x = 1024; x < 1280; x++)
                {
                    dataGrid_Adress[1, x].Value = y;
                    y++;
                }
                y = 0;
                for (int x = 1280; x < 1536; x++)
                {
                    dataGrid_Adress[1, x].Value = y;
                    y++;
                }
                y = 0;
                for (int x = 1536; x < 1791; x++)
                {
                    dataGrid_Adress[1, x].Value = y;
                    y++;
                }
                y = 0;
                for (int x = 1791; x < 2047; x++)
                {
                    dataGrid_Adress[1, x].Value = y;
                    y++;
                }
                y = 0;

                // RAM
                for (int i = 1; i < 512; i += 8)
                {
                    dataGrid_RAM[1, i / 8].Value = i - 1;
                }
                #endregion ROM
            }
        }

        private void txt_AddrIn_TextChanged(object sender, EventArgs e)
        {

        }

        private void txt_valueIn_TextChanged(object sender, EventArgs e)
        {

        }

        #region Interruption's Buttom
        private void bt_INT0_Click(object sender, EventArgs e)
        {
            if(Program.addrInterruption[0] < 513)
                Program.StackInterruption(0);
            else
            {
                MessageBox.Show("Endereço de entrada interrupção não alocado! A execução será encerrada!", "Erro!", MessageBoxButtons.OK);
                EndExecution();
            }
        }

        private void bt_INT1_Click(object sender, EventArgs e)
        {
            if (Program.addrInterruption[1] < 513)
                Program.StackInterruption(1);
            else
            {
                MessageBox.Show("Endereço de entrada interrupção não alocado! A execução será encerrada!", "Erro!", MessageBoxButtons.OK);
                EndExecution();
            }
        }

        private void bt_INT2_Click(object sender, EventArgs e)
        {
            if (Program.addrInterruption[2] < 513)
                Program.StackInterruption(2);
            else
            {
                MessageBox.Show("Endereço de entrada interrupção não alocado! A execução será encerrada!", "Erro!", MessageBoxButtons.OK);
                EndExecution();
            }
        }

        private void bt_INT3_Click(object sender, EventArgs e)
        {
            if (Program.addrInterruption[3] < 513)
                Program.StackInterruption(3);
            else
            {
                MessageBox.Show("Endereço de entrada interrupção não alocado! A execução será encerrada!", "Erro!", MessageBoxButtons.OK);
                EndExecution();
            }
        }

        private void bt_INT4_Click(object sender, EventArgs e)
        {
            if (Program.addrInterruption[4] < 513)
                Program.StackInterruption(4);
            else
            {
                MessageBox.Show("Endereço de entrada interrupção não alocado! A execução será encerrada!", "Erro!", MessageBoxButtons.OK);
                EndExecution();
            }
        }

        private void bt_INT5_Click(object sender, EventArgs e)
        {
            if (Program.addrInterruption[5] < 513)
                Program.StackInterruption(5);
            else
            {
                MessageBox.Show("Endereço de entrada interrupção não alocado! A execução será encerrada!", "Erro!", MessageBoxButtons.OK);
                EndExecution();
            }
        }
        #endregion Interruption's Buttom

        // Seta endereço de entrada
        private void btn_IN_Click(object sender, EventArgs e)
        {
            if (valueHexa) MessageBox.Show("Não é possível mudar de endereço com a conversão para Hexadecimal ativa!", "Erro!", MessageBoxButtons.OK);
            else
            {
                inAddr = Convert.ToInt32(txt_AddrIn.Text);
                inValue = Convert.ToInt32(txt_valueIn.Text);
                Program.inValues[inAddr] = inValue;
                Program.inOrOut[inAddr] = 1;                                        // 1 = endereço de entrada
            }
        }

        // Seta endereço de interrupções
        private void Bt_setAddrInt_Click(object sender, EventArgs e)
        {
            int[] addr = new int[6] {0, 0, 0, 0, 0, 0 };
            if (valueHexa) MessageBox.Show("Não é possível mudar de endereço com a conversão para Hexadecimal ativa!", "Erro!", MessageBoxButtons.OK);
            else
            {
                addr[0] = Convert.ToInt32(tb_AddrInt0.Text);
                if(addr[0] >= 512)
                {
                    MessageBox.Show("Os endereços de entrada vão de 0 a 511!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                Program.addrInterruption[0] = addr[0];
                Program.io.SetAddrType(addr[0], 3);

                addr[1] = Convert.ToInt32(tb_AddrInt1.Text);
                if (addr[1] >= 512)
                {
                    MessageBox.Show("Os endereços de entrada vão de 0 a 511!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                else if (addr[1] != addr[0])
                {
                    Program.addrInterruption[1] = addr[1];
                    Program.io.SetAddrType(addr[1], 3);
                }
                else
                {
                    MessageBox.Show("Os endereços devem ser diferentes!", "Erro!", MessageBoxButtons.OK);
                    return;
                }

                addr[2] = Convert.ToInt32(tb_AddrInt2.Text);
                if (addr[2] >= 512)
                {
                    MessageBox.Show("Os endereços de entrada vão de 0 a 511!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                else if (addr[2] != addr[1] && addr[2] != addr[0])
                {
                    Program.addrInterruption[2] = addr[2];
                    Program.io.SetAddrType(addr[2], 3);
                }
                else
                {
                    MessageBox.Show("Os endereços devem ser diferentes!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                

                addr[3] = Convert.ToInt32(tb_AddrInt3.Text);
                if (addr[3] >= 512)
                {
                    MessageBox.Show("Os endereços de entrada vão de 0 a 511!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                else if (addr[3] != addr[2] && addr[3] != addr[1] && addr[3] != addr[0])
                {
                    Program.addrInterruption[3] = addr[3];
                    Program.io.SetAddrType(addr[3], 3);
                }
                else
                {
                    MessageBox.Show("Os endereços devem ser diferentes!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                

                addr[4] = Convert.ToInt32(tb_AddrInt4.Text);
                if (addr[4] >= 512)
                {
                    MessageBox.Show("Os endereços de entrada vão de 0 a 511!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                else if (addr[4] != addr[3] && addr[4] != addr[2] && addr[4] != addr[1] && addr[4] != addr[0])
                {
                    Program.addrInterruption[4] = addr[4];
                    Program.io.SetAddrType(addr[4], 3);
                }
                else
                {
                    MessageBox.Show("Os endereços devem ser diferentes!", "Erro!", MessageBoxButtons.OK);
                    return;
                }

                addr[5] = Convert.ToInt32(tb_AddrInt5.Text);
                if (addr[5] >= 512)
                {
                    MessageBox.Show("Os endereços de entrada vão de 0 a 511!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
                else if (addr[5] != addr[4] && addr[5] != addr[3] && addr[5] != addr[2] && addr[5] != addr[1] && addr[5] != addr[0])
                {
                    Program.addrInterruption[5] = addr[5];
                    Program.io.SetAddrType(addr[5], 3);
                }
                else
                {
                    MessageBox.Show("Os endereços devem ser diferentes!", "Erro!", MessageBoxButtons.OK);
                    return;
                }
            }
        }

        #region Open VHDL Code Buttons
        private void Lbl_controller_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "controller";
            vhd.Show();
        }

        private void Lbl_lifo_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "lifo";
            vhd.Show();
        }

        private void Lbl_bco_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "bco";
            vhd.Show();
        }

        private void Lbl_rom_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "rom";
            vhd.Show();
        }

        private void Lbl_ram_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "ram";
            vhd.Show();
        }

        private void Lbl_ula_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "ula";
            vhd.Show();
        }

        private void Lbl_ula3_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "ula";
            vhd.Show();
        }

        private void Lbl_ula2_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "ula";
            vhd.Show();
        }

        private void Lbl_ula5_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "ula";
            vhd.Show();
        }

        private void Lbl_ula4_Click(object sender, EventArgs e)
        {
            Vhdl_File vhd = new Vhdl_File();
            Program.vhdl = "ula";
            vhd.Show();
        }
        #endregion Open VHDL Code Buttons

        private void Form_uPD_Load(object sender, EventArgs e)
        {
            LineNumber.Font = rchtxt_Editor.Font;
            rchtxt_Editor.Select();
            AddLineNumbers();
        }

        #region Editor's Events
        private void rchtxt_Editor_SelectionChanged(object sender, EventArgs e)
        {
            Point pt = rchtxt_Editor.GetPositionFromCharIndex(rchtxt_Editor.SelectionStart);
            if (pt.X == 1)
            {
                AddLineNumbers();
            }
        }

        private void rchtxt_Editor_VScroll(object sender, EventArgs e)
        {
            LineNumber.Text = "";
            AddLineNumbers();
            LineNumber.Invalidate();
        }

        private void rchtxt_Editor_TextChanged(object sender, EventArgs e)
        {
            if (rchtxt_Editor.Text == "")
            {
                AddLineNumbers();
            }
        }

        private void rchtxt_Editor_FontChanged(object sender, EventArgs e)
        {
            LineNumber.Font = rchtxt_Editor.Font;
            rchtxt_Editor.Select();
            AddLineNumbers();
        }

        private void rchtxt_Editor_MouseDown(object sender, MouseEventArgs e)
        {
            rchtxt_Editor.Select();
            LineNumber.DeselectAll();
        }
        #endregion Editor's Events

        private void Form_uPD_Resize(object sender, EventArgs e)
        {
            AddLineNumbers();
        }

        private void btn_LoadVHDLFile_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowDialog();

            if(string.IsNullOrEmpty(fbd.SelectedPath) == false)
            {
                try
                {
                    // Arquivo de banco de registradores
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "banco_registradores.vhd")))
                    {
                        sw.Write(Componentes_VHDL.banco_registradores);
                        sw.Flush();
                    }

                    // Arquivo de caminho de controle
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "caminho_controle.vhd")))
                    {
                        sw.Write(Componentes_VHDL.caminho_controle);
                        sw.Flush();
                    }

                    // Arquivo de caminho de dados
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "caminho_dados.vhd")))
                    {
                        sw.Write(Componentes_VHDL.caminho_dados);
                        sw.Flush();
                    }

                    // Arquivo de unidade de controle
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "controle.vhd")))
                    {
                        sw.Write(Componentes_VHDL.controle);
                        sw.Flush();
                    }

                    // Arquivo de CPU
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "CPU.vhd")))
                    {
                        sw.Write(Componentes_VHDL.CPU);
                        sw.Flush();
                    }

                    // Arquivo de interrupções
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "interrupcao.vhd")))
                    {
                        sw.Write(Componentes_VHDL.Interrupcao);
                        sw.Flush();
                    }

                    // Arquivo de lifo
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "lifo.vhd")))
                    {
                        sw.Write(Componentes_VHDL.lifo);
                        sw.Flush();
                    }

                    // Arquivo de memoria RAM
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "memoria.vhd")))
                    {
                        sw.Write(Componentes_VHDL.memoria);
                        sw.Flush();
                    }

                    // Arquivo de PLL
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "pll.vhd")))
                    {
                        sw.Write(Componentes_VHDL.pll);
                        sw.Flush();
                    }

                    // Arquivo do processador
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "processador.vhd")))
                    {
                        sw.Write(Componentes_VHDL.processador);
                        sw.Flush();
                    }

                    // Arquivo de prom
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "prom.vhd")))
                    {
                        sw.Write(Componentes_VHDL.prom);
                        sw.Flush();
                    }

                    // Arquivo de registrador
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "registrador.vhd")))
                    {
                        sw.Write(Componentes_VHDL.registrador);
                        sw.Flush();
                    }

                    // Arquivo de sram
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "sram.vhd")))
                    {
                        sw.Write(Componentes_VHDL.sram);
                        sw.Flush();
                    }

                    // Arquivo da ULA
                    using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "ula.vhd")))
                    {
                        sw.Write(Componentes_VHDL.ula);
                        sw.Flush();
                    }

                    // Gravar Nova ROM
                    try
                    {
                        using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "rom.vhd")))
                        {
                            for(uint i = 0; i < newRom.Length; i++)
                            {
                                sw.WriteLine(newRom[i]);
                            }
                            sw.Flush();
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("O Arquivo de ROM com o código compilado não pode ser gravado! Será gravado o arquivo padrão de ROM!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        // Arquivo da ROM padrão
                        using (StreamWriter sw = new StreamWriter(Path.Combine(fbd.SelectedPath, "rom.vhd")))
                        {
                            sw.Write(Componentes_VHDL.rom);
                            sw.Flush();
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("O Arquivo de ROM não pode ser gravado!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
    

