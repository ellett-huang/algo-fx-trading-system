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
using System.Windows.Forms.DataVisualization.Charting;
using System.Threading;

namespace AlgoTest
{
    public partial class AlgoMain : Form
    {
        # region "Private Variables"
        private bool _paintControl = false;
        private bool _paintControl2 = false;
        private int offset_height = 0;
        private int offset_width = 0;
        private List<AlgoTradingRule> result=null;
        private List<TradingData> tradingDataSource = new List<TradingData>();
        private StrategyPathFinder theStrategyPathFinder = new StrategyPathFinder();
        System.IO.StreamReader reader = null;
        private delegate void updateUI(int Progress);
        #endregion

        # region "Public Methods"
        public AlgoMain()
        {
            InitializeComponent();
        }
        #endregion

        #region Private Methods

        private void Form1_Load(object sender, EventArgs e)
        {
            txtBoxTimeFrame.Text = "60";
            //dataGridView1.Visible = false;
            TextBoxMinPriceMargin.Text = BaseConstants.MinPriceMargin.ToString();
            TextBoxMinTimeInterval.Text = BaseConstants.MaxTimeInterval.ToString();
            TextBoxMaxHolding.Text = BaseConstants.MaxHoldingInterval.ToString();
            TextBoxStopLoss.Text = (BaseConstants.StopLoss * 100).ToString();
            TextBoxTakeProfit.Text = (BaseConstants.TakeProfit * 100).ToString();
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

                    y = panelInital.Height * ((theStrategyPathFinder.StrategyList[i].SellIndex - theStrategyPathFinder.StrategyList[i].BuyIndex) * 1000 / Int32.Parse(TextBoxMinTimeInterval.Text)) / 1000.0f;
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
                Random rnd = new Random();
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
                    if (columns[1].Length > 3 && Int16.TryParse(columns[1].Substring(0, 4), out test))
                    {
                        DateTime tradeTime = new DateTime(Int16.Parse(columns[1].Substring(0, 4)), Int16.Parse(columns[1].Substring(4, 2)), Int16.Parse(columns[1].Substring(6, 2)), Int16.Parse(columns[1].Substring(8, 2)), Int16.Parse(columns[1].Substring(10, 2)), 0);
                        if (tradingDataSource.Count == 0 || (tradeTime - tradingDataSource.Last<TradingData>().TradeDateTime).TotalMinutes > Int32.Parse(txtBoxTimeFrame.Text.ToString()))
                        {
                            TextureGenerator theTextureGenerator = new TextureGenerator();
                            TradingData theTradingData = new TradingData()
                            {
                                TradeDateTime = tradeTime,
                                Open = Single.Parse(columns[2]),
                                High = Single.Parse(columns[3]),
                                Low = Single.Parse(columns[4]),
                                Close = (float)double.Parse(columns[5]),
                                Volume = Single.Parse(columns[6]),
                                featureTexture = new FeatureTexture()
                            };
                            tradingDataSource.Add(theTradingData);
                            var data = tradingDataSource.FirstOrDefault(c => c.TradeDateTime == theTradingData.TradeDateTime);
                            data.featureTexture = new FeatureTexture(theTextureGenerator.GenerateTexture(tradingDataSource.Count - 1, tradingDataSource));
                        }
                        else
                        {

                            tradingDataSource.Last<TradingData>().High = tradingDataSource.Last<TradingData>().High > Single.Parse(columns[3]) ? tradingDataSource.Last<TradingData>().High : Single.Parse(columns[3]);
                            tradingDataSource.Last<TradingData>().Low = tradingDataSource.Last<TradingData>().Low < Single.Parse(columns[4]) ? tradingDataSource.Last<TradingData>().Low : Single.Parse(columns[4]);
                            tradingDataSource.Last<TradingData>().Close = (float)double.Parse(columns[5]);
                            tradingDataSource.Last<TradingData>().Volume = tradingDataSource.Last<TradingData>().Volume + Single.Parse(columns[6]);

                        }
                    }


                }
                tradingDataSource = (from t in tradingDataSource
                                     orderby t.TradeDateTime
                                     select t).ToList<TradingData>();
                _paintControl2 = true;
                panelTrading.Invalidate();

                //etc
            }
        }

        private void buttonInitial_Click(object sender, EventArgs e)
        {
            _paintControl = true;

            ReloadParameters();
            theStrategyPathFinder.StrategyList.Clear();
            theStrategyPathFinder.CalculatePath(Int16.Parse(txtBoxTimeFrame.Text), tradingDataSource);
            //  dataGridView1.Visible = false;

            panelInital.Refresh();
        }

        private void buttonAnalysis_Click(object sender, EventArgs e)
        { 
            progressBar1.Visible = true;
            labProgress.Visible = true;
            labProgress.Text = "0";
            Thread newThread = new Thread(FinPathProcess);
            newThread.Start();
           
            while (theStrategyPathFinder.percetage < 99)
            {
                progressBar1.Value = (int)theStrategyPathFinder.percetage;
                labProgress.Text = progressBar1.Value.ToString();
                Thread.Sleep(1000);
            }
            dataGridView1.DataSource = result;
            dataGridView1.Visible = true;
            progressBar1.Visible = false;
            labProgress.Visible = false;
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            ReloadParameters();
            dataGridView1.DataSource = (from s in theStrategyPathFinder.BackForwardTest(tradingDataSource, 100)
                                        select new { s.Accuracy, s.PositionType, s.FailedMatch, s.SuccessMatch, s.Score, Profit = s.Profit * Single.Parse(txtLeverage.Text) / tradingDataSource[10].Close, s.Description }).ToList();
            dataGridView1.Visible = true;
        }

        private void Sort_Click(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView1.DataSource = (from s in theStrategyPathFinder.StrategyList
                                        orderby s.Profit descending
                                        select s).ToList<AlgoTradingRule>();
        }


        private void UpdateUIProgress(int Progress,List<AlgoTradingRule> result)
        {
            progressBar1.Value = Progress;
            if (result != null)
            {
                dataGridView1.DataSource = result;
                dataGridView1.Visible = true;
            }
        }

        private void FinPathProcess()
        {
           result= theStrategyPathFinder.CalculateAccuracyStrategyList(tradingDataSource);

        }

        private void buttonManul_Click(object sender, EventArgs e)
        {
            ReloadParameters();
            if (!checkBoxGroup.Checked)
                dataGridView1.DataSource = (from s in theStrategyPathFinder.BackForwardTest(tradingDataSource, 100)
                                            select new
                                            {
                                                s.Accuracy,
                                                s.PositionType,
                                                s.FailedMatch,
                                                s.SuccessMatch,
                                                TotalMaxDrawdown = decimal.Round(100 - ((decimal)tradingDataSource[10].Close + (decimal)s.TotalMaxDrawdown.EndValue * decimal.Parse(txtLeverage.Text)) * 100
                                                / ((decimal)s.TotalMaxDrawdown.StartValue * decimal.Parse(txtLeverage.Text) + (decimal)tradingDataSource[10].Close), 2).ToString() + "%",
                                                MDDStartDate = s.TotalMaxDrawdown.StartDate,
                                                MDDEndDate = s.TotalMaxDrawdown.EndDate,
                                                TotalPeak = decimal.Round((decimal)s.TotalPeak.Value * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",

                                                MaxDrawdown = decimal.Round((decimal)s.MaxDrawdown * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",
                                                Peak = decimal.Round((decimal)s.Peak * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",
                                                Profit = decimal.Round((decimal)s.Profit * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",
                                                s.Description
                                            }).ToList();
            else
            {
                List<AlgoTradingRule> result = theStrategyPathFinder.CalculateAccuracy(tradingDataSource);
                var gridResult = (from s in result
                                  select new
                                  {
                                      s.Accuracy,
                                      s.PositionType,
                                      s.FailedMatch,
                                      s.SuccessMatch,
                                      TotalMaxDrawdown = decimal.Round(100 - ((decimal)tradingDataSource[10].Close + (decimal)s.TotalMaxDrawdown.EndValue * decimal.Parse(txtLeverage.Text)) * 100
                                     / ((decimal)s.TotalMaxDrawdown.StartValue * decimal.Parse(txtLeverage.Text) + (decimal)tradingDataSource[10].Close), 2).ToString() + "%",
                                      MDDStartDate = s.TotalMaxDrawdown.StartDate,
                                      MDDEndDate = s.TotalMaxDrawdown.EndDate,
                                      TotalPeak = decimal.Round((decimal)s.TotalPeak.Value * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",
                                      PeakDate = s.TotalPeak.Date,
                                      MaxDrawdown = decimal.Round((decimal)s.MaxDrawdown * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",
                                      Peak = decimal.Round((decimal)s.Peak * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",
                                      Profit = decimal.Round((decimal)s.Profit * decimal.Parse(txtLeverage.Text) * 100 / (decimal)tradingDataSource[10].Close, 2).ToString() + "%",
                                      s.Description
                                  });
                dataGridView1.DataSource = gridResult.ToList();
                LoadChartPerformance(result);
            }
            dataGridView1.Visible = true;
            dataGridView1.Refresh();

            var profit = from S in theStrategyPathFinder.StrategyList
                         group S by S.Profit into p
                         select p.Sum(S => S.Profit);
            dataGridView1.Columns["TotalMaxDrawdown"].DisplayIndex = 4;
            dataGridView1.Columns["TotalPeak"].DisplayIndex = 8;
            dataGridView1.Columns["Description"].DisplayIndex = 10;


        }

        private void LoadChartPerformance(List<AlgoTradingRule> result)
        {
            List<float> totalProfit = new List<float>();
            chartPerformance.Series.Clear();
            chartPerformance.Series.Add("Total Profit");
            chartPerformance.Series["Total Profit"].ChartType = SeriesChartType.Line;
            chartPerformance.Series["Total Profit"].Color = Color.Red;
            result = result.OrderBy(x => x.PerformanceList.Count).ToList();
            foreach (AlgoTradingRule rule in result)
            {
                chartPerformance.Series.Add(rule.PositionType.ToString());
                chartPerformance.Series[rule.PositionType.ToString()].ChartType = SeriesChartType.Line;
                for (int pointIndex = 0; pointIndex < rule.PerformanceList.Count; pointIndex++)
                {
                    float currentProfit = rule.PerformanceList[pointIndex] * float.Parse(txtLeverage.Text) * 100 / tradingDataSource[10].Close;

                    if (totalProfit.Count <= pointIndex)
                    {
                        if (totalProfit.Count > 0)
                        {
                            float lastProfit = rule.PerformanceList[pointIndex - 1] * float.Parse(txtLeverage.Text) * 100 / tradingDataSource[10].Close;
                            if (totalProfit[pointIndex - 1] != lastProfit)
                                totalProfit.Add(totalProfit[pointIndex - 1] - lastProfit + currentProfit);
                            else
                                totalProfit.Add(currentProfit);
                        }
                        else
                            totalProfit.Add(currentProfit);
                    }
                    else
                        totalProfit[pointIndex] += currentProfit;
                    chartPerformance.Series[rule.PositionType.ToString()].Points.AddY(currentProfit);

                }
            }
            for (int pointIndex = 0; pointIndex < result[result.Count - 1].PerformanceList.Count; pointIndex++)
            {
                chartPerformance.Series["Total Profit"].Points.AddY(totalProfit[pointIndex]);
            }

        }
        private void ReloadParameters()
        {
            BaseConstants.MinPriceMargin = Single.Parse(TextBoxMinPriceMargin.Text);
            BaseConstants.MaxTimeInterval = Int16.Parse(TextBoxMinTimeInterval.Text);
            BaseConstants.MaxHoldingInterval = Int16.Parse(TextBoxMaxHolding.Text);
            BaseConstants.StopLoss = Single.Parse(TextBoxStopLoss.Text) / 100f;
            BaseConstants.TakeProfit = Single.Parse(TextBoxTakeProfit.Text) / 100f;
        }

        private void On_Paint_Chart(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = panelTrading.CreateGraphics();
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
                    float scollHeight = panelTrading.Height;
                    panelTrading.AutoScrollMinSize = new Size(tradingDataSource.Count * 15 / 10, (int)(scollHeight - panelTrading.Height));


                    for (int i = offset_width + 1; i < tradingDataSource.Count; i++)
                    {
                        if (i > offset_width + panelTrading.Width / 2)
                            break;
                        theStrategyPathFinder.StrategyEntries.ForEach(s =>
                        {
                            if (s.BuyIndex < tradingDataSource.Count)
                            {
                                int Y = (int)(panelTrading.Height - ((tradingDataSource[s.BuyIndex].Close - buttom[0]) / range) * 0.8 * panelTrading.Height) - 20 + offset_height;
                                g.DrawRectangle(RedPen, (s.BuyIndex - offset_width) * 2, Y, width, height);
                                PointF drawPoint = new PointF((s.BuyIndex - offset_width) * 2F - 10, Y * 1.0F - 20);
                                if (s.IsLong)
                                    g.DrawString("Buy", new Font("Arial", 8), new SolidBrush(Color.Red), drawPoint);
                                else
                                    g.DrawString("Short", new Font("Arial", 8), new SolidBrush(Color.Red), drawPoint);
                                Y = (int)(panelTrading.Height - ((tradingDataSource[s.SellIndex].Close - buttom[0]) / range) * 0.8 * panelTrading.Height) - 20 + offset_height;
                                g.DrawRectangle(BluePen, (s.SellIndex - offset_width) * 2, Y, width, height);
                                drawPoint = new PointF((s.SellIndex - offset_width) * 2F, Y * 1.0F);
                                //if (s.IsLong)
                                //    g.DrawString("Sell", new Font("Arial", 8), new SolidBrush(Color.Black), drawPoint);
                                //else
                                //    g.DrawString("Cover", new Font("Arial", 8), new SolidBrush(Color.Black), drawPoint);
                            }
                        });
                        Point from = new Point();
                        from.X = (i - offset_width - 1) * 2;
                        from.Y = (int)(panelTrading.Height - ((tradingDataSource[i - 1].Close - buttom[0]) / range) * 0.8 * panelTrading.Height) - 20 + offset_height;
                        Point to = new Point();
                        to.X = (i - offset_width) * 2;
                        to.Y = (int)(panelTrading.Height - ((tradingDataSource[i].Close - buttom[0]) / range) * 0.8 * panelTrading.Height) - 20 + offset_height;
                        g.DrawLine(blackPen, from, to);
                        //  g.DrawRectangle(RedPen, x, y, width, height);
                    }
                    // _paintControl2 = false;
                }
                blackPen.Dispose();
                BluePen.Dispose();
                RedPen.Dispose();
                g.Dispose();

            }
            catch (Exception er)
            {
                throw;
            }
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.HorizontalScroll)
                offset_width = e.NewValue;
            else
                offset_height = panelTrading.Height * 2 - e.NewValue;
            _paintControl2 = true;
            panelTrading.Invalidate();
        }

        private void Grid_DBClick(object sender, EventArgs e)
        {
            if (checkBoxGroup.Checked)
            {
                //  string description = theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex].Description;
                theStrategyPathFinder.CalculateAccuracy(tradingDataSource);
                //  theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex].Description = description;
            }
            else
            {
                string description = theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex].Description;
                theStrategyPathFinder.CalculateAccuracy(theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex], tradingDataSource);
                theStrategyPathFinder.StrategyList[dataGridView1.SelectedCells[0].RowIndex].Description = description;
            }
            _paintControl2 = true;
            panelTrading.Invalidate();
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
                theRule.BuyfeatureTexture.RSI.Weight = 0;

            if (textBoxSellADX.Text != "")
            {
                theRule.SellfeatureTexture.ADX.FeatureValue = Single.Parse(textBoxSellADX.Text);
                theRule.SellfeatureTexture.ADX.Weight = 1;
            }
            else
                theRule.SellfeatureTexture.ADX.Weight = 0;
            if (textBoxSellCCI.Text != "")
            {
                theRule.SellfeatureTexture.CCI.FeatureValue = Single.Parse(textBoxSellCCI.Text);
                theRule.SellfeatureTexture.CCI.Weight = 1;
            }
            else
                theRule.SellfeatureTexture.CCI.Weight = 0;
            if (textBoxSellALI.Text != "")
            {
                theRule.SellfeatureTexture.ALI.FeatureValue = Single.Parse(textBoxSellALI.Text);
                theRule.SellfeatureTexture.ALI.Weight = 1;
            }
            else
                theRule.SellfeatureTexture.ALI.Weight = 0;
            if (textBoxSellATR.Text != "")
            {
                theRule.SellfeatureTexture.ATR.FeatureValue = Single.Parse(textBoxSellATR.Text);
                theRule.SellfeatureTexture.ATR.Weight = 1;
            }
            else
                theRule.SellfeatureTexture.ATR.Weight = 0;
            if (textBoxSellTSI.Text != "")
            {
                theRule.SellfeatureTexture.TSI.FeatureValue = Single.Parse(textBoxSellTSI.Text);
                theRule.SellfeatureTexture.TSI.Weight = 1;
            }
            else
                theRule.SellfeatureTexture.TSI.Weight = 0;
            if (textBoxSellMACD.Text != "")
            {
                theRule.SellfeatureTexture.MACD.Add(new Feature());
                theRule.SellfeatureTexture.MACD[0].FeatureValue = Single.Parse(textBoxSellMACD.Text);
                theRule.SellfeatureTexture.MACD[0].Weight = 1;
            }
            else
            {
                theRule.SellfeatureTexture.MACD.Add(new Feature());
                theRule.SellfeatureTexture.MACD[0].Weight = 0;
            }
            if (textBoxSellRSI.Text != "")
            {
                theRule.SellfeatureTexture.RSI.FeatureValue = Single.Parse(textBoxSellRSI.Text);
                theRule.SellfeatureTexture.RSI.Weight = 1;

            }
            else
                theRule.SellfeatureTexture.RSI.Weight = 0;


            theRule.PositionType = checkBoxShort.Checked == true ? PositionType.Short : PositionType.Long;
            //      if (textBoxProfit.Text == "")
            //           theStrategyPathFinder.StrategyList.Clear();
            theStrategyPathFinder.StrategyList.Add(theRule);
            dataGridView1.DataSource = (from s in theStrategyPathFinder.StrategyList
                                        select new { s.Accuracy, s.PositionType, s.FailedMatch, s.SuccessMatch, s.Score, s.Profit, s.Description }).ToList();
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
            string constr = "Data Source=ALGOBLADE;Initial Catalog=AlgoTrade;Persist Security Info=True;Integrated Security=SSPI;";
            string ssql = "Insert into AlgoRules ([Symbol],[BuyFeatureTexture]           ,[PositionTypeValue]           ,[PositionTypeName]           ,[ActionTypeValue]           ,[ActionTypeName]           ,[Accuracy]           ,[IsEnable]           ,[Token]           ,[SellFeatureTexture],description) values ("
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].Symbol + "',"
                + "'" + Serialize<FeatureTexture>(theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].BuyfeatureTexture) + "',"
                + "1,"
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].PositionType.ToString() + "',"
                + "'','',"
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].Accuracy + "',1,"
                + "'" + theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].RuleID + "',"
                + "'" + Serialize<FeatureTexture>(theStrategyPathFinder.StrategyList[dataGridView1.CurrentRow.Index].SellfeatureTexture) + "',"
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

        private void buttonRemove_Click(object sender, EventArgs e)
        {
            theStrategyPathFinder.StrategyList.RemoveAt(dataGridView1.CurrentRow.Index);
            dataGridView1.DataSource = (from s in theStrategyPathFinder.StrategyList
                                        select new { s.Accuracy, s.PositionType, s.FailedMatch, s.SuccessMatch, s.Score, s.Profit, s.Description }).ToList();
            dataGridView1.Visible = true;
            dataGridView1.Refresh();
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
        #endregion
        
        #region static method
        public static XElement Serialize<T>(T value)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            XElement xl;
            XDocument doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                xmlSerializer.Serialize(writer, value);
            }
            xl = doc.Root;
            return xl;
        }
       

       
        #endregion

    }
}
