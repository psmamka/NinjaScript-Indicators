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
	public class NDayHiLo : Indicator
	{

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Highs and Lows for the past N days and today";
				Name										= "NDayHiLo";
				Calculate									= Calculate.OnPriceChange;
				IsOverlay									= true;
				DisplayInDataBox							= false;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= false;
				DrawVerticalGridLines						= false;
				PaintPriceMarkers							= false;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
												
				Days2Load									= 10;
				
				HiStroke = new Stroke(Brushes.CornflowerBlue, DashStyleHelper.Solid, 1, 35);
				
				LoStroke = new Stroke(Brushes.Orchid, DashStyleHelper.Solid, 1, 35);

			}
			else if (State == State.Configure)
			{
				string instrName = Bars.Instrument.MasterInstrument.Name;
				
				if (Bars.Instrument.MasterInstrument.InstrumentType == InstrumentType.Future)
					instrName += " " + Bars.Instrument.MasterInstrument.GetNextExpiry(DateTime.Now).ToString("MM-yy");
				else if (Bars.Instrument.MasterInstrument.InstrumentType == InstrumentType.Index)
					instrName = "^" + instrName;
				
				AddDataSeries(
					instrumentName: 	instrName, //Bars.Instrument.MasterInstrument.Name + " " + Bars.Instrument.MasterInstrument.GetNextExpiry(DateTime.Now).ToString("MM-yy"),
					barsPeriod: 		new BarsPeriod { BarsPeriodType = BarsPeriodType.Day, Value = 1 },
					barsToLoad: 		Days2Load,				// Past N Days
					tradingHoursName: 	"CME US Index Futures ETH",
					isResetOnNewTradingDay: false
				);
				
				for (int i = Days2Load; i >= 0; i--)		// N previous days plus today
				{
					AddLine(HiStroke, 0, 	"H" + i.ToString());	// H_N, H_N-1, ..., H0
					AddLine(LoStroke, 0, 	"L" + i.ToString());	// L_N, L_N-1, ..., L0
				}
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress == 1)
			{
				// set high and low vals
				Lines[(2 * Days2Load - 2 * CurrentBar) % (2 * Days2Load)].Value = Highs[1][0];
				Lines[(2 * Days2Load - 2 * CurrentBar + 1) % (2 * Days2Load)].Value = Lows[1][0];
			}
		}

		#region Properties
		
		[NinjaScriptProperty]
		[Description("High Stroke")]
		public Stroke HiStroke { get; set; }
		
		[NinjaScriptProperty]
		[Description("Low Stroke")]
		public Stroke LoStroke { get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Num. Days", Description="Number of Days to Look Back", Order=5, GroupName="Parameters")]
		public int Days2Load
		{ get; set; }
		
		#endregion
	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private MyIndicators.NDayHiLo[] cacheNDayHiLo;
		public MyIndicators.NDayHiLo NDayHiLo(Stroke hiStroke, Stroke loStroke, int days2Load)
		{
			return NDayHiLo(Input, hiStroke, loStroke, days2Load);
		}

		public MyIndicators.NDayHiLo NDayHiLo(ISeries<double> input, Stroke hiStroke, Stroke loStroke, int days2Load)
		{
			if (cacheNDayHiLo != null)
				for (int idx = 0; idx < cacheNDayHiLo.Length; idx++)
					if (cacheNDayHiLo[idx] != null && cacheNDayHiLo[idx].HiStroke == hiStroke && cacheNDayHiLo[idx].LoStroke == loStroke && cacheNDayHiLo[idx].Days2Load == days2Load && cacheNDayHiLo[idx].EqualsInput(input))
						return cacheNDayHiLo[idx];
			return CacheIndicator<MyIndicators.NDayHiLo>(new MyIndicators.NDayHiLo(){ HiStroke = hiStroke, LoStroke = loStroke, Days2Load = days2Load }, input, ref cacheNDayHiLo);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MyIndicators.NDayHiLo NDayHiLo(Stroke hiStroke, Stroke loStroke, int days2Load)
		{
			return indicator.NDayHiLo(Input, hiStroke, loStroke, days2Load);
		}

		public Indicators.MyIndicators.NDayHiLo NDayHiLo(ISeries<double> input , Stroke hiStroke, Stroke loStroke, int days2Load)
		{
			return indicator.NDayHiLo(input, hiStroke, loStroke, days2Load);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MyIndicators.NDayHiLo NDayHiLo(Stroke hiStroke, Stroke loStroke, int days2Load)
		{
			return indicator.NDayHiLo(Input, hiStroke, loStroke, days2Load);
		}

		public Indicators.MyIndicators.NDayHiLo NDayHiLo(ISeries<double> input , Stroke hiStroke, Stroke loStroke, int days2Load)
		{
			return indicator.NDayHiLo(input, hiStroke, loStroke, days2Load);
		}
	}
}

#endregion
