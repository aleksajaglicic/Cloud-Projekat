interface Transaction {
    Id?: number;
    User_Id: string;
    Date_and_time: string;
    Type: 'bought' | 'sold';
    Currency: string;
    Amount_paid_dollars: number;
};

export default Transaction;