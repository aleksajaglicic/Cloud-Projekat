import ITranscation from './ITransaction';

interface Transcations {
    transactions: ITranscation[];
    fetchTransactions: () => void;
    fetchPortfolio: () => void;
};

export default Transcations;