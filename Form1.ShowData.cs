using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialSplitter
{
    public partial class Form1 : Form
    {
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIN1 = serialPort1.ReadLine();
            this.Invoke(new EventHandler(ShowData1));
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIN2 = serialPort2.ReadLine();
            this.Invoke(new EventHandler(ShowData2));
        }

        private void serialPort3_DataReceived(object sender, SerialDataReceivedEventArgs e)    // Data received from Generator
        {
            dataIN3 = serialPort3.ReadLine();
            if (dataIN3.Length > 4) message = dataIN3.Remove(4); else message = "";
            if (dataIN3.Contains("ACK"))
            {
                ACK = true;
                // GUI_Sound(1);
            }
            if (dataIN3.Contains("NACK"))
            {
                NACK = true;
            }
            Counter = 0;
            try
            {
                if (RX_On) this.Invoke(new EventHandler(ShowDataReduced)); else this.Invoke(new EventHandler(ShowData3));
            }
            catch (Exception err)
            {
                // MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowData1(object sender, EventArgs e)
        {
            // if (DEBUG) DisplayData(1, dataIN1);
            if (dataIN1.Contains("Ax"))
            {
                AnalogData = Convert.ToInt32(dataIN1.Substring(2));
                if (DEBUG) DisplayData(1, dataIN1);
                serialPort1.WriteLine("ACK");
                if (DEBUG) DisplayData(4, "ACK");
            }
            else
            {
                dataOUT2 = dataIN1;
                serialPort2.WriteLine(dataOUT2);
                if (DEBUG) DisplayData(5, dataOUT2);
            }
        }

        private void ShowData2(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dataIN2))
                return;

            if (DEBUG) DisplayData(2, dataIN2);

            // Always forward data to serialPort1
            serialPort1.WriteLine(dataIN2);

            // Handle specific commands
            if (dataIN2.Contains("FluoroOff") || dataIN2.Contains("CineOff"))
            {
                AEC_Lock = false;
                RX_On = false;
                Demora_SendKV = 20;
            }
            if (dataIN2.Contains("FluoroOn") || dataIN2.Contains("CineOn"))
            {
                if (dataIN2.Contains("CineOn")) Cine = true; else Cine = false;
                Demora_AEC = 10;
                RX_On = true;
            }
            if (dataIN2.Contains("AEC_Locked"))
            {                 
                AEC_Lock = true;
            }
            if (DEBUG) DisplayData(4, dataIN2);
        }

        private void ShowData3(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dataIN3))
                return;

            // Helper to safely trim trailing \r, \n, or whitespace
            string SafeTrim(string s)
            {
                return string.IsNullOrEmpty(s) ? "?" : s.TrimEnd('\r', '\n', ' ');
            }

            if (DEBUG) DisplayData(3, dataIN3);

            // Extract message type and payload
            string msgType = dataIN3.Length > 4 ? dataIN3.Substring(0, 4) : dataIN3;
            string msg = dataIN3.Length > 4 ? dataIN3.Substring(4) : "";

            // Remove trailing whitespace for easier comparison
            string msgTrimmed = msg.Trim();

            switch (msgType)
            {
                case "EZ: ":
                    HandleEZ(msgTrimmed);
                    break;

                case "ER: ":
                    HandleER(msgTrimmed);
                    break;

                case "ET: ":
                    textBoxET.Text = msg;
                    Cine = (msgTrimmed == "CINE");
                    break;

                case "SN: ":
                    SerialNumber = msg;
                    break;

                case "SW: ":
                    if (msg != SW_Version)
                    {
                        MessageBox.Show("Error de Software, Versiones incompatibles de Generador y GUI");
                        SW_Ready = false;
                        textBoxER.Text = "Software error (DUE != GUI)";
                        LoggearError();
                    }
                    else
                    {
                        SW_Ready = true;
                    }
                    break;

                case "Kv: ":
                    if (textBoxKV.Text != msg)
                        textBoxKV.Text = msg;
                    break;

                case "mA: ":
                    if (textBoxMA.Text != msg)
                        textBoxMA.Text = msg;
                    break;

                case "SKv:":
                    textBoxKVF.Text = msg;
                    break;

                case "SmA:":
                    textBoxMAF.Text = msg;
                    break;

                case "ms: ":
                    if (textBoxMS.Text != msg)
                        textBoxMS.Text = msg;
                    break;

                case "mf: ":
                    if (textBoxSms.Text != msg)
                        textBoxSms.Text = msg;
                    break;

                case "mc: ":
                    if (textBoxCms.Text != msg)
                        textBoxCms.Text = msg;
                    break;

                case "Kv+:":
                    textKVP = msg;
                    break;

                case "Kv-:":
                    textKVN = msg;
                    break;

                case "RmA:":
                    textRmA = msg;
                    try
                    {
                        decimal mARead = Convert.ToDecimal(textRmA);
                        int mASet = Convert.ToInt32(textBoxMA.Text);
                        int mAReal = (int)(mARead * mASet) / 1000;
                        textmAReal = mAReal.ToString();
                    }
                    catch
                    {
                        textmAReal = "???";
                    }
                    break;

                case "SFI:":
                    textSFI = msg;
                    break;

                case "SRE:":
                    textSRE = msg;
                    break;

                case "SCC:":
                    textSCC = msg;
                    break;

                case "SIC:":
                    textSIC = msg;
                    break;

                case "SUC:":
                    textSUC = msg;
                    break;

                case "UPW:":
                    textUPW = msg;
                    break;

                case "CAL:":
                    if (msgTrimmed == "1")
                    {
                        dataOUT3 = "DB0";
                        serialPort3.WriteLine(dataOUT3);
                        if (DEBUG) DisplayData(6, dataOUT3);
                    }
                    break;

                case "VCC:":
                    textVCC = msg;
                    break;

                case "HU: ":
                    textHU = msg;
                    textBoxHU.Text = msg;
                    break;

                case "FT: ":
                    UpdateFluoroButton(msgTrimmed);
                    break;

                case "RD: ":
                    if ((buttonPW.BackColor == Color.LightSkyBlue) && AutoON)
                    {
                        buttonPW_Click(sender, e);
                    }
                    UpdateRadButton(msgTrimmed);
                    break;

                case "LOG:":
                    logger.LogInfo("VCC:" + SafeTrim(textVCC) +
                                   " Kv:" + SafeTrim(textBoxKV.Text) +
                                   " mA:" + SafeTrim(textBoxMA.Text) +
                                   " ms:" + SafeTrim(textBoxMS.Text) +
                                   " Kv+:" + SafeTrim(textKVP) +
                                   " Kv-:" + SafeTrim(textKVN) +
                                   " mA:" + SafeTrim(textmAReal) + " %HU:" + SafeTrim(textHU));
                    break;

                // Add other cases as needed

                default:
                    // Optionally handle unknown message types
                    break;
            }

            // Update Power button color based on equipment state
            if (textBoxET.Text == "OFF\r" || textBoxET.Text == "ERROR\r" || textBoxET.Text == "\r")
            {
                buttonPW.BackColor = Color.LightSkyBlue;
            }
            else if (textBoxET.Text == "IDLE\r")
            {
                buttonPW.BackColor = Color.LightGreen;
            }
        }

        // Helper for EZ: errors
        private void HandleEZ(string msg)
        {
            switch (msg)
            {
                case "SBE1":
                    textBoxER.Text = "Falla de tarjeta de Estator";
                    LoggearError();
                    break;
                case "HBE1":
                    textBoxER.Text = "Falla de tarjeta de Calefaccion";
                    LoggearError();
                    break;
                case "IBM1":
                    textBoxER.Text = "Falla de tarjeta de Inversor";
                    LoggearError();
                    break;
                case "FPE1":
                    textBoxER.Text = "Verificar Relay Preparacion";
                    LoggearError();
                    break;
                case "TMP1":
                    textBoxER.Text = "Temperatura de Tubo Exedida";
                    LoggearError();
                    break;
                    // Add more as needed
            }
        }

        // Helper for ER: errors
        private void HandleER(string msg)
        {
            if (msg != "")
                textBoxER.Text = "";

            switch (msg)
            {
                case "LHB":
                    textBoxER.Text = "Falla de Lampara Testigo Calefaccion";
                    LoggearError();
                    break;
                case "CAP":
                    textBoxER.Text = "Falla de Estator (UCap)";
                    LoggearError();
                    break;
                case "COM":
                    textBoxER.Text = "Falla de Estator (ICom)";
                    LoggearError();
                    break;
                case "IBE":
                    button1.BackColor = Color.Red;
                    textBoxER.Text = "Falla de Inversor";
                    LoggearError();
                    break;
                case "IBZ":
                    button1.BackColor = Color.Red;
                    textBoxER.Text = "GAT Desconectado";
                    LoggearError();
                    break;
                case "FIL":
                    button2.BackColor = Color.Red;
                    textBoxER.Text = "Falla de Filamento";
                    LoggearError();
                    break;
                case "FCC":
                    textBoxER.Text = "Filamento en Corto Circuito";
                    button2.BackColor = Color.Red;
                    LoggearError();
                    break;
                case "TMP":
                    button3.BackColor = Color.Red;
                    textBoxER.Text = "Temperatura de Tubo Exedida";
                    LoggearError();
                    break;
                case "EEE":
                    textBoxER.Text = "Falla de Memoria EEPROM";
                    LoggearError();
                    break;
                case "SYM":
                    textBoxER.Text = "Simulador Activado";
                    break;
                case "UPW":
                    button1.BackColor = Color.Red;
                    textBoxER.Text = "Baja Tension en UPower";
                    LoggearError();
                    break;
                case "CPM":
                    textBoxER.Text = "Falta Placa Estator";
                    button3.BackColor = Color.Red;
                    LoggearError();
                    break;
                case "FPE1":
                    textBoxER.Text = "Verificar Relay Preparacion";
                    LoggearError();
                    break;
                case "ESF0":
                    textBoxER.Text = "Falla de Relay Foco Fino";
                    LoggearError();
                    break;
                case "ESF1":
                    textBoxER.Text = "Falla de Relay Foco Grueso";
                    LoggearError();
                    break;
                default:
                    if (msg != "") textBoxER.Text = msg;
                    break;
            }
        }

        // Helper for FT: (Fluoro Type) button
        private void UpdateFluoroButton(string msg)
        {
            switch (msg)
            {
                case "0":
                    buttonFM.BackColor = Color.LightGray;
                    buttonFM.Text = "No Fluoro";
                    break;
                case "1":
                    buttonFM.BackColor = Color.LightYellow;
                    buttonFM.Text = "Fluoro C";
                    break;
                case "2":
                    buttonFM.BackColor = Color.LightGreen;
                    buttonFM.Text = "Fluoro P";
                    break;
            }
        }

        // Helper for RD: (Rad Mode) button
        private void UpdateRadButton(string msg)
        {
            switch (msg)
            {
                case "0":
                    buttonRM.BackColor = Color.LightYellow;
                    buttonRM.Text = "RAD0";
                    break;
                case "1":
                    buttonRM.BackColor = Color.LightGreen;
                    buttonRM.Text = "RAD1";
                    break;
                case "2":
                    buttonRM.BackColor = Color.LightGreen;
                    buttonRM.Text = "RAD2";
                    break;
                case "3":
                    buttonRM.BackColor = Color.LightGreen;
                    buttonRM.Text = "CINE";
                    break;
                case "4":
                    buttonRM.BackColor = Color.Red;
                    buttonRM.Text = "Service";
                    break;
            }
        }

        private void ShowDataReduced(object sender, EventArgs e)
        {
            string msg;
            if (dataIN3.Length > 4) msg = dataIN3.Remove(0, 4); else msg = "";
            // ACK = false;
            // NACK = false;
            switch (message)
            {
                case "ER: ":
                    if (msg != "\r")
                    {
                        textBoxER.Text = "";
                    }
                    switch (msg)
                    {
                        case "LHB\r":
                            textBoxER.Text = "Falla de Lampara Testigo Calefaccion";
                            LoggearError();
                            break;

                        case "CAP\r":
                            textBoxER.Text = "Falla de Estator (UCap)";
                            LoggearError();
                            break;

                        case "COM\r":
                            textBoxER.Text = "Falla de Estator (ICom)";
                            LoggearError();
                            break;

                        case "IBE\r":
                            button1.BackColor = Color.Red;
                            textBoxER.Text = "Falla de Inversor";
                            LoggearError();
                            break;

                        case "IBZ\r":
                            button1.BackColor = Color.Red;  // Inverter error
                            textBoxER.Text = "GAT Desconectado";
                            LoggearError();
                            break;

                        case "FIL\r":
                            button2.BackColor = Color.Red;
                            textBoxER.Text = "Falla de Filamento";
                            LoggearError();
                            break;

                        case "FCC\r":
                            textBoxER.Text = "Filamento en Corto Circuito";
                            button2.BackColor = Color.Red;
                            LoggearError();
                            break;

                        case "TMP\r":
                            button3.BackColor = Color.Red;        // Temperatura Tubo
                            textBoxER.Text = "Temperatura de Tubo Exedida";
                            LoggearError();
                            break;

                        case "EEE\r":
                            textBoxER.Text = "Falla de Memoria EEPROM";
                            LoggearError();
                            break;

                        case "SYM\r":
                            textBoxER.Text = "Simulador Activado";
                            break;

                        case "UPW\r":
                            button1.BackColor = Color.Red;
                            textBoxER.Text = "Baja Tension en UPower";
                            LoggearError();
                            break;

                        case "CPM\r":
                            // Error Stator Boar Missing
                            textBoxER.Text = "Falta Placa Estator";
                            button3.BackColor = Color.Red;
                            LoggearError();
                            break;

                        case "FPE1\r":
                            // Fin Prep Board Missing o Relay Pegado
                            textBoxER.Text = "Verificar Relay Preparacion";
                            LoggearError();
                            break;

                        case "ESF0\r":
                            textBoxER.Text = "Falla de Relay Foco Fino";
                            LoggearError();
                            break;

                        case "ESF1\r":
                            textBoxER.Text = "Falla de Relay Foco Grueso";
                            LoggearError();
                            break;


                        default:
                            if (msg != "\r") textBoxER.Text = msg;
                            break;
                    }
                    // buttonCal.BackColor = Color.RosyBrown;
                    break;
                case "ET: ":
                    textBoxET.Text = dataIN3.Remove(0, 4);
                    break;
                case "Kv: ":
                //   if (textBoxKV.Text != dataIN3.Remove(0, 4))
                //   {
                //       textBoxKV.Text = dataIN3.Remove(0, 4);
                //   }
                    // kvs = Int32.Parse(textBoxKV.Text);
                    break;
                case "SKv:":
                    textBoxKVF.Text = dataIN3.Remove(0, 4);
                    break;
                case "CL: ":
                    break;

                case "LOG:":
                    // GUI_Sound(4);
                    /*   logger.LogInfo("VCC:" + textVCC.Substring(0, textVCC.Length - 1) +
                                       " Kv:" + textBoxKV.Text.Substring(0, textBoxKV.Text.Length - 1) +
                                       " mA:" + textBoxMA.Text.Substring(0, textBoxMA.Text.Length - 1) +
                                       " ms:" + textBoxMS.Text.Substring(0, textBoxMS.Text.Length - 1) +
                                       " Kv+:" + textKVP.Substring(0, textKVP.Length - 1) +
                                       " Kv-:" + textKVN.Substring(0, textKVN.Length - 1) +
                                       " mA:" + textmAReal + " %HU:" + textHU);   */

                    // logger.LogWarning("Este es un mensaje de advertencia.");
                    // logger.LogError("Este es un mensaje de error.");
                    break;

                default:
                    break;
            }

            if ((textBoxET.Text == "OFF\r") || (textBoxET.Text == "ERROR\r") || (textBoxET.Text == "\r"))
            {
                buttonPW.BackColor = Color.LightSkyBlue;
            }

            if ((textBoxET.Text == "IDLE\r")) // || (textBox1.Text == "ERROR\r") || (textBox1.Text == "\r")
            {
                buttonPW.BackColor = Color.LightGreen;
            }
        }

        private void DisplayData(int port, string data)
        {
            switch (port)
            {
                case 1:
                    textBoxDO.Text = data;
                    break;
                case 2:
                    textBoxNO.Text = data;
                    break;
                case 3:
                    textBoxGO.Text = data;
                    break;
                case 4:
                    textBoxDI.Text = data;
                    break;
                case 5:
                    textBoxNI.Text = data;
                    break;
                case 6:
                    textBoxGI.Text = data;
                    break;
                default:
                    break;
            }
        }
    }
}
