import React, { FormEvent, useEffect, useState } from 'react';
import axios, { AxiosResponse } from 'axios';
import { check_session, end_session } from '../../session/session-manager';
import LoginData from '../../interfaces/ILogin';
import IRegistration from '../../interfaces/IRegistration';
import Navbar from '../../components/navbar/navbar';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

const Change = (): React.JSX.Element => {
  const defaultData: IRegistration = {
    Name: '',
    LastName: '',
    Address: '',
    City: '',
    Country: '',
    Number: '',
    Email: '',
    Password: '',
    file: null
  }
  const [formData, setFormData] = useState<IRegistration>(defaultData);
  const [message, setMessage] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [loadedUser, setLoadedUser] = useState<LoginData | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  function logout() {
    end_session();
    window.location.reload(); // return to homepage
  }

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();

    // resetovanje greske i poruke
    setMessage('');
    setError('');

    // AXIOS POZIV KA APIJU
    try {
      const response: AxiosResponse = await axios.post('http://127.0.0.1:10100/api/Users/edit', formData, {
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (response.status === 200) {
        toast.success(response.data.data);
      } else {
        toast.error(response.data.data); // axiosresponse ima request, response, data,
        // kako smo mi slali jsonify({data: poruka}), bice respone.data pa nas data response.data.data
      }
    } catch (error: any) {
      if (error.response) {
        // The request was made and the server responded with a status code that falls out of the range of 2xx
        toast.error(error.response.data.data); // Axios response has request, response, data properties
      } else if (error.request) {
        // The request was made but no response was received
        toast.error('No response received from the server.');
      } else {
        // Something happened in setting up the request that triggered an error
        toast.error(error.message);
      }
    }
  };

  useEffect(() => {

    async function fetch_user(): Promise<boolean> {
      try {
        let user = check_session(); //  korisnik iz local storage

        if (user == null) {
          return false;
        }
        const response = await axios.post('http://127.0.0.1:10100/api/User/checkUser', { email: user.Email }, {
          headers: {
            'Content-Type': 'application/json',
          },
        });

        if (response.status === 200) {
          setLoadedUser(response.data)
          setFormData(response.data)
          return true;
        }
        else {
          setLoadedUser(null)
          return false;
        }
      }
      catch (error) {
        setLoadedUser(null)
        return false;
      }
    }

    async function handleUserFetchAndLogout(): Promise<void> {
      try {
        const userFetched: boolean = await fetch_user();

        if (userFetched === false) {
          logout();
        }
      } catch (error) {
        // Handle errors, if any, during the user fetch process
        console.error("Error fetching user:", error);
        toast.error('Error fetching user.');
        logout(); // Log out the user in case of an error during fetch
      }
    }

    // Call the function to initiate the process
    handleUserFetchAndLogout();
  }, [])

  return (
    <div>
      <Navbar />
      <ToastContainer />
      {/* ternarni operator, uslovno renderovanje, osnove reactjs --> google or ig posts */}
      {loadedUser != null &&
        <div>
          <div className="columns ">
          <div className="column">
            <a href="/">
              <img
                src="regis.png"
                alt=""
              />
            </a>
          </div>
          <div className="column">  
    
          <form onSubmit={handleSubmit} className='container' style={{backgroundColor: 'white', padding: 20, paddingLeft:50, paddingRight: 50, borderRadius: 15}}>
            <div className="field">
              <label className="label">Name:</label>
              <div className="control">
                <input
                  className="input"
                  type="text"
                  name="name"
                  value={formData.Name}
                  onChange={handleChange}
                  placeholder="Enter your first name"
                />
              </div>
            </div>
            <div className="field">
              <label className="label">Surname:</label>
              <div className="control">
                <input
                  className="input"
                  type="text"
                  name="surname"
                  value={formData.LastName}
                  onChange={handleChange}
                  placeholder="Enter your last name"
                />
              </div>
            </div>
            <div className="field">
              <label className="label">Address:</label>
              <div className="control">
                <input
                  className="input"
                  type="text"
                  name="address"
                  value={formData.Address}
                  onChange={handleChange}
                  placeholder="Enter your address"
                />
              </div>
            </div>
            <div className="field">
              <label className="label">City:</label>
              <div className="control">
                <input
                  className="input"
                  type="text"
                  name="city"
                  value={formData.City}
                  onChange={handleChange}
                  placeholder="Enter your city"
                />
              </div>
            </div>
            <div className="field">
              <label className="label">Country:</label>
              <div className="control">
                <input
                  className="input"
                  type="text"
                  name="country"
                  value={formData.Country}
                  onChange={handleChange}
                  placeholder="Enter your country"
                />
              </div>
            </div>
            <div className="field">
              <label className="label">Number:</label>
              <div className="control">
                <input
                  className="input"
                  type="text"
                  name="number"
                  value={formData.Number}
                  onChange={handleChange}
                  placeholder="Enter your phone number"
                />
              </div>
            </div>
            <div className="field">
              <label className="label">Email:</label>
              <div className="control">
                <input
                  className="input"
                  type="email"
                  name="email"
                  value={formData.Email}
                  onChange={handleChange}
                  placeholder="Enter your email"
                />
              </div>
            </div>
            <div className="field">
              <label className="label">Password:</label>
              <div className="control">
                <input
                  className="input"
                  type="password"
                  name="password"
                  value={formData.Password}
                  onChange={handleChange}
                  placeholder="Enter your password"
                />
              </div>
            </div>
            <div>
              {/* Poruka za gresku */}
              {error !== '' && <h2 className='has-text-danger'>{error}</h2>}

              {/* Poruka o uspesnom dodavanju */}
              {message !== '' && <h2 className='has-text-success'>{message}</h2>}
            </div>
            <div className="field mt-2">
              <div className="control">
                <button className="button is-info" type="submit">
                  Save
                </button>
              </div>
            </div>
          </form>
        </div>
        </div>
        </div>
      }
    </div>

  );
};

export default Change;