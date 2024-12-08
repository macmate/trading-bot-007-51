# Trading Bot 007-51

A cTrader automated trading bot implementing the Golden Eye (007) and Area 51 trading strategies.

## Overview

This bot implements two distinct trading strategies:

1. Golden Eye (007)
   - Trading Window: 4PM to 7PM EST
   - Range-based breakout strategy
   - Optional EMA200 filter
   - Configurable break-even rules

2. Area 51
   - Trading Window: 12AM to 2AM EST
   - Range-based breakout strategy
   - Optional EMA200 filter
   - Configurable break-even rules

## Features

- Configurable time windows for trading
- Risk management with 1:3 risk-reward ratio
- Optional break-even stop loss movement
- EMA200 trend filter (optional)
- Session-based range calculation
- Multiple risk management approaches

## Performance Summary

Backtest results (June-August 2024):

### With EMA Filter

Golden Eye (007):
- Win Rate: 66.67%
- Profit Factor: 6.0
- Net Profit: $26,000

Area 51:
- Win Rate: 68.18%
- Profit Factor: 6.4
- Net Profit: $34,000

### Without EMA Filter

Golden Eye (007):
- Win Rate: 48.39%
- Net Profit: $13,000

Area 51:
- Win Rate: 50%
- Net Profit: $19,000

## Project Structure

```
src/
├── Indicators/     # Custom indicators including EMA
├── Strategies/     # Strategy implementations
├── RiskManagement/ # Position sizing and risk management
├── TimeManagement/ # Session time window handling
└── Utils/         # Helper functions
```

## Installation

[Coming Soon]

## Configuration

[Coming Soon]

## Usage

[Coming Soon]

## License

[Coming Soon]