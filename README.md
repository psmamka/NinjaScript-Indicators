# Ninjascript Indicators
Indicators written in Ninjascript

## General Warning
Trading financial instruments, due to its appearance of "easy money", attracts certain unscrupulous types:
 
 * Beware of anyone who is trying to sell you their "magic indicators".
 * Beware of any "financial gurus" who never disclose their full P&L statisitcs. 
 * Beware of anyone showing only their winning trades as a "proof" of their technique.

Simply put: **"Beware of Evil Workers!"**

## Disclosures
The indocators presented here are provided solely for informational purposes.

This publication does not constitute a recommendation for the NinjaTrader trading platform, nor a recommendation for the NinjaTrader brokerage services or any other third party services provided alongside of them. This is also not an encouragement to engage in any kind of trading, whether involving futures or any other financial instuments.


## List of Indicators

Note: I put all my cutom files are inside a NT subfolder titled `MyIndicators`, to keep them separate for NT's own built-in indi's. Change the namespace in line 25 of each file to match the name of your desired subfolder.

### Moving Average Serpent / Snake
The old serpent beloved by many chartists. The idea is to remove some of the clutter that comes with having too many moving averages criss-crossing the chart. This particular example implements for pairs of EMA's. You can repurpose it to use any other type of indicator, or even pairs consisting of different indicator types (e.g. EMA vs SMA).

Note: If you don't like the indicator name wasting your chart real estate, set the `label` property to an empty string in the indicator menu.

![Ema Serpents](figures/EmaSerpents.png)
