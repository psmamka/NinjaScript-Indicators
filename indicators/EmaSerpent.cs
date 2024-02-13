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
	public class EmaSerpent : Indicator
	{
		private EMA ema1;
		private EMA ema2;
		private int crossingIndex = 0;
		private int areaLabel = 0;
		private Brush areaBrush = Brushes.DeepSkyBlue;
		
		private int firstBar2Draw = 0;
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Two Ema's with Shaded Area in Between";
				Name										= "EmaSerpent";
				Calculate									= Calculate.OnBarClose;
				IsOverlay									= true;
				DisplayInDataBox							= true;
				DrawOnPricePanel							= true;
				DrawHorizontalGridLines						= true;
				DrawVerticalGridLines						= true;
				PaintPriceMarkers							= true;
				ScaleJustification							= NinjaTrader.Gui.Chart.ScaleJustification.Right;
				//Disable this property if your indicator requires custom values that cumulate with each new market data event. 
				//See Help Guide for additional information.
				IsSuspendedWhileInactive					= true;
				BarsRequiredToPlot							= 10;
				
				Period1										= 10;
				Period2										= 20;
				UseMedians									= false; // use medians instead of closes for ema's
				Opacity										= 25;
				AreaBrushAbove								= Brushes.DeepSkyBlue;
				AreaBrushBelow								= Brushes.Orchid;
			}
			else if (State == State.Configure)
			{
				if (UseMedians)
				{
					ema1 = EMA(Median, Period1);
					ema2 = EMA(Median, Period2);
				}
				else
				{
					ema1 = EMA(Period1);
					ema2 = EMA(Period2);
				}
				
				firstBar2Draw = 10;
			}
		}

		protected override void OnBarUpdate()
		{
			if (CurrentBar < firstBar2Draw) { return; }
			
			Draw.Region(owner: this,
						tag: "EMA Snake" + "_" + Period1 + "_" + Period2 + "_" + (areaLabel % 100).ToString(),
						startBarsAgo: CurrentBar - crossingIndex, 
						endBarsAgo: 0, 
						series1: ema1,
						series2: ema2, 
						outlineBrush: Brushes.Transparent, 
						areaBrush: areaBrush, 
						areaOpacity: Opacity);
			
			if (CrossAbove(ema1, ema2, 1) || CrossBelow(ema1, ema2, 1)) 
			{ 
				crossingIndex = CurrentBar;
				areaLabel++;
			}

			areaBrush = (ema1[0] >= ema2[0]) ? AreaBrushAbove : AreaBrushBelow;
		}

		#region Properties
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Above Brush", Description="Area Brush Above", Order=10, GroupName="Parameters")]
		public Brush AreaBrushAbove
		{ get; set; }
		
		[NinjaScriptProperty]
		[XmlIgnore]
		[Display(Name="Below Brush", Description="Area Brush Below", Order=15, GroupName="Parameters")]
		public Brush AreaBrushBelow
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(0, 100)]
		[Display(Name="Opacity", Description="Area Color Opacity", Order=20, GroupName="Parameters")]
		public int Opacity
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Use Medians", Description="Use Medians Switch", Order = 30, GroupName="Parameters")]
		public bool UseMedians
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period1", Description="Period 1", Order=40, GroupName="Parameters")]
		public int Period1
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="Period2", Description="Period 2", Order=42, GroupName="Parameters")]
		public int Period2
		{ get; set; }
		
//		[XmlIgnore]
//		public Brush AreaBrushAbove { get; set; }
		
//		[XmlIgnore]
//		public Brush AreaBrushBelow { get; set; }
		
		[Browsable(false)]
	    public string AreaBrushAboveSerialize
	    {
	      get { return Serialize.BrushToString(AreaBrushAbove); }
	      set { AreaBrushAbove = Serialize.StringToBrush(value); }
	    }
		
		[Browsable(false)]
	    public string AreaBrushBelowSerialize
	    {
	      get { return Serialize.BrushToString(AreaBrushBelow); }
	      set { AreaBrushBelow = Serialize.StringToBrush(value); }
	    }

		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private MyIndicators.EmaSerpent[] cacheEmaSerpent;
		public MyIndicators.EmaSerpent EmaSerpent(Brush areaBrushAbove, Brush areaBrushBelow, int opacity, bool useMedians, int period1, int period2)
		{
			return EmaSerpent(Input, areaBrushAbove, areaBrushBelow, opacity, useMedians, period1, period2);
		}

		public MyIndicators.EmaSerpent EmaSerpent(ISeries<double> input, Brush areaBrushAbove, Brush areaBrushBelow, int opacity, bool useMedians, int period1, int period2)
		{
			if (cacheEmaSerpent != null)
				for (int idx = 0; idx < cacheEmaSerpent.Length; idx++)
					if (cacheEmaSerpent[idx] != null && cacheEmaSerpent[idx].AreaBrushAbove == areaBrushAbove && cacheEmaSerpent[idx].AreaBrushBelow == areaBrushBelow && cacheEmaSerpent[idx].Opacity == opacity && cacheEmaSerpent[idx].UseMedians == useMedians && cacheEmaSerpent[idx].Period1 == period1 && cacheEmaSerpent[idx].Period2 == period2 && cacheEmaSerpent[idx].EqualsInput(input))
						return cacheEmaSerpent[idx];
			return CacheIndicator<MyIndicators.EmaSerpent>(new MyIndicators.EmaSerpent(){ AreaBrushAbove = areaBrushAbove, AreaBrushBelow = areaBrushBelow, Opacity = opacity, UseMedians = useMedians, Period1 = period1, Period2 = period2 }, input, ref cacheEmaSerpent);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MyIndicators.EmaSerpent EmaSerpent(Brush areaBrushAbove, Brush areaBrushBelow, int opacity, bool useMedians, int period1, int period2)
		{
			return indicator.EmaSerpent(Input, areaBrushAbove, areaBrushBelow, opacity, useMedians, period1, period2);
		}

		public Indicators.MyIndicators.EmaSerpent EmaSerpent(ISeries<double> input , Brush areaBrushAbove, Brush areaBrushBelow, int opacity, bool useMedians, int period1, int period2)
		{
			return indicator.EmaSerpent(input, areaBrushAbove, areaBrushBelow, opacity, useMedians, period1, period2);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MyIndicators.EmaSerpent EmaSerpent(Brush areaBrushAbove, Brush areaBrushBelow, int opacity, bool useMedians, int period1, int period2)
		{
			return indicator.EmaSerpent(Input, areaBrushAbove, areaBrushBelow, opacity, useMedians, period1, period2);
		}

		public Indicators.MyIndicators.EmaSerpent EmaSerpent(ISeries<double> input , Brush areaBrushAbove, Brush areaBrushBelow, int opacity, bool useMedians, int period1, int period2)
		{
			return indicator.EmaSerpent(input, areaBrushAbove, areaBrushBelow, opacity, useMedians, period1, period2);
		}
	}
}

#endregion
