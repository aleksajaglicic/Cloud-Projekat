import React, { FormEvent, useEffect, useState } from 'react';
import axios, { AxiosResponse } from 'axios';
import { check_session, create_session } from '../../session/session-manager';
import LoginData from '../../interfaces/ILogin';
import IRegistration from '../../interfaces/IRegistration';
import Navbar from '../../components/navbar/navbar';

const Registration = (): React.JSX.Element => {
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
  const [user, setUser] = useState<LoginData | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value, files } = e.target;
    if (name === 'file' && files && files.length > 0) {
      setFormData({ ...formData, file: files[0] });
    } else {
      setFormData({ ...formData, [name]: value });
    }
  };

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
  e.preventDefault();

  // Prepare the form data
  const data = new FormData();
  const { file, ...userDetails } = formData;
  data.append('user', JSON.stringify(userDetails));  // Append user details as JSON string
  if (file) {
    data.append('file', file);  // Append the file if it exists
  }

  try {
    console.log('Sending data to server:', data);  // Log the form data being sent
    const response: AxiosResponse = await axios.post('http://127.0.0.1:10100/api/User/add', data, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });

    if (response.status === 201) {
      setMessage(response.data.data);

      // Save the currently logged-in user to localStorage
      create_session(formData);
      window.location.reload();
    } else {
      setError(response.data.data);  // Axios response has request, response, data properties
    }
  } catch (error: any) {
    if (error.response) {
      // The request was made and the server responded with a status code that falls out of the range of 2xx
      setError(error.response.data.data);  // Axios response has request, response, data properties
    } else if (error.request) {
      // The request was made but no response was received
      setError('No response received from the server');
    } else {
      // Something happened in setting up the request that triggered an error
      setError(error.message);
    }
  }
};


  useEffect(() => {
    const check : LoginData | null =  check_session()
    setUser(check); //  korisnik iz local storage
    if(check != null)
      window.location.href = "/login"

  }, [])

  return (
    <div>
      <Navbar />
      {/* ternarni operator, uslovno renderovanje, osnove reactjs --> google or ig posts */}
      {user == null &&
        <div className="columns">
          <div className="column">
            <a href="/">
              <img
                src="regist.png"
                alt=""
              />
            </a>
          </div>
          <div className="column">
          <h1 style={{fontSize: 62, marginTop: 0, marginBottom: 40, marginLeft: 60, paddingLeft:50,}}>Register</h1>
            <form onSubmit={handleSubmit} className='container' style={{backgroundColor: 'white', marginTop: 20, padding: 20, paddingLeft:50, paddingRight: 50, borderRadius: 15}}>
              <div className="field">
                <label className="label">Name:</label>
                <div className="control">
                  <input
                    className="input"
                    type="text"
                    name="Name"
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
                    name="LastName"
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
                    name="Address"
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
                    name="City"
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
                    name="Country"
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
                    name="Number"
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
                    name="Email"
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
                    name="Password"
                    value={formData.Password}
                    onChange={handleChange}
                    placeholder="Enter your password"
                  />
                  <div className="field">
                    <label className="label">File:</label>
                    <div className="control">
                      <input
                        className="input"
                        type="file"
                        name="file"
                        onChange={handleChange}
                      />
                    </div>
                  </div>
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
                    Register
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      }
      <br/><br/>
    </div>

  );
};

export default Registration;