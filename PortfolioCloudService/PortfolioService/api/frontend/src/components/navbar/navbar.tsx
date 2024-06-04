import { useEffect, useState } from "react"
import { check_session, end_session } from "../../session/session-manager";
import axios from "axios";
import LoginData from "../../interfaces/ILogin";
import React from "react";
import AutocompleteSearch from "../search/searchBar";
import ExchangeRates from "../../interfaces/ExchangeRates";

interface Option {
    label: string;
    value: string;
}

const Navbar = () => {
    const [current, setCurrent] = useState<LoginData | null>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [options, setOptions] = useState<Option[]>([]);

    useEffect(() => {
        setLoading(true)
        async function check() {
            try {
                const user: LoginData | null = check_session();
                if (user == null)
                    return false
                setCurrent(user)
                const response = await axios.post('http://127.0.0.1:10100/api/Auth/login', { email: user.Email, password: user.Password }, {
                    headers: {
                        'Content-Type': 'application/json',
                    },
                });
                if (response.status === 201)
                    return true
                else
                    return false
            }
            catch (error) {
                return false
            }
        }

        const fetchExchangeRates = async () => {
            try {
                const response = await axios.get<ExchangeRates>("http://127.0.0.1:10100/api/Crypto/get_rates", {
                    headers: {
                    'Content-Type': 'application/json',
                    },
                });
        
                const jsonrates: ExchangeRates = response.data;

                var pomocni = [];
                for (const [key, value] of Object.entries(jsonrates.rates)) {
                    const novi: Option = {'label': key + " " + jsonrates.rates[key] + "", 'value': value.toString()}
                    pomocni.push(novi);
                }

                setOptions(pomocni);
                
                } catch (error) {
                console.error("Error fetching exchange rates:", error);
                }
            };

        check();
        fetchExchangeRates();
        setLoading(false)
    }, [])
    return (
        <div>
            {!loading &&
                <nav className="navbar" role="navigation" aria-label="main navigation">
                    <div className="navbar-brand mr-2">
                        <a className="navbar-item" href="/">
                            <img
                                src="logomain.png"
                                width={32}
                                height={32}
                                alt=""
                            /> &nbsp;&nbsp;<span style={{fontWeight: '500'}}>Crypto Portfolio</span>
                        </a>
                        <a
                            role="button"
                            className="navbar-burger"
                            aria-label="menu"
                            aria-expanded="false"
                            data-target="navbarBasicExample"
                            href="# "
                        >
                            <span aria-hidden="true" />
                            <span aria-hidden="true" />
                            <span aria-hidden="true" />
                        </a>
                    </div>
                    <div id="navbarBasicExample" className="navbar-menu">
                        <div className="navbar-start">
                            <a href="/" className="navbar-item">Home</a>
                            <a href="/portfolio" className="navbar-item">My Portfolio</a>
                            <div style={{ marginLeft: 30, width: 400 }}>
                                <AutocompleteSearch options={options} />
                            </div>
                        </div>
                        <div className="navbar-end">
                            <div className="navbar-item">
                                {current == null ?

                                    <div className="buttons">
                                        <a href="/registration" className="button has-background-link has-text-white">
                                            <strong>Register</strong>
                                        </a>
                                        <a href="/login" className="button is-info is-outlined">Log In</a>
                                    </div>
                                    :
                                    <div>
                                        <div className="buttons">
                                            <h1>Welcome, {current.Email} &emsp;</h1>

                                            <button onClick={() => { window.location.href = "/edit" }} className="button is-info is-outlined" >
                                                Edit profile
                                            </button>
                                            <button onClick={() => { end_session(); window.location.href = "/" }} className="button is-danger">
                                                <strong>Log Out</strong>
                                            </button>
                                        </div>
                                    </div>}
                            </div>
                        </div>
                    </div>
                </nav>
            }
        </div>
    )
}

export default Navbar