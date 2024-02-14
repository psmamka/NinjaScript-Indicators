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
	public class VWAP5Day : Indicator
	{
		List<double>	cumVolumeLst		= new List<double> {0, 0, 0, 0, 0};		// cumulative vol for n days
		List<double>	cumTypicalVolumeLst	= new List<double> {0, 0, 0, 0, 0};		// same as ↑ for typical vol
																		// typical vol is vol * typical price
																		// see first 2 lines under OnBarUpdate()
		
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"VWAPs for days 1, 2, 3, 4, and 5";
				Name										= "VWAP5Day";
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
				
				ShowLabelsSw								= false;
				TransparentFirstBoD							= true;		// to avoid unseemly lines in session transitions
				
				AddPlot(Brushes.DarkRed, 	"PlotDay5");
				AddPlot(Brushes.Red, 		"PlotDay4");
				AddPlot(Brushes.OrangeRed, 	"PlotDay3");
				AddPlot(Brushes.Orange, 	"PlotDay2");
				AddPlot(Brushes.Yellow, 	"PlotDay1");
			}
			else if (State == State.Configure)
			{
			}
		}

		protected override void OnBarUpdate()
		{
			// Increments
			double VolInc 			= VOL()[0];
			double TypicalVolInc 	= VOL()[0] * ((High[0] + Low[0] + Close[0]) / 3);
			
			if (Bars.IsFirstBarOfSession)
			{	
				// Rotations in the beginning of the session
				for (int idx = cumVolumeLst.Count - 1; idx > 0; idx--)
				{
					cumVolumeLst[idx] 			= cumVolumeLst[idx - 1] 		+ VolInc;
					cumTypicalVolumeLst[idx] 	= cumTypicalVolumeLst[idx - 1]  + TypicalVolInc;
				}
				cumVolumeLst[0] 		= VolInc;
				cumTypicalVolumeLst[0] 	= TypicalVolInc;
			}
			else
			{
				// just increment
				for (int idx = cumVolumeLst.Count - 1; idx >= 0; idx--)
				{
					cumVolumeLst[idx] 			+= VolInc;
					cumTypicalVolumeLst[idx] 	+= TypicalVolInc;
				}
			}

			// update plots
			PlotDay5[0] = (cumTypicalVolumeLst[4] / cumVolumeLst[4]);
			PlotDay4[0] = (cumTypicalVolumeLst[3] / cumVolumeLst[3]);
			PlotDay3[0] = (cumTypicalVolumeLst[2] / cumVolumeLst[2]);
			PlotDay2[0] = (cumTypicalVolumeLst[1] / cumVolumeLst[1]);
			PlotDay1[0] = (cumTypicalVolumeLst[0] / cumVolumeLst[0]);
			
			if (ShowLabelsSw)
			{
				Text textD1 = Draw.Text(this, "D1", "D1", -5, PlotDay1[0], Brushes.Yellow);
				Text textD2 = Draw.Text(this, "D2", "D2", -5, PlotDay2[0], Brushes.Orange);
				Text textD3 = Draw.Text(this, "D3", "D3", -5, PlotDay3[0], Brushes.OrangeRed);
				Text textD4 = Draw.Text(this, "D4", "D4", -5, PlotDay4[0], Brushes.Red);
				Text textD5 = Draw.Text(this, "D5", "D5", -5, PlotDay5[0], Brushes.DarkRed);
			}
			
			if (Bars.IsFirstBarOfSession && TransparentFirstBoD)
			{
				for (int i=0; i<5; i++)
					PlotBrushes[i][0] = Brushes.Transparent;
			}
			
		}

		#region Properties

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PlotDay1
		{
			get { return Values[0]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PlotDay2
		{
			get { return Values[1]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PlotDay3
		{
			get { return Values[2]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PlotDay4
		{
			get { return Values[3]; }
		}

		[Browsable(false)]
		[XmlIgnore]
		public Series<double> PlotDay5
		{
			get { return Values[4]; }
		}
		
		[NinjaScriptProperty]
		[Display(Name="Show Labels", Description="Show Labels Switch", Order=5, GroupName="Parameters")]
		public bool ShowLabelsSw
		{ get; set; }
		
		[NinjaScriptProperty]
		[Display(Name="Invisible 1st BoD", Description="Make first bar of the day invisible", Order=10, GroupName="Parameters")]
		public bool TransparentFirstBoD
		{ get; set; }
		
		#endregion

	}
}

#region NinjaScript generated code. Neither change nor remove.

namespace NinjaTrader.NinjaScript.Indicators
{
	public partial class Indicator : NinjaTrader.Gui.NinjaScript.IndicatorRenderBase
	{
		private MyIndicators.VWAP5Day[] cacheVWAP5Day;
		public MyIndicators.VWAP5Day VWAP5Day(bool showLabelsSw, bool transparentFirstBoD)
		{
			return VWAP5Day(Input, showLabelsSw, transparentFirstBoD);
		}

		public MyIndicators.VWAP5Day VWAP5Day(ISeries<double> input, bool showLabelsSw, bool transparentFirstBoD)
		{
			if (cacheVWAP5Day != null)
				for (int idx = 0; idx < cacheVWAP5Day.Length; idx++)
					if (cacheVWAP5Day[idx] != null && cacheVWAP5Day[idx].ShowLabelsSw == showLabelsSw && cacheVWAP5Day[idx].TransparentFirstBoD == transparentFirstBoD && cacheVWAP5Day[idx].EqualsInput(input))
						return cacheVWAP5Day[idx];
			return CacheIndicator<MyIndicators.VWAP5Day>(new MyIndicators.VWAP5Day(){ ShowLabelsSw = showLabelsSw, TransparentFirstBoD = transparentFirstBoD }, input, ref cacheVWAP5Day);
		}
	}
}

namespace NinjaTrader.NinjaScript.MarketAnalyzerColumns
{
	public partial class MarketAnalyzerColumn : MarketAnalyzerColumnBase
	{
		public Indicators.MyIndicators.VWAP5Day VWAP5Day(bool showLabelsSw, bool transparentFirstBoD)
		{
			return indicator.VWAP5Day(Input, showLabelsSw, transparentFirstBoD);
		}

		public Indicators.MyIndicators.VWAP5Day VWAP5Day(ISeries<double> input , bool showLabelsSw, bool transparentFirstBoD)
		{
			return indicator.VWAP5Day(input, showLabelsSw, transparentFirstBoD);
		}
	}
}

namespace NinjaTrader.NinjaScript.Strategies
{
	public partial class Strategy : NinjaTrader.Gui.NinjaScript.StrategyRenderBase
	{
		public Indicators.MyIndicators.VWAP5Day VWAP5Day(bool showLabelsSw, bool transparentFirstBoD)
		{
			return indicator.VWAP5Day(Input, showLabelsSw, transparentFirstBoD);
		}

		public Indicators.MyIndicators.VWAP5Day VWAP5Day(ISeries<double> input , bool showLabelsSw, bool transparentFirstBoD)
		{
			return indicator.VWAP5Day(input, showLabelsSw, transparentFirstBoD);
		}
	}
}

#endregion
