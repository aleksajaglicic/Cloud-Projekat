interface CurrencyInfo {
    currency: string;
    total_amount: number;
    type: "bought";
    difference: number;
    userId?: string;
}

export default CurrencyInfo;