import CurrencyInfo from './ICryptoCurrency';

interface IPortfolio {
    transactions: CurrencyInfo[];
    userId: string;
    fetchTransactions: () => void;
    fetchPortfolio: () => void;
  }

  export default IPortfolio;