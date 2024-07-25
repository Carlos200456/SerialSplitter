using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
using System.IO.Ports;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Diagnostics;

namespace SerialSplitter
{
    public partial class Form1 : Form
    {
        string SW_Version = "3.0\r";        // =======> Version de software para compatibilidad
        string SerialNumber = "";
        string dataIN1 = "", dataIN2 = "", dataIN3 = "", dataOUT1 = "", dataOUT2 = "", dataOUT3 = "", path, textKVP, textKVN, textmAReal, textRmA, LastER, textSFI, textSRE, textSCC, textSIC, textSUC, textUPW, textHU, textVCC, message;
        string Serial1PortName, Serial1BaudRate, Serial1DataBits, Serial1StopBits, Serial1Parity, Serial2PortName, Serial2BaudRate, Serial2DataBits, Serial2StopBits, Serial2Parity, Serial3PortName, Serial3BaudRate, Serial3DataBits, Serial3StopBits, Serial3Parity;

        readonly string[] mA_Table = new string[8] { "50\r", "100\r", "200\r", "300\r", "400\r", "500\r", "600\r", "700\r" };
        readonly string[] ms_Table = new string[30] { "2\r", "5\r", "8\r", "10\r", "20\r", "30\r", "40\r", "50\r", "60\r", "80\r", "100\r", "120\r", "150\r", "200\r", "250\r", "300\r", "400\r", "500\r", "600\r", "800\r", "1000\r", "1200\r", "1500\r", "2000\r", "2500\r", "3000\r", "3500\r", "4000\r", "4500\r", "5000\r" };
        Boolean ACK = false;
        Boolean NACK = false;
        Boolean AutoON = true;
        Boolean SW_Ready = false;
        int kvs, mas, mss, Counter, PROK, RXOK;
        float mxs;

        string fileToCopy = "C:\\TechXA\\LogIFDUE.txt";
        string destinationDirectory = "G:\\My Drive\\Logs\\";
        StringBuilder sb = new StringBuilder();
        char LF = (char)10;
        char CR = (char)13;

        System.Windows.Forms.Timer t = null;

        Logger logger = new Logger("C:\\TechXA\\LogIFDUE.txt");    // Ruta del archivo de log

        public Form1()
        {
            InitializeComponent();
            path = AppDomain.CurrentDomain.BaseDirectory;
            // SetImage(path + "About.png");
            // Create an isntance of XmlTextReader and call Read method to read the file  
            XmlTextReader configReader = new XmlTextReader("C:\\TechXA\\Config_DUE_IF.xml");

            LastER = "";
            PROK = 0;
            RXOK = 0;

            try
            {
                configReader.Read();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // If the node has value  
            while (configReader.Read())
            {
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "SerialPort1")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    Serial1PortName = getBetween(s1, "name=", 4);
                    Serial1BaudRate = getBetween(s1, "baudrate=", 5);
                    Serial1DataBits = getBetween(s1, "databits=", 1);
                    Serial1StopBits = getBetween(s1, "stopbits=", 3);
                    Serial1Parity = getBetween(s1, "parity=", 4);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "SerialPort2")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    Serial2PortName = getBetween(s1, "name=", 4);
                    Serial2BaudRate = getBetween(s1, "baudrate=", 5);
                    Serial2DataBits = getBetween(s1, "databits=", 1);
                    Serial2StopBits = getBetween(s1, "stopbits=", 3);
                    Serial2Parity = getBetween(s1, "parity=", 4);
                }
                if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "SerialPort3")
                {
                    string s1 = configReader.ReadElementContentAsString();
                    Serial3PortName = getBetween(s1, "name=", 4);
                    Serial3BaudRate = getBetween(s1, "baudrate=", 5);
                    Serial3DataBits = getBetween(s1, "databits=", 1);
                    Serial3StopBits = getBetween(s1, "stopbits=", 3);
                    Serial3Parity = getBetween(s1, "parity=", 4);
                }
                
            }
            CheckPortsNames();
            this.TopMost = true;
        }

        void CheckPortsNames()
        {
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; i++)
            {
                if (Serial1PortName == ports[i])
                {
                    OpenSerial1();
                    radioButton1.Checked = true;
                    radioButton1.BackColor = Color.Green;
                    radioButton1.ForeColor = Color.Green;
                }
                if (Serial2PortName == ports[i])
                {
                    OpenSerial2();
                    radioButton2.Checked = true;
                    radioButton2.BackColor = Color.Green;
                    radioButton2.ForeColor = Color.Green;
                }
                if (Serial3PortName == ports[i])
                {
                    OpenSerial3();
                    radioButton3.Checked = true;
                    radioButton3.BackColor = Color.Red;
                }
            }
        }

        public static string getBetween(string strSource, string strStart, int largo)
        {
            if (strSource.Contains(strStart))
            {
                int Start, End;
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = Start + largo;
                return strSource.Substring((Start + 1), End - Start);
            }

            return "";
        }

        public void OpenSerial1()     // Serial Port para la comunicacion con el Software Vieworks
        {
            serialPort1.PortName = Serial1PortName;
            serialPort1.BaudRate = int.Parse(Serial1BaudRate);  // 115200  Valid values are 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, or 115200.
            serialPort1.DataBits = int.Parse(Serial1DataBits);
            serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), Serial1StopBits);
            serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), Serial1Parity);
            serialPort1.Encoding = Encoding.GetEncoding("iso-8859-1");
            // Encoding = Encoding.GetEncoding("Windows-1252");
            serialPort1.Open();
            serialPort1.DtrEnable = false;
            Thread.Sleep(50);
            serialPort1.DtrEnable = true;
            Thread.Sleep(100);
        }

        public void OpenSerial2()     // Serial Port para la comunicacion con el Generador
        {
            serialPort2.PortName = Serial2PortName;
            serialPort2.BaudRate = int.Parse(Serial2BaudRate);  // 115200  Valid values are 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, or 115200.
            serialPort2.DataBits = int.Parse(Serial2DataBits);
            serialPort2.StopBits = (StopBits)Enum.Parse(typeof(StopBits), Serial2StopBits);
            serialPort2.Parity = (Parity)Enum.Parse(typeof(Parity), Serial2Parity);
            serialPort2.Encoding = Encoding.GetEncoding("iso-8859-1");
            // Encoding = Encoding.GetEncoding("Windows-1252");
            serialPort2.Open();
            serialPort2.DtrEnable = false;
            Thread.Sleep(50);
            serialPort2.DtrEnable = true;
            Thread.Sleep(800);
        }

        public void OpenSerial3()     // Serial Port para la comunicacion con el Generador
        {
            serialPort3.PortName = Serial3PortName;
            serialPort3.BaudRate = int.Parse(Serial3BaudRate);  // 115200  Valid values are 110, 300, 1200, 2400, 4800, 9600, 19200, 38400, 57600, or 115200.
            serialPort3.DataBits = int.Parse(Serial3DataBits);
            serialPort3.StopBits = (StopBits)Enum.Parse(typeof(StopBits), Serial3StopBits);
            serialPort3.Parity = (Parity)Enum.Parse(typeof(Parity), Serial3Parity);
            serialPort3.Encoding = Encoding.GetEncoding("iso-8859-1");
            // Encoding = Encoding.GetEncoding("Windows-1252");
            serialPort3.Open();
            serialPort3.DtrEnable = false;
            Thread.Sleep(50);
            serialPort3.DtrEnable = true;
            Thread.Sleep(800);
            StartTimer();
        }

        private void StartTimer()
        {
            t = new System.Windows.Forms.Timer();
            t.Interval = 2000;
            t.Tick += new EventHandler(t_Tick);
            t.Enabled = true;
        }

        void t_Tick(object sender, EventArgs e)
        {
            Counter += 1;
            if (Counter == 10)
            {
                buttonGRST_Click(sender, e);
            }
            dataOUT3 = "HS";
            serialPort3.WriteLine(dataOUT3);
            if (WaitForACK())
            {
                buttonPW.BackColor = Color.LightGreen;
                ACK = false;
                LastER = "";
            }
            else
            {
                buttonPW.BackColor = Color.Red;
                ACK = false;
                if (LastER != "Hand Shake Error")
                {
                    logger.LogError("Hand Shake Error");
                    LastER = "Hand Shake Error";
                }
            }
        }

        Boolean WaitForACK()
        {
            int start_time, elapsed_time;
            start_time = DateTime.Now.Second;
            while (!ACK && !NACK)
            {
                elapsed_time = DateTime.Now.Second - start_time;
                if ((elapsed_time >= 2) || (elapsed_time < 0))
                {
                    NACK = true;
                    // textBoxER.Text = "Timeout en Comunicacion";
                    return false;
                }
            }
            if (ACK) return true; else return false;
        }

        private void buttonNRST_Click(object sender, EventArgs e)
        {
            serialPort2.DtrEnable = false;
            Thread.Sleep(50);
            serialPort2.DtrEnable = true;
            Thread.Sleep(100);
            serialPort2.DtrEnable = false;
        }

        private void buttonGRST_Click(object sender, EventArgs e)
        {
            serialPort3.DtrEnable = false;
            Thread.Sleep(50);
            serialPort3.DtrEnable = true;
            Thread.Sleep(100);
            serialPort3.DtrEnable = false;
            buttonPW.BackColor = Color.LightGray;
            button2.BackColor = Color.LightGray;
            button3.BackColor = Color.LightGray;
            buttonPW.BackColor = Color.LightGray;
            textBoxER.Text = "";
            textBoxKV.Text = "";
            textBoxMA.Text = "";
            textBoxMS.Text = "";
            textBoxKVF.Text = "";
            textBoxMAF.Text = "";
            Refresh();
        }

        private void LoggearError()
        {
            if (LastER != textBoxER.Text)
            {
                try
                {
                    logger.LogError(textBoxER.Text);
                    logger.LogWarning("VCC:" + textVCC.Substring(0, textVCC.Length - 1) +
                                " ,Sense Ref:" + textSRE.Substring(0, textSRE.Length - 1) +
                                " ,Sense Fil:" + textSFI.Substring(0, textSFI.Length - 1) +
                                " ,Sense CC:" + textSCC.Substring(0, textSCC.Length - 1) +
                                " ,U Cap:" + textSUC.Substring(0, textSUC.Length - 1) +
                                " ,I Com:" + textSIC.Substring(0, textSIC.Length - 1) +
                                " ,U Power:" + textUPW.Substring(0, textUPW.Length - 1) +
                                " ,%HU:" + textHU.Substring(0, textHU.Length - 1));
                    LastER = textBoxER.Text;
                }
                catch (Exception err)
                {
                    // MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            logger.LogInfo("Salida de la Aplicación por el operador");
            // Call Form1_FormClosing
            Form_FormClosing(sender, new FormClosingEventArgs(CloseReason.UserClosing, false));
            Application.Exit();
        }

        private void buttonPW_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                if (buttonPW.BackColor == Color.LightSkyBlue)
                {
                    if (SW_Ready)
                    {
                        dataOUT3 = "PW1";
                        serialPort3.WriteLine(dataOUT3);
                        this.Size = new Size(350, 98);
                        this.Left = 680;
                        this.Top = 1016;
                        this.ControlBox = false;
                        this.Text = "";
                        // logger.LogInfo("Turn On by Operator");
                        AutoON = true;
                    }
                    else
                    {
                        MessageBox.Show("Error de Software, Versiones incompatibles de Generador y GUI");
                    }
                }
                else
                {
                    dataOUT3 = "PW0";
                    serialPort3.WriteLine(dataOUT3);
                    logger.LogInfo("Turn Off by Operator");
                    AutoON = false;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (SerialNumber.Substring(SerialNumber.Length - 1) == "\r")
                {
                    SerialNumber = SerialNumber.Substring(0, SerialNumber.Length - 1);
                }
                if (File.Exists(destinationDirectory + "DUE_Serial_" + SerialNumber + ".txt"))
                {
                    File.Delete(destinationDirectory + "DUE_Serial_" + SerialNumber + ".txt");
                }
                File.Copy(fileToCopy, destinationDirectory + "DUE_Serial_" + SerialNumber + ".txt");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "GDrive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", "C:\\TechXA\\LogIFDUE.txt");
        }

        private void Form_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (serialPort1.IsOpen || serialPort2.IsOpen || serialPort3.IsOpen)
            {
                e.Cancel = true; //cancel the fom closing
                Thread CloseDown = new Thread(new ThreadStart(CloseSerialOnExit)); //close port in new thread to avoid hang
                CloseDown.Start(); //close port in new thread to avoid hang
            }
        }

        private void CloseSerialOnExit()
        {
            try
            {
                serialPort1.Close(); //close the serial port
                serialPort2.Close();
                serialPort3.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); //catch any serial port closing error messages
            }
            this.Invoke(new EventHandler(NowClose)); //now close back in the main thread
        }

        private void NowClose(object sender, EventArgs e)
        {
            this.Close(); //now close the form
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
                this.Invoke(new EventHandler(ShowData3));
            }
            catch (Exception err)
            {
                // MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowData3(object sender, EventArgs e)
        {
            DoubleBuffered = true;
            string msg;
            if (dataIN3.Length > 4) msg = dataIN3.Remove(0, 4); else msg = "";
            // ACK = false;
            // NACK = false;
            switch (message)
            {
                case "EZ: ":
                    switch (msg)
                    {
                        case "SBE0\r":
                            break;

                        case "SBE1\r":
                            textBoxER.Text = "Falla de tarjeta de Estator";
                            LoggearError();

                            break;

                        case "HBE0\r":
                            break;

                        case "HBE1\r":
                            textBoxER.Text = "Falla de tarjeta de Calefaccion";
                            LoggearError();
                            break;

                        case "IBM0\r":
                            break;

                        case "IBM1\r":
                            textBoxER.Text = "Falla de tarjeta de Inversor";
                            LoggearError();
                            break;

                        case "FPE0\r":
                            break;

                        case "FPE1\r":
                            textBoxER.Text = "Verificar Relay Preparacion";
                            LoggearError();
                            break;

                        case "TMP0\r":
                            break;

                        case "TMP1\r":
                            textBoxER.Text = "Temperatura de Tubo Exedida";
                            LoggearError();
                            break;

                        default:
                            break;
                    }
                    break;
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
                case "SN: ":
                    SerialNumber = dataIN3.Remove(0, 4);
                    break;
                case "SW: ":
                    if (dataIN3.Remove(0, 4) != SW_Version)
                    {
                        MessageBox.Show("Error de Software, Versiones incompatibles de Generador y GUI");
                        SW_Ready = false;
                        textBoxER.Text = "Software error (DUE != GUI)";
                        LoggearError();
                        break;
                    }
                    SW_Ready = true;
                    break;
                case "Kv: ":
                    if (textBoxKV.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxKV.Text = dataIN3.Remove(0, 4);
                    }
                    kvs = Int32.Parse(textBoxKV.Text);
                    break;
                case "mA: ":
                    if (textBoxMA.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxMA.Text = dataIN3.Remove(0, 4);
                    }
                    mas = Int32.Parse(textBoxMA.Text);
                    break;
                case "SKv:":
                    textBoxKVF.Text = dataIN3.Remove(0, 4);
                    break;
                case "SmA:":
                    textBoxMAF.Text = dataIN3.Remove(0, 4);
                    break;
                case "ms: ":
                    if (textBoxMS.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxMS.Text = dataIN3.Remove(0, 4);
                    }
                    mss = Int32.Parse(textBoxMS.Text);
                    break;
                case "Kv+:":
                    textKVP = dataIN3.Remove(0, 4);
                    break;
                case "Kv-:":
                    textKVN = dataIN3.Remove(0, 4);
                    break;
                case "RmA:":
                    textRmA = dataIN3.Remove(0, 4);
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
                    textSFI = dataIN3.Remove(0, 4);
                    break;
                case "SRE:":
                    textSRE = dataIN3.Remove(0, 4);
                    break;
                case "SCC:":
                    textSCC = dataIN3.Remove(0, 4);
                    break;
                case "SIC:":
                    textSIC = dataIN3.Remove(0, 4);
                    break;
                case "SUC:":
                    textSUC = dataIN3.Remove(0, 4);
                    break;
                case "UPW:":
                    textUPW = dataIN3.Remove(0, 4);
                    break;
                case "TST:":
                    // textBoxTST.Text = dataIN3.Remove(0, 4);
                    break;
                case "FO: ":
                    break;
                case "HO: ":
                    // textBoxHO.Text = dataIN3.Remove(0, 4);
                    break;
                case "CAL:":
                    if (msg == "1\r")
                    {
                        dataOUT3 = "DB0";
                        serialPort3.WriteLine(dataOUT3);   // Send data to Generator to turn off Calibration
                    }
                    break;
                case "VCC:":
                    textVCC = dataIN3.Remove(0, 4);
                    if (textVCC != "")
                    {
                        try
                        {
                            float Test = float.Parse(textVCC);
                            if (Test < 15.0 || Test > 15.6) radioButton3.BackColor = Color.Red;
                            else
                            {
                                if (Test < 15.2 || Test > 15.4) radioButton3.BackColor = Color.Yellow;
                                else
                                {
                                    if (Test > 15.19 || Test < 15.39) radioButton3.BackColor = Color.LightGreen;
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            //   MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    break;
                case "HU: ":
                    textHU = dataIN3.Remove(0, 4);
                    break;
                case "FT: ":
                    if (msg == "0\r")
                    {
                        buttonFM.BackColor = Color.LightGray;
                        buttonFM.Text = "No Fluoro";
                    }
                    if (msg == "1\r")
                    {
                        buttonFM.BackColor = Color.LightYellow;
                        buttonFM.Text = "Fluoro C";
                    }
                    if (msg == "2\r")
                    {
                        buttonFM.BackColor = Color.LightGreen;
                        buttonFM.Text = "Fluoro P";
                    }
                    break;
                case "RD: ":
                    if ((buttonPW.BackColor == Color.LightSkyBlue) && AutoON)
                    {
                        buttonPW_Click(sender, e);
                    }

                    if (msg == "0\r")
                    {
                        buttonRM.BackColor = Color.LightYellow;
                        buttonRM.Text = "RAD";
                    }
                    if (msg == "1\r")
                    {
                        buttonRM.BackColor = Color.LightGreen;
                        buttonRM.Text = "RAD 1";
                    }
                    if (msg == "2\r")
                    {
                        buttonRM.BackColor = Color.LightGreen;
                        buttonRM.Text = "RAD 2";
                    }
                    if (msg == "3\r")
                    {
                        buttonRM.BackColor = Color.LightGreen;
                        buttonRM.Text = "CINE";
                    }
                    if (msg == "4\r")
                    {
                        buttonRM.BackColor = Color.Red;
                        buttonRM.Text = "Service";
                    }
                    break;
                case "CL: ":
                    break;
                case "POK:":
                    if (msg == "0\r")
                    {
                        PROK = 0;
                    }
                    if (msg == "1\r")
                    {
                        PROK = 1;
                    }
                    if (msg == "2\r")
                    {
                        PROK = 2;
                    }
                    break;
                case "XOK:":
                    if (msg == "0\r")
                    {
                        // buttonPrep
                        RXOK = 0;
                    }
                    if (msg == "1\r")
                    {
                        RXOK = 1;
                    }
                    break;
                case "EEP:":
                    // ConfigSize = Convert.ToInt32(dataIN.Remove(0, 4));
                    // ConfigReady = true;
                    break;

                case "TC1:":
                    // textBoxTC1.Text = dataIN.Remove(0, 4) + "ºC";
                    break;

                case "LOG:":
                    // GUI_Sound(4);
                    logger.LogInfo("VCC:" + textVCC.Substring(0, textVCC.Length - 1) +
                                   " Kv:" + textBoxKV.Text.Substring(0, textBoxKV.Text.Length - 1) +
                                   " mA:" + textBoxMA.Text.Substring(0, textBoxMA.Text.Length - 1) +
                                   " ms:" + textBoxMS.Text.Substring(0, textBoxMS.Text.Length - 1) +
                                   " Kv+:" + textKVP.Substring(0, textKVP.Length - 1) +
                                   " Kv-:" + textKVN.Substring(0, textKVN.Length - 1) +
                                   " mA:" + textmAReal + " %HU:" + textHU);

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
    }

    // Add a public class logger to the project
    public class Logger
    {
        private string logFilePath;

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
            // Si el archivo no existe, crea uno nuevo; si existe, lo abre en modo append (agregar al final).
            File.AppendAllText(logFilePath, "=== Inicio de Log Interface Pimax Digirad ===" + Environment.NewLine);
        }

        public void LogInfo(string message)
        {
            WriteLog("INFO", message);
        }

        public void LogWarning(string message)
        {
            WriteLog("WARNING", message);
        }

        public void LogError(string message)
        {
            WriteLog("ERROR", message);
        }

        private void WriteLog(string logLevel, string message)
        {
            string logEntry = $"{DateTime.Now} [{logLevel}] - {message}{Environment.NewLine}";
            File.AppendAllText(logFilePath, logEntry);
        }
    }

}