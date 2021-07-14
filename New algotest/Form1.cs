using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AlgoTradeLib.Algo;
using AlgoTrade.Common.Entities;
using AlgoTrade.Common.Constants;
using System.Data.SqlClient;
using System.Xml.Serialization;
using System.Xml.Linq;

namespace AlgoTest
{
    public partial class Form1 : Form
    {
        private bool _paintControl = false;
        private bool _paintControl2 = false;
        private int offset_height = 0;
        private int offset_width = 0;
        private List<TradingData> tradingDataSource = new List<TradingData>();
        private StrategyPathFinder theStrategyPathFinder = new StrategyPathFinder();
        System.IO.StreamReader reader = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBoxTimeFrame.SelectedIndex=0;
            //dataGridView1.Visible = false;
            TextBoxMinPriceMargin.Text = BaseConstants.MinPriceMargin.ToString();
            TextBoxMinTimeInterval.Text = BaseConstants.MaxTimeInterval.ToString();
            TextBoxMaxHolding.Text = BaseConstants.MaxHoldingInterval.ToString();
            TextBoxStopLoss.Text = BaseConstants.StopLoss.ToString();
            TextBoxTakeProfit.Text = BaseConstants.TakeProfit.ToString();
        }

        private void OnPaint(object sender, PaintEventArgs e)
        {
            Pen blackPen = new Pen(Color.Black, 1);

            // Create location and size of rectangle.
            float x = 7.0F;
            float y = 11.0F;
            float width = 1.0F;
            float height = 1.0F;
            //for (int i = 0; i < 600; i++)
            //{

            //    //y =  (theStrategyPathFinder.CandidateList[i*10].Score);
            //    //x = i/2;
            //    y = i/2;
            //    x = i;
            //    e.Graphics.DrawRectangle(blackPen, x, y, width, height);
            //}
            if (_paintControl)
            { // Create pen.

                int j = 0;
                Random R = new Random((int)DateTime.Now.Millisecond);

                for (int i = 0; i < theStrategyPathFinder.StrategyList.Count; i++)
                {

                    y = panel1.Height*((theStrategyPathFinder.StrategyList[i].SellIndex - theStrategyPathFinder.StrategyList[i].BuyIndex)*1000/Int32.Parse(TextBoxMinTimeInterval.Text))/1000.0f ;
                    if (j > 600)
                        j = 0;
                    else
                        j++;
                    x = j;
                    e.Graphics.DrawRectangle(blackPen, x, y, width, height);
                }
                //_paintControl = false;
            }
        }

        private void buttonBrowser_Click(object sender, EventArgs e)
        {
            var FD = new System.Windows.Forms.OpenFileDialog();
            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileToOpen = FD.FileName;
                textBoxInputFile.Text = fileToOpen;
                System.IO.FileInfo File = new System.IO.FileInfo(FD.FileName);

                //OR

                reader = new System.IO.StreamReader(fileToOpen);
                string line = string.Empty;
                string[] columns;
                short test = 0;
                tradingDataSource.Clear();
                int rowCount = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    //<ticker>,<date>,<open>,<high>,<low>,<close>,<vol>
                    if (rowCount < Int32.Parse(textBoxStart.Text) || rowCount > Int32.Parse(textBoxStop.Text))
                    {
                        rowCount++;
                        continue;
                    }
                    else
                        rowCount++;
                    columns = line.Split(',');

                    string[] hours;
                    hours = columns[1].ToString().Split(':');
                    if (hours.Length > 1 && (columns[0][4] == '/' || columns[0][4] == '.'))
                    {
                        if (hours[0].Length == 1)
                            hours[0] = "0" + hours[0];
                        if (hours[1].Length == 1)
                            hours[1] = "0" + hours[1];
                        columns[1] = columns[0].Substring(0, 4) + columns[0].Substring(5, 2) + columns[0].Substring(8, 2) + hours[0] + hours[1];
                    }
                    if (hours.Length > 1 && (columns[0][2] == '/' || columns[0][2] == '.'))
                    {
                        if (hours[0].Length == 1)
                            hours[0] = "0" + hours[0];
                        if (hours[1].Length == 1)
                            hours[1] = "0" + hours[1];
                        columns[1] = columns[0].Substring(6, 4) + columns[0].Substring(3, 2) + columns[0].Substring(0, 2) + hours[0] + hours[1];
                    }
                    if (columns[1].Length>3 && Int16.TryParse(columns[1].Substring(0, 4), out test))
                    {
                        TextureGenerator theTextureGenerator = new TextureGenerator();
                        DateTime tradeTime = new DateTime(Int16.Parse(columns[1].Substring(0, 4)), Int16.Parse(columns[1].Substring(4, 2)), Int16.Parse(columns[1].Substring(6, 2)), Int16.Parse(columns[1].Substring(8, 2)), Int16.Parse(columns[1].Substring(10, 2)), 0);
                        TradingData theTradingData = new TradingData()
                        {
                            TradeDateTime = tradeTime,
                            Open = Single.Parse(columns[2]),
                            High = Single.Parse(columns[3]),
                            Low = Single.Parse(columns[4]),
                            Close = Single.Parse(columns[5]),
                            Volume = Single.Parse(columns[6]),
                            featureTexture = new FeatureTexture()
                        };
                        tradingDataSource.Add(theTradingData);
                        var data = tradingDataSource.FirstOrDefault(c => c.TradeDateTime == theTradingData.TradeDateTime);
                        data.featureTexture = new FeatureTexture(theTextureGenerator.GenerateTexture(tradingDataSource.Count - 1, tradingDataSource));
                    }


                }
                tradingDataSource = (from t in tradingDataSource
                                     orderby t.TradeDateTime
                                     select t).ToList<TradingData>();
                _paintControl2 = true;
                panel2.Invalidate();
                
                //etc
            }
        }

        private void buttonInitial_Click(object sender, EventArgs e)
        {
            _paintControl = true;

            ReloadParameters();
            theStrategyPathFinder.StrategyList.Clear();
            theStrategyPathFinder.CalculatePath(Int16.Parse(comboBoxTimeFrame.Text), tradingDataSource);
          //  dataGridView1.Visible = false;
            
            panel1.Refresh();
        }

        private void buttonAnalysis_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = theStrategyPathFinder.CalculateAccuracyStrategyList(tradingDataSource);
            dataGridView1.Visible = true;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            ReloadParameters();
            dataGridView1.DataSource = (from s in theStrategyPathFinder.BackForwardTest(tradingDataSource, 100) 
                                     select new {s.Accuracy,s.PositionType,s.FailedMatch,s.SuccessMatch,s.Score,s.Profit,s.Description }).ToList();
            dataGridView1.Visible = true;
        }

        private void Sort_Click(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.DataSource = (from s in theStrategyPathFinder.StrategyList
                                        orderby s.Profit descending
                                        select s).ToList < AlgoTradingRule>();
        }

        private void buttonManul_Click(object sender, EventArgs e)
        {           
            ReloadParameters();
            dataGridView1.DataSource = theStrategyPathFinder.BackForwardTest(tradingDataSource, 100);
            dataGridView1.Visible = true;
            var profit = from S in theStrategyPathFinder.StrategyList
                         group S by S.Profit into p
                         select p.Sum(S=>S.Profit);
            textBoxProfit.Text = profit.ToList<Single>()[0].ToString();

        }

        private void ReloadParameters()
        {
            BaseConstants.MinPriceMargin = Single.Parse(TextBoxMinPriceMargin.Text);
            BaseConstants.MaxTimeInterval = Int16.Parse(TextBoxMinTimeInterval.Text);
            BaseConstants.MaxHoldingInterval = Int16.Parse(TextBoxMaxHolding.Text);
            BaseConstants.StopLoss = Single.Parse(TextBoxStopLoss.Text);
            BaseConstants.TakeProfit = Single.Parse(TextBoxTakeProfit.Text);
        }

        private void On_Paint_Chart(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = panel2.CreateGraphics();
                Pen blackPen = new Pen(Color.Black, 1);
                Pen BluePen = new Pen(Color.Blue, 3);
                Pen RedPen = new Pen(Color.Red, 3);
                // Create location and size of rectangle.
                float x = 7.0F;
                float y = 11.0F;
                float width = 2.0F;
                float height = 2.0F;

                if (_paintControl2)
                { // Create pen.
                    var top = (from t in tradingDataSource
                               orderby t.Close descending
                               select t.Close).Take(1).ToArray<float>();
                    var buttom = (from t in tradingDataSource
                                  orderby t.Close
                                  select t.Close).Take(1).ToArray<float>();
                    float range = top[0] - buttom[0];
                    float scollHeight = panel2.Height * 4;
                    panel2.AutoScrollMinSize = new Size(tradingDataSource.Count * 15 / 10, (int)(scollHeight - panel2.Height));


                    for (int i = offset_width + 1; i < tradingDataSource.Count; i++)
                    {
                        if (i > offset_width + panel2.Width / 2)
                            break;
                        theStrategyPathFinder.StrategyEntries.ForEach(s =>
                        {
                            if (s.BuyIndex < tradingDataSource.Count)
                            {
                                int Y = (int)(panel2.Height - ((tradingDataSource[s.BuyIndex].Close - buttom[0]) / range) * 2 * panel2.Height) + offset_height;
                                g.DrawRectangle(RedPen, (s.BuyIndex - offset_width) * 2, Y, width, height);
                                Y = (int)(panel2.Height - ((tradingDataSource[s.SellIndex].Close - buttom[0]) / range) * 2 * panel2.Height) + offset_height;
                                g.DrawRectangle(BluePen, (s.SellIndex - offset_width) * 2, Y, width, height);
                            }
                        });
                        Point from = new Point();
                        from.X = (i - offset_width - 1) * 2;
                        from.Y = (int)(panel2.Height - ((tradingDataSource[i - 1].Close - buttom[0]) / range) * 2 * panel2.Height) + offset_height;
                        Point to = new Point();
                        to.X = (i - offset_width) * 2;
                        to.Y = (int)(panel2.Height - ((tradingDataSource[i].Close - buttom[0]) / range) * 2 * panel2.Height) + offset_height;
                        g.DrawLine(blackPen, from, to);
                        //  g.DrawRectangle(RedPen, x, y, width, height);
                    }
                    // _paintControl2 = false;
                }
                blackPen.Dispose();
                g.Dispose();
            }
            catch(Exception er)
            {
                throw;
            }
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                offset_width = e.NewValue;
            else
                offset_height = panel2.Height*2-e.NewValue;
            _paintControl2 = true;
            panel2.Invalidate();
        }

        private void Grid_DBClick(object sender, EventArgs e)
        {
            string description = theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex].Description;
            theStrategyPathFinder.CalculateAccuracy(theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex], tradingDataSource);
            theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex].Description = description;
            _paintControl2 = true;
            panel2.Invalidate();
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            AlgoTradingRule theRule = new AlgoTradingRule();
            if (textBoxADX.Text != "")
            {
                theRule.BuyfeatureTexture.ADX.FeatureValue = Single.Parse(textBoxADX.Text);
                theRule.BuyfeatureTexture.ADX.Weight = 1;
            }
            else
                theRule.BuyfeatureTexture.ADX.Weight = 0;
            if (textBoxCCI.Text != "")
            {
                theRule.BuyfeatureTexture.CCI.FeatureValue = Single.Parse(textBoxCCI.Text);
                theRule.BuyfeatureTexture.CCI.Weight = 1;
            }
            else
                theRule.BuyfeatureTexture.CCI.Weight = 0;
            if (textBoxALI.Text != "")
            {
                theRule.BuyfeatureTexture.ALI.FeatureValue = Single.Parse(textBoxALI.Text);
                theRule.BuyfeatureTexture.ALI.Weight = 1;
            }
            else
                theRule.BuyfeatureTexture.ALI.Weight = 0;
            if (textBoxATR.Text != "")
            {
                theRule.BuyfeatureTexture.ATR.FeatureValue = Single.Parse(textBoxATR.Text);
                theRule.BuyfeatureTexture.ATR.Weight = 1;
            }
            else
                theRule.BuyfeatureTexture.ATR.Weight = 0;
            if (textBoxTSI.Text != "")
            {
                theRule.BuyfeatureTexture.TSI.FeatureValue = Single.Parse(textBoxTSI.Text);
                theRule.BuyfeatureTexture.TSI.Weight = 1;
            }
            else
                theRule.BuyfeatureTexture.TSI.Weight = 0;
            if (textBoxMACD.Text != "")
            {
                theRule.BuyfeatureTexture.MACD.Add(new Feature());
                theRule.BuyfeatureTexture.MACD[0].FeatureValue = Single.Parse(textBoxMACD.Text);
                theRule.BuyfeatureTexture.MACD[0].Weight = 1;
            }
            else
            {
                theRule.BuyfeatureTexture.MACD.Add(new Feature());
                theRule.BuyfeatureTexture.MACD[0].Weight = 0;
            }
            if (textBoxRSI.Text != "")
            {
                theRule.BuyfeatureTexture.RSI.FeatureValue = Single.Parse(textBoxRSI.Text);
                theRule.BuyfeatureTexture.RSI.Weight = 1;
            }
            else
            {
                theRule.BuyfeatureTexture.RSI.Weight = 0;
            }
           
            theRule.SellfeatureTexture = theRule.BuyfeatureTexture;
            theRule.PositionType = checkBoxShort.Checked == true ? PositionType.Short : PositionType.Long;
            if(textBoxProfit.Text=="")
                theStrategyPathFinder.StrategyList.Clear();
            theStrategyPathFinder.StrategyList.Add(theRule);
            dataGridView1.DataSource = (from s in theStrategyPathFinder.StrategyList
                                       select new {s.Accuracy,s.PositionType,s.FailedMatch,s.SuccessMatch,s.Score,s.Profit,s.Description }).ToList();
            dataGridView1.Visible = true;
            dataGridView1.Refresh();
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            buttonBrowser.PerformClick();
            buttonInitial.PerformClick();
            buttonAnalysis.PerformClick();
        }

        private void buttonAddtoDB_Click(object sender, EventArgs e)
        {
            string constr = "Data Source=TALETT-PC\\SQLEXPRESS;Initial Catalog=AlgoTrade;Persist Security Info=True;Integrated Security=SSPI;";
            string ssql = "Insert into AlgoRules ([Symbol],[BuyFeatureTexture]           ,[PositionTypeValue]           ,[PositionTypeName]           ,[ActionTypeValue]           ,[ActionTypeName]           ,[Accuracy]           ,[IsEnable]           ,[Token]           ,[SellFeatureTexture],description) values ("
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].Symbol + "',"
                + "'" + Serialize<FeatureTexture>(theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index ].BuyfeatureTexture) + "',"                
                + "1,"
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].PositionType.ToString() + "'," 
                + "'','',"
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index ].Accuracy + "',1,"
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index ].RuleID + "',"
                + "'" + Serialize<FeatureTexture>(theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index ].SellfeatureTexture) + "',"
                 + "'" + textBoxDescription.Text + "'"
                + ")";
            System.Data.SqlClient.SqlConnection conn = new SqlConnection(constr);

            try
            {
                conn.Open();
                System.Data.SqlClient.SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = ssql;
                System.Data.SqlClient.SqlDataAdapter Da = new SqlDataAdapter(cmd);
                System.Data.DataTable dt = new System.Data.DataTable();
                Da.Fill(dt);

            }
            catch (Exception ex)
            {

                //PNLuser.Visible = true;
            }
            finally
            {
                conn.Close();
            }
        }




        public static XElement Serialize<T>(T value)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XElement xl ;
            XDocument doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                xmlSerializer.Serialize(writer, value);
            }
            xl = doc.Root;
            return xl;
        }

       


    }
}
