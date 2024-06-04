import React, { FormEvent, useEffect, useState } from 'react';
import axios, { AxiosResponse } from 'axios';
import { check_session, create_session } from '../../session/session-manager';
import LoginData from '../../interfaces/ILogin';
import Navbar from '../../components/navbar/navbar';

const Login = (): React.JSX.Element => {
  const [formData, setFormData] = useState({
    Email: '',
    Password: ''
  });

  const [message, setMessage] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [user, setUser] = useState<LoginData | null>(null);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
  };

  async function handleSubmit(e: FormEvent<HTMLFormElement>) {
    e.preventDefault();

    // resetovanje greske i poruke
    setMessage('');
    setError('');

    // AXIOS POZIV KA APIJU
    try {
      const response: AxiosResponse = await axios.post(`http://localhost:10100/api/Auth/login`, formData, {
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (response.status === 200) {
        setMessage(response.data.data);

        // cuva trenutno prijavljen korisnik se u localstorage
        create_session(formData);
        window.location.reload();

      } else {
        setError(response.data.data); // axiosresponse ima request, response, data,
        // kako smo mi slali jsonify({data: poruka}), bice respone.data pa nas data response.data.data
      }
    } catch (error: any) {
      if (error.response) {
        // The request was made and the server responded with a status code that falls out of the range of 2xx
        setError(error.response.data.data); // Axios response has request, response, data properties
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
    setUser(check_session()); //  korisnik iz local storage
  }, [])

  return (
    <div>
      <Navbar />
      {/* ternarni operator, uslovno renderovanje, osnove reactjs --> google or ig posts */}
      {user != null ?
        <div className='login-page'>
          <div className='columns mt-5 ml-5'>

            <div className='column' style={{ marginLeft: -500 }}>

            </div>
            <div className='column'>
              <h1>Welcome to your Crypto Portfolio!</h1>
              <h2>🚀 Track. Manage. Succeed.</h2>
              <a className="login-item" href="/login">
                <img
                  loading="lazy"
                  src="cryptohome.png"
                  alt=""
                />
              </a>

            </div>
            <div className='column' style={{ marginTop: 450 }}>
              <h2>Dive into the world of digital assets with our cutting-edge
                Crypto Portfolio platform. Whether you're a seasoned trader
                or a novice investor, we provide you with the tools you need
                to stay ahead in the dynamic cryptocurrency market.</h2><br></br>

              &nbsp;
              <button className="button is-normal is-responsive" onClick={() => { window.location.href = "/portfolio" }}>
                My Portfolio
              </button>

            </div>

            <div className='column' style={{ marginTop: -53 }}>
              <a href="/portfolio">
                <img
                  loading="lazy"
                  src="phone.png"
                  alt="Preview your Crypto wallet!"
                />
              </a>

            </div>
          </div>
        </div>

        :

        <div className="columns ">
          <div className="column">
            <a href="/">
              <img
                loading="lazy"
                src="regis.png"
                alt=""
              />
            </a>
          </div>
          <div className="column">
            <form onSubmit={handleSubmit} className='container' style={{ backgroundColor: 'white', padding: 20, paddingLeft: 50, paddingRight: 50, borderRadius: 15 }}>
              <h1 style={{ fontSize: 62, marginTop: -120, marginBottom: 50 }}>Log In</h1>
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
                    Log In
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      }
    </div>
  );
};

export default Login;