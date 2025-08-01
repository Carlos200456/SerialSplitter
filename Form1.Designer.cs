﻿namespace SerialSplitter
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.serialPort3 = new System.IO.Ports.SerialPort(this.components);
            this.textBoxDI = new System.Windows.Forms.TextBox();
            this.textBoxNI = new System.Windows.Forms.TextBox();
            this.textBoxGI = new System.Windows.Forms.TextBox();
            this.textBoxGO = new System.Windows.Forms.TextBox();
            this.textBoxNO = new System.Windows.Forms.TextBox();
            this.textBoxDO = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonGRST = new System.Windows.Forms.Button();
            this.buttonNRST = new System.Windows.Forms.Button();
            this.buttonPW = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBoxER = new System.Windows.Forms.TextBox();
            this.textBoxKV = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxMA = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxMS = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxKVF = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxMAF = new System.Windows.Forms.TextBox();
            this.buttonExit = new System.Windows.Forms.Button();
            this.textBoxET = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonFM = new System.Windows.Forms.Button();
            this.buttonRM = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxCms = new System.Windows.Forms.TextBox();
            this.textBoxSms = new System.Windows.Forms.TextBox();
            this.buttonSV = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxUDP = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.buttonAEC = new System.Windows.Forms.Button();
            this.textBoxHU = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // serialPort2
            // 
            this.serialPort2.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort2_DataReceived);
            // 
            // serialPort3
            // 
            this.serialPort3.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort3_DataReceived);
            // 
            // textBoxDI
            // 
            this.textBoxDI.Location = new System.Drawing.Point(147, 175);
            this.textBoxDI.Name = "textBoxDI";
            this.textBoxDI.Size = new System.Drawing.Size(100, 20);
            this.textBoxDI.TabIndex = 0;
            // 
            // textBoxNI
            // 
            this.textBoxNI.Location = new System.Drawing.Point(147, 230);
            this.textBoxNI.Name = "textBoxNI";
            this.textBoxNI.Size = new System.Drawing.Size(100, 20);
            this.textBoxNI.TabIndex = 1;
            // 
            // textBoxGI
            // 
            this.textBoxGI.Location = new System.Drawing.Point(147, 285);
            this.textBoxGI.Name = "textBoxGI";
            this.textBoxGI.Size = new System.Drawing.Size(100, 20);
            this.textBoxGI.TabIndex = 2;
            // 
            // textBoxGO
            // 
            this.textBoxGO.Location = new System.Drawing.Point(284, 285);
            this.textBoxGO.Name = "textBoxGO";
            this.textBoxGO.Size = new System.Drawing.Size(100, 20);
            this.textBoxGO.TabIndex = 5;
            // 
            // textBoxNO
            // 
            this.textBoxNO.Location = new System.Drawing.Point(284, 230);
            this.textBoxNO.Name = "textBoxNO";
            this.textBoxNO.Size = new System.Drawing.Size(100, 20);
            this.textBoxNO.TabIndex = 4;
            // 
            // textBoxDO
            // 
            this.textBoxDO.Location = new System.Drawing.Point(284, 175);
            this.textBoxDO.Name = "textBoxDO";
            this.textBoxDO.Size = new System.Drawing.Size(100, 20);
            this.textBoxDO.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(185, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "IN";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(313, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "OUT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Digirad";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 230);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "Nano_IF";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 285);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 20);
            this.label5.TabIndex = 10;
            this.label5.Text = "Generador";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoCheck = false;
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(114, 179);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(14, 13);
            this.radioButton1.TabIndex = 11;
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoCheck = false;
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(114, 234);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(14, 13);
            this.radioButton2.TabIndex = 12;
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoCheck = false;
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(114, 289);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(14, 13);
            this.radioButton3.TabIndex = 13;
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(105, 156);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Conn";
            // 
            // buttonGRST
            // 
            this.buttonGRST.Location = new System.Drawing.Point(400, 33);
            this.buttonGRST.Name = "buttonGRST";
            this.buttonGRST.Size = new System.Drawing.Size(56, 23);
            this.buttonGRST.TabIndex = 15;
            this.buttonGRST.Text = "Reset";
            this.buttonGRST.UseVisualStyleBackColor = true;
            this.buttonGRST.Click += new System.EventHandler(this.buttonGRST_Click);
            // 
            // buttonNRST
            // 
            this.buttonNRST.Location = new System.Drawing.Point(399, 230);
            this.buttonNRST.Name = "buttonNRST";
            this.buttonNRST.Size = new System.Drawing.Size(56, 23);
            this.buttonNRST.TabIndex = 16;
            this.buttonNRST.Text = "Reset";
            this.buttonNRST.UseVisualStyleBackColor = true;
            this.buttonNRST.Click += new System.EventHandler(this.buttonNRST_Click);
            // 
            // buttonPW
            // 
            this.buttonPW.Location = new System.Drawing.Point(12, 4);
            this.buttonPW.Name = "buttonPW";
            this.buttonPW.Size = new System.Drawing.Size(55, 34);
            this.buttonPW.TabIndex = 17;
            this.buttonPW.Text = "Power";
            this.buttonPW.UseVisualStyleBackColor = true;
            this.buttonPW.Click += new System.EventHandler(this.buttonPW_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(279, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(55, 23);
            this.button1.TabIndex = 18;
            this.button1.Text = "Driver";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(340, 10);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(55, 23);
            this.button2.TabIndex = 19;
            this.button2.Text = "Filament";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(401, 10);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(55, 23);
            this.button3.TabIndex = 20;
            this.button3.Text = "Stator";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // textBoxER
            // 
            this.textBoxER.Location = new System.Drawing.Point(11, 118);
            this.textBoxER.Name = "textBoxER";
            this.textBoxER.Size = new System.Drawing.Size(239, 20);
            this.textBoxER.TabIndex = 21;
            // 
            // textBoxKV
            // 
            this.textBoxKV.Location = new System.Drawing.Point(12, 53);
            this.textBoxKV.Name = "textBoxKV";
            this.textBoxKV.Size = new System.Drawing.Size(55, 20);
            this.textBoxKV.TabIndex = 22;
            this.textBoxKV.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(29, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "KV";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(89, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(22, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "mA";
            // 
            // textBoxMA
            // 
            this.textBoxMA.Location = new System.Drawing.Point(73, 53);
            this.textBoxMA.Name = "textBoxMA";
            this.textBoxMA.Size = new System.Drawing.Size(55, 20);
            this.textBoxMA.TabIndex = 24;
            this.textBoxMA.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(151, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 13);
            this.label9.TabIndex = 27;
            this.label9.Text = "ms";
            // 
            // textBoxMS
            // 
            this.textBoxMS.Location = new System.Drawing.Point(134, 53);
            this.textBoxMS.Name = "textBoxMS";
            this.textBoxMS.Size = new System.Drawing.Size(55, 20);
            this.textBoxMS.TabIndex = 26;
            this.textBoxMS.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(209, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 13);
            this.label10.TabIndex = 29;
            this.label10.Text = "KVF";
            // 
            // textBoxKVF
            // 
            this.textBoxKVF.Location = new System.Drawing.Point(195, 53);
            this.textBoxKVF.Name = "textBoxKVF";
            this.textBoxKVF.Size = new System.Drawing.Size(55, 20);
            this.textBoxKVF.TabIndex = 28;
            this.textBoxKVF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(269, 37);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 13);
            this.label11.TabIndex = 31;
            this.label11.Text = "mAF";
            // 
            // textBoxMAF
            // 
            this.textBoxMAF.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxMAF.Location = new System.Drawing.Point(256, 53);
            this.textBoxMAF.Name = "textBoxMAF";
            this.textBoxMAF.Size = new System.Drawing.Size(55, 20);
            this.textBoxMAF.TabIndex = 30;
            this.textBoxMAF.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(400, 101);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(56, 23);
            this.buttonExit.TabIndex = 32;
            this.buttonExit.Text = "App Exit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.buttonExit_Click);
            // 
            // textBoxET
            // 
            this.textBoxET.Location = new System.Drawing.Point(317, 53);
            this.textBoxET.Name = "textBoxET";
            this.textBoxET.Size = new System.Drawing.Size(55, 20);
            this.textBoxET.TabIndex = 33;
            this.textBoxET.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(325, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(40, 13);
            this.label12.TabIndex = 34;
            this.label12.Text = "Estado";
            // 
            // buttonFM
            // 
            this.buttonFM.Location = new System.Drawing.Point(134, 4);
            this.buttonFM.Name = "buttonFM";
            this.buttonFM.Size = new System.Drawing.Size(55, 34);
            this.buttonFM.TabIndex = 35;
            this.buttonFM.Text = "Fluoro Mode";
            this.buttonFM.UseVisualStyleBackColor = true;
            this.buttonFM.Click += new System.EventHandler(this.buttonFM_Click);
            // 
            // buttonRM
            // 
            this.buttonRM.Location = new System.Drawing.Point(73, 4);
            this.buttonRM.Name = "buttonRM";
            this.buttonRM.Size = new System.Drawing.Size(55, 34);
            this.buttonRM.TabIndex = 36;
            this.buttonRM.Text = "RAD Mode";
            this.buttonRM.UseVisualStyleBackColor = true;
            this.buttonRM.Click += new System.EventHandler(this.buttonRM_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(256, 121);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(29, 13);
            this.label13.TabIndex = 37;
            this.label13.Text = "Error";
            // 
            // textBoxCms
            // 
            this.textBoxCms.Location = new System.Drawing.Point(143, 79);
            this.textBoxCms.Name = "textBoxCms";
            this.textBoxCms.Size = new System.Drawing.Size(37, 20);
            this.textBoxCms.TabIndex = 38;
            this.textBoxCms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBoxSms
            // 
            this.textBoxSms.BackColor = System.Drawing.SystemColors.Info;
            this.textBoxSms.Location = new System.Drawing.Point(264, 79);
            this.textBoxSms.Name = "textBoxSms";
            this.textBoxSms.Size = new System.Drawing.Size(38, 20);
            this.textBoxSms.TabIndex = 39;
            this.textBoxSms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonSV
            // 
            this.buttonSV.Location = new System.Drawing.Point(319, 101);
            this.buttonSV.Name = "buttonSV";
            this.buttonSV.Size = new System.Drawing.Size(50, 23);
            this.buttonSV.TabIndex = 40;
            this.buttonSV.Text = "Set Val";
            this.buttonSV.UseVisualStyleBackColor = true;
            this.buttonSV.Click += new System.EventHandler(this.buttonSV_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(212, 82);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(52, 13);
            this.label14.TabIndex = 41;
            this.label14.Text = "Fluoro ms";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(99, 82);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(44, 13);
            this.label15.TabIndex = 42;
            this.label15.Text = "Cine ms";
            // 
            // textBoxUDP
            // 
            this.textBoxUDP.Location = new System.Drawing.Point(147, 197);
            this.textBoxUDP.Name = "textBoxUDP";
            this.textBoxUDP.Size = new System.Drawing.Size(100, 20);
            this.textBoxUDP.TabIndex = 43;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(117, 200);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 44;
            this.label16.Text = "UDP";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(400, 56);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(56, 23);
            this.button4.TabIndex = 45;
            this.button4.Text = "KV +";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(400, 78);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(56, 23);
            this.button5.TabIndex = 46;
            this.button5.Text = "KV -";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // buttonAEC
            // 
            this.buttonAEC.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAEC.Location = new System.Drawing.Point(316, 78);
            this.buttonAEC.Name = "buttonAEC";
            this.buttonAEC.Size = new System.Drawing.Size(56, 23);
            this.buttonAEC.TabIndex = 47;
            this.buttonAEC.Text = "AEC";
            this.buttonAEC.UseVisualStyleBackColor = true;
            this.buttonAEC.Click += new System.EventHandler(this.buttonAEC_Click);
            // 
            // textBoxHU
            // 
            this.textBoxHU.Location = new System.Drawing.Point(203, 18);
            this.textBoxHU.Name = "textBoxHU";
            this.textBoxHU.Size = new System.Drawing.Size(39, 20);
            this.textBoxHU.TabIndex = 48;
            this.textBoxHU.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(207, 4);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(31, 13);
            this.label17.TabIndex = 49;
            this.label17.Text = "%HU";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 372);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.textBoxHU);
            this.Controls.Add(this.buttonAEC);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.textBoxUDP);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.buttonSV);
            this.Controls.Add(this.textBoxSms);
            this.Controls.Add(this.textBoxCms);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.buttonRM);
            this.Controls.Add(this.buttonFM);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.textBoxET);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBoxMAF);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.textBoxKVF);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.textBoxMS);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.textBoxMA);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.textBoxKV);
            this.Controls.Add(this.textBoxER);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonPW);
            this.Controls.Add(this.buttonNRST);
            this.Controls.Add(this.buttonGRST);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxGO);
            this.Controls.Add(this.textBoxNO);
            this.Controls.Add(this.textBoxDO);
            this.Controls.Add(this.textBoxGI);
            this.Controls.Add(this.textBoxNI);
            this.Controls.Add(this.textBoxDI);
            this.Name = "Form1";
            this.Text = "Serial Splitter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.IO.Ports.SerialPort serialPort2;
        private System.IO.Ports.SerialPort serialPort3;
        private System.Windows.Forms.TextBox textBoxDI;
        private System.Windows.Forms.TextBox textBoxNI;
        private System.Windows.Forms.TextBox textBoxGI;
        private System.Windows.Forms.TextBox textBoxGO;
        private System.Windows.Forms.TextBox textBoxNO;
        private System.Windows.Forms.TextBox textBoxDO;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonGRST;
        private System.Windows.Forms.Button buttonNRST;
        private System.Windows.Forms.Button buttonPW;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxER;
        private System.Windows.Forms.TextBox textBoxKV;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxMA;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxMS;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxKVF;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxMAF;
        private System.Windows.Forms.Button buttonExit;
        private System.Windows.Forms.TextBox textBoxET;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button buttonFM;
        private System.Windows.Forms.Button buttonRM;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxCms;
        private System.Windows.Forms.TextBox textBoxSms;
        private System.Windows.Forms.Button buttonSV;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxUDP;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button buttonAEC;
        private System.Windows.Forms.TextBox textBoxHU;
        private System.Windows.Forms.Label label17;
    }
}

