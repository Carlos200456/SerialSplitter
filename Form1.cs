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
using UDP;


namespace SerialSplitter
{
    public partial class Form1 : Form
    {
        string SW_Version = "3.3\r";        // =======> Version de software para compatibilidad
        string VHKV, VHMA, VHMS, VAKV, VAMA, VAMS, VEKV, VEMA, VEMS, CIKV, CIMA, CIMS, FMKV, FMMA, FMMS, F1KV, F1MA, F1MS, F2KV, F2MA, F2MS, F3KV, F3MA, F3MS, F4KV, F4MA, F4MS, SerialNumber = "";
        string dataIN1 = "", dataIN2 = "", dataIN3 = "", dataOUT1 = "", dataOUT2 = "", dataOUT3 = "", path, textKVP, textKVN, textmAReal, textRmA, LastER, textSFI, textSRE, textSCC, textSIC, textSUC, textUPW, textHU, textVCC, message;
        string Serial1PortName, Serial1BaudRate, Serial1DataBits, Serial1StopBits, Serial1Parity, Serial2PortName, Serial2BaudRate, Serial2DataBits, Serial2StopBits, Serial2Parity, Serial3PortName, Serial3BaudRate, Serial3DataBits, Serial3StopBits, Serial3Parity;

        readonly string[] mA_Table = new string[8] { "50\r", "100\r", "200\r", "300\r", "400\r", "500\r", "600\r", "700\r" };
        readonly string[] ms_Table = new string[30] { "2\r", "5\r", "8\r", "10\r", "20\r", "30\r", "40\r", "50\r", "60\r", "80\r", "100\r", "120\r", "150\r", "200\r", "250\r", "300\r", "400\r", "500\r", "600\r", "800\r", "1000\r", "1200\r", "1500\r", "2000\r", "2500\r", "3000\r", "3500\r", "4000\r", "4500\r", "5000\r" };
        bool ACK = false;
        bool NACK = false;
        bool AutoON = true;
        bool SW_Ready = false;
#if !DEBUG
        bool DEBUG = false;
#else
        bool DEBUG = true;
#endif
        bool DisplayInibit = false;
        bool Cine = false;
        bool AEC_Locked = true;
        bool RX_On = false;
        bool AEC_Lock = true;
        int AEC_Lock_Quantity = 0;
        public static float VCC = 0.0f;
        int Counter, LOW_Limit, HI_Limit, Cine_LOW_Limit, Cine_HI_Limit, pasos, AnalogData, ValorMedioCine, ValorMedioFluoro;

        float mxs;

        StringBuilder sb = new StringBuilder();
        char LF = (char)10;
        char CR = (char)13;

        System.Windows.Forms.Timer t = null;
        System.Windows.Forms.Timer f = null;

        // Lock for app path
        private static readonly object _lock = new object();
        private static string _appPath;
        public static string AppPath
        {
            get
            {
                lock (_lock)
                {
                    if (string.IsNullOrEmpty(_appPath))
                    {
                        _appPath = AppDomain.CurrentDomain.BaseDirectory;
                    }
                    return _appPath;
                }
            }
        }

        static string logFilePath = AppPath + "LogIFDUE.txt";
        static string destinationDirectory = "G:\\My Drive\\Logs\\";

        Logger logger = new Logger(logFilePath);    // Ruta del archivo de log

        // Create an isntance of IPC with DRAXA
        SimpleUDP u = new SimpleUDP(45000);
        // ...
        // string s = u.Read();
        // u.Write("Welcome App1");

        public Form1()
        {
            InitializeComponent();
            LastER = "";

            try
            {
                using (XmlTextReader configReader = new XmlTextReader(AppPath + "Config_DUE_IF.xml"))
                {
                    // Move to <Configuration>
                    while (configReader.Read())
                    {
                        if (configReader.NodeType == XmlNodeType.Element && configReader.Name == "Configuration")
                            break;
                    }

                    // Now read the configuration elements inside <Configuration>
                    while (configReader.Read())
                    {
                        if (configReader.NodeType == XmlNodeType.Element)
                        {
                            string elementName = configReader.Name;
                            string s1 = configReader.ReadElementContentAsString();

                            switch (elementName)
                            {
                                case "SerialPort1":
                                    Serial1PortName = getBetween(s1, "name=", 4);
                                    Serial1BaudRate = getBetween(s1, "baudrate=", 5);
                                    Serial1DataBits = getBetween(s1, "databits=", 1);
                                    Serial1StopBits = getBetween(s1, "stopbits=", 3);
                                    Serial1Parity = getBetween(s1, "parity=", 4);
                                    break;
                                case "SerialPort2":
                                    Serial2PortName = getBetween(s1, "name=", 4);
                                    Serial2BaudRate = getBetween(s1, "baudrate=", 5);
                                    Serial2DataBits = getBetween(s1, "databits=", 1);
                                    Serial2StopBits = getBetween(s1, "stopbits=", 3);
                                    Serial2Parity = getBetween(s1, "parity=", 4);
                                    break;
                                case "SerialPort3":
                                    Serial3PortName = getBetween(s1, "name=", 4);
                                    Serial3BaudRate = getBetween(s1, "baudrate=", 5);
                                    Serial3DataBits = getBetween(s1, "databits=", 1);
                                    Serial3StopBits = getBetween(s1, "stopbits=", 3);
                                    Serial3Parity = getBetween(s1, "parity=", 4);
                                    break;
                                case "VascularHead":
                                    VHKV = getBetween(s1, "Kv=", 3);
                                    VHMA = getBetween(s1, "mA=", 3);
                                    VHMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "VascularAbdomen":
                                    VAKV = getBetween(s1, "Kv=", 3);
                                    VAMA = getBetween(s1, "mA=", 3);
                                    VAMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "VascularExtremity":
                                    VEKV = getBetween(s1, "Kv=", 3);
                                    VEMA = getBetween(s1, "mA=", 3);
                                    VEMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "Cine":
                                    CIKV = getBetween(s1, "Kv=", 3);
                                    CIMA = getBetween(s1, "mA=", 3);
                                    CIMS = getBetween(s1, "ms=", 3);
                                    break;
                                case "FLUOROMAP":
                                    FMKV = getBetween(s1, "Kv=", 3);
                                    FMMA = getBetween(s1, "mA=", 2);
                                    FMMS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO1":
                                    F1KV = getBetween(s1, "Kv=", 3);
                                    F1MA = getBetween(s1, "mA=", 2);
                                    F1MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO2":
                                    F2KV = getBetween(s1, "Kv=", 3);
                                    F2MA = getBetween(s1, "mA=", 2);
                                    F2MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO3":
                                    F3KV = getBetween(s1, "Kv=", 3);
                                    F3MA = getBetween(s1, "mA=", 2);
                                    F3MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "FLUORO4":
                                    F4KV = getBetween(s1, "Kv=", 3);
                                    F4MA = getBetween(s1, "mA=", 2);
                                    F4MS = getBetween(s1, "ms=", 2);
                                    break;
                                case "Power":
                                    VCC = Convert.ToSingle(getBetween(s1, "VCC=", 5));
                                    LOW_Limit = Convert.ToInt32(getBetween(s1, "LOW_Limit=", 3));
                                    HI_Limit = Convert.ToInt32(getBetween(s1, "HI_Limit=", 3));
                                    Cine_LOW_Limit = Convert.ToInt32(getBetween(s1, "Cine_LOW_Limit=", 3));
                                    Cine_HI_Limit = Convert.ToInt32(getBetween(s1, "Cine_HI_Limit=", 3));
                                    break;
                                default:
                                    break;
                            }
                        }
                        // Stop if we reach the end of <Configuration>
                        if (configReader.NodeType == XmlNodeType.EndElement && configReader.Name == "Configuration")
                            break;
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
               }
                if (Serial2PortName == ports[i])
                {
                    OpenSerial2();
                    radioButton2.Checked = true;
                    radioButton2.BackColor = Color.Green;
                }
                if (Serial3PortName == ports[i])
                {
                    OpenSerial3();
                    radioButton3.Checked = true;
                    radioButton3.BackColor = Color.Green;
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

        public async void OpenSerial1()     // Serial Port para la comunicacion con el Software Digirad
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
            await Task.Delay(50);
            serialPort1.DtrEnable = true;
            await Task.Delay(100);
        }

        public async void OpenSerial2()     // Serial Port para la comunicacion con la Nano_IF
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
            await Task.Delay(50);
            serialPort2.DtrEnable = true;
            await Task.Delay(800);
        }

        public async void OpenSerial3()     // Serial Port para la comunicacion con el Generador
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
            await Task.Delay(50);
            serialPort3.DtrEnable = true;
            await Task.Delay(800);
            StartTimer();
        }

        private void StartTimer()
        {
            t = new System.Windows.Forms.Timer();
            t.Interval = 3000;
            t.Tick += new EventHandler(t_Tick);
            t.Enabled = true;
            f = new System.Windows.Forms.Timer();
            f.Interval = 200;
            f.Tick += new EventHandler(f_Tick);
            f.Enabled = true;
        }

        // Make t_Tick async
        private async void t_Tick(object sender, EventArgs e)
        {
            Counter += 1;
            pasos += 1;
            if (Counter == 10)
            {
                buttonGRST_Click(sender, e);
            }
            if (pasos > 1)
            {
                DisplayInibit = false;
                pasos = 0;
            }
            dataOUT3 = "HS";
            serialPort3.WriteLine(dataOUT3);
            if (DEBUG) DisplayData(6, dataOUT3);
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

        // Make f_Tick async
        private async void f_Tick(object sender, EventArgs e)
        {
            if (AEC_Lock_Quantity > 0)
            {
                if (AEC_Lock_Quantity == 1)
                {
                    AEC_Locked = true;
                    AEC_Lock_Quantity = 0;
                }
                else
                {
                    AEC_Lock_Quantity -= 1;
                }
            }
            if (!AEC_Locked && RX_On) AnalyzeDataABC(AnalogData);
            await ReadUPD_Data(e); // Await the async method
        }

        bool WaitForACK()
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

        private void AnalyzeDataABC(int value)
        {
            int dif_aec = 0;
            ValorMedioCine = ((Cine_HI_Limit - Cine_LOW_Limit) / 2) + Cine_LOW_Limit;
            ValorMedioFluoro = ((HI_Limit - LOW_Limit) / 2) + LOW_Limit;
            if (!Cine)   // AEC Fluoroscopia
            {
                if (value < LOW_Limit)
                {
                    dif_aec = (ValorMedioFluoro - value) / 2;
                    if (dif_aec > 10) dif_aec = 10;
                    if (dif_aec < 1) dif_aec = 1;
                    dataOUT3 = "K+" + dif_aec.ToString();
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                    AEC_Locked = false;
                } 
                if (value > HI_Limit)
                {
                    dif_aec = (value - ValorMedioFluoro) / 2;
                    if (dif_aec > 10) dif_aec = 10;
                    if (dif_aec < 1) dif_aec = 1;
                    dataOUT3 = "K-" + dif_aec.ToString();
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                    AEC_Locked = false;
                } 
                if ((value > LOW_Limit) && (value < HI_Limit)) AEC_Locked = true;
            } 
            else    // AEC Cine
            {
                if (value < Cine_LOW_Limit)
                {
                    dif_aec = (ValorMedioCine - value) / 2;
                    if (dif_aec > 10) dif_aec = 10;
                    if (dif_aec < 1) dif_aec = 1;
                    dataOUT3 = "K+" + dif_aec.ToString();
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                    AEC_Locked = false;
                }
                if (value > Cine_HI_Limit)
                {
                    dif_aec = (value - ValorMedioCine) / 2;
                    if (dif_aec > 10) dif_aec = 10;
                    if (dif_aec < 1) dif_aec = 1;
                    dataOUT3 = "K-" + dif_aec.ToString();
                    serialPort3.Write(dataOUT3);
                    DisplayInibit = true;
                    AEC_Locked = false;
                }
                if ((value > Cine_LOW_Limit) && (value < Cine_HI_Limit)) AEC_Locked = true;
            }
            if (DEBUG) DisplayData(6, dataOUT3);
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
            button1.BackColor = Color.LightGray;
            button2.BackColor = Color.LightGray;
            button3.BackColor = Color.LightGray;
            buttonPW.BackColor = Color.LightGray;
            textBoxER.Text = "";
            textBoxKV.Text = "";
            textBoxMA.Text = "";
            textBoxMS.Text = "";
            textBoxKVF.Text = "";
            textBoxMAF.Text = "";
            textBoxCms.Text = "";
            textBoxSms.Text = "";
            Refresh();
        }

        private void LoggearError()
        {
            if (LastER != textBoxER.Text)
            {
                try
                {
                    logger.LogError(textBoxER.Text);

                    // Helper to safely trim trailing \r, \n, or whitespace
                    string SafeTrim(string s)
                    {
                        return string.IsNullOrEmpty(s) ? "?" : s.TrimEnd('\r', '\n', ' ');
                    }

                    logger.LogWarning(
                        "VCC:" + SafeTrim(textVCC) +
                        " ,Sense Ref:" + SafeTrim(textSRE) +
                        " ,Sense Fil:" + SafeTrim(textSFI) +
                        " ,Sense CC:" + SafeTrim(textSCC) +
                        " ,U Cap:" + SafeTrim(textSUC) +
                        " ,I Com:" + SafeTrim(textSIC) +
                        " ,U Power:" + SafeTrim(textUPW) +
                        " ,%HU:" + SafeTrim(textHU)
                    );

                    LastER = textBoxER.Text;
                }
                catch (Exception ex)
                {
                    // Optionally log to a fallback location or show a message in DEBUG mode
#if DEBUG
                    Debug.WriteLine("LoggearError exception: " + ex.Message);
#endif
                    // Do not show MessageBox to avoid UI blocking in production
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
                        if (DEBUG) DisplayData(6, dataOUT3);
                        // Omitir la siguiente linea en Debug
#if !DEBUG
                        this.Size = new Size(488,120);
                        this.Left = 100;  // 680;   // Centrado
                        this.Top = 948;
                        this.ControlBox = false;
                        this.Text = "";
#endif
                        logger.LogInfo("Turn On by Operator");
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
                    if (DEBUG) DisplayData(6, dataOUT3);
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
                File.Copy(logFilePath, destinationDirectory + "DUE_Serial_" + SerialNumber + ".txt");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "GDrive Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process.Start("notepad.exe", logFilePath);
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
                // Close serial ports if open, suppress exceptions to avoid blocking shutdown
                if (serialPort1 != null && serialPort1.IsOpen)
                {
                    try { serialPort1.Close(); } catch { /* Ignore */ }
                }
                if (serialPort2 != null && serialPort2.IsOpen)
                {
                    try { serialPort2.Close(); } catch { /* Ignore */ }
                }
                if (serialPort3 != null && serialPort3.IsOpen)
                {
                    try { serialPort3.Close(); } catch { /* Ignore */ }
                }
            }
            catch
            {
                // Suppress all exceptions to ensure shutdown continues
            }
            finally
            {
                // Ensure form close is invoked on UI thread, but only if not disposed
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.Invoke(new EventHandler(NowClose));
                }
            }
        }

        private void NowClose(object sender, EventArgs e)
        {
            this.Close(); //now close the form
        }

        private void buttonFM_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                if (buttonFM.Text == "No Fluoro")
                {
                    dataOUT3 = "TF1";
                }
                if (buttonFM.Text == "Fluoro C")
                {
                    dataOUT3 = "TF2";
                }
                if (buttonFM.Text == "Fluoro P") dataOUT3 = "TF0";
                serialPort3.WriteLine(dataOUT3);
                if (DEBUG) DisplayData(6, dataOUT3);
            }

        }

        private void buttonRM_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                if (buttonRM.Text == "RAD0")
                {
                    dataOUT3 = "TE1";
                }
                if (buttonRM.Text == "RAD1")
                {
                    dataOUT3 = "TE2";
                }
                if (buttonRM.Text == "RAD2")
                {
                    dataOUT3 = "TE3";
                }
                if (buttonRM.Text == "CINE") dataOUT3 = "TE0";       // TODO Service Mode in Generator
                // if (buttonSPot1.Text == "Service") dataOUT = "TE0";
                serialPort3.WriteLine(dataOUT3);
                if (DEBUG) DisplayData(6, dataOUT3);
            }
        }

        private void buttonSV_Click(object sender, EventArgs e)
        {
            if (serialPort3.IsOpen)
            {
                dataOUT3 = "MZ" + textBoxMAF.Text;
                serialPort3.WriteLine(dataOUT3);
                Thread.Sleep(300);
                dataOUT3 = "TX" + textBoxSms.Text;
                serialPort3.WriteLine(dataOUT3);
            }
        }

        void SendCommands(string command1, string command2, string command3)
        {
            if (serialPort3.IsOpen)
            {
                dataOUT3 = command1;
                serialPort3.WriteLine(dataOUT3);
                Thread.Sleep(300);
                dataOUT3 = command2;
                serialPort3.WriteLine(dataOUT3);
                Thread.Sleep(300);
                dataOUT3 = command3;
                serialPort3.WriteLine(dataOUT3);
            }
        }

        // Make ReadUPD_Data async
        private async Task ReadUPD_Data(EventArgs e)
        {
            string s = u.Read();
            if (string.IsNullOrEmpty(s))
                return;

            textBoxUDP.Text = s;

            // Helper to send a sequence of commands with delay
            async Task SendCommands(params string[] commands)
            {
                foreach (var cmd in commands)
                {
                    serialPort3.WriteLine(cmd);
                    if (DEBUG) DisplayData(6, cmd);
                    await Task.Delay(300); // Non-blocking delay
                }
            }

            switch (s)
            {
                case "Head":
                    await SendCommands("KV" + VHKV, "MA" + VHMA, "TC" + VHMS);
                    break;
                case "Abdomen":
                    await SendCommands("KV" + VAKV, "MA" + VAMA, "TC" + VAMS);
                    break;
                case "Extremity":
                    await SendCommands("KV" + VEKV, "MA" + VEMA, "TC" + VEMS);
                    break;
                case "Cine":
                    await SendCommands("KV" + CIKV, "MA" + CIMA, "TC" + CIMS);
                    break;
                case "FLUOROMAP":
                    await SendCommands("KZ" + FMKV, "MZ" + FMMA, "TX" + FMMS);
                    break;
                case "FLUORO1":
                    await SendCommands("KZ" + F1KV, "MZ" + F1MA, "TX" + F1MS);
                    break;
                case "FLUORO2":
                    await SendCommands("KZ" + F2KV, "MZ" + F2MA, "TX" + F2MS);
                    break;
                case "FLUORO3":
                    await SendCommands("KZ" + F3KV, "MZ" + F3MA, "TX" + F3MS);
                    break;
                case "FLUORO4":
                    await SendCommands("KZ" + F4KV, "MZ" + F4MA, "TX" + F4MS);
                    break;
                case "FluoroOff":
                    // No action needed
                    break;
                case "CineOff":
                    // No action needed
                    break;
                default:
                    // Optionally log unknown command
                    break;
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIN1 = serialPort1.ReadLine();
            this.Invoke(new EventHandler(ShowData1));
        }

        private void ShowData1(object sender, EventArgs e)
        {
            if (DEBUG) DisplayData(1, dataIN1);
            if (dataIN1.Contains("LK"))
            {
                AEC_Lock = true;
                AEC_Lock_Quantity = Convert.ToInt32(dataIN1.Substring(2));
                if (DEBUG) DisplayData(4, dataIN1);
                serialPort1.WriteLine("ACK");
                if (DEBUG) DisplayData(4, "ACK");
            }
            if (dataIN1.Contains("Ax"))
            {
                AnalogData = Convert.ToInt32(dataIN1.Substring(2));
                if (DEBUG) DisplayData(1, dataIN1);
                serialPort1.WriteLine("ACK");
                if (DEBUG) DisplayData(4, "ACK");
                AEC_Locked = false;
            }
            else
            {
                dataOUT2 = dataIN1;
                serialPort2.WriteLine(dataOUT2);
                if (DEBUG) DisplayData(5, dataOUT2);
            }
        }

        private void serialPort2_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            dataIN2 = serialPort2.ReadLine();
            this.Invoke(new EventHandler(ShowData2));
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
                dataOUT3 = "KV" + textBoxKVF.Text;
                serialPort3.WriteLine(dataOUT3);
                if (DEBUG) DisplayData(6, dataOUT3);
                // Early return since this is a terminal action
                return;
            }
            if (dataIN2.Contains("FluoroOn") || dataIN2.Contains("CineOn"))
            {
                RX_On = true;
            }

            if (DEBUG) DisplayData(4, dataIN2);
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
                // DisplayInibit = false;
                if (!DisplayInibit) this.Invoke(new EventHandler(ShowData3)); else this.Invoke(new EventHandler(ShowDataReduced));
            }
            catch (Exception err)
            {
                // MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowData3(object sender, EventArgs e)
        {
            DoubleBuffered = true;

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
            DoubleBuffered = true;
            if (DEBUG) DisplayData(3, dataIN3);
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
                    if (textBoxKV.Text != dataIN3.Remove(0, 4))
                    {
                        textBoxKV.Text = dataIN3.Remove(0, 4);
                    }
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
    }

// Add a public class logger to the project
    public class Logger
    {
        private string logFilePath;

        public Logger(string logFilePath)
        {
            this.logFilePath = logFilePath;
            // Si el archivo no existe, crea uno nuevo; si existe, lo abre en modo append (agregar al final).
            File.AppendAllText(logFilePath, "=== Log Interface Aspor - Digirad ===" + Environment.NewLine);
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