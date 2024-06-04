import React, { useEffect, useState } from "react";
import Transaction from '../../interfaces/ITransaction';
import ExchangeRates from "../../interfaces/ExchangeRates";
import axios from "axios";

interface TransactionFormProps {
  userId: string;
  EnteredData: (transaction: Transaction) => void;
  CloseForm: React.Dispatch<React.SetStateAction<boolean>>;
}

const BuyCryptoForm: React.FC<TransactionFormProps> = ({ EnteredData, CloseForm, userId }) => {
  const [exchangeRates, setExchangeRates] = useState<ExchangeRates | null>(null);
  const [formData, setFormData] = useState<Transaction>({
    User_Id: "", // Set a default user_id or fetch it from somewhere
    Date_and_time: "",
    Type: "bought",
    Currency: "",
    Amount_paid_dollars: 0,
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData((prevData) => ({ ...prevData, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    formData.User_Id = userId;
    EnteredData(formData);
    CloseForm(false); // close form

    // reset the form after submission
    setFormData({
      User_Id: "", // Set a default user_id or fetch it from somewhere
      Date_and_time: "",
      Type: "bought",
      Currency: "",
      Amount_paid_dollars: 0,
    });
  };

  useEffect(() => {
    const fetchExchangeRates = async () => {
      try {
        const response = await axios.get<ExchangeRates>("http://127.0.0.1:10100/api/Crypto/get_rates", {
          headers: {
            'Content-Type': 'application/json',
          },
        });

        setExchangeRates(response.data);
      } catch (error) {
        console.error("Error fetching exchange rates:", error);
      }
    };

    fetchExchangeRates();
  }, []);

  return (
    <form onSubmit={handleSubmit} className="has-background-link-light" style={{padding: 10, borderRadius: 10}}>
      <div className="field">
        <label className="label">Date & Time</label>
        <div className="control">
          <input
            className="input"
            type="datetime-local"
            name="Date_and_time"
            value={formData.Date_and_time}
            onChange={handleChange}
            required
          />
        </div>
      </div>

      <div className="field">
        <label className="label">Sale / Buy</label>
        <div className="control">
          <div className="select">
            <select
              name="Type"
              value={formData.Type}
              onChange={handleChange}
              required
            >
              <option value="bought">Bought</option>
            </select>
          </div>
        </div>
      </div>

      <div className="field">
        <label className="label">Currency</label>
        <div className="control">
          <div className="select">
            <select
              name="Currency"
              value={formData.Currency}
              onChange={handleChange}
              required
            >
              <option value="" disabled>Select a currency</option>
              {exchangeRates?.rates &&
                Object.keys(exchangeRates.rates).map((currency) => (
                  <option key={currency} value={currency}>
                    {currency}
                  </option>
                ))}
            </select>
          </div>
        </div>
      </div>

      <div className="field">
        <label className="label">Net worth in $</label>
        <div className="control">
          <input
            className="input"
            type="number"
            name="Amount_paid_dollars"
            value={formData.Amount_paid_dollars}
            onChange={handleChange}
            required
          />
        </div>
      </div>

      <div className="field">
        <div className="control">
          <button className="button has-background-primary-dark has-text-white" type="submit" style={{ borderRadius: 7 }}>
            Buy Crypto
          </button>

          <button className="button ml-3 has-background-danger-dark has-text-white" type="button" style={{ borderRadius: 7 }} onClick={() => CloseForm(false)}>
            Cancel
          </button>
        </div>
      </div>
    </form>
  );
};

export default BuyCryptoForm;
