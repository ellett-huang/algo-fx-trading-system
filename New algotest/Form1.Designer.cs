namespace AlgoTest
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonInitial = new System.Windows.Forms.Button();
            this.textBoxInputFile = new System.Windows.Forms.TextBox();
            this.buttonInput = new System.Windows.Forms.Button();
            this.buttonBrowser = new System.Windows.Forms.Button();
            this.buttonAnalysis = new System.Windows.Forms.Button();
            this.comboBoxTimeFrame = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BoxStopLoss = new System.Windows.Forms.Label();
            this.BoxTakeProfit = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.TextBoxStopLoss = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.TextBoxTakeProfit = new System.Windows.Forms.TextBox();
            this.TextBoxMaxHolding = new System.Windows.Forms.TextBox();
            this.TextBoxMinPriceMargin = new System.Windows.Forms.TextBox();
            this.TextBoxMinTimeInterval = new System.Windows.Forms.TextBox();
            this.textBoxStart = new System.Windows.Forms.TextBox();
            this.textBoxStop = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxADX = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxCCI = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxMACD = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxRSI = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxTSI = new System.Windows.Forms.TextBox();
            this.buttonManul = new System.Windows.Forms.Button();
            this.checkBoxShort = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxALI = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxATR = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.textBoxProfit = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.textBoxDescription = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(744, 16);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(743, 350);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaint);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(39, 376);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(1448, 202);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Sort_Click);
            this.dataGridView1.DoubleClick += new System.EventHandler(this.Grid_DBClick);
            // 
            // buttonInitial
            // 
            this.buttonInitial.Location = new System.Drawing.Point(38, 332);
            this.buttonInitial.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonInitial.Name = "buttonInitial";
            this.buttonInitial.Size = new System.Drawing.Size(112, 35);
            this.buttonInitial.TabIndex = 1;
            this.buttonInitial.Text = "Data Initial";
            this.buttonInitial.UseVisualStyleBackColor = true;
            this.buttonInitial.Click += new System.EventHandler(this.buttonInitial_Click);
            // 
            // textBoxInputFile
            // 
            this.textBoxInputFile.Location = new System.Drawing.Point(43, 18);
            this.textBoxInputFile.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxInputFile.Name = "textBoxInputFile";
            this.textBoxInputFile.Size = new System.Drawing.Size(459, 26);
            this.textBoxInputFile.TabIndex = 2;
            // 
            // buttonInput
            // 
            this.buttonInput.Location = new System.Drawing.Point(137, 50);
            this.buttonInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonInput.Name = "buttonInput";
            this.buttonInput.Size = new System.Drawing.Size(112, 35);
            this.buttonInput.TabIndex = 3;
            this.buttonInput.Text = "All In One";
            this.buttonInput.UseVisualStyleBackColor = true;
            this.buttonInput.Click += new System.EventHandler(this.buttonInput_Click);
            // 
            // buttonBrowser
            // 
            this.buttonBrowser.Location = new System.Drawing.Point(42, 50);
            this.buttonBrowser.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonBrowser.Name = "buttonBrowser";
            this.buttonBrowser.Size = new System.Drawing.Size(86, 35);
            this.buttonBrowser.TabIndex = 4;
            this.buttonBrowser.Text = "Browser";
            this.buttonBrowser.UseVisualStyleBackColor = true;
            this.buttonBrowser.Click += new System.EventHandler(this.buttonBrowser_Click);
            // 
            // buttonAnalysis
            // 
            this.buttonAnalysis.Location = new System.Drawing.Point(160, 332);
            this.buttonAnalysis.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonAnalysis.Name = "buttonAnalysis";
            this.buttonAnalysis.Size = new System.Drawing.Size(141, 35);
            this.buttonAnalysis.TabIndex = 1;
            this.buttonAnalysis.Text = "Data Analysis";
            this.buttonAnalysis.UseVisualStyleBackColor = true;
            this.buttonAnalysis.Click += new System.EventHandler(this.buttonAnalysis_Click);
            // 
            // comboBoxTimeFrame
            // 
            this.comboBoxTimeFrame.FormattingEnabled = true;
            this.comboBoxTimeFrame.Items.AddRange(new object[] {
            "1",
            "5",
            "30",
            "60",
            "120",
            "240"});
            this.comboBoxTimeFrame.Location = new System.Drawing.Point(189, 90);
            this.comboBoxTimeFrame.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.comboBoxTimeFrame.Name = "comboBoxTimeFrame";
            this.comboBoxTimeFrame.Size = new System.Drawing.Size(140, 28);
            this.comboBoxTimeFrame.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 96);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 20);
            this.label1.TabIndex = 6;
            this.label1.Text = "Time Frame : ";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(308, 332);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(165, 35);
            this.button1.TabIndex = 1;
            this.button1.Text = "Back/Forward Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 138);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Min Price Margin : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 179);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "Max Time Interval : ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(339, 96);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(164, 20);
            this.label4.TabIndex = 12;
            this.label4.Text = "Max Holding Interval : ";
            // 
            // BoxStopLoss
            // 
            this.BoxStopLoss.AutoSize = true;
            this.BoxStopLoss.Location = new System.Drawing.Point(339, 139);
            this.BoxStopLoss.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BoxStopLoss.Name = "BoxStopLoss";
            this.BoxStopLoss.Size = new System.Drawing.Size(93, 20);
            this.BoxStopLoss.TabIndex = 14;
            this.BoxStopLoss.Text = "Stop Loss : ";
            // 
            // BoxTakeProfit
            // 
            this.BoxTakeProfit.AutoSize = true;
            this.BoxTakeProfit.Location = new System.Drawing.Point(340, 179);
            this.BoxTakeProfit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.BoxTakeProfit.Name = "BoxTakeProfit";
            this.BoxTakeProfit.Size = new System.Drawing.Size(97, 20);
            this.BoxTakeProfit.TabIndex = 15;
            this.BoxTakeProfit.Text = "Take Profit : ";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(512, 179);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(145, 26);
            this.textBox1.TabIndex = 16;
            // 
            // TextBoxStopLoss
            // 
            this.TextBoxStopLoss.Location = new System.Drawing.Point(512, 135);
            this.TextBoxStopLoss.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TextBoxStopLoss.Name = "TextBoxStopLoss";
            this.TextBoxStopLoss.Size = new System.Drawing.Size(145, 26);
            this.TextBoxStopLoss.TabIndex = 16;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(512, 95);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(145, 26);
            this.textBox3.TabIndex = 16;
            // 
            // TextBoxTakeProfit
            // 
            this.TextBoxTakeProfit.Location = new System.Drawing.Point(512, 176);
            this.TextBoxTakeProfit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TextBoxTakeProfit.Name = "TextBoxTakeProfit";
            this.TextBoxTakeProfit.Size = new System.Drawing.Size(145, 26);
            this.TextBoxTakeProfit.TabIndex = 16;
            // 
            // TextBoxMaxHolding
            // 
            this.TextBoxMaxHolding.Location = new System.Drawing.Point(512, 95);
            this.TextBoxMaxHolding.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TextBoxMaxHolding.Name = "TextBoxMaxHolding";
            this.TextBoxMaxHolding.Size = new System.Drawing.Size(145, 26);
            this.TextBoxMaxHolding.TabIndex = 16;
            // 
            // TextBoxMinPriceMargin
            // 
            this.TextBoxMinPriceMargin.Location = new System.Drawing.Point(189, 135);
            this.TextBoxMinPriceMargin.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TextBoxMinPriceMargin.Name = "TextBoxMinPriceMargin";
            this.TextBoxMinPriceMargin.Size = new System.Drawing.Size(145, 26);
            this.TextBoxMinPriceMargin.TabIndex = 16;
            // 
            // TextBoxMinTimeInterval
            // 
            this.TextBoxMinTimeInterval.Location = new System.Drawing.Point(189, 176);
            this.TextBoxMinTimeInterval.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.TextBoxMinTimeInterval.Name = "TextBoxMinTimeInterval";
            this.TextBoxMinTimeInterval.Size = new System.Drawing.Size(145, 26);
            this.TextBoxMinTimeInterval.TabIndex = 16;
            // 
            // textBoxStart
            // 
            this.textBoxStart.Location = new System.Drawing.Point(330, 53);
            this.textBoxStart.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxStart.Name = "textBoxStart";
            this.textBoxStart.Size = new System.Drawing.Size(56, 26);
            this.textBoxStart.TabIndex = 17;
            this.textBoxStart.Text = "0";
            // 
            // textBoxStop
            // 
            this.textBoxStop.Location = new System.Drawing.Point(439, 55);
            this.textBoxStop.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxStop.Name = "textBoxStop";
            this.textBoxStop.Size = new System.Drawing.Size(58, 26);
            this.textBoxStop.TabIndex = 17;
            this.textBoxStop.Text = "47000";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(265, 58);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 20);
            this.label5.TabIndex = 6;
            this.label5.Text = "Start : ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(389, 60);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 20);
            this.label6.TabIndex = 6;
            this.label6.Text = "Stop : ";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(38, 222);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 20);
            this.label7.TabIndex = 10;
            this.label7.Text = "ADX : ";
            // 
            // textBoxADX
            // 
            this.textBoxADX.Location = new System.Drawing.Point(87, 216);
            this.textBoxADX.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxADX.Name = "textBoxADX";
            this.textBoxADX.Size = new System.Drawing.Size(44, 26);
            this.textBoxADX.TabIndex = 16;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(136, 222);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 20);
            this.label8.TabIndex = 10;
            this.label8.Text = "CCI : ";
            // 
            // textBoxCCI
            // 
            this.textBoxCCI.Location = new System.Drawing.Point(184, 216);
            this.textBoxCCI.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxCCI.Name = "textBoxCCI";
            this.textBoxCCI.Size = new System.Drawing.Size(44, 26);
            this.textBoxCCI.TabIndex = 16;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(232, 222);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 20);
            this.label9.TabIndex = 10;
            this.label9.Text = "MACD : ";
            // 
            // textBoxMACD
            // 
            this.textBoxMACD.Location = new System.Drawing.Point(302, 216);
            this.textBoxMACD.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxMACD.Name = "textBoxMACD";
            this.textBoxMACD.Size = new System.Drawing.Size(44, 26);
            this.textBoxMACD.TabIndex = 16;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(38, 261);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(49, 20);
            this.label10.TabIndex = 10;
            this.label10.Text = "RSI : ";
            // 
            // textBoxRSI
            // 
            this.textBoxRSI.Location = new System.Drawing.Point(86, 255);
            this.textBoxRSI.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxRSI.Name = "textBoxRSI";
            this.textBoxRSI.Size = new System.Drawing.Size(44, 26);
            this.textBoxRSI.TabIndex = 16;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(134, 261);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(46, 20);
            this.label11.TabIndex = 10;
            this.label11.Text = "TSI : ";
            // 
            // textBoxTSI
            // 
            this.textBoxTSI.Location = new System.Drawing.Point(184, 255);
            this.textBoxTSI.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxTSI.Name = "textBoxTSI";
            this.textBoxTSI.Size = new System.Drawing.Size(44, 26);
            this.textBoxTSI.TabIndex = 16;
            // 
            // buttonManul
            // 
            this.buttonManul.Location = new System.Drawing.Point(510, 333);
            this.buttonManul.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonManul.Name = "buttonManul";
            this.buttonManul.Size = new System.Drawing.Size(112, 35);
            this.buttonManul.TabIndex = 1;
            this.buttonManul.Text = "Manul Test";
            this.buttonManul.UseVisualStyleBackColor = true;
            this.buttonManul.Click += new System.EventHandler(this.buttonManul_Click);
            // 
            // checkBoxShort
            // 
            this.checkBoxShort.AutoSize = true;
            this.checkBoxShort.Location = new System.Drawing.Point(512, 221);
            this.checkBoxShort.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.checkBoxShort.Name = "checkBoxShort";
            this.checkBoxShort.Size = new System.Drawing.Size(74, 24);
            this.checkBoxShort.TabIndex = 18;
            this.checkBoxShort.Text = "Short";
            this.checkBoxShort.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.AutoScrollMargin = new System.Drawing.Size(5, 5);
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(39, 586);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1448, 339);
            this.panel2.TabIndex = 19;
            this.panel2.Scroll += new System.Windows.Forms.ScrollEventHandler(this.OnScroll);
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.On_Paint_Chart);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(232, 262);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(46, 20);
            this.label12.TabIndex = 10;
            this.label12.Text = "ALI : ";
            // 
            // textBoxALI
            // 
            this.textBoxALI.Location = new System.Drawing.Point(300, 256);
            this.textBoxALI.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxALI.Name = "textBoxALI";
            this.textBoxALI.Size = new System.Drawing.Size(44, 26);
            this.textBoxALI.TabIndex = 16;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(357, 222);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 20);
            this.label13.TabIndex = 10;
            this.label13.Text = "ATR : ";
            // 
            // textBoxATR
            // 
            this.textBoxATR.Location = new System.Drawing.Point(411, 218);
            this.textBoxATR.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxATR.Name = "textBoxATR";
            this.textBoxATR.Size = new System.Drawing.Size(44, 26);
            this.textBoxATR.TabIndex = 16;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(510, 253);
            this.buttonAdd.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(74, 35);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // textBoxProfit
            // 
            this.textBoxProfit.Location = new System.Drawing.Point(411, 258);
            this.textBoxProfit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxProfit.Name = "textBoxProfit";
            this.textBoxProfit.Size = new System.Drawing.Size(68, 26);
            this.textBoxProfit.TabIndex = 16;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(357, 261);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(58, 20);
            this.label14.TabIndex = 10;
            this.label14.Text = "Porfit : ";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(510, 293);
            this.button2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 35);
            this.button2.TabIndex = 1;
            this.button2.Text = "Add to DB";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.buttonAddtoDB_Click);
            // 
            // textBoxDescription
            // 
            this.textBoxDescription.Location = new System.Drawing.Point(86, 292);
            this.textBoxDescription.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.Size = new System.Drawing.Size(394, 26);
            this.textBoxDescription.TabIndex = 16;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(38, 296);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(50, 20);
            this.label15.TabIndex = 10;
            this.label15.Text = "Des : ";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1500, 929);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.checkBoxShort);
            this.Controls.Add(this.textBoxStop);
            this.Controls.Add(this.textBoxStart);
            this.Controls.Add(this.TextBoxMaxHolding);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBoxTSI);
            this.Controls.Add(this.textBoxRSI);
            this.Controls.Add(this.textBoxMACD);
            this.Controls.Add(this.textBoxATR);
            this.Controls.Add(this.textBoxDescription);
            this.Controls.Add(this.textBoxProfit);
            this.Controls.Add(this.textBoxALI);
            this.Controls.Add(this.textBoxCCI);
            this.Controls.Add(this.textBoxADX);
            this.Controls.Add(this.TextBoxMinTimeInterval);
            this.Controls.Add(this.TextBoxMinPriceMargin);
            this.Controls.Add(this.TextBoxTakeProfit);
            this.Controls.Add(this.TextBoxStopLoss);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.BoxTakeProfit);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.BoxStopLoss);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBoxTimeFrame);
            this.Controls.Add(this.buttonBrowser);
            this.Controls.Add(this.buttonInput);
            this.Controls.Add(this.textBoxInputFile);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonAnalysis);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.buttonManul);
            this.Controls.Add(this.buttonInitial);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Form1";
            this.Text = "Algo Pathfinder";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonInitial;
        private System.Windows.Forms.TextBox textBoxInputFile;
        private System.Windows.Forms.Button buttonInput;
        private System.Windows.Forms.Button buttonBrowser;
        private System.Windows.Forms.Button buttonAnalysis;
        private System.Windows.Forms.ComboBox comboBoxTimeFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label BoxStopLoss;
        private System.Windows.Forms.Label BoxTakeProfit;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox TextBoxStopLoss;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox TextBoxTakeProfit;
        private System.Windows.Forms.TextBox TextBoxMaxHolding;
        private System.Windows.Forms.TextBox TextBoxMinPriceMargin;
        private System.Windows.Forms.TextBox TextBoxMinTimeInterval;
        private System.Windows.Forms.TextBox textBoxStart;
        private System.Windows.Forms.TextBox textBoxStop;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxADX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxCCI;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxMACD;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxRSI;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxTSI;
        private System.Windows.Forms.Button buttonManul;
        private System.Windows.Forms.CheckBox checkBoxShort;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxALI;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxATR;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.TextBox textBoxProfit;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBoxDescription;
        private System.Windows.Forms.Label label15;



    }
}

