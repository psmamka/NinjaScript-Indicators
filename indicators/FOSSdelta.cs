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
	public class FOSSdelta : Indicator
	{
		private DeltaDur deltaDurType;	// duration of the delta: /Session/ or a single /Bar/
		private double sessionCumulDelta = 0;	//  Session Cumulative Delta 
		
		private double	buys = 0;		// Keep track of the buys
		private double	sells = 0;		//						and sells
		private int activeBar = 0;	// Keep track of the bar we are in
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Calculate Delta From Buy/Sell Market Events";
				Name										= "FOSSdelta";
				Calculate									= Calculate.OnEachTick;
				BarsRequiredToPlot							= 0;
				IsOverlay									= false;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= false;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
//				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
//				IsSuspendedWhileInactive					= false;
				AddPlot(new Stroke(Brushes.MediumOrchid, 1), PlotStyle.Hash, "Delta");
			}
//			else if (State == State.Configure)
//			{
//				AddDataSeries(Data.BarsPeriodType.Tick, 1);
//			}
			else if (State == State.Historical)
			{
				if (Calculate != Calculate.OnEachTick)	// calculation needs to be done on each tick
				{
					Draw.TextFixed(this, "NinjaScriptInfo", string.Format(NinjaTrader.Custom.Resource.NinjaScriptOnBarCloseError, Name), TextPosition.BottomRight);
					Log(string.Format(NinjaTrader.Custom.Resource.NinjaScriptOnBarCloseError, Name), LogLevel.Error);
				}
			}
		}
		
		protected override void OnMarketData(MarketDataEventArgs e) // core logic, similar to BuySellVolume
		{
			if(e.MarketDataType == MarketDataType.Last)
			{
				if(e.Price >= e.Ask)
					buys += e.Volume;
				else if (e.Price <= e.Bid)
					sells += e.Volume;
			}
		}

		protected override void OnBarUpdate()
		{	
//			if (CurrentBar < activeBar || CurrentBar <= BarsRequiredToPlot)
//				return;

			// New Bar has been formed
			// - update session cumulative delta (if DeltaDur.Session)
			// - Assign last volume counted to the prior bar
			// - Reset volume count for new bar
			if (CurrentBar != activeBar)									// beginning of a new bar
			{
//				Print("CB: " + CurrentBar + "| AB: " + activeBar);			// debug
				
				Delta[1] = sessionCumulDelta + buys - sells;				// update previous bar
				
				if (deltaDurType == DeltaDur.Session) 
					sessionCumulDelta += buys - sells;						// update session delta
				
				if (Bars.IsFirstBarOfSession) sessionCumulDelta = 0;		// reset session cumulative delta
				
				buys = 0;													// flush buys
				sells = 0;													//           and sells
				activeBar = CurrentBar;										// update active bar
			}

			Delta[0] = sessionCumulDelta + buys - sells;					// update current bar per tick
			
		}
	

		#region Properties
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Delta Duration", GroupName = "Parameters", Order = 0)]
		public DeltaDur DeltaDurType
		{
			get { return deltaDurType; }
			set { deltaDurType = value; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> Delta
		{
			get { return Values[0]; }
		}
		#endregion

	}
}

public enum DeltaDur
{
	Session,
	Bar,
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private MyIndicators.FOSSdelta[] cacheFOSSdelta;
		public MyIndicators.FOSSdelta FOSSdelta(DeltaDur deltaDurType)
		{
			return FOSSdelta(Input, deltaDurType);
		}

		public MyIndicators.FOSSdelta FOSSdelta(ISeries<double> input, DeltaDur deltaDurType)
		{
			if (cacheFOSSdelta != null)
				for (int idx = 0; idx < cacheFOSSdelta.Length; idx++)
					if (cacheFOSSdelta[idx] != null && cacheFOSSdelta[idx].DeltaDurType == deltaDurType && cacheFOSSdelta[idx].EqualsInput(input))
						return cacheFOSSdelta[idx];
			return CacheIndicator<MyIndicators.FOSSdelta>(new MyIndicators.FOSSdelta(){ DeltaDurType = deltaDurType }, input, ref cacheFOSSdelta);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MyIndicators.FOSSdelta FOSSdelta(DeltaDur deltaDurType)
		{
			return indicator.FOSSdelta(Input, deltaDurType);
		}

		public Indicators.MyIndicators.FOSSdelta FOSSdelta(ISeries<double> input , DeltaDur deltaDurType)
		{
			return indicator.FOSSdelta(input, deltaDurType);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MyIndicators.FOSSdelta FOSSdelta(DeltaDur deltaDurType)
		{
			return indicator.FOSSdelta(Input, deltaDurType);
		}

		public Indicators.MyIndicators.FOSSdelta FOSSdelta(ISeries<double> input , DeltaDur deltaDurType)
		{
			return indicator.FOSSdelta(input, deltaDurType);
		}
	}
}

#endregion
