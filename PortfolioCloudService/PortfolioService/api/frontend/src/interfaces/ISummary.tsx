interface Profit {
    id: number;
    user_id: string;
    type: 'profit' | 'loss';
    summary: number;
    net_worth: number;
}

export default Profit;  