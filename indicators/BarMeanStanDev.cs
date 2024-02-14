#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Indicators in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Indicators.MyIndicators
{
	public class BarMeanStanDev : Indicator
	{
		private double priceVWSum = 0;			// Price Volume-Weighted Sum
		private double priceSqVWSum = 0;		// Price^2 Volume-Weighted Sum
		private double volSum = 0;				// Volume Sum
		
		private double curMean = 0;				// current mean
		private double curVar = 0;				// current variance
		
		private VOL littleVol;					// vol for little bars
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Show the Mean and +/- K * Stn.Dev of points in the Candle. Uses 10-tick data to do computations.";
				Name										= "BarMeanStanDev";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				IsAutoScale 								= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				BarsRequiredToPlot 							= 1;
				
				K											= 2.0;
				MeanStroke									= new Stroke(Brushes.Gold, DashStyleHelper.Solid, 1, 100);
				SideStroke									= new Stroke(Brushes.Gold, DashStyleHelper.Solid, 1, 100);
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Second, 1);		// <==== BarsArray[1]: 1-sec updates
				
				AddPlot(MeanStroke, PlotStyle.Block, "MeanPlot");
				AddPlot(SideStroke, PlotStyle.Hash, "UpperPlot");
				AddPlot(SideStroke, PlotStyle.Hash, "LowerPlot");
			}
			else if (State == State.DataLoaded)
			{
				littleVol = VOL(BarsArray[1]);
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < 1) return;
			
			if (BarsInProgress == 0)
			{
				Values[0][0] = curMean;
				
				double curStDev = Math.Sqrt(curVar);
				Values[1][0] = curMean + K * curStDev;
				Values[2][0] = curMean - K * curStDev;
				
				ResetPartialVals();
				
			}
			else if (BarsInProgress == 1)
			{
				UpdatePartialVals();
			}
		}
		
		private void ResetPartialVals()
		{
			priceVWSum = 0;
			priceSqVWSum = 0;
			volSum = 0;
			
			curMean = 0;
			curVar = 0;
		}
		
		private void UpdatePartialVals()
		{
			priceVWSum += Closes[1][0] * littleVol[0];
			priceSqVWSum += Closes[1][0] * Closes[1][0] * littleVol[0];
			volSum += littleVol[0];
			
			// calculate mean, stdev so far. But how to display them? Values[x][0] would change previous bar
			if (volSum != 0) 	
			{
				curMean = priceVWSum / volSum;
				curVar = priceSqVWSum / volSum - curMean * curMean;		// biased variance est: Σ((x-μ)^2 * v) / Σ(v)
				
				double curStDev = Math.Sqrt(curVar);
				double upper = curMean + K * curStDev;
				double lower = curMean - K * curStDev;
			}
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(0.0001, double.MaxValue)]
		[Display(Name="K", Description="Show: Mean +/- K * Stn.Dev", Order=1, GroupName="Parameters")]
		public double K
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name = "Mean Brush Stroke", GroupName = "Parameters", Order = 5)]
		public Stroke MeanStroke { get; set; }

		[NinjaScriptProperty]
		[Display(Name = "StD Brush Stroke", GroupName = "Parameters", Order = 10)]
		public Stroke SideStroke { get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> MeanPlot
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> UpperPlot
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> LowerPlot
		{
			get { return Values[2]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private MyIndicators.BarMeanStanDev[] cacheBarMeanStanDev;
		public MyIndicators.BarMeanStanDev BarMeanStanDev(double k, Stroke meanStroke, Stroke sideStroke)
		{
			return BarMeanStanDev(Input, k, meanStroke, sideStroke);
		}

		public MyIndicators.BarMeanStanDev BarMeanStanDev(ISeries<double> input, double k, Stroke meanStroke, Stroke sideStroke)
		{
			if (cacheBarMeanStanDev != null)
				for (int idx = 0; idx < cacheBarMeanStanDev.Length; idx++)
					if (cacheBarMeanStanDev[idx] != null && cacheBarMeanStanDev[idx].K == k && cacheBarMeanStanDev[idx].MeanStroke == meanStroke && cacheBarMeanStanDev[idx].SideStroke == sideStroke && cacheBarMeanStanDev[idx].EqualsInput(input))
						return cacheBarMeanStanDev[idx];
			return CacheIndicator<MyIndicators.BarMeanStanDev>(new MyIndicators.BarMeanStanDev(){ K = k, MeanStroke = meanStroke, SideStroke = sideStroke }, input, ref cacheBarMeanStanDev);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MyIndicators.BarMeanStanDev BarMeanStanDev(double k, Stroke meanStroke, Stroke sideStroke)
		{
			return indicator.BarMeanStanDev(Input, k, meanStroke, sideStroke);
		}

		public Indicators.MyIndicators.BarMeanStanDev BarMeanStanDev(ISeries<double> input , double k, Stroke meanStroke, Stroke sideStroke)
		{
			return indicator.BarMeanStanDev(input, k, meanStroke, sideStroke);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MyIndicators.BarMeanStanDev BarMeanStanDev(double k, Stroke meanStroke, Stroke sideStroke)
		{
			return indicator.BarMeanStanDev(Input, k, meanStroke, sideStroke);
		}

		public Indicators.MyIndicators.BarMeanStanDev BarMeanStanDev(ISeries<double> input , double k, Stroke meanStroke, Stroke sideStroke)
		{
			return indicator.BarMeanStanDev(input, k, meanStroke, sideStroke);
		}
	}
}

#endregion
