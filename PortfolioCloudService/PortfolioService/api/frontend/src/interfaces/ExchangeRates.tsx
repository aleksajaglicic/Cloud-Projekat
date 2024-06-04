interface ExchangeRates {
    success: boolean;
    timestamp: number;
    base: string;
    date: string;
    rates: Record<string, number>;
  };

export default ExchangeRates;