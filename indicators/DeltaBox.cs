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
	public class DeltaBox : Indicator
	{
		private OrderFlowCumulativeDelta cumulativeDelta;
		
		private int signChangeIndex = 0;		// index of the last crossing
		private double curSum		= 0.0;		// cummulative price since the last crossing, NOT including the current candle
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Test indicator for drawing Boxes for price changes in the same dir. Bar Close version";
				Name										= "DeltaBox";
				BarsRequiredToPlot							= 1;
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= false;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				
				
				Opacity										= 50;
				
				IsSuspendedWhileInactive					= true;
				
				AddPlot(stroke: new Stroke(brush: Brushes.CornflowerBlue, 
											dashStyleHelper: DashStyleHelper.Solid, 
											width: 10, 
											opacity: Opacity),
						plotStyle: PlotStyle.Bar, 
						name: "PlotAbove");
				
				AddPlot(stroke: new Stroke(brush: Brushes.Orchid, 
											dashStyleHelper: DashStyleHelper.Solid, 
											width: 10, 
											opacity: Opacity), 
						plotStyle: PlotStyle.Bar, 
						name: "PlotBelow");
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Tick, 1);
				
				Plots[0].Opacity = Opacity;
				Plots[1].Opacity = Opacity;
			}
			else if (State == State.DataLoaded)
			{
	      		cumulativeDelta = OrderFlowCumulativeDelta(CumulativeDeltaType.BidAsk, CumulativeDeltaPeriod.Session, 0);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress == 0)
			{
				if (CurrentBar < 1 || Bars.IsFirstBarOfSession)
				{
					curSum = cumulativeDelta.DeltaClose[0] - cumulativeDelta.DeltaOpen[0];
//					return;
				}
				else if (CurrentBar == 1)
				{
					curSum = cumulativeDelta.DeltaClose[0] - cumulativeDelta.DeltaClose[1];
					Values[0][0] = curSum >= 0 ? curSum : 0;
					Values[1][0] = curSum < 0 ? curSum : 0;
				}
				else
				{
					ProcessPreviousBar();
				}
			}
			else if (BarsInProgress == 1)
			{
				cumulativeDelta.Update(cumulativeDelta.BarsArray[1].Count - 1, 1);
			}
		}
		
		private void ProcessPreviousBar()
		{
			double diff = cumulativeDelta.DeltaClose[0] - cumulativeDelta.DeltaClose[1];
			if (SameSign(curSum, diff))	// add the last candle to the existing box
			{
				curSum += diff;
				for (int idx = 0; idx <= CurrentBar - signChangeIndex; idx++)
				{
					Values[0][idx] = curSum >= 0 ? curSum : 0;
					Values[1][idx] = curSum < 0 ? curSum : 0;
				}
			}
			else							// new box established
			{
				signChangeIndex = CurrentBar;
				curSum = diff;
				Values[0][0] = curSum >= 0 ? curSum : 0;
				Values[1][0] = curSum <  0 ? curSum : 0;
			}

		}
		
		private bool SameSign(double a, double b)
		{
			// note the edge case of b == 0 with no sign change
			return (a >= 0 && b >= 0) || (a < 0 && b <= 0);
		}
		
		
		#region Properties
		
		[NinjaScriptProperty]
		[Range(0, 100)]
		[Display(Name="Opacity", Description="Bar Color Opacity", Order=10, GroupName="Parameters")]
		public int Opacity
		{ get; set; }

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PlotAbove
		{
		  get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PlotBelow
		{
		  get { return Values[1]; }
		}
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private MyIndicators.DeltaBox[] cacheDeltaBox;
		public MyIndicators.DeltaBox DeltaBox(int opacity)
		{
			return DeltaBox(Input, opacity);
		}

		public MyIndicators.DeltaBox DeltaBox(ISeries<double> input, int opacity)
		{
			if (cacheDeltaBox != null)
				for (int idx = 0; idx < cacheDeltaBox.Length; idx++)
					if (cacheDeltaBox[idx] != null && cacheDeltaBox[idx].Opacity == opacity && cacheDeltaBox[idx].EqualsInput(input))
						return cacheDeltaBox[idx];
			return CacheIndicator<MyIndicators.DeltaBox>(new MyIndicators.DeltaBox(){ Opacity = opacity }, input, ref cacheDeltaBox);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MyIndicators.DeltaBox DeltaBox(int opacity)
		{
			return indicator.DeltaBox(Input, opacity);
		}

		public Indicators.MyIndicators.DeltaBox DeltaBox(ISeries<double> input , int opacity)
		{
			return indicator.DeltaBox(input, opacity);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MyIndicators.DeltaBox DeltaBox(int opacity)
		{
			return indicator.DeltaBox(Input, opacity);
		}

		public Indicators.MyIndicators.DeltaBox DeltaBox(ISeries<double> input , int opacity)
		{
			return indicator.DeltaBox(input, opacity);
		}
	}
}

#endregion
